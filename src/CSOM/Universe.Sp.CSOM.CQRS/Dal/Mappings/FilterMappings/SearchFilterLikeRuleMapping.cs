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

using AutoMapper;
using Universe.Sp.Common.CSOM.Caml;
using Universe.Sp.CSOM.CQRS.Dal.Mappings.Extensions;
using Universe.Sp.CSOM.CQRS.Dal.Mappings.Framework;
using Universe.Sp.CSOM.CQRS.Models.Condition;
using Universe.Sp.CSOM.CQRS.Models.Filter;

namespace Universe.Sp.CSOM.CQRS.Dal.Mappings.FilterMappings
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    internal class SearchFilterLikeRuleMapping : AutoMap<ContainsConfiguration, CamlChainRule>
    {
        protected override void Configure(IMappingExpression<ContainsConfiguration, CamlChainRule> config)
        {
            base.Configure(config);
            config.Map(x => x.RuleBody, x => CamlHelper.GetContainsText(this.GetFieldName(x.LeftOperand), this.GetValue(x.RightOperand)));
        }

        private string GetFieldName(IArgumentConfiguration operand)
        {
            var fieldConfig = operand as FieldArgumentConfiguration;
            var name = fieldConfig?.Field?.SpFieldName;
            return name;
        }

        private string GetValue(IArgumentConfiguration operand)
        {
            var valueConfig = operand as ValueArgumentConfiguration;
            var value = valueConfig?.Expression.Replace("'", "");
            return value;
        }
    }
}