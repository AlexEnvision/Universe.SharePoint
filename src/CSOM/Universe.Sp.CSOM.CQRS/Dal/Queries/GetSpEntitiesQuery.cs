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

using System.Linq;
using System.Linq.Expressions;
using Microsoft.SharePoint.Client;
using Universe.Sp.CSOM.CQRS.Dal.Queries.Base;
using Universe.Sp.CSOM.CQRS.Dal.MetaInfo;
using Universe.Sp.CSOM.CQRS.Extensions;
using Universe.Sp.CSOM.CQRS.Infrastructure;
using Universe.Sp.CSOM.CQRS.Models;
using Universe.Sp.CSOM.CQRS.Models.Filter;
using Universe.Sp.CSOM.CQRS.Models.Req;
using Universe.Sp.CSOM.DataAccess.Models;
using Universe.Types.Collection;

namespace Universe.Sp.CSOM.CQRS.Dal.Queries
{
    /// <summary>
    ///     Запрос на получение множества сущностей, с полным включением связанных сущностей
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TEntitySp"></typeparam>
    public class GetSpEntitiesQuery<TEntitySp> : BaseQuery where TEntitySp : class, IEntitySp, new()
    {
        public virtual SpRequestedPage<TEntitySp> Execute(GetSpEntitiesReq req)
        {
            SetSp<TEntitySp> setSp = SpCtx.Set<TEntitySp>();

            var query = new QueryBuilder<TEntitySp>();

            if (req.SpQuery == null)
            {
                var container = req.FieldMapContainer as FieldMapContainer<TEntitySp>;

                // Построение метаинформации для фильтрации и сортировки
                QueryableMetaInfo<TEntitySp> mi = query.CreateDbRequestMetaInfo(container?.FieldMap, true);

                var availableItems = req.IsAllWithPaging
                    ? query
                        .ApplyFiltersAtQuery(req.Filters, mi)
                        .ApplySortingAtQuery(req.Sorting, mi)
                        .GetAllItemsAsOnePageExtension(setSp, req.Paging)
                    : query
                        .ApplyFiltersAtQuery(req.Filters, mi)
                        .ApplySortingAtQuery(req.Sorting, mi)
                        .GetCurrentPageExtension(setSp, req.Paging);

                return availableItems;
            }

            var spListItems = new MatList<ListItem>();
            do
            {
                var collection = setSp.SpList.GetItems(req.SpQuery);

                SpCtx.SpContext.Load(collection);
                SpCtx.SpContext.ExecuteQuery();

                spListItems += collection.Cast<ListItem>().ToList();
                req.SpQuery.ListItemCollectionPosition = collection.ListItemCollectionPosition;
            } while (req.SpQuery.ListItemCollectionPosition != null || 
                     (spListItems.Count < req.Paging.CountOnPage && !req.IsAllWithPaging));

            var mapper = new SpMapper();
            var items = spListItems.Select(x => mapper.SafeReverseMap(x, new TEntitySp
            {
                Id = x.Id,
                ListItem = x
            })).ToList();

            var fmc = req.FieldMapContainer as FieldMapContainer<TEntitySp>;
            if (fmc != null)
            {
                foreach (var fmo in fmc.FieldMap)
                {
                    var kvp = fmo.Key;
                    var exp = fmo.Value;

                    var spName = exp.Name;
                    var entityField = exp.Body;
                }
            }

            return new SpRequestedPage<TEntitySp>
            {
                Items = items
            };
        }
    }
}