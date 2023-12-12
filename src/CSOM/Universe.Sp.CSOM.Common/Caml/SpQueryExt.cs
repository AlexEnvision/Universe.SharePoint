using System;
using System.Collections.Generic;
using System.Linq;
//using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Query;
using Microsoft.SharePoint.News.DataModel;

namespace Universe.Sp.Common.CSOM.Caml
{
    //using DiagnosticsService;

    /// <summary>
    /// SPQuery and SPSiteDataQuery.
    /// </summary>
    public static class SpQueryExt
    {
        ///// <summary>
        ///// Getting items contained in a SPFolder
        ///// </summary>
        ///// <param name="spFolder">SPFolder containing necessary items</param>
        ///// <param name="viewFieldNames">Names of selected fields</param>
        ///// <param name="camlQuery">CAML Query for recieve items</param>
        ///// <param name="rowLimit">Specified row limit</param>
        ///// <returns>Return SPListItem collection</returns>
        //public static List<ListItem> GetItems(
        //    Folder spFolder,
        //    List<string> viewFieldNames,
        //    string camlQuery = "",
        //    uint rowLimit = 500)
        //{
        //    if (spFolder == null)
        //        throw new ArgumentNullException(nameof(spFolder));

        //    if (viewFieldNames == null)
        //        throw new ArgumentNullException(nameof(viewFieldNames));

        //    var spList = spFolder.ParentWeb.Lists[spFolder.ParentListId];
        //    if (spList == null)
        //        throw new NullReferenceException($"Parent list for folder (Url: {spFolder.Url}) not found");

        //    var spQuery = spList.RootFolder.Url.Equals(spFolder.Url)
        //        ? GetSpQuery(null, viewFieldNames, camlQuery, rowLimit)
        //        : GetSpQuery(spFolder, viewFieldNames, camlQuery, rowLimit);

        //    var resultItems = new List<ListItem>();
        //    do
        //    {
        //        var listItemCollection = spList.GetItems(spQuery);
        //        resultItems.AddRange(listItemCollection.Cast<ListItem>());

        //        spQuery.ListItemCollectionPosition = listItemCollection.ListItemCollectionPosition;
        //    }
        //    while (spQuery.ListItemCollectionPosition != null);

        //    return resultItems;
        //}

        ///// <summary>
        ///// Get items by CAML Query
        ///// </summary>
        ///// <param name="list">SPList</param>
        ///// <param name="viewAttributes">View item condition</param>
        ///// <param name="where">Selection data condition</param>
        ///// <param name="order">Order caml setting</param>
        ///// <param name="includePermissions">Permissions</param>
        ///// <param name="viewFields">Names of the selected fields</param>
        ///// <param name="rowLimit">Specified row limit</param>
        ///// <returns>Return SPListItem collection</returns>
        //public static IEnumerable<ListItem> GetItemsByQuery(
        //    this List list,
        //    string viewAttributes = "Scope=\"Recursive\"",
        //    string where = "",
        //    string order = "",
        //    bool includePermissions = false,
        //    string viewFields = null,
        //    uint rowLimit = 0)
        //{
        //    if (list == null)
        //        throw new ArgumentNullException(nameof(list));

        //    return list.GetItems(ItemsQuery(viewAttributes, where, order, includePermissions, viewFields, rowLimit)).Cast<ListItem>();
        //}

        /// <summary>
        /// Get items by CAML Query for Client Model SP
        /// </summary>
        /// <param name="list">SPList</param>
        /// <param name="ctx">Client Context</param>
        /// <param name="where">Selection data condition</param>
        /// <param name="order">Order caml setting</param>
        /// <param name="includePermissions">Permissions</param>
        /// <param name="viewFields">Names of the selected fields</param>
        /// <param name="rowLimit">Specified row limit</param>
        /// <returns>Return SPListItem collection</returns>
        public static IEnumerable<ListItem> GetItemsByQuery(
            this List list,
            ClientContext ctx,
            string where = "",
            string order = "",
            bool includePermissions = false,
            string viewFields = null,
            uint rowLimit = 0)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            var items = list.GetItems(ItemsQuery(where, order, includePermissions, viewFields, rowLimit));
            ctx.Load(items);
            ctx.ExecuteQuery();
            return items.Cast<ListItem>();
        }

