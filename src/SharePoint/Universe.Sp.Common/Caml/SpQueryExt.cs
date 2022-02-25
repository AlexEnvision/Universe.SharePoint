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
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Universe.Helpers.Extensions;

namespace Universe.Sp.Common.Caml
{
    /// <summary>
    ///     Расширения для <see cref="SPQuery"/> и <see cref="SPSiteDataQuery"/>.
    ///     Extensions for <see cref="SPQuery"/> and <see cref="SPSiteDataQuery"/>.
    /// <author>Alex Envision</author>
    /// </summary>
    public static class SpQueryExt
    {
        /// <summary>
        ///     Получает строки данных по запросу.
        ///     Gets the data row by query.
        /// </summary>
        /// <param name="web">
        ///     Узел.
        ///     The web.
        /// </param>
        /// <param name="isSiteCollection">
        ///     если установлено в <c>true</c> [то это сайт-коллекция].
        ///     if set to <c>true</c> [is site collection].
        /// </param>
        /// <param name="listsBaseType">
        ///     Значение для заполнения атрибута BaseType у тега Lists.
        ///     Type of the lists base.
        /// </param>
        /// <param name="listsServerTemplate">
        ///     Значение для заполнения атрибута ServerTemplate у тега Lists.
        ///     The lists server template.
        /// </param>
        /// <param name="listsIds">
        ///     Идентификаторы списков.
        ///     The lists ids.
        /// </param>
        /// <param name="where">
        ///     Оператор условий.
        ///     The where.
        /// </param>
        /// <param name="order">
        ///     Оператор сортировки
        ///     The order.
        /// </param>
        /// <param name="viewFields">
        ///     Выводимые поля.
        ///     The view fields.
        /// </param>
        /// <param name="rowLimit">
        ///     Лимит выводимых строк в одном запросе.
        ///     The row limit.
        /// </param>
        /// <returns></returns>
        public static IEnumerable<DataRow> GetDataRowByQuery(
            this SPWeb web,
            bool isSiteCollection = false,
            int? listsBaseType = null,
            int? listsServerTemplate = null,
            List<Guid> listsIds = null,
            string where = "",
            string order = "",
            string viewFields = null,
            uint? rowLimit = null)
        {
            var webs = "<Webs Scope='Recursive'>";
            if (isSiteCollection)
                webs = "<Webs Scope='SiteCollection'>";

            var lists = string.Empty;
            if (listsIds?.Count > 0)
            {
                var sb = new StringBuilder();
                sb.Append("<Lists>");
                foreach (var listsId in listsIds)
                {
                    sb.Append($@"<List ID=""{listsId}"" />");
                }

                sb.Append("</Lists>");
            }
            else if (listsServerTemplate != null)
            {
                lists = $"<Lists ServerTemplate='{listsServerTemplate.Value}'/>";
            }
            else if (listsBaseType != null)
            {
                lists = $"<Lists BaseType='{listsBaseType.Value}'/>";
            }

            var spSiteDataQuery = new SPSiteDataQuery {
                Webs = webs,
                Lists = lists,
                Query = where + order
            };

            if (viewFields != null)
                spSiteDataQuery.ViewFields = viewFields;

            if (rowLimit != null)
                spSiteDataQuery.RowLimit = rowLimit.Value;

            return web.GetSiteData(spSiteDataQuery).Rows.Cast<DataRow>();
        }

        /// <summary>
        ///     Получение элементов, содержащихся в SPFolder.
        ///     Getting items contained in a SPFolder.
        /// </summary>
        /// <param name="spFolder">
        ///     SPFolder, содержащая необходимые элементы.
        ///     SPFolder containing necessary items.
        /// </param>
        /// <param name="viewFieldNames">
        ///     Имена выбираемых полей.
        ///     Names of selected fields.
        /// </param>
        /// <param name="camlQuery">
        ///     CAML Запрос на получение предметов. 
        ///     CAML Query for recieve items.
        /// </param>
        /// <param name="rowLimit">
        ///     Указываемый предел строк на один запрос.
        ///     Specified row limit.
        /// </param>
        /// <returns>
        ///     Возвращает коллекцию элементов SPListItem.
        ///     Return SPListItem collection.
        /// </returns>
        public static List<SPListItem> GetItems(
            SPFolder spFolder,
            List<string> viewFieldNames,
            string camlQuery = "",
            uint rowLimit = 500)
        {
            if (spFolder == null)
                throw new ArgumentNullException(nameof(spFolder));

            if (viewFieldNames == null)
                throw new ArgumentNullException(nameof(viewFieldNames));

            var spList = spFolder.ParentWeb.Lists[spFolder.ParentListId];
            if (spList == null)
                throw new NullReferenceException($"Parent list for folder (Url: {spFolder.Url}) not found");

            var spQuery = spList.RootFolder.Url.Equals(spFolder.Url)
                ? GetSpQuery(null, viewFieldNames, camlQuery, rowLimit)
                : GetSpQuery(spFolder, viewFieldNames, camlQuery, rowLimit);

            var resultItems = new List<SPListItem>();
            do
            {
                var listItemCollection = spList.GetItems(spQuery);
                resultItems.AddRange(listItemCollection.Cast<SPListItem>());

                spQuery.ListItemCollectionPosition = listItemCollection.ListItemCollectionPosition;
            }
            while (spQuery.ListItemCollectionPosition != null);

            return resultItems;
        }

