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
using System.Security.Claims;
using System.Security.Principal;

namespace Universe.Sp.CQRS.Security.Principal
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class SpWebAppPrincipal : WindowsPrincipal, ISpWebAppPrincipal
    {
        public virtual ISpWebAppIdentity WebAppIdentity { get; private set; }

        public static SpWebAppPrincipal CreateFromWindowsPrincipal(WindowsPrincipal principal, SpWebAppIdentity identity)
        {
            identity = identity ?? throw new ArgumentNullException(nameof(identity));
            principal = principal ?? throw new ArgumentNullException(nameof(principal));

            var windowsIdentity = principal.Identity as WindowsIdentity;
            if (windowsIdentity == null)
                throw new ArgumentNullException(nameof(windowsIdentity));

            var webAppPrincipal = new SpWebAppPrincipal(windowsIdentity);
            webAppPrincipal.AddIdentities(principal.Identities);
            webAppPrincipal.WebAppIdentity = identity;
            return webAppPrincipal;
        }

        public static SpWebAppPrincipal CreateFromClaimsPrincipal(ClaimsPrincipal principal, SpWebAppIdentity identity)
        {
            identity = identity ?? throw new ArgumentNullException(nameof(identity));
            principal = principal ?? throw new ArgumentNullException(nameof(principal));

            var windowsIdentity = principal.Identity as WindowsIdentity;
            if (windowsIdentity == null)
                throw new ArgumentNullException(nameof(windowsIdentity));

            var webAppPrincipal = new SpWebAppPrincipal(windowsIdentity);
            webAppPrincipal.AddIdentities(principal.Identities);
            webAppPrincipal.WebAppIdentity = identity;
            return webAppPrincipal;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Principal.WindowsPrincipal" /> class by using the specified <see cref="T:System.Security.Principal.WindowsIdentity" /> object.</summary>
        /// <param name="ntIdentity">The object from which to construct the new instance of <see cref="T:System.Security.Principal.WindowsPrincipal" />. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="ntIdentity" /> is <see langword="null" />. </exception>
        public SpWebAppPrincipal(WindowsIdentity ntIdentity) : base(ntIdentity)
        {
        }
    }
}