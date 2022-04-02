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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Universe.Helpers.Extensions;
using Universe.Sp.Common.Caml;
using Universe.Sp.CQRS.Dal.MetaInfo;
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
        /// <param name="mi"></param>
        /// <returns>Запрос к SP с примененными фильтрами</returns>
        public static QueryBuilder<T> ApplyFiltersAtQuery<T>(this QueryBuilder<T> query,
            IList<ConditionConfiguration> conditions, 
            QueryableMetaInfo<T> mi)
            where T : class
        {
            var conditionsResolvedFieldNames = ReplaceFieldNamesByMetaInfo(conditions, mi);

            var filters = ResolveSearchFilters(conditionsResolvedFieldNames);

            QueryBuilder<T> possiballyFilteredQuery = query;
            possiballyFilteredQuery = possiballyFilteredQuery.WhereByFilters(filters);

            if (possiballyFilteredQuery != null)
                query = possiballyFilteredQuery;
            return query;
        }

        private static IList<ConditionConfiguration> ReplaceFieldNamesByMetaInfo<T>(
            IList<ConditionConfiguration> conditions,
            QueryableMetaInfo<T> mi)
            where T : class
        {
            foreach (var fieldMetaInfo in mi.FieldsMetaInfo)
            {
                var item = fieldMetaInfo as QueryableFieldMetaInfo<T>;
                if (item != null)
                {
                    var name = item.Name;

                    var selector = item.DbFieldSelector;

                    string resolveName = "";
                    if (selector.Body is UnaryExpression)
                    {
                        var expression = selector.Body as UnaryExpression;
                        var operand = expression?.Operand as MemberExpression;
                        resolveName = operand?.Member.Name ?? string.Empty;
                    }

                    if (selector.Body is MemberExpression)
                    {
                        var operand = selector.Body as MemberExpression;
                        resolveName = operand?.Member.Name ?? string.Empty;
                    }

                    for (var index = 0; index < conditions.Count; index++)
                    {
                        var conditionConfiguration = conditions[index];
                        conditionConfiguration =
                            FilterConfigurationReplaceName(conditionConfiguration, name, resolveName);
                        conditions[index] = conditionConfiguration;
                    }
                }
            }

            return conditions;
        }

        private static ConditionConfiguration FilterConfigurationReplaceName(ConditionConfiguration configuration, string from, string to)
        {
            switch (configuration)
            {
                case AndConfiguration andConfiguration:
                    foreach (var andConfigurationOperand in andConfiguration.Operands)
                    {
                        FilterConfigurationReplaceName(andConfigurationOperand, from, to);
                    }

                    return andConfiguration;

                case BetweenConfiguration betweenConfiguration:
                    betweenConfiguration.LeftOperand = SetFieldArgument(from, to, betweenConfiguration.LeftOperand);
                    return betweenConfiguration;
                    
                case ContainsConfiguration containsConfiguration:
                    containsConfiguration.LeftOperand = SetFieldArgument(from, to, containsConfiguration.LeftOperand);
                    return containsConfiguration;

                case EqConfiguration eqConfiguration:
                    eqConfiguration.LeftOperand = SetFieldArgument(from, to, eqConfiguration.LeftOperand);
                    return eqConfiguration;

                case InConfiguration inConfiguration:
                    inConfiguration.LeftOperand = SetFieldArgument(from, to, inConfiguration.LeftOperand);
                    return inConfiguration;

                case IsNotNullConfiguration isNotNullConfiguration:
                    isNotNullConfiguration.LeftOperand = SetFieldArgument(from, to, isNotNullConfiguration.LeftOperand);
                    return isNotNullConfiguration;

                case IsNullConfiguration isNullConfiguration:
                    isNullConfiguration.LeftOperand = SetFieldArgument(from, to, isNullConfiguration.LeftOperand);
                    return isNullConfiguration;

                case MembershipConfiguration membershipConfiguration:
                    membershipConfiguration.LeftOperand = SetFieldArgument(from, to, membershipConfiguration.LeftOperand);
                    return membershipConfiguration;

                case NeqConfiguration neqConfiguration:
                    neqConfiguration.LeftOperand = SetFieldArgument(from, to, neqConfiguration.LeftOperand);
                    return neqConfiguration;

                case OrConfiguration orConfiguration:
                    foreach (var andConfigurationOperand in orConfiguration.Operands)
                    {
                        FilterConfigurationReplaceName(andConfigurationOperand, from, to);
                    }

                    return orConfiguration;

                case Models.Filter.Custom.BetweenConfiguration betweenConfigurationCustom:
                    betweenConfigurationCustom.LeftOperand = SetFieldArgument(from, to, betweenConfigurationCustom.LeftOperand);
                    return betweenConfigurationCustom;

                default:
                    throw new ArgumentOutOfRangeException(nameof(configuration));
            }
        }

        private static IArgumentConfiguration SetFieldArgument(string from, string to, IArgumentConfiguration fieldArgument)
        {
            var fieldArgumentConfig = fieldArgument as FieldArgumentConfiguration;
            if (fieldArgumentConfig != null)
            {
                var field = fieldArgumentConfig.Field as FieldConfiguration;
                if (field != null && field.SpFieldName.PrepareToCompare() == @from.PrepareToCompare())
                {
                    field.SpFieldName = to;
                }

                fieldArgumentConfig.Field = field;
            }

            return fieldArgumentConfig;
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