        /// <summary>
        ///     Получение предметов по запросу CAML.
        ///     Get items by CAML Query
        /// </summary>
        /// <param name="list">
        ///     Список.
        ///     SPList
        /// </param>
        /// <param name="viewAttributes">
        ///     Условие отображения элемента.
        ///     View item condition
        /// </param>
        /// <param name="where">
        ///     Условие выборки данных.
        ///     Selection data condition
        /// </param>
        /// <param name="order">
        ///     Настройка условия сортировки.
        ///     Order caml setting
        /// </param>
        /// <param name="includePermissions">
        ///     Разрешения.
        ///     Permissions.
        /// </param>
        /// <param name="viewFields">
        ///     Названия выбираемых полей.
        ///     Names of the selected fields
        /// </param>
        /// <param name="rowLimit">
        ///     Указываемый предел строк на один запрос.
        ///     Specified row limit
        /// </param>
        /// <returns>
        ///     Возвращает коллекцию элементов SPListItem.
        ///     Return SPListItem collection
        /// </returns>
        public static IEnumerable<SPListItem> GetItemsByQuery(
            this SPList list,
            string viewAttributes = "Scope=\"Recursive\"",
            string where = "",
            string order = "",
            bool includePermissions = false,
            string viewFields = null,
            uint rowLimit = 0)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            return list.GetItems(ItemsQuery(viewAttributes, where, order, includePermissions, viewFields, rowLimit)).Cast<SPListItem>();
        }

        /// <summary>
        ///     Получение предметов по запросу CAML.
        ///     Get items by CAML Query
        /// </summary>
        /// <param name="list">
        ///     Список.
        ///     SPList
        /// </param>
        /// <param name="viewAttributes">
        ///     Условие отображения элемента.
        ///     View item condition
        /// </param>
        /// <param name="where">
        ///     Условие выборки данных.
        ///     Selection data condition
        /// </param>
        /// <param name="order">
        ///     Настройка условия сортировки.
        ///     Order caml setting
        /// </param>
        /// <param name="includePermissions">
        ///     Разрешения.
        ///     Permissions.
        /// </param>
        /// <param name="fieldsAndColumns">
        ///     Названия выбираемых полей для <see cref="DataTable"/>.
        ///     Names of selected fields for the <see cref="DataTable"/>.
        /// </param>
        /// <param name="rowLimit">
        ///     Указываемый предел строк на один запрос.
        ///     Specified row limit
        /// </param>
        /// <returns>
        ///     Возвращает данные как <see cref="DataTable"/>.
        ///     Return data as a <see cref="DataTable"/>.
        /// </returns>
        public static DataTable GetItemsByQuery(
            this SPList list,
            string viewAttributes = "Scope=\"Recursive\"",
            string where = "",
            string order = "",
            bool includePermissions = false,
            Dictionary<string, string> fieldsAndColumns = null,
            uint rowLimit = 0)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (fieldsAndColumns == null)
                fieldsAndColumns = new Dictionary<string, string>();

