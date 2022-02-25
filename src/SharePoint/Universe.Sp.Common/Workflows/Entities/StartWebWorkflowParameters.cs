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
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;

namespace Universe.Sp.Common.Workflows.Entities
{
    /// <summary>
    ///     Параметры запуска рабочего процесса на уровне узла.
    ///     Parameters for start workflow on web.
    /// <author>Alex Envision</author>
    /// </summary>
    public class StartWebWorkflowParameters
    {
        /// <summary>
        ///     Инициализирует новый экземпляр класса <see cref="StartWebWorkflowParameters"/>.
        ///     Initializes a new instance of the <see cref="StartWebWorkflowParameters" /> class.
        /// </summary>
        /// <param name="web">
        ///     Узел.
        ///     The web.
        /// </param>
        /// <param name="runOptions">
        ///     Параметры запуска в API рабочих процессов.
        ///     The run options.
        /// </param>
        /// <param name="associationName">
        ///     Название ассоциированное с рабочим процессом.
        ///     Name of the association.
        /// </param>
        /// <param name="eventData">
        ///     Данные события.
        ///     The event data.
        /// </param>
        public StartWebWorkflowParameters(
            SPWeb web,
            SPWorkflowRunOptions runOptions,
            string associationName,
            KeyValuePair<string, string>[] eventData) : this(web.Site.ID, web.ID, runOptions, associationName,
            eventData)
        {
        }

        /// <summary>
        ///     Инициализирует новый экземпляр класса <see cref="StartWebWorkflowParameters"/>.
        ///     Initializes a new instance of the <see cref="StartWebWorkflowParameters" /> class.
        /// </summary>
        /// <param name="siteId">
        ///     Идентификатор сайта.
        ///     The site identifier.
        /// </param>
        /// <param name="webId">
        ///     Иденцификатор узла.
        ///     The web identifier.
        /// </param>
        /// <param name="runOptions">
        ///     Параметры запуска в API рабочих процессов.
        ///     The run options.
        /// </param>
        /// <param name="associationName">
        ///     Название ассоциированное с рабочим процессом.
        ///     Name of the association.</param>
        /// <param name="eventData">
        ///     Данные события.
        ///     The event data.
        /// </param>
        public StartWebWorkflowParameters(
            Guid siteId,
            Guid webId,
            SPWorkflowRunOptions runOptions,
            string associationName,
            params KeyValuePair<string, string>[] eventData)
        {
            SiteId = siteId;
            WebId = webId;
            RunOptions = runOptions;
            AssociationName = associationName;
            EventData = eventData;
        }

        /// <summary>
        ///     Получает название ассоциированное с рабочим процессом.
        ///     Gets the name of the association.
        /// </summary>
        /// <value>
        ///     Значение названия ассоциированное с рабочим процессом.
        ///     The name of the association.
        /// </value>
        public string AssociationName { get; }

        /// <summary>
        ///     Получает данные события.
        ///     Gets the event data.
        /// </summary>
        /// <value>
        ///     Пара ключ-значение с данными события.
        ///     The event data.
        /// </value>
        public KeyValuePair<string, string>[] EventData { get; }

        /// <summary>
        ///     Палучает параметры запуска в API рабочих процессов.
        ///     Gets the run options.
        /// </summary>
        /// <value>
        ///     Значение параметров запуска в API рабочих процессов.
        ///     The run options.
        /// </value>
        public SPWorkflowRunOptions RunOptions { get; }

        /// <summary>
        ///     Получает идентификатор сайта.
        ///     Gets the site identifier.
        /// </summary>
        /// <value>
        ///     Идентификатор сайта.
        ///     The site identifier.
        /// </value>
        public Guid SiteId { get; }

        /// <summary>
        ///     Получает идентификатор узла.
        ///     Gets the web identifier.
        /// </summary>
        /// <value>
        ///     Иденцификатор узла.
        ///     The web identifier.
        /// </value>
        public Guid WebId { get; }
    }
}