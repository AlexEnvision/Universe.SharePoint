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
using System.Linq.Expressions;
using AutoMapper;

namespace Universe.Sp.CQRS.Dal.Mappings.Extensions
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    internal static class MappingExpressionExtensions
    {
        public static IMappingExpression<TFrom, TTo> Ignore<TFrom, TTo>(
            this IMappingExpression<TFrom, TTo> config,
            Expression<Func<TTo, object>> to)
        {
            config.ForMember(to, opt => opt.Ignore());
            return config;
        }

        public static IMappingExpression<TFrom, TTo> Map<TFrom, TFromRes, TTo>(
            this IMappingExpression<TFrom, TTo> config,
            Expression<Func<TTo, object>> to,
            Expression<Func<TFrom, TFromRes>> from)
        {
            config.ForMember(to, opt => opt.MapFrom(from));
            return config;
        }

        public static IMappingExpression<TFrom, TTo> MapOnlyField<TFrom, TFromRes, TTo>(
            this IMappingExpression<TFrom, TTo> config,
            Expression<Func<TTo, object>> ignore,
            Expression<Func<TTo, object>> to,
            Expression<Func<TFrom, TFromRes>> from)
        {
            config.ForMember(ignore, opt => opt.Ignore());
            config.ForMember(to, opt => opt.MapFrom(from));
            return config;
        }

        public static IMappingExpression<TFrom, TTo> Map<TFrom, TFromRes, TTo>(
            this IMappingExpression<TFrom, TTo> config,
            string name,
            Expression<Func<TFrom, TFromRes>> from)
        {
            config.ForMember(name, opt => opt.MapFrom(from));
            return config;
        }

        public static IMappingExpression<TFrom, TTo> Ignore<TFrom, TTo>(
            this IMappingExpression<TFrom, TTo> config,
            string name)
        {
            config.ForMember(name, opt => opt.Ignore());
            return config;
        }
    }
}