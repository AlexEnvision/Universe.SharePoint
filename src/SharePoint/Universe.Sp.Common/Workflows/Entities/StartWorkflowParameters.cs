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
    ///     Параметры запуска рабочего процесса на <see cref="SPListItem" />.
    ///     Parameters for start workflow on <see cref="SPListItem" />.
    /// <author>Alex Envision</author>
    /// </summary>
    /// <seealso cref="StartWebWorkflowParameters" />
    public class StartWorkflowParameters : StartWebWorkflowParameters
    {
        /// <summary>
        ///     Инициализирует новый экземпляр класса <see cref="StartWorkflowParameters"/>.
        ///     Initializes a new instance of the <see cref="StartWorkflowParameters" /> class.
        /// </summary>
        /// <param name="spListItem">
        ///     Элемент списка SharePoint.
        ///     The sp list item.
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
        public StartWorkflowParameters(SPListItem spListItem,
            SPWorkflowRunOptions runOptions,
            string associationName,
            params KeyValuePair<string, string>[] eventData)
            : this(spListItem.Web.Site.ID,
                spListItem.Web.ID,
                spListItem.ParentList.ID,
                runOptions,
                spListItem.ID,
                associationName,
                eventData)
        {
        }

        /// <summary>
        ///     Инициализирует новый экземпляр класса <see cref="StartWorkflowParameters"/>.
        ///     Initializes a new instance of the <see cref="StartWorkflowParameters" /> class.
        /// </summary>
        /// <param name="siteId">
        ///     Идентификатор сайта.
        ///     The site identifier.
        /// </param>
        /// <param name="webId">
        ///     Иденцификатор узла.
        ///     The web identifier.
        /// </param>
        /// <param name="listId">
        ///     Идентификатор списка.
        ///     The list identifier.
        /// </param>
        /// <param name="runOptions">
        ///     Параметры запуска в API рабочих процессов.
        ///     The run options.
        /// </param>
        /// <param name="listItemId">
        ///     Идентификатор элемента списка.
        ///     The list item identifier.
        /// </param>
        /// <param name="associationName">
        ///     Название ассоциированное с рабочим процессом.
        ///     Name of the association.</param>
        /// <param name="eventData">
        ///     Данные события.
        ///     The event data.
        /// </param>
        public StartWorkflowParameters(Guid siteId,
            Guid webId,
            Guid listId,
            SPWorkflowRunOptions runOptions,
            int listItemId,
            string associationName,
            params KeyValuePair<string, string>[] eventData) : base(siteId, webId, runOptions, associationName,
            eventData)
        {
            ListId = listId;
            ListItemId = listItemId;
        }

        /// <summary>
        ///     Получает идентификатор списка.
        ///     Gets the list identifier.
        /// </summary>
        /// <value>
        ///     Значение идентификатора списка.
        ///     The list identifier.
        /// </value>
        public Guid ListId { get; }

        /// <summary>
        ///     Получает идентификатор элемента списка.
        ///     Gets the list item identifier.
        /// </summary>
        /// <value>
        ///     Значение идентификатора элемента списка.
        ///     The list item identifier.
        /// </value>
        public int ListItemId { get; }
    }
}