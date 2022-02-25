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

using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Universe.Sp.Common.Caml;
using Universe.Sp.CQRS.Models.Filter;

namespace Universe.Sp.CQRS.Models
{
    public class QueryBuilder<T> where T: class
    {
        public SPQuery SpQuery
        {
            get => SpQueryExt.ItemsQuery(
                where: CamlWhere ?? string.Empty,
                order: CamlOrder ?? string.Empty,
                viewFields: CamlViewFields ?? string.Empty);
        }

        public string CamlWhere { get; set; }

        public string CamlOrder { get; set; }

        public string CamlViewFields { get; set; }

        public QueryBuilder<T> WhereByFilters(List<CamlChainRule> filters)
        {
            if (filters == null)
                return this;

            var chains = filters.Select(x => x.RuleBody).ToArray();

            CamlWhere = CamlHelper.GetCamlWhere(CamlHelper.CamlChain(
                CamlHelper.LogicalOperators.OR,
                chains));

            return this;
        }

        public QueryBuilder<T> OrderBy(List<CamlSortRule> rules)
        {
            var descriptors = rules.Select(x => x.RuleBody).ToArray();

            CamlOrder = CamlHelper.GetCamlOrderBy(descriptors);

            return this;
        }
    }
}