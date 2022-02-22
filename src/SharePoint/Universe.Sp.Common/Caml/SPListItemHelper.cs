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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Microsoft.SharePoint;
using Universe.Helpers.Extensions;

namespace Universe.Sp.Common.Caml
{
    /// <summary>
    ///     Методы получения типизированных значений из <see cref="SPListItem"/>.
    ///     Methods for get typed value from <see cref="SPListItem"/>.
    /// <author>Alex Envision</author>
    /// </summary>
    public static class SpListItemHelper
    {
        /// <summary>
        ///     SPFieldMultiChoiceValue the enumerable.
        /// </summary>
        /// <param name="multiChoiceValue">The multi choice value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">multiChoiceValue</exception>
        public static IEnumerable<string> AsEnumerable(this SPFieldMultiChoiceValue multiChoiceValue)
        {
            if (multiChoiceValue == null)
                throw new ArgumentNullException(nameof(multiChoiceValue));

            for (var i = 0; i < multiChoiceValue.Count; i++)
            {
                yield return multiChoiceValue[i];
            }
        }

        /// <summary>
        /// The get bool.
        /// </summary>
        /// <param name="spItem">
        /// The sp list item.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see cref="T:bool?"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",
             MessageId = "System.Convert.ToBoolean(System.Object)")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static bool? GetBool(this SPItem spItem, string fieldName)
        {
            if (spItem == null)
                throw new ArgumentNullException(nameof(spItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            var b = spItem[fieldName];
            if (b == null)
                return null;

            if (b is bool)
                return (bool)b;

            return Convert.ToBoolean(b);
        }

        /// <summary>
        /// The get bool.
        /// </summary>
        /// <param name="spItem">
        /// The sp list item.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <param name="defaultIfIsNull">
        /// The default if is null.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bool")]
        public static bool GetBool(this SPItem spItem, string fieldName, bool defaultIfIsNull)
        {
            if (spItem == null)
                throw new ArgumentNullException(nameof(spItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            var b = spItem.GetBool(fieldName);
            if (b == null)
                return defaultIfIsNull;

            return b.Value;
        }

        /// <summary>
        /// Gets the date time.
        /// </summary>
        /// <param name="spListItem">The sp list item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// spListItem
        /// or
        /// fieldName
        /// </exception>
        public static DateTime GetDateTime(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            var d = spListItem.GetDateTimeNullable(fieldName);
            return d ?? DateTime.MinValue;
        }

        /// <summary>
        /// Gets the date time.
        /// </summary>
        /// <param name="spListItem">The sp list item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="formatProvider">DateTime Culture and format</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// spListItem
        /// or
        /// fieldName
        /// </exception>
        public static DateTime GetDateTime(this SPListItem spListItem, string fieldName, IFormatProvider formatProvider)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            var d = spListItem.GetDateTimeNullable(fieldName, formatProvider);
            return d ?? DateTime.MinValue;
        }

        /// <summary>
        /// Gets the date time nullable.
        /// </summary>
        /// <param name="spListItem">The sp list item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// spListItem
        /// or
        /// fieldName
        /// </exception>
        public static DateTime? GetDateTimeNullable(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            var d = spListItem[fieldName];
            if (d == null)
                return null;

            if (d is DateTime)
                return (DateTime)d;

            return Convert.ToDateTime(d, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Gets the date time nullable.
        /// </summary>
        /// <param name="spListItem">The sp list item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="formatProvider">DateTime Culture and format</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// spListItem
        /// or
        /// fieldName
        /// </exception>
        public static DateTime? GetDateTimeNullable(this SPListItem spListItem, string fieldName, IFormatProvider formatProvider)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            var d = spListItem[fieldName];
            if (d == null)
                return null;

            if (d is DateTime)
                return (DateTime)d;

            return Convert.ToDateTime(d, formatProvider);
        }

        /// <summary>
        /// The get date time offset.
        /// </summary>
        /// <param name="spListItem">
        /// The sp list item.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see cref="DateTimeOffset"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static DateTimeOffset GetDateTimeOffset(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            var d = spListItem.GetDateTimeOffsetNullable(fieldName);
            if (d == null)
                return DateTimeOffset.MinValue;

            return d.Value;
        }

        /// <summary>
        /// The get date time offset nullable.
        /// </summary>
        /// <param name="spListItem">
        /// The sp list item.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see cref="T:DateTimeOffset?"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static DateTimeOffset? GetDateTimeOffsetNullable(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            var d = spListItem[fieldName];
            if (d == null)
                return null;

            if (d is DateTime)
                return new DateTimeOffset((DateTime)d);

            return new DateTimeOffset(Convert.ToDateTime(d, CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// The get date time or null.
        /// </summary>
        /// <param name="dateTimeOffset">
        /// The date time offset.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static DateTime? GetDateTimeOrNull(this DateTimeOffset? dateTimeOffset)
        {
            return dateTimeOffset?.LocalDateTime;
        }

        /// <summary>
        /// The get decimal.
        /// </summary>
        /// <param name="spListItem">
        /// The sp list item.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static decimal GetDecimal(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            var i = spListItem.GetDecimalNullable(fieldName);
            if (i != null)
                return i.Value;

            return 0;
        }

        /// <summary>
        /// The get decimal nullable.
        /// </summary>
        /// <param name="spListItem">
        /// The sp list item.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see cref="T:decimal?"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",
             MessageId = "System.Convert.ToDecimal(System.Object)")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static decimal? GetDecimalNullable(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            if (spListItem[fieldName] != null)
                return Convert.ToDecimal(spListItem[fieldName]);

            return null;
        }

        /// <summary>
        /// Gets the guarant user value collection.
        /// </summary>
        /// <param name="spListItem">The sp list item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// spListItem
        /// or
        /// fieldName
        /// </exception>
        /// <exception cref="System.NotSupportedException"></exception>
        public static SPFieldUserValueCollection GetGuarantUserValueCollection(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            if (spListItem[fieldName] != null)
            {
                if (spListItem[fieldName] is SPFieldUserValueCollection)
                    return (SPFieldUserValueCollection)spListItem[fieldName];

                if (spListItem[fieldName] is string)
                    return new SPFieldUserValueCollection(spListItem.Web, (string)spListItem[fieldName]);

                if (spListItem[fieldName] is SPFieldUserValue)
                {
                    var userValue = (SPFieldUserValue)spListItem[fieldName];

                    var userValueCollection = new SPFieldUserValueCollection {
                        userValue
                    };

                    return userValueCollection;
                }

                throw new NotSupportedException();
            }

            return null;
        }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <param name="spListItem">The sp list item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// spListItem
        /// or
        /// fieldName
        /// </exception>
        public static Guid? GetGuid(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            var v = spListItem[fieldName];
            if (v == null)
                return null;

            if (v is Guid)
                return (Guid)v;

            var vStr = v as string;
            if (vStr == null)
                vStr = v.ToString();

            if (vStr.IsNullOrEmpty())
                return null;

            return new Guid(vStr);
        }

        /// <summary>
        /// The get int 32.
        /// </summary>
        /// <param name="spListItem">
        /// The sp list item.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static int GetInt32(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            var i = spListItem.GetInt32Nullable(fieldName);
            if (i != null)
                return i.Value;

            return 0;
        }

        /// <summary>
        /// The get int 32 nullable.
        /// </summary>
        /// <param name="spListItem">
        /// The sp list item.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see cref="T:int?"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",
             MessageId = "System.Convert.ToInt32(System.Object)")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static int? GetInt32Nullable(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            if (spListItem[fieldName] != null)
                return Convert.ToInt32(spListItem[fieldName]);

            return null;
        }

        /// <summary>
        /// The get double.
        /// </summary>
        /// <param name="spListItem">
        /// The sp list item.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static double GetDouble(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            var i = spListItem.GetDoubleNullable(fieldName);
            if (i != null)
                return i.Value;

            return 0;
        }

