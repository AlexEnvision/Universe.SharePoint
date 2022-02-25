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

using Universe.Sp.CQRS.Dal.Commands.Base;
using Universe.Sp.CQRS.Dal.Queries.Base;
using Universe.Sp.CQRS.Infrastructure;
using Universe.Sp.DataAccess;

namespace Universe.Sp.CQRS.Dal
{
    /// <summary>
    ///     Построитель запросов
    /// <author>Alex Envision</author>
    /// </summary>
    public static class CommandQueryBuilder
    {
        public static T CreateCommand<T>(IUniverseSpScope scope) where T : BaseCommand, new()
        {
            return new T
            {
                SpCtx = scope.SpCtx,
                User = scope.CurrentUser
            };
        }

        public static T CreateQuery<T>(IUniverseSpScope scope) where T : BaseQuery, new()
        {
            return new T
            {
                SpCtx = scope.SpCtx,
                User = scope.CurrentUser,
            };
        }

        public static T CreateCommand<T, TUniverseDbContext>(UniverseSpScope<TUniverseDbContext> scope) 
            where T : BaseCommand, new()
            where TUniverseDbContext : UniverseSpContext, new()
        {
            return new T
            {
                SpCtx = scope.SpCtx,
                User = scope.CurrentUser
            };
        }

        public static T CreateQuery<T, TUniverseDbContext>(UniverseSpScope<TUniverseDbContext> scope)
            where T : BaseQuery, new()
            where TUniverseDbContext : UniverseSpContext, new()
        {
            return new T
            {
                SpCtx = scope.SpCtx,
                User = scope.CurrentUser,
            };
        }
    }
}