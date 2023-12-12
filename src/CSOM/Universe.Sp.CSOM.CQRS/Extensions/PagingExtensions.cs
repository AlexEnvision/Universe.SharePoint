//  ╔═════════════════════════════════════════════════════════════════════════════════╗
//  ║                                                                                 ║
//  ║   Copyright 2022 Universe.SharePoint                                            ║
//  ║                                                                                 ║
//  ║   Licensed under the Apache License, Version 2.0 (the "License");               ║
//  ║   you may not use this file except in compliance with the License.              ║
//  ║   You may obtain a copy of the License at                                       ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0                                ║
//  ║                                                                                 ║
//  ║   Unless required by applicable law or agreed to in writing, software           ║
//  ║   distributed under the License is distributed on an "AS IS" BASIS,             ║
//  ║   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.      ║
//  ║   See the License for the specific language governing permissions and           ║
//  ║   limitations under the License.                                                ║
//  ║                                                                                 ║
//  ║                                                                                 ║
//  ║   Copyright 2022 Universe.SharePoint                                            ║
//  ║                                                                                 ║
//  ║   Лицензировано согласно Лицензии Apache, Версия 2.0 ("Лицензия");              ║
//  ║   вы можете использовать этот файл только в соответствии с Лицензией.           ║
//  ║   Вы можете найти копию Лицензии по адресу                                      ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0.                               ║
//  ║                                                                                 ║
//  ║   За исключением случаев, когда это регламентировано существующим               ║
//  ║   законодательством или если это не оговорено в письменном соглашении,          ║
//  ║   программное обеспечение распространяемое на условиях данной Лицензии,         ║
//  ║   предоставляется "КАК ЕСТЬ" и любые явные или неявные ГАРАНТИИ ОТВЕРГАЮТСЯ.    ║
//  ║   Информацию об основных правах и ограничениях,                                 ║
//  ║   применяемых к определенному языку согласно Лицензии,                          ║
//  ║   вы можете найти в данной Лицензии.                                            ║
//  ║                                                                                 ║
//  ╚═════════════════════════════════════════════════════════════════════════════════╝

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using iSys.Chatbot.Tools.Caml;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Query;
using Microsoft.SharePoint.News.DataModel;
using Universe.Sp.Common.CSOM.Caml;
using Universe.Sp.CSOM.CQRS.Infrastructure;
using Universe.Sp.CSOM.CQRS.Models;
using Universe.Sp.CSOM.CQRS.Models.Page;
using Universe.Sp.CSOM.DataAccess.Models;

