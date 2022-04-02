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
using Microsoft.SharePoint;
using Universe.Sp.CQRS.Dal.MetaInfo;
using Universe.Sp.CQRS.Dal.Queries.Base;
using Universe.Sp.CQRS.Extensions;
using Universe.Sp.CQRS.Infrastructure;
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

            var spListItems = new MatList<SPListItem>();
            do
            {
                var collection = setSp.SpList.GetItems(req.SpQuery);
                spListItems += collection.Cast<SPListItem>().ToList();
                req.SpQuery.ListItemCollectionPosition = collection.ListItemCollectionPosition;
            } while (req.SpQuery.ListItemCollectionPosition != null);

            var mapper = new SpMapper();
            var items = spListItems.Select(x => mapper.ReverseMap(x, new TEntitySp
            {
                Id = x.ID,
                ListItem = x
            })).ToList();

            return new SpRequestedPage<TEntitySp>
            {
                Items = items
            };
        }
    }
}