        /// <summary>
        /// The get double nullable.
        /// </summary>
        /// <param name="spListItem">
        /// The sp list item.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see cref="T:double?"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",
            MessageId = "System.Convert.ToInt32(System.Object)")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static double? GetDoubleNullable(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            if (spListItem[fieldName] != null)
                return Convert.ToDouble(spListItem[fieldName]);

            return null;
        }

        /// <summary>
        /// The get item by id with fields.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="throwErrorIfIsNotFound">
        /// The throw error if is not found.
        /// </param>
        /// <param name="fieldNames">
        /// The field names.
        /// </param>
        /// <returns>
        /// The <see cref="SPListItem"/>.
        /// </returns>
        public static SPListItem GetItemByIdWithFields(this SPList list, int id, bool throwErrorIfIsNotFound, params string[] fieldNames)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            var forBuildFieldsRef = fieldNames;
            if (forBuildFieldsRef == null || forBuildFieldsRef.Length == 0)
                forBuildFieldsRef = new[] {
                    ListInfo.BaseListInfo.Fields.ID,
                    ListInfo.BaseListInfo.Fields.Title
                };

            var spQuery = new SPQuery {
                ViewFields = CamlHelper.BuildFieldsRef(forBuildFieldsRef),
                ViewFieldsOnly = true,
                ViewAttributes = "Scope=\"Recursive\"",
                Query = CamlHelper.GetCamlWhere(
                    CamlHelper.GetEqInteger(ListInfo.BaseListInfo.Fields.ID, id))
            };

            var spItems = list.GetItems(spQuery);

            if (spItems.Count == 0 && !throwErrorIfIsNotFound)
                return null;

            if (spItems.Count == 0 && throwErrorIfIsNotFound)
                throw new Exception("Элемент с ИД={0} не найден в списке {1}.".F(id, list.Title));

            return spItems[0];
        }

        /// <summary>
        /// The get lookup value.
        /// </summary>
        /// <param name="spListItem">
        /// The sp list item.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see cref="SPFieldLookupValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static SPFieldLookupValue GetLookupValue(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            var v = spListItem[fieldName];
            if (v is SPFieldLookupValue)
                return (SPFieldLookupValue)v;

            if (v != null)
                return new SPFieldLookupValue(v.ToString());

            return null;
        }

        /// <summary>
        /// Gets the lookup value.
        /// </summary>
        /// <param name="spItem">The sp item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// spItem
        /// or
        /// fieldName
        /// </exception>
        public static SPFieldLookupValue GetLookupValue(this SPItem spItem, string fieldName)
        {
            if (spItem == null)
                throw new ArgumentNullException(nameof(spItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            var v = spItem[fieldName];
            if (v is SPFieldLookupValue)
                return (SPFieldLookupValue)v;

            if (v != null)
                return new SPFieldLookupValue(v.ToString());

            return null;
        }

        /// <summary>
        /// Gets lookup value collection from list item field.
        /// </summary>
        /// <param name="spListItem">
        /// The list item.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see cref="SPFieldLookupValueCollection"/>.
        /// </returns>
        public static SPFieldLookupValueCollection GetLookupValueCollection(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            var v = spListItem[fieldName];
            var collection = v as SPFieldLookupValueCollection;
            if (collection != null)
                return collection;

            var fieldLookupValue = v as SPFieldLookupValue;
            if (fieldLookupValue != null)
                return new SPFieldLookupValueCollection {
                    fieldLookupValue
                };

            if (v != null)
                return new SPFieldLookupValueCollection(v.ToString());

            return null;
        }

        /// <summary>
        /// Gets lookup value generic list from list item field.
        /// </summary>
        /// <param name="spListItem">
        /// The list item.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// Always return generic list.
        /// </returns>
        public static List<SPFieldLookupValue> GetLookupValueList(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(fieldName));

            return spListItem.GetLookupValueCollection(fieldName)?.ToList() ?? new List<SPFieldLookupValue>();
        }

        /// <summary>
        /// Gets the multi choice value.
        /// </summary>
        /// <param name="spListItem">The sp list item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// spListItem
        /// or
        /// fieldName
        /// </exception>
        /// <exception cref="System.NotSupportedException"></exception>
        public static SPFieldMultiChoiceValue GetMultiChoiceValue(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            var valueObj = spListItem[fieldName];
            if (valueObj != null)
            {
                if (valueObj is SPFieldMultiChoiceValue)
                    return (SPFieldMultiChoiceValue)valueObj;

                if (valueObj is string)
                    return new SPFieldMultiChoiceValue(valueObj as string);

                throw new NotSupportedException();
            }

            return null;
        }

        /// <summary>
        /// The get string.
        /// </summary>
        /// <param name="spListItem">
        /// The sp list item.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static string GetString(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            var s = spListItem[fieldName];
            if (s == null)
                return null;

            if (s is string)
                return (string)s;

            return s.ToString();
        }       

        /// <summary>
        /// The get user value.
        /// </summary>
        /// <param name="spListItem">
        /// The sp list item.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see cref="SPFieldUserValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static SPFieldUserValue GetUserValue(this SPItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            if (spListItem[fieldName] != null)
            {
                if (spListItem[fieldName] is SPFieldUserValue)
                    return (SPFieldUserValue)spListItem[fieldName];

                if (spListItem[fieldName] is string)
                {
                    var userField = (SPFieldUser)spListItem.Fields.GetFieldByInternalName(fieldName);
                    var userFieldValue = (SPFieldUserValue)userField.GetFieldValue(spListItem[fieldName].ToString());
                    return userFieldValue;
                }

                throw new NotImplementedException("Поддерживаются только поля-лукапы пользователей!");
            }

            return null;
        }

        /// <summary>
        /// The get user value collection.
        /// </summary>
        /// <param name="spListItem">
        /// The sp list item.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see cref="SPFieldUserValueCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static SPFieldUserValueCollection GetUserValueCollection(this SPListItem spListItem, string fieldName)
        {
            if (spListItem == null)
                throw new ArgumentNullException(nameof(spListItem));

            if (fieldName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fieldName));

            if (spListItem[fieldName] != null)
            {
                if (spListItem[fieldName] is SPFieldUserValueCollection)
                    return (SPFieldUserValueCollection)spListItem[fieldName];

                if (spListItem[fieldName] is string)
                    return new SPFieldUserValueCollection(spListItem.Web, (string)spListItem[fieldName]);

                throw new NotImplementedException("Поддерживаются только поля-мультилукапы пользователей!");
            }

            return null;
        }

        /// <summary>
        /// Get user value list.
        /// </summary>
        /// <param name="spListItem">The sp list item.</param>
        /// <param name="fieldName">The field name.</param>
        /// <returns> Generic list of SPFieldUserValue.</returns>
        public static List<SPFieldUserValue> GetUserValueList(this SPListItem spListItem, string fieldName)
        {
            var v = spListItem.GetUserValueCollection(fieldName);

            return v?.GetRange(0, v.Count) ?? new List<SPFieldUserValue>();
        }

        /// <summary>
        /// Gets the name of the value by internal.
        /// </summary>
        /// <param name="spListItem">The sp list item.</param>
        /// <param name="internalName">Name of the internal.</param>
        /// <returns></returns>
        public static object GetValueByInternalName(this SPListItem spListItem, string internalName)
        {
            if (spListItem.Fields.ContainsField(internalName))
            {
                var field = spListItem.Fields.GetFieldByInternalName(internalName);
                return spListItem[field.Id];
            }

            return null;
        }
    }

    /// <summary>
    /// Информация о списках.
    /// </summary>
    public static class ListInfo
    {
        /// <summary>
        /// Общая информация о списках.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class BaseListInfo
        {
            /// <summary>
            /// Поля списка.
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
            public static class Fields
            {
                /// <summary>
                /// Поле списка: Вложения.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string Attachments = "Attachments";

                /// <summary>
                /// Поле списка: Кем создано.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string Author = "Author";

                /// <summary>
                /// Поле списка: Имя файла.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string BaseName = "BaseName";

                /// <summary>
                /// Поле списка: Тип контента.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string ContentType = "ContentType";

                /// <summary>
                /// Поле списка: Идентификатор типа контента.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string ContentTypeId = "ContentTypeId";

                /// <summary>
                /// Поле списка: Создан.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string Created = "Created";

                /// <summary>
                /// Поле списка: Создан.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string Created_x0020_Date = "Created_x0020_Date";

                /// <summary>
                /// Поле списка: Тип.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string DocIcon = "DocIcon";

                /// <summary>
                /// Поле списка: Изменить.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string Edit = "Edit";

                /// <summary>
                /// Поле списка: кем изменено.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string Editor = "Editor";

                /// <summary>
                /// Поле списка: Зашифрованный абсолютный URL.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string EncodedAbsUrl = "EncodedAbsUrl";

                /// <summary>
                /// Поле списка: Тип элемента.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string FSObjType = "FSObjType";

                /// <summary>
                /// Поле списка: Путь.
                /// </summary>
                [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dir")]
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string FileDirRef = "FileDirRef";

                /// <summary>
                /// Поле списка: Имя.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string FileLeafRef = "FileLeafRef";

                /// <summary>
                /// Поле списка: Путь URL-адреса.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string FileRef = "FileRef";

                /// <summary>
                /// Поле списка: Тип файла.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string File_x0020_Type = "File_x0020_Type";

                /// <summary>
                /// Поле списка: Число дочерних папок.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string FolderChildCount = "FolderChildCount";

                /// <summary>
                /// Поле списка: Идентификатор GUID.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string GUID = "GUID";

                /// <summary>
                /// Поле списка: Тип HTML-файла.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string HTML_x0020_File_x0020_Type = "HTML_x0020_File_x0020_Type";

                /// <summary>
                /// Поле списка: ИД.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string ID = "ID";

                /// <summary>
                /// Поле списка: Идентификатор экземпляра.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string InstanceID = "InstanceID";

                /// <summary>
                /// Поле списка: Число дочерних элементов.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string ItemChildCount = "ItemChildCount";

                /// <summary>
                /// Поле списка: Изменен.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string Last_x0020_Modified = "Last_x0020_Modified";

                /// <summary>
                /// Поле списка: Имя.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly",
                     MessageId = "Filename")]
                public static string LinkFilename = "LinkFilename";

                /// <summary>
                /// Поле списка: Имя.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly",
                     MessageId = "Filename")]
                public static string LinkFilename2 = "LinkFilename2";

                /// <summary>
                /// Поле списка: Имя.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly",
                     MessageId = "Filename")]
                public static string LinkFilenameNoMenu = "LinkFilenameNoMenu";

                /// <summary>
                /// Поле списка: Название.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string LinkTitle = "LinkTitle";

                /// <summary>
                /// Поле списка: Название.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string LinkTitle2 = "LinkTitle2";

                /// <summary>
                /// Поле списка: Название.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string LinkTitleNoMenu = "LinkTitleNoMenu";

                /// <summary>
                /// Поле списка: Контейнер свойств.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string MetaInfo = "MetaInfo";

                /// <summary>
                /// Поле списка: Изменен.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string Modified = "Modified";

                /// <summary>
                /// Поле списка: Порядок.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string Order = "Order";

                /// <summary>
                /// Поле списка: Маска эффективных разрешений.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string PermMask = "PermMask";

                /// <summary>
                /// Поле списка: ProgId.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string ProgId = "ProgId";

                /// <summary>
                /// Поле списка: ScopeId.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string ScopeId = "ScopeId";

                /// <summary>
                /// Поле списка: Выбрать.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string SelectTitle = "SelectTitle";

                /// <summary>
                /// Поле списка: Относительный URL-адрес сервера.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string ServerUrl = "ServerUrl";

                /// <summary>
                /// Поле списка: Тип сортировки.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string SortBehavior = "SortBehavior";

                /// <summary>
                /// Поле списка: Идентификатор клиента.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string SyncClientId = "SyncClientId";

                /// <summary>
                /// Поле списка: Название.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string Title = "Title";

                /// <summary>
                /// Поле списка: Уникальный идентификатор.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string UniqueId = "UniqueId";

                /// <summary>
                /// Поле списка: Идентификатор экземпляра рабочего процесса.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string WorkflowInstanceID = "WorkflowInstanceID";

                /// <summary>
                /// Поле списка: Версия рабочего процесса.
                /// </summary>
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string WorkflowVersion = "WorkflowVersion";

                /// <summary>
                /// Поле списка: owshiddenversion.
                /// </summary>
                [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly",
                     MessageId = "owshiddenversion")]
                [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
                public static string owshiddenversion = "owshiddenversion";
            }
        }
    }
}