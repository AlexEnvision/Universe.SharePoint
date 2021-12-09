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
using System.Security;

namespace Universe.Sp.Common.CSOM.Models
{
    /// <summary>
    ///     Параметры запуска рабочего процесса уровня узла на клиентской модели SharePoint.
    ///     Parameters for starting a web workflow on the Client SharePoint Object Model.
    /// <author>Alex Envision</author>
    /// </summary>
    public class StartCsomWebWorkflowParameters
    {
        /// <summary>
        ///     Инициализирует новый экземпляр класса <see cref="StartCsomWebWorkflowParameters"/>.
        ///     Initializes a new instance of the <see cref="StartCsomWebWorkflowParameters"/> class.
        /// </summary>
        /// <param name="webUrl">
        ///     URL узла.
        ///     The web url.
        /// </param>
        /// <param name="workflowName">
        ///     Название ассоциированное с рабочим процессом.
        ///     Name of the association.
        /// </param>
        /// <param name="eventData">
        ///     Данные события. 
        ///     The event data.
        /// </param>
        public StartCsomWebWorkflowParameters(
            string webUrl,
            string workflowName,
            Dictionary<string, object> eventData)
        {
            WebUrl = webUrl;
            WorkflowName = workflowName;
            EventData = eventData;
        }

        /// <summary>
        ///     Инициализирует новый экземпляр класса <see cref="StartCsomWebWorkflowParameters"/>.
        ///     Initializes a new instance of the <see cref="StartCsomWebWorkflowParameters"/> class.
        /// </summary>
        /// <param name="webUrl">
        ///     URL узла.
        ///     The web url.
        /// </param>
        /// <param name="workflowName">
        ///     Название ассоциированное с рабочим процессом.
        ///     Name of the association.
        /// </param>
        /// <param name="login">
        ///     Логин.
        ///     Login.
        /// </param>
        /// <param name="password">
        ///     Пароль.
        ///     Password.
        /// </param>
        /// <param name="eventData">
        ///     Данные события. 
        ///     The event data.
        /// </param>
        public StartCsomWebWorkflowParameters(
            string webUrl,
            string workflowName,
            string login,
            string password,
            Dictionary<string, object> eventData)
        {
            WebUrl = webUrl;
            WorkflowName = workflowName;
            EventData = eventData;
            LoginId = login;
            SecurePassword = ToSecureString(password);
            Password = password;
        }

        private SecureString ToSecureString(string plainString)
        {
            if (plainString == null)
                return null;

            SecureString secureString = new SecureString();
            foreach (char c in plainString)
            {
                secureString.AppendChar(c);
            }
            return secureString;
        }

        /// <summary>
        ///     Получает название рабочего процесса.
        ///     Gets the name of the association.
        /// </summary>
        /// <value>
        ///     Значение названия рабочего процесса.
        ///     The name of the association.
        /// </value>
        public string WorkflowName { get; }

        /// <summary>
        ///     Получает данные события.
        ///     Gets the event data.
        /// </summary>
        /// <value>
        ///     Словарь с данными события.
        ///     The event data.
        /// </value>
        public Dictionary<string, object> EventData { get; }

        /// <summary>
        ///     Получает адрес узла.
        ///     Gets the web url.
        /// </summary>
        /// <value>
        ///     Значение адреса узла.
        ///     The web url.
        /// </value>
        public string WebUrl { get; }

        /// <summary>
        ///     Получает логин-идентификатор.
        ///     Gets the login identifier.
        /// </summary>
        /// <value>
        ///     Значение логин-идентификатора.
        ///     The login identifier.
        /// </value>
        public string LoginId { get; }

        /// <summary>
        ///     Получает пароль в виде защищенной строки.
        ///     Gets the secure Password
        /// </summary>
        /// <value>
        ///     Значение пароля в виде защищенной строки.
        ///     The secure password.
        ///</value>
        public SecureString SecurePassword { get; set; }

        /// <summary>
        ///     Получает пароль.
        ///     Gets the password.
        /// </summary>
        /// <value>
        ///     Значение пароля.
        ///     The password.
        /// </value>
        public string Password { get; }
    }
}