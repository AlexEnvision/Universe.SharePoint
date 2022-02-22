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

using System;
using Unity;
using Universe.Helpers.Extensions;
using Universe.Sp.CQRS.Models.Dto;
using Universe.Sp.DataAccess;

namespace Universe.Sp.CQRS.Infrastructure
{
    public class UniverseSpScope<TUniverseSpContext> : IUniverseSpScope, IDisposable where TUniverseSpContext : class, IUniverseSpContext
    {
        private readonly ISpWebAppPrincipalResolver _principalResolver;

        private SpUserDto _user;

        public IUnityContainer Container { get; }

        public UniverseSpScope(ISpWebAppSettings appSettings, ISpWebAppPrincipalResolver principalResolver,
            IUnityContainer container)
        {
            Container = container;
            if (appSettings == null)
                throw new ArgumentNullException(nameof(appSettings));

            _principalResolver = principalResolver ?? throw new ArgumentNullException(nameof(principalResolver));

            var webUrl = appSettings.WebUrl;
            var login = appSettings.WebLogin;

            var universeSpContext = CreateSpContext(webUrl, login);
            SpCtx = universeSpContext;

            SessionId = Guid.NewGuid();
        }

        public UniverseSpScope(ISpWebAppSettings appSettings, IUnityContainer container)
        {
            Container = container;
            if (appSettings == null)
                throw new ArgumentNullException(nameof(appSettings));

            var webUrl = appSettings.WebUrl;
            var login = appSettings.WebLogin;

            var universeSpContext = CreateSpContext(webUrl, login);
            SpCtx = universeSpContext;

            SessionId = Guid.NewGuid();
        }

        public UniverseSpScope()
        {
        }

        /// <summary>
        ///     ИД выполняющейся сессии
        /// </summary>
        public Guid SessionId { get; }

        /// <summary>
        ///     Текущий пользователь
        /// </summary>
        public virtual SpUserDto CurrentUser
        {
            get { return _user ?? (_user = GetUser(_principalResolver)); }
            protected set { _user = value; }
        }

        public IUniverseSpContext SpCtx { get; set; }


        public void Dispose()
        {
            SpCtx?.Dispose();
        }

        protected virtual SpUserDto GetUser(ISpWebAppPrincipalResolver principalResolver)
        {
            if (principalResolver == null)
                return new SpUserDto();

            var principal = principalResolver.GetCurrentPrincipal();
            if (principal == null)
                return new SpUserDto();

            var identity = principal.WebAppIdentity;
            if (identity == null)
                throw new ArgumentException(nameof(identity));

            var userName = identity.Name;
            if (userName.IsNullOrEmpty())
                throw new Exception("userName.IsNullOrEmpty()");

            var user = new SpUserDto
            {
                Name = userName
            };

            return user;
        }

        private TUniverseSpContext CreateSpContext(string webUrl, string login)
        {
            // Приходится использовать рефлексию, ибо дженерики с параметрами в конструктуре так просто не создаются
            // Опять же это создание контекста базы, а это сама по себе медленная операция,
            // и поэтому данный подход влияние по производительности сам по себе оказывает минимальное
            var instance = Activator.CreateInstance(typeof(TUniverseSpContext), webUrl, login);
            var typedInstance = instance as TUniverseSpContext;
            return typedInstance;
        }
    }
}