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
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.SharePoint;
using Universe.Helpers.Extensions;
using Universe.Sp.Common.BatchProcess.Entities.Base;

namespace Universe.Sp.Common.BatchProcess
{
    /// <summary>
    ///     Генерирует xml-разметку на основе элемента
    /// <author>Alex Envision</author>
    /// </summary>
    public static class BatchSpHelper
    {
        /// <summary>
        ///     Префикс для имен полей
        /// </summary>
        private const string BatchFieldPrefix = "urn:schemas-microsoft-com:office:office#";

        /// <summary>
        ///     Генерация команды создания/изменения элемента
        /// </summary>
        /// <param name="spo">Sharepoint Object</param>
        /// <returns></returns>
        public static string GetBatchSaveCommand<TSpo>(this TablePartItemContainer<TSpo> spo) where TSpo : ISpo
        {
            if (spo == null)
                throw new ArgumentNullException(nameof(spo));

            return GetBatchSaveCommand(spo, null);
        }

        /// <summary>
        ///     Генерация команды создания/изменения элемента
        /// </summary>
        /// <param name="spo">Sharepoint Object</param>
        /// <returns></returns>
        public static string GetBatchSaveCommand<TSpo>(this TSpo spo) where TSpo : ISpo
        {
            if (spo == null)
                throw new ArgumentNullException(nameof(spo));

            return GetBatchSaveCommand(spo, null);
        }

        /// <summary>
        ///     Генерация команды создания/изменения элемента
        /// </summary>
        /// <param name="spoContainer">Sharepoint Object table part container</param>
        /// <param name="listId">Id списка</param>
        /// <returns></returns>
        public static string GetBatchSaveCommand<TSpo>(this TablePartItemContainer<TSpo> spoContainer, Guid? listId)
            where TSpo : ISpo
        {
            var detailFolderUrl = spoContainer.RootFolder;
            if (detailFolderUrl.IsNullOrEmpty())
                throw new ArgumentException("Не указана корневая папка для табличной части!");

            listId = listId ?? spoContainer.ListId;
            return GetBatchSaveCommandWithRootFolder(spoContainer.SpoItem, listId, detailFolderUrl);
        }

        /// <summary>
        ///     Генерация команды создания/изменения элемента
        /// </summary>
        /// <param name="spo">Sharepoint Object</param>
        /// <param name="listId">Id списка</param>
        /// <param name="rootFolder">Корневая папка элемента</param>
        /// <returns></returns>
        private static string GetBatchSaveCommandWithRootFolder<TSpo>(TSpo spo, Guid? listId, string rootFolder = "")
            where TSpo : ISpo
        {
            if (spo == null)
                throw new ArgumentNullException(nameof(spo));

            var objType = typeof(TSpo);
            var properties = objType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var sb = new StringBuilder();

            sb.AppendFormat(@"<Method ID=""{0}, Save"">{1}",
                Guid.NewGuid(),
                Environment.NewLine);

            var splistItem = spo.SpListItem;
            if (splistItem == null)
                throw new ArgumentException("У элемента отсутствует значение SPListItem!");

            var id = spo.SpListItem.ID;

            var listIdStr = listId != null ? listId.ToString() : splistItem.ParentList.ID.ToString();
            sb.AppendLine($"<SetList>{listIdStr}</SetList>");

            var idStr = id > 0 ? id.ToString() : "New";
            sb.AppendLine($"<SetVar Name=\"ID\">{idStr}</SetVar>");

            sb.AppendLine(@"<SetVar Name=""Cmd"">Save</SetVar>");

            sb.AppendLine($"<SetVar Name=\"RootFolder\">{rootFolder}</SetVar>");

            foreach (var property in properties)
            {
                if (property.Name == "SpListItem" ||
                    property.Name == "SpItem" ||
                    property.Name == "Attachments" ||
                    property.Name == "_ModerationStatus" ||
                    property.Name == "EncodedAbsUrl" ||
                    property.Name == "Lmi" ||
                    property.Name == "RootFolder" ||
                    property.Name == "_Level" ||
                    property.Name == "Created" ||
                    property.Name == "Modified" ||
                    property.Name == "PermMask" ||
                    property.Name == "ID" && id > 0)
                    continue;

                //var att = property.GetCustomAttribute<SpoFieldPropertiesAttribute>();
                //var readOnly = att?.ReadOnly;
                //if (readOnly.HasValue && readOnly.Value)
                //    continue;

                object raw = null;
                try
                {
                    raw = property.GetValue(spo);
                }
                catch (Exception ex)
                {
                    if (ex.IsRethrow()) throw;
                    // ignored
                }

                string value;

                if (raw is bool)
                {
                    value = (bool) raw ? "1" : "0";
                }
                else if (raw is DateTime)
                {
                    if ((DateTime) raw == new DateTime())
                        continue;

                    value = $"{raw:yyyy-MM-ddTHH:mm:ssZ}";
                }
                else if (raw is SPFieldLookupValueCollection)
                {
                    value = raw.ToString();
                }
                else
                {
                    value = raw?.ToString() ?? string.Empty;
                }

                var name = property.Name;
                if (string.IsNullOrEmpty(value))
                    value = string.Empty;

                sb.AppendLine(
                    $"<SetVar Name=\"{BatchFieldPrefix}{name}\">{value}</SetVar>");
            }
            sb.AppendLine(@"</Method>");
            return sb.ToString();
        }

