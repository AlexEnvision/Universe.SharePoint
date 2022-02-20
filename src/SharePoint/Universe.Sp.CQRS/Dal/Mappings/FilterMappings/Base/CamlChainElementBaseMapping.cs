//  ╔═════════════════════════════════════════════════════════════════════════════════╗
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Framework                                             ║
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
//  ║   Copyright 2021 Universe.Framework                                             ║
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
using AutoMapper;
using Universe.Sp.CQRS.Dal.Mappings.Framework;
using Universe.Sp.CQRS.Models.Condition;
using Universe.Sp.CQRS.Models.Filter;

namespace Universe.Sp.CQRS.Dal.Mappings.FilterMappings.Base
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    internal abstract class CamlChainElementBaseMapping<TFrom, TTo> : AutoMap<TFrom, TTo>
        where TFrom : ConditionConfiguration
        where TTo : CamlChainRule
    {
        protected IList<CamlChainRule> SearchFilterRulesResolver(ICollection<ConditionConfiguration> operands)
        {
            IList<CamlChainRule> rules = new List<CamlChainRule>();

            foreach (var c in operands)
            {
                switch (c.Operator)
                {
                    case "eq":
                        rules.Add(Mapper.Map<EqConfiguration, CamlChainRule>(c as EqConfiguration));
                        break;
                    case "neq":
                        rules.Add(Mapper.Map<NeqConfiguration, CamlChainRule>(c as NeqConfiguration));
                        break;
                    case "and":
                        rules.Add(Mapper.Map<AndConfiguration, CamlChainRule>(c as AndConfiguration));
                        break;
                    case "or":
                        rules.Add(Mapper.Map<OrConfiguration, CamlChainRule>(c as OrConfiguration));
                        break;
                    case "in":
                        rules.Add(Mapper.Map<InConfiguration, CamlChainRule>(c as InConfiguration));
                        break;
                    case "contains":
                        rules.Add(Mapper.Map<ContainsConfiguration, CamlChainRule>(c as ContainsConfiguration));
                        break;
                    case "between":
                        rules.Add(Mapper.Map<BetweenConfiguration, CamlChainRule>(c as BetweenConfiguration));
                        break;
                    default:
                        throw new ArgumentException("Неподдерживаемая конфигурация фильтров.");
                }
            }

            return rules;
        }
    }
}