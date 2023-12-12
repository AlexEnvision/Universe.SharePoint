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
using System.Net;
using Microsoft.SharePoint.Client;
using Universe.Helpers.Extensions;

namespace Universe.Sp.CSOM.DataAccess
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class UniverseSpCsomContext : IUniverseSpCsomContext
    {
        public string WebUrl { get; }

        public string WebLogin { get; }

        public string Domain { get; }

        protected string Password { get; }

        public Web Web { get; set; }

        public ClientContext SpContext { get; set; }

        public UniverseSpCsomContext(string webUrl, string webLogin, string domain, string password)
        {
            if (webUrl.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(webUrl));

            if (webLogin.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(webLogin));

            if (domain.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(domain));

            if (password.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(password));

            WebUrl = webUrl;
            WebLogin = webLogin;
            Domain = domain;
            Password = password;

            Initialize(webUrl, webLogin, domain, password);
        }

        public ClientContext CreateContext()
        {
            var context = new ClientContext(WebUrl);
            context.Credentials = new NetworkCredential(WebLogin, Password, Domain);

            return context;
        }

        protected void Initialize(string webUrl, string webLogin, string domain, string password)
        {
            SpContext = CreateContext();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            SpContext.Load(SpContext.Web, w => w.Lists, w => w.Title, w => w.Url, w => w.Id);
            SpContext.ExecuteQuery();

            Web = SpContext.Web;
        }

        public static TUniverseSpContext CreateDbContext<TUniverseSpContext>(string connectionString) where TUniverseSpContext: UniverseSpCsomContext, new()
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
            SpContext?.Dispose();
        }
    }

    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class UniverseSpCsomContext<TUniverseSpCsomContext> : UniverseSpCsomContext
        where TUniverseSpCsomContext : UniverseSpCsomContext, new()
    {
        public UniverseSpCsomContext(string webUrl, string webLogin, string domain, string password) : base(webUrl, webLogin, domain, password)
        {
        }
    }
}