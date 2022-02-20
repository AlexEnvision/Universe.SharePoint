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
using Microsoft.SharePoint;
using Universe.Helpers.Extensions;

namespace Universe.Sp.DataAccess
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class UniverseSpContext : IUniverseSpContext
    {
        public string WebUrl { get; }

        public string WebLogin { get; }

        public SPWeb Web { get; set; }

        public UniverseSpContext(string webUrl, string webLogin)
        {
            if (webUrl.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(webUrl));

            if (webLogin.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(webLogin));

            WebUrl = webUrl;
            WebLogin = webLogin;

            Initialize(webUrl, webLogin);
        }

        protected void Initialize(string webUrl, string webLogin)
        {
            SPSecurity.RunWithElevatedPrivileges(
                delegate {
                    using (var gateSpSite = new SPSite(webUrl))
                    using (var gateSpWeb = gateSpSite.OpenWeb())
                    {
                        var spUser = gateSpWeb.AllUsers[webLogin];
                        var authFailed = spUser == null;
                        if (authFailed)
                            throw new Exception(
                                $"Попытка авторизоваться под логином '{webLogin}' завершилась неудачей. На сайте '{webUrl}' не обнаружено ни одного пользователя с таким логином.");

                        var userToken = spUser.UserToken;

                        var site = new SPSite(webUrl, userToken);
                        Web = site.OpenWeb();
                    }
                });
        }

        public static TUniverseSpContext CreateDbContext<TUniverseSpContext>(string connectionString) where TUniverseSpContext: UniverseSpContext, new()
        {
            // Приходится использовать рефлексию, ибо дженерики с параметрами в конструктуре так просто не создаются
            // Опять же это создание контекста базы, а это сама по себе медленная операция,
            // и поэтому данный подход влияние по производительности сам по себе оказывает минимальное
            var instance = Activator.CreateInstance(typeof(TUniverseSpContext), connectionString);
            var typedInstance = instance as TUniverseSpContext;
            return typedInstance;
        }

        public void Dispose()
        {
            Web?.Dispose();
        }
    }

    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class UniverseSpContext<TUniverseSpContext> : UniverseSpContext
        where TUniverseSpContext : UniverseSpContext, new()
    {
        public UniverseSpContext(string webUrl, string webLogin) : base(webUrl, webLogin)
        {
        }
    }
}