            return list.GetItems(
                    ItemsQuery(
                            viewAttributes,
                            where,
                            order,
                            includePermissions,
                            CamlHelper.BuildFieldsRef(fieldsAndColumns.Keys.ToArray()),
                            rowLimit
                        ))
                .GetDataTable().SetTableHeaders(fieldsAndColumns);
        }

        /// <summary>
        ///     CAML-запрос.
        ///     CAML Query.
        /// </summary>
        /// <param name="viewAttributes">
        ///     Условие отображения элемента.
        ///     View item condition.
        /// </param>
        /// <param name="where">
        ///     Условие выборки данных.
        ///     Selection data condition.
        /// </param>
        /// <param name="order">
        ///     Настройка условия сортировки. 
        ///     Order caml setting
        /// </param>
        /// <param name="includePermissions">
        ///     Разрешения.
        ///     Permissions.
        /// </param>
        /// <param name="viewFields">
        ///     Названия выбираемых полей.
        ///     Names of selected fields
        /// </param>
        /// <param name="rowLimit">
        ///     Указываемый предел строк на один запрос.
        ///     Specified row limit
        /// </param>
        /// <returns>
        ///     Возвращает <see cref="SPQuery"/>
        ///     Return <see cref="SPQuery"/>
        /// </returns>
        public static SPQuery ItemsQuery(
            string viewAttributes = "Scope=\"Recursive\"",
            string where = "",
            string order = "",
            bool includePermissions = false,
            string viewFields = null,
            uint rowLimit = 0)
        {
            var spQuery = new SPQuery {
                ViewAttributes = viewAttributes,
                Query = where + order,
                ViewFieldsOnly = !viewFields.IsNullOrEmpty(),
                ViewFields = viewFields,
                IncludePermissions = includePermissions
            };
            if (rowLimit > 0)
                spQuery.RowLimit = rowLimit;

            return spQuery;
        }

        /// <summary>
        ///     CAML-запрос для <see cref="SPFolder"/>.
        ///     CAML Query for <see cref="SPFolder"/>.
        /// </summary>
        /// <param name="spFolder">
        ///     SPFolder, содержащая необходимые элементы.
        ///     SPFolder containing necessary items
        /// </param>
        /// <param name="viewFieldNames">
        ///     Названия выводимых полей.
        ///     Names of the view fields
        /// </param>
        /// <param name="camlQuery">
        ///     CAML-запрос
        /// </param>
        /// <param name="rowLimit">
        ///     Указываемый предел строк на один запрос.
        ///     Specified row limit.
        /// </param>
        /// <returns>
        ///     Возвращает <see cref="SPQuery"/>
        ///     Return <see cref="SPQuery"/>
        /// </returns>
        private static SPQuery GetSpQuery(SPFolder spFolder, List<string> viewFieldNames, string camlQuery = "", uint rowLimit = 500)
        {
            var sbViewFields = new StringBuilder();
            viewFieldNames.ForEach(viewFieldName => sbViewFields.AppendFormat("<FieldRef Name='{0}' />", viewFieldName));

            var spQuery = new SPQuery {
                Folder = spFolder,
                ViewAttributes = "Scope=\"Recursive\"",
                Query = string.IsNullOrEmpty(camlQuery) ? "<Where></Where>" : camlQuery,
                RowLimit = rowLimit,
                ViewFields = sbViewFields.ToString()
            };

            if (!string.IsNullOrEmpty(spQuery.ViewFields))
                spQuery.ViewFieldsOnly = true;

            return spQuery;
        }

        /// <summary>
        ///     Получение предметов по запросу CAML.
        ///     Get items by CAML Query.
        ///     Использует пейджинг, получает столько сколько указано в rowLimit порциями по столько сколько указано в pageLimit.
        ///     Если rowLimit == 1 то выполняется только один запрос.
        ///     If folder then default Scope=FilesOnly.
        /// </summary>
        /// <param name="spList">
        ///     Список.
        ///     SPList.
        /// </param>
        /// <param name="spFolder">
        ///     SPFolder, содержащая необходимые элементы.
        ///     SPFolder containing necessary items.
        /// </param>
        /// <param name="viewAttributes">
        ///     Условие отображения элемента.
        ///     View item condition.
        /// </param>
        /// <param name="pageLimit">
        ///     Предел строк на страницу
        ///     Rows limit on a page.
        /// </param>
        /// <param name="where">
        ///     Условие выборки данных.
        ///     Selection data condition.
        /// </param>
        /// <param name="order">
        ///     Настройка условия сортировки.
        ///     Order caml setting.
        /// </param>
        /// <param name="viewFieldNames">
        ///     Названия выводимых полей.
        ///     Names of the view fields.
        /// </param>
        /// <param name="includePermissions">
        ///     Разрешения.
        ///     Permissions.
        /// </param>
        /// <param name="viewFields">
        ///     Выбираемые поля.
        ///     Names of selected fields.
        /// </param>
        /// <param name="rowLimit">
        ///     Указываемый предел строк на один запрос.
        ///     Specified row limit.
        /// </param>
        /// <returns>
        ///     Возвращает коллекцию элементов SPListItem.
        ///     Return SPListItem collection
        /// </returns>
        public static List<SPListItem> GetItemsInPortions(
            this SPList spList,
            SPFolder spFolder = null,
            string viewAttributes = null,
            uint? rowLimit = null,
            uint? pageLimit = null,
            string where = null,
            string order = null,
            bool includePermissions = false,
            string viewFields = null,
            List<string> viewFieldNames = null)
        {
            if (pageLimit == null)
                pageLimit = 500;

            var spQuery = CreateSpQuery(spFolder, viewAttributes, pageLimit, where, order, includePermissions, viewFields, viewFieldNames);

            if (rowLimit == 1) //в таком случае пейджинг точно не нужен
                return spList.GetItems(spQuery).Cast<SPListItem>().ToList();

            var resultItems = new List<SPListItem>();
            do
            {
                var listItemCollection = spList.GetItems(spQuery);
                resultItems.AddRange(listItemCollection.Cast<SPListItem>());

                if (rowLimit != null && rowLimit <= resultItems.Count)
                    return resultItems;

                spQuery.ListItemCollectionPosition = listItemCollection.ListItemCollectionPosition;
            }
            while (spQuery.ListItemCollectionPosition != null);

            return resultItems;
        }

        /// <summary>
        ///     CAML-запрос.
        ///     Если папка, то по умолчанию Scope=FilesOnly.
        ///     CAML Query.
        ///     If folder then default Scope=FilesOnly.
        /// </summary>
        /// <param name="spFolder">
        ///     Папка.
        ///     SPFolder
        /// </param>
        /// <param name="viewAttributes">
        ///     Условие отображения элемента.
        ///     View item condition
        /// </param>
        /// <param name="where">
        ///     Условие выборки данных.
        ///     Selection data condition
        /// </param>
        /// <param name="order">
        ///     Настройка условия сортировки выборки.
        ///     Order caml setting
        /// </param>
        /// <param name="viewFieldNames">
        ///     Названия выводимых полей.
        ///     Names of the view fields.
        /// </param>
        /// <param name="includePermissions">
        ///     Разрешения.
        ///     Permissions.
        /// </param>
        /// <param name="viewFields">
        ///     Выбираемые поля.
        ///     Names of selected fields.
        /// </param>
        /// <param name="rowLimit">
        ///     казываемый предел строк на один запрос.
        ///     Specified row limit.
        /// </param>
        /// <returns>
        ///     Возвращает <see cref="SPQuery"/>
        ///     Return <see cref="SPQuery"/>
        /// </returns>
        public static SPQuery CreateSpQuery(
            SPFolder spFolder = null,
            string viewAttributes = null,
            uint? rowLimit = null,
            string where = null,
            string order = null,
            bool includePermissions = false,
            string viewFields = null,
            List<string> viewFieldNames = null)
        {
            if (string.IsNullOrEmpty(viewAttributes))
                if (spFolder != null)
                    viewAttributes = "Scope=\"FilesOnly\"";
                else
                    viewAttributes = "Scope=\"RecursiveAll\"";

            if (viewFieldNames != null)
            {
                if (viewFields == null)
                    viewFields = string.Empty;

                viewFields += CamlHelper.BuildFieldsRef(viewFieldNames);
            }

            var spQuery = new SPQuery
            {
                ViewAttributes = viewAttributes,
                Query = where + order,
                ViewFieldsOnly = !viewFields.IsNullOrEmpty(),
                ViewFields = viewFields,
                IncludePermissions = includePermissions
            };

            if (spFolder != null)
                spQuery.Folder = spFolder;

            if (rowLimit != null && rowLimit > 0)
                spQuery.RowLimit = rowLimit.Value;

            return spQuery;
        }

        /// <summary>
        ///     Задаёт имена для столбцов DataTable. 
        ///     Set names for DataTable columns.
        /// </summary>
        private static DataTable SetTableHeaders(this DataTable inputTable, Dictionary<string, string> columnNames)
        {
            if (inputTable == null)
                inputTable = new DataTable();
            else
                foreach (var kvp in columnNames)
                {
                    inputTable.Columns[kvp.Key].ColumnName = kvp.Value;
                }

            return inputTable;
        }

        public static SPQuery Clone(this SPQuery query)
        {
            return new SPQuery
            {
                Query = query.Query,
                ViewFields = query.ViewFields,
                RowLimit = query.RowLimit,
                IncludePermissions = query.IncludePermissions,
                ViewFieldsOnly = query.ViewFieldsOnly,
                ViewAttributes = query.ViewAttributes
            };
        }
    }
}