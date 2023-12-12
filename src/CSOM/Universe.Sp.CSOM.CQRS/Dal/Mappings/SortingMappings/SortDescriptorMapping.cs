﻿//  ╔═════════════════════════════════════════════════════════════════════════════════╗
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
using Universe.Sp.CSOM.CQRS.Models.Filter;
using Universe.Sp.CSOM.CQRS.Models.Sort;

namespace Universe.Sp.CSOM.CQRS.Dal.Mappings.SortingMappings
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    internal sealed class SortDescriptorMapping : AutoMap<SortConfiguration, CamlSortRule>
    {
        protected override void Configure(IMappingExpression<SortConfiguration, CamlSortRule> config)
        {
            base.Configure(config);
            config.Map(x => x.RuleBody,
                x => CamlHelper.GetCamlOrderByElement(this.MapFieldWithUpperLetter(x.Field),
                    x.Direction == SortDirection.Asc));
        }

        private string MapFieldWithUpperLetter(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return str.Substring(0, 1).ToUpper() + str.Substring(1, str.Length - 1);
        }
    }
}