        /// <summary>
        ///     Генерация команды создания/изменения элемента
        /// </summary>
        /// <param name="spo">Sharepoint Object</param>
        /// <param name="listId">Id списка</param>
        /// <returns></returns>
        public static string GetBatchSaveCommand<TSpo>(this TSpo spo, Guid? listId) where TSpo : ISpo
        {
            if (spo == null)
                throw new ArgumentNullException(nameof(spo));

            return GetBatchSaveCommandWithRootFolder(spo, listId);
        }

        /// <summary>
        ///     Генерация команды удаления элемента
        /// </summary>
        /// <param name="spo">Sharepoint Object</param>
        /// <returns></returns>
        public static string GetBatchDeleteCommand<TSpo>(this TSpo spo) where TSpo : ISpo
        {
            if (spo == null)
                throw new ArgumentNullException(nameof(spo));

            return GetBatchDeleteCommand(spo, null);
        }

        /// <summary>
        ///     Генерация команды удаления элемента
        /// </summary>
        /// <param name="spo">Sharepoint Object</param>
        /// <param name="listId">Id списка</param>
        /// <returns></returns>
        public static string GetBatchDeleteCommand<TSpo>(this TSpo spo, Guid? listId) where TSpo : ISpo
        {
            if (spo == null)
                throw new ArgumentNullException(nameof(spo));

            var objType = spo.GetType();
            var properties = objType.GetProperties();
            var sb = new StringBuilder();

            sb.AppendFormat(@"<Method ID=""{0}, Delete"">{1}",
                Guid.NewGuid(),
                Environment.NewLine);

            var splistItemField = properties.Where(x => x.Name == "SpListItem").Select(x => x).FirstOrDefault();
            if (splistItemField == null)
                throw new ArgumentException("Не указано или неверное поле сопоставления - 'SpListItem'");

            var idField = properties.Where(x => x.Name == "ID").Select(x => x).FirstOrDefault();
            if (idField == null)
                throw new ArgumentException("У элемента нет поля 'ID'!");

            var idFieldValueStr = idField.GetValue(spo).ToString();
            int.TryParse(idFieldValueStr, out var id);

            var splistItem = splistItemField.GetValue(spo) as SPListItem;
            if (splistItem == null)
                throw new ArgumentException("У элемента отсутствует SPListItem!");

            sb.AppendFormat(@"<SetList>{0}</SetList>{1}",
                listId != null ? listId.ToString() : splistItem.ParentList.ID.ToString(),
                Environment.NewLine);
            sb.AppendFormat(@"<SetVar Name=""ID"">{0}</SetVar>{1}",
                id,
                Environment.NewLine);

            sb.AppendLine(@"<SetVar Name=""Cmd"">Delete</SetVar>");
            sb.AppendLine(@"</Method>");
            return sb.ToString();
        }
    }
}