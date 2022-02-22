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

using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Universe.Sp.Common.Caml;
using Universe.Sp.CQRS.Models;
using Universe.Sp.CQRS.Models.Condition;
using Universe.Sp.CQRS.Models.Filter;

namespace Universe.Sp.CQRS.Extensions
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public static class FilterExtensions
    {
        /// <summary>
        /// Применение фильтров (условий)
        /// </summary>
        /// <param name="query">Запрос к БД</param>
        /// <param name="conditions">Условия</param>
        /// <returns>Запрос к SP с примененными фильтрами</returns>
        public static QueryBuilder<T> ApplyFiltersAtQuery<T>(
            this QueryBuilder<T> query,
            IList<ConditionConfiguration> conditions)
            where T : class
        {
            var filters = ResolveSearchFilters(conditions);

            QueryBuilder<T> possiballyFilteredQuery = query;
            possiballyFilteredQuery = possiballyFilteredQuery.WhereByFilters(filters);

            if (possiballyFilteredQuery != null)
                query = possiballyFilteredQuery;
            return query;
        }

        /// <summary>
        /// Применение фильтров (условий)
        /// </summary>
        /// <param name="query">Запрос к БД</param>
        /// <param name="conditions">Условия</param>
        /// <returns>Запрос к SP с примененными фильтрами</returns>
        public static QueryBuilder<T> ApplyFiltersAtQuery<T, TEntityDto>(
            this QueryBuilder<T> query,
            IList<ConditionConfiguration> conditions) 
            where T : class
        {
            var filters = ResolveSearchFilters(conditions);

            QueryBuilder<T> possiballyFilteredQuery = query;
            possiballyFilteredQuery = possiballyFilteredQuery.WhereByFilters(filters);

            if (possiballyFilteredQuery != null)
                query = possiballyFilteredQuery;
            return query;
        }

        public static CamlChainRule ResolveSearchFilters(ConditionConfiguration c)
        {
            switch (c.Operator)
            {
                case "and":
                    return Mapper.Map<AndConfiguration, CamlChainRule>(c as AndConfiguration);
                case "or":
                    return Mapper.Map<OrConfiguration, CamlChainRule>(c as OrConfiguration);
                case "eq":
                    return new CamlChainRule
                    {
                        RuleBody = CamlHelper.CamlChain(
                            CamlHelper.LogicalOperators.AND,
                            Mapper.Map<EqConfiguration, CamlChainRule>(c as EqConfiguration).RuleBody
                        )
                    };
                case "neq":
                    return new CamlChainRule
                    {
                        RuleBody = CamlHelper.CamlChain(
                            CamlHelper.LogicalOperators.AND,
                            Mapper.Map<NeqConfiguration, CamlChainRule>(c as NeqConfiguration).RuleBody
                        )
                    };
                case "in":
                    return new CamlChainRule
                    {
                        RuleBody = CamlHelper.CamlChain(
                            CamlHelper.LogicalOperators.AND,
                            Mapper.Map<InConfiguration, CamlChainRule>(c as InConfiguration).RuleBody
                        )
                    };
                case "contains":
                    return new CamlChainRule
                    {
                        RuleBody = CamlHelper.CamlChain(
                            CamlHelper.LogicalOperators.AND,
                            Mapper.Map<ContainsConfiguration, CamlChainRule>(c as ContainsConfiguration).RuleBody
                        )
                    };
                case "between":
                    return new CamlChainRule
                    {
                        RuleBody = CamlHelper.CamlChain(
                            CamlHelper.LogicalOperators.AND,
                            Mapper.Map<BetweenConfiguration, CamlChainRule>(c as BetweenConfiguration).RuleBody
                        )
                    };
                case "isNull":
                    return new CamlChainRule
                    {
                        RuleBody = CamlHelper.CamlChain(
                            CamlHelper.LogicalOperators.AND,
                            Mapper.Map<IsNullConfiguration, CamlChainRule>(c as IsNullConfiguration).RuleBody
                        )
                    };
                default:
                    return Mapper.Map<CamlChainRule>(c);
            }
        }

        public static List<CamlChainRule> ResolveSearchFilters(IList<ConditionConfiguration> conditions)
        {
            var filters = new List<CamlChainRule>();
            if (conditions == null)
                return filters;

            filters.AddRange(conditions.Select(ResolveSearchFilters));
            return filters;
        }
    }
}