namespace Universe.Sp.CSOM.CQRS.Extensions
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public static class PagingExtensions
    {
        public static SpRequestedPage<T> GetAllItemsAsOnePageExtension<T>(
            this QueryBuilder<T> query,
            SetSp<T> setSp,
            Paging paging)
            where T : class, IEntitySp, new()
        {
            if (setSp == null) 
                throw new ArgumentNullException(nameof(setSp));

            if (paging == null)
                throw new ArgumentNullException(nameof(paging));

            var spquery = query.SpQuery;

            if (spquery == null)
                spquery = new CamlQuery();

            // TODO -> Possible in the future it will have a problem
            //if (spquery.RowLimit == 0)
            //    spquery.RowLimit = (uint)paging.CountOnPage;

            var items = new List<ListItem>();

            ListItemCollection result;
            do
            {
                result = setSp.SpList.GetItems(spquery);
                var castsItems = result.Cast<ListItem>();
                items.AddRange(castsItems);

                spquery.ListItemCollectionPosition = result.ListItemCollectionPosition;
            }
            while (spquery.ListItemCollectionPosition != null);

            int? positionIndex = GetPageItemId(result);

            var mapper = new SpMapper();
            var entitiesSp = items.Select(x => mapper.ReverseMap(x, new T
            {
                Id = x.Id,
                ListItem = x
            })).ToList();

            return new SpRequestedPage<T>
            {
                Items = entitiesSp,
                EnableNext = spquery.ListItemCollectionPosition != null,
                Position = positionIndex
            };
        }

        public static SpRequestedPage<T> GetCurrentPageExtension<T>(
            this QueryBuilder<T> query,
            SetSp<T> setSp,
            Paging paging)
            where T : class, IEntitySp, new()
        {
            if (setSp == null) 
                throw new ArgumentNullException(nameof(setSp));

            if (paging == null)
                throw new ArgumentNullException(nameof(paging));

            var spquery = query.SpQuery;

            if (spquery == null)
                spquery = new CamlQuery();

            var list = setSp.SpList;

            // TODO -> Possible in the future it will have a problem
            //spquery.RowLimit = (uint)paging.CountOnPage;

            var items = new List<ListItem>();

            // Проверяем на наличие поданной на вход позиции. Если нет, то строим страницы и берём ИД первого элемента со страницы
            if (paging.PageIndex > 1)
            {
                paging.Position = paging.Position ?? GetFieldPositionByPageIndex(
                    list,
                    spquery,
                    paging.CountOnPage,
                    paging.PageIndex);

                var position = new ListItemCollectionPosition();
                position.PagingInfo = $"Paged=TRUE&p_ID={paging.Position ?? 0}";

                spquery.ListItemCollectionPosition = position;
            }

            var result = list.GetItems(spquery);
            var castsItems = result.Cast<ListItem>();
            items.AddRange(castsItems);

            spquery.ListItemCollectionPosition = result.ListItemCollectionPosition;
            int? positionIndex = GetPageItemId(result);

            var mapper = new SpMapper();
            var entitiesSp = items.Select(x => mapper.ReverseMap(x, new T {
                Id = x.Id,
                ListItem = x
            })).ToList();

            return new SpRequestedPage<T>
            {
                Items = entitiesSp,
                EnableNext = spquery.ListItemCollectionPosition != null,
                Position = positionIndex
            };
        }

        private static int? GetPageItemId(ListItemCollection items)
        {
            if (items.ListItemCollectionPosition == null)
                return null;

            try
            {
                string page = items.ListItemCollectionPosition.PagingInfo.Split(
                    new[] {
                        "&p_ID="
                    },
                    StringSplitOptions.RemoveEmptyEntries)[1].Split('&')[0];
                int.TryParse(page, out var position);
                return position;
            }
            catch
            {
                return null;
            }
        }

        private static int? GetFieldPositionByPageIndex(List list, CamlQuery query, int countOnPage, int pageIndex)
        {
            var pageQuery = query.Clone();
            var items = new List<ListItem>();

            var viewFields = CamlHelper.BuildFieldsRef(CamlHelper.GetFieldRef("ID"));
            var rowLimit = 0;

            // TODO -> Check work it or not
            var queryXml = $"<Query></Query>";
            var viewFieldsXml = !string.IsNullOrEmpty(viewFields) ? $"<ViewFields>{viewFields}</ViewFields>" : string.Empty;
            var rowLimitXml = rowLimit > 0 ? $"<RowLimit>{rowLimit}</RowLimit>" : string.Empty;
            var viewXml = $@"<View>{queryXml}{viewFieldsXml}{rowLimitXml}</View>";

            pageQuery.ViewXml = viewXml;

            do
            {
                var result = list.GetItems(pageQuery);
                var castsItems = result.Cast<ListItem>();
                items.AddRange(castsItems);

                pageQuery.ListItemCollectionPosition = result.ListItemCollectionPosition;
            }
            while (pageQuery.ListItemCollectionPosition != null);

            var maxPage = items.Count / countOnPage;

            if (pageIndex > maxPage)
                pageIndex = maxPage;

            var page = items.Skip(pageIndex * countOnPage).Take(countOnPage).ToList();
            var firstItem = page.FirstOrDefault();

            return firstItem?.Id;
        }
    }
}