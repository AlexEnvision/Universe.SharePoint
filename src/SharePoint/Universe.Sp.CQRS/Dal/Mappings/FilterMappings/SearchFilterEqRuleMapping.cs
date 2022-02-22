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
using Newtonsoft.Json;
using Universe.Helpers.Extensions;
using Universe.Sp.Common.Caml;
using Universe.Sp.CQRS.Dal.Mappings.Extensions;
using Universe.Sp.CQRS.Dal.Mappings.Framework;
using Universe.Sp.CQRS.Models.Condition;
using Universe.Sp.CQRS.Models.Filter;
using Universe.Sp.CQRS.Models.Filter.Custom;

namespace Universe.Sp.CQRS.Dal.Mappings.FilterMappings
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    internal sealed class SearchFilterEqRuleMapping : AutoMap<EqConfiguration, CamlChainRule>
    {
        protected override void Configure(IMappingExpression<EqConfiguration, CamlChainRule> config)
        {
            base.Configure(config);
            config.Map(x => x.RuleBody, x => GetEqRule(x.LeftOperand, x.RightOperand));
        }

        private string GetEqRule(IArgumentConfiguration leftOperand, IArgumentConfiguration rightOperand)
        {
            var rightOperandType = rightOperand.Type;
            switch (rightOperandType)
            {
                case "int":
                    return CamlHelper.GetEqInteger(this.GetFieldName(leftOperand), this.GetIntegerValue(rightOperand));

                case "bool":
                    return CamlHelper.GetEqBool(this.GetFieldName(leftOperand), this.GetBooleanValue(rightOperand));

                case "lookup":
                    return CamlHelper.GetEqLookup(this.GetFieldName(leftOperand), this.GetLookupId(rightOperand));

                default:
                    return CamlHelper.GetEqText(this.GetFieldName(leftOperand), this.GetValue(rightOperand));
            }
        }

        private string GetFieldName(IArgumentConfiguration operand)
        {
            var fieldConfig = operand as FieldArgumentConfiguration;
            var name = fieldConfig?.Field?.SpFieldName;
            return name;
        }

        private bool GetBooleanValue(IArgumentConfiguration operand)
        {
            var valueConfig = operand as ValueArgumentConfiguration;
            var value = valueConfig?.Expression?.Replace("'", "");
            return bool.TryParse(value, out var boolValue) && boolValue;
        }

        private int GetIntegerValue(IArgumentConfiguration operand)
        {
            var valueConfig = operand as ValueArgumentConfiguration;
            var value = valueConfig?.Expression?.Replace("'", "");
            return int.TryParse(value, out var intValue) ? intValue : 0;
        }

        private int GetLookupId(IArgumentConfiguration operand)
        {
            var valueConfig = operand as ValueArgumentConfiguration;
            var possibleObject = valueConfig?.Expression;
            if (possibleObject != null && (!possibleObject.IsNullOrEmpty() &&
                                           (possibleObject.StartsWith("{") ||
                                            possibleObject.StartsWith("["))))
            {
                var expressionHasObject = JsonConvert.DeserializeObject<List<LookupValueConfiguration>>(valueConfig?.Expression);
                var obj = expressionHasObject?.FirstOrDefault();
                if (obj != null)
                {
                    var lookupvalue = obj?.LookupId ?? 0;
                    return lookupvalue;
                }
            }

            return 0;
        }

        private string GetValue(IArgumentConfiguration operand)
        {
            var valueConfig = operand as ValueArgumentConfiguration;
            var possibleObject = valueConfig?.Expression;
            if (possibleObject != null && (!possibleObject.IsNullOrEmpty() &&
                                           (possibleObject.StartsWith("{") ||
                                            possibleObject.StartsWith("["))))
            {
                var expressionHasObject = JsonConvert.DeserializeObject<List<LookupValueConfiguration>>(valueConfig?.Expression);
                var obj = expressionHasObject?.FirstOrDefault();
                if (obj != null)
                {
                    var lookupvalue = obj.LookupValue?.Replace("'", "") ?? string.Empty;
                    return lookupvalue;
                }
            }

            var value = valueConfig?.Expression?.Replace("'", "");
            return value;
        }
    }
}