        /// <summary>
        /// Get items by CAML Query for Client Model SP
        /// </summary>
        /// <param name="list">SPList</param>
        /// <param name="where">Selection data condition</param>
        /// <param name="order">Order caml setting</param>
        /// <param name="includePermissions">Permissions</param>
        /// <param name="viewFields">Names of the selected fields</param>
        /// <param name="rowLimit">Specified row limit</param>
        /// <returns>Return SPListItem collection</returns>
        public static IEnumerable<ListItem> GetItemsByQuery(
            this List list,
            string where = "",
            string order = "",
            bool includePermissions = false,
            string viewFields = null,
            uint rowLimit = 0)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            var ctx = list.Context;
            var items = list.GetItems(ItemsQuery(where, order, includePermissions, viewFields, rowLimit));
            ctx.Load(items);
            ctx.ExecuteQuery();
            return items.Cast<ListItem>();
        }

        ///// <summary>
        ///// CAML Query
        ///// </summary>
        ///// <param name="viewAttributes">View item condition</param>
        ///// <param name="where">Selection data condition</param>
        ///// <param name="order">Order caml setting</param>
        ///// <param name="includePermissions">Permissions</param>
        ///// <param name="viewFields">Names of selected fields</param>
        ///// <param name="rowLimit">Specified row limit</param>
        ///// <returns>Return SPQuery</returns>
        //public static CamlQuery ItemsQuery(
        //    string viewAttributes = "Scope=\"Recursive\"",
        //    string where = "",
        //    string order = "",
        //    bool includePermissions = false,
        //    string viewFields = null,
        //    uint rowLimit = 0)
        //{
        //    var spQuery = new CamlQuery
        //    {
        //        ViewAttributes = viewAttributes,
        //        Query = where + order,
        //        ViewFieldsOnly = !string.IsNullOrEmpty(viewFields),
        //        ViewFields = viewFields,
        //        IncludePermissions = includePermissions
        //    };
        //    spQuery.ViewXml = viewFields;

        //    if (rowLimit > 0)
        //        spQuery.RowLimit = rowLimit;

        //    return spQuery;
        //}

        /// <summary>
        /// CAML Query for Client Model SP
        /// </summary>
        /// <param name="where">Selection data condition</param>
        /// <param name="order">Order caml setting</param>
        /// <param name="includePermissions">Permissions</param>
        /// <param name="viewFields">Names of selected fields</param>
        /// <param name="rowLimit">Specified row limit</param>
        /// <returns>Return SPQuery</returns>
        public static CamlQuery ItemsQuery(
            string where = "",
            string order = "",
            bool includePermissions = false,
            string viewFields = null,
            uint rowLimit = 0)
        {
            var queryXml = $"<Query>{where}{order}</Query>";
            var viewFieldsXml = !string.IsNullOrEmpty(viewFields) ? $"<ViewFields>{viewFields}</ViewFields>" : string.Empty;
            var rowLimitXml = rowLimit > 0 ? $"<RowLimit>{rowLimit}</RowLimit>" : string.Empty;

            var spQuery = new CamlQuery
            {
                ViewXml = $@"<View>{queryXml}{viewFieldsXml}{rowLimitXml}</View>"
            };

            return spQuery;
        }

        ///// <summary>
        ///// CAML Query for SPFolder
        ///// </summary>
        ///// <param name="spFolder">SPFolder containing necessary items</param>
        ///// <param name="viewFieldNames"></param>
        ///// <param name="camlQuery"></param>
        ///// <param name="rowLimit">Specified row limit</param>
        ///// <returns>Return SPQuery</returns>
        //private static SPQuery GetSpQuery(SPFolder spFolder, List<string> viewFieldNames, string camlQuery = "", uint rowLimit = 500)
        //{
        //    var sbViewFields = new StringBuilder();
        //    viewFieldNames.ForEach(viewFieldName => sbViewFields.AppendFormat("<FieldRef Name='{0}' />", viewFieldName));

        //    var spQuery = new SPQuery {
        //        Folder = spFolder,
        //        ViewAttributes = "Scope=\"Recursive\"",
        //        Query = string.IsNullOrEmpty(camlQuery) ? "<Where></Where>" : camlQuery,
        //        RowLimit = rowLimit,
        //        ViewFields = sbViewFields.ToString()
        //    };

        //    if (!string.IsNullOrEmpty(spQuery.ViewFields))
        //        spQuery.ViewFieldsOnly = true;

        //    return spQuery;
        //}

        public static CamlQuery Clone(this CamlQuery query)
        {
            var spQuery = new CamlQuery
            {
                ViewXml = query.ViewXml,
                FolderServerRelativePath = query.FolderServerRelativePath,
                FolderServerRelativeUrl = query.FolderServerRelativeUrl,
                AllowIncrementalResults = query.AllowIncrementalResults,
                DatesInUtc = query.DatesInUtc,
                ListItemCollectionPosition = query.ListItemCollectionPosition
            };

            return spQuery;
        }
    }
}