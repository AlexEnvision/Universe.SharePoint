//  ╔═════════════════════════════════════════════════════════════════════════════════╗
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.SharePoint                                            ║
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
//  ║   Copyright 2021 Universe.SharePoint                                            ║
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
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Universe.Sp.CQRS.Dal.Queries.Base;
using Universe.Sp.CQRS.Extensions;
using Universe.Sp.CQRS.Models;
using Universe.Sp.CQRS.Models.Filter;
using Universe.Sp.CQRS.Models.Req;
using Universe.Sp.DataAccess.Models;
using Universe.Types.Collection;

namespace Universe.Sp.CQRS.Dal.Queries
{
    /// <summary>
    ///     Запрос на получение множества сущностей, с полным включением связанных сущностей
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TEntitySp"></typeparam>
    public class GetSpEntitiesQuery<TEntitySp> : BaseQuery where TEntitySp : class, IEntitySp, new()
    {
        public virtual RequestedPage<TEntitySp> Execute(GetSpEntitiesReq req)
        {
            //var query = this.DbCtx.Set<TEntitySp>().AsQueryable();

            //var container = req.FieldMapContainer as FieldMapContainer<TEntityDb>;

            //// Построение метаинформации для фильтрации и сортировки
            //var mi = query.CreateDbRequestMetaInfo(container?.FieldMap, true);

            //var availableItems = query
            //    .ApplyFiltersAtQuery(req.Filters, mi, req.AllowNoTrackingMode)
            //    .ApplySortingAtQuery(req.Sorting, mi)
            //    .GetCurrentPageExtension(req.Paging);

            var listUrl = new TEntitySp().ListUrl;
            var list = SpCtx.Web.GetList(listUrl);

            if (req.SpQuery == null)
            {
                var query = new QueryBuilder<TEntitySp>();

                query = query.ApplyFiltersAtQuery(req.Filters);

                throw new NotImplementedException();
            }
            else
            {
                var spListItems = new MatList<SPListItem>();
                do
                {
                    var collection = list.GetItems(req.SpQuery);
                    spListItems += collection.Cast<SPListItem>().ToList();
                    req.SpQuery.ListItemCollectionPosition = collection.ListItemCollectionPosition;
                } while (req.SpQuery.ListItemCollectionPosition != null);

                var items = spListItems.Select(x => new TEntitySp
                {
                    Id = x.ID,
                    ListItem = x
                }).ToList();

                return new RequestedPage<TEntitySp>
                {
                    Items = items
                };
            }
        }
    }
}