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
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Universe.Helpers.Extensions;

namespace Universe.Sp.Common.Caml
{
    /// <summary>
    ///     Помощник для построения запросов CAML.
    ///     Assistant for building CAML queries.
    /// <author>Alex Envision</author>
    /// </summary>
    public static class CamlHelper
    {
        /// <summary>
        ///     Строит CAML выражение: значение поля входит в указанный интервал времени
        ///     (с такого то дня (с 00.00 часов) по такой то день (по 23.59.59.999 часов) включительно).
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <param name="beginDate">
        ///     Начальная дата (если null то фильтрация не осуществляется,
        ///     работает как меньше либо равно конечной дате).
        /// </param>
        /// <param name="endDate">
        ///     Конечная дата (если null то фильтрация не осуществляется,
        ///     работает как больше либо равно начальной дате)).
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string BuildCamlPeriodFilter(string fieldName, DateTime? beginDate, DateTime? endDate)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            var beginFilter = string.Empty;
            var endFilter = string.Empty;

            if (beginDate != null && beginDate.Value != DateTime.MinValue)
                beginFilter = GetComparisonOperators(
                    ComparisonOperators.GEQ,
                    fieldName,
                    false,
                    Types.DATE_TIME,
                    SPUtility.CreateISO8601DateTimeFromSystemDateTime(
                        beginDate.Value.Date),
                    false);

            if (endDate != null && endDate.Value != DateTime.MinValue)
                endFilter = GetComparisonOperators(
                    ComparisonOperators.LT,
                    fieldName,
                    false,
                    Types.DATE_TIME,
                    SPUtility.CreateISO8601DateTimeFromSystemDateTime(
                        endDate.Value.Date.Add(new TimeSpan(1, 0, 0, 0))),
                    false);

            if (beginDate != null && endDate != null
                && beginDate.Value != DateTime.MinValue
                && endDate.Value != DateTime.MinValue)
                return CamlChain(LogicalOperators.AND, beginFilter, endFilter);

            return beginFilter + endFilter;
        }

        /// <summary>
        ///     Возвращает CAML представлении ссылок на SP поля.
        /// </summary>
        /// <param name="namesFields">
        ///     Массив из SP полей.
        /// </param>
        /// <returns>
        ///     Возвращает строку с CAML представлением фильтра.
        /// </returns>
        public static string BuildFieldsRef(params string[] namesFields)
        {
            if (namesFields == null)
                throw new ArgumentNullException(nameof(namesFields));

            return BuildFieldsRef(new List<string>(namesFields));
        }

        /// <summary>
        ///     Возвращает CAML представлении ссылок на SP поля.
        /// </summary>
        /// <param name="namesFields">
        ///     Коллекция SP полей.
        /// </param>
        /// <returns>
        ///     Возвращает строку с CAML представлением фильтра.
        /// </returns>
        public static string BuildFieldsRef(List<string> namesFields)
        {
            if (namesFields == null)
                throw new ArgumentNullException(nameof(namesFields));

            var sb = new StringBuilder();
            foreach (var param in namesFields)
            {
                sb.Append(GetFieldRef(param));
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Возвращает CAML представлении ссылок на SP поля.
        /// </summary>
        /// <param name="idsFields">
        ///     Массив из SP полей.
        /// </param>
        /// <returns>
        ///     Возвращает строку с CAML представлением фильтра.
        /// </returns>
        public static string BuildFieldsRefID(params Guid[] idsFields)
        {
            if (idsFields == null)
                throw new ArgumentNullException(nameof(idsFields));

            return BuildFieldsRefID(new List<Guid>(idsFields));
        }

        /// <summary>
        ///     Возвращает CAML представлении ссылок на SP поля.
        /// </summary>
        /// <param name="idsFields">
        ///     Коллекция SP полей.
        /// </param>
        /// <returns>
        ///     Возвращает строку с CAML представлением фильтра.
        /// </returns>
        public static string BuildFieldsRefID(List<Guid> idsFields)
        {
            if (idsFields == null)
                throw new ArgumentNullException(nameof(idsFields));

            var sb = new StringBuilder();
            foreach (var param in idsFields)
            {
                sb.Append(GetFieldRefID(param));
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Создаёт теги FieldRef с атрибутами ID и Nullable = "TRUE". 
        ///     Builds the FieldRef tags with attributes ID and Nullable="TRUE".
        /// </summary>
        /// <param name="idsFields">
        ///     Идентификаторы полей.
        ///     The fields ids.
        /// </param>
        /// <returns>
        ///     Сформированные теги FieldRef.
        ///     FieldRef tags.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     Идентификаторы полей.
        ///     Fields ids.
        /// </exception>
        public static string BuildFieldsRefIDNullable(List<Guid> idsFields)
        {
            if (idsFields == null)
                throw new ArgumentNullException(nameof(idsFields));

            var sb = new StringBuilder();
            foreach (var param in idsFields)
            {
                sb.Append(GetFieldRefIDNullable(param));
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Создаёт теги FieldRef с атрибутами ID и Nullable = "TRUE".
        ///     Builds the FieldRef tags with attributes ID and Nullable="TRUE".
        /// </summary>
        /// <param name="idsFields">
        ///     Идентификаторы полей.
        ///     The fields ids.
        /// </param>
        /// <returns>
        ///     Сформированные теги FieldRef.
        ///     FieldRef tags.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     Идентификаторы полей.
        ///     Fields ids.
        /// </exception>
        public static string BuildFieldsRefIDNullable(params Guid[] idsFields)
        {
            if (idsFields == null)
                throw new ArgumentNullException(nameof(idsFields));

            var sb = new StringBuilder();
            foreach (var param in idsFields)
            {
                sb.Append(GetFieldRefIDNullable(param));
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Создаёт теги FieldRef с атрибутами Name и Nullable = "TRUE".
        ///     Builds the FieldRef tags with attributes Name and Nullable="TRUE".
        /// </summary>
        /// <param name="namesFields">
        ///     Наименования полей.
        ///     The name fields.
        /// </param>
        /// <returns>
        ///     Сформированные теги FieldRef.
        ///     FieldRef tags.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">namesFields</exception>
        public static string BuildFieldsRefNullable(params string[] namesFields)
        {
            if (namesFields == null)
                throw new ArgumentNullException(nameof(namesFields));

            var sb = new StringBuilder();
            foreach (var param in namesFields)
            {
                sb.Append(GetFieldRefNullable(param));
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Создаёт теги FieldRef с атрибутами Name и Nullable = "TRUE".
        ///     Builds the FieldRef tags with attributes Name and Nullable="TRUE".
        /// </summary>
        /// <param name="namesFields">
        ///     Наименования полей.
        ///     The name fields.
        /// </param>
        /// <returns>
        ///     Сформированные теги FieldRef.
        ///     FieldRef tags.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">namesFields</exception>
        public static string BuildFieldsRefNullable(List<string> namesFields)
        {
            if (namesFields == null)
                throw new ArgumentNullException(nameof(namesFields));

            var sb = new StringBuilder();
            foreach (var param in namesFields)
            {
                sb.Append(GetFieldRefNullable(param));
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Возвращает CAML представление цепочки условных выражений.
        /// </summary>
        /// <param name="operation">
        ///     Тип условия (Or, And, etc.).
        /// </param>
        /// <param name="operands">
        ///     Строки с содержанием условных выражений.
        /// </param>
        /// <returns>
        ///     Возвращает строку с CAML цепочкой условных выражений.
        /// </returns>
        public static string CamlChain(string operation, params string[] operands)
        {
            if (string.IsNullOrEmpty(operation))
                throw new ArgumentNullException(nameof(operation));

            if (operands == null)
                throw new ArgumentNullException(nameof(operands));

            return CamlChain(operation, (IEnumerable<string>)operands);
        }

        /// <summary>
        ///     Возвращает CAML представление цепочки условных выражений.
        /// </summary>
        /// <param name="operation">
        ///     Тип условия (Or, And, etc.).
        /// </param>
        /// <param name="operands">
        ///     Строки с содержанием условных выражений.
        /// </param>
        /// <returns>
        ///     Возвращает строку с CAML цепочкой условных выражений.
        /// </returns>
        public static string CamlChain(string operation, IEnumerable<string> operands)
        {
            if (string.IsNullOrEmpty(operation))
                throw new ArgumentNullException(nameof(operation));

            if (operands == null)
                throw new ArgumentNullException(nameof(operands));

            var sb = new StringBuilder();

            // Строим список непустых операндов
            var notEmptyOperands = operands.Where(operand => !string.IsNullOrEmpty(operand)).ToList();

            if (notEmptyOperands.Count == 1)
            {
                sb.Append(notEmptyOperands[0]);
            }
            else
            {
                for (var i = 0; i < notEmptyOperands.Count - 1; i++)
                {
                    sb.AppendFormat("<{0}>", operation);
                }

                if (notEmptyOperands.Count > 0)
                    sb.Append(notEmptyOperands[0]);

                for (var i = 1; i < notEmptyOperands.Count; i++)
                {
                    sb.Append(notEmptyOperands[i]);
                    sb.AppendFormat("</{0}>", operation);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Строит CAML выражение: значение поля начинается с указанного значения.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <param name="value">
        ///     Значение.
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetBeginsWithText(string fieldName, string value)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            return !string.IsNullOrEmpty(value)
                ? GetComparisonOperators(ComparisonOperators.BEGINS_WITH, fieldName, false, Types.TEXT, value)
                : GetIsNullOrEmpty(fieldName);
        }

        /// <summary>
        ///     Строит CAML выражение сортировки.
        /// </summary>
        /// <param name="elements">
        ///     Элементы по которым будет осуществляться сортировка.
        /// </param>
        /// <returns>
        ///     Возвращает строку с CAML выражением сортировки.
        /// </returns>
        public static string GetCamlOrderBy(params string[] elements)
        {
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            var sb = new StringBuilder();
            foreach (var item in elements)
            {
                sb.Append(item);
            }

            return !string.IsNullOrEmpty(sb.ToString()) ? string.Format(Tags.ORDER_BY_TAG, sb) : string.Empty;
        }

        /// <summary>
        ///     Строит CAML выражение сортировки по полю.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <param name="isAscending">
        ///     Тип сортировки (true - по возрастанию, false - по убыванию).
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetCamlOrderByElement(string fieldName, bool isAscending)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            return string.Format(Tags.ORDER_BY_ELEMENT, fieldName, isAscending.ToString().ToUpper());
        }

        /// <summary>
        ///     Построение CAML FieldRef для упорядочиванию по значению поля. 
        ///     Gets the caml FieldRef for order by element.
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор поля. 
        ///     The field identifier.
        /// </param>
        /// <param name="isAscending">
        ///     Если устрановлено <c>true</c> [по-возрастанию].
        ///     if set to <c>true</c> [is ascending].
        /// </param>
        /// <returns></returns>
        public static string GetCamlOrderByElement(Guid fieldId, bool isAscending)
        {
            return string.Format(Tags.ORDER_BY_ELEMENT_ID, fieldId.ToString("B"), isAscending.ToString().ToUpper());
        }

        /// <summary>
        ///     Строит CAML выражение сортировки.
        /// </summary>
        /// <param name="elements">
        ///     Элементы по которым будет осуществляться сортировка.
        /// </param>
        /// <returns>
        ///     Возвращает строку с CAML выражением сортировки.
        /// </returns>
        public static string GetCamlOrderByOverride(params string[] elements)
        {
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            var sb = new StringBuilder();
            foreach (var item in elements)
            {
                sb.Append(item);
            }

            return !string.IsNullOrEmpty(sb.ToString()) ? string.Format(Tags.ORDER_BY_OVERRIDE_TAG, sb) : string.Empty;
        }

        /// <summary>
        ///     Строит CAML оператор where.
        /// </summary>
        /// <param name="operands">
        ///     Строка с CAML выражением условных операторов.
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetCamlWhere(string operands)
        {
            return !string.IsNullOrEmpty(operands)
                ? string.Format(Tags.WHERE, operands)
                : string.Empty;
        }

        /// <summary>
        ///     Построение разметки условного оператора where в CAML. 
        ///     Gets the caml where.
        /// </summary>
        /// <param name="operands">
        ///     Операнды.
        ///     The operands.
        /// </param>
        /// <returns></returns>
        public static string GetCamlWhere(params string[] operands)
        {
            return GetCamlWhere(CamlChain(LogicalOperators.AND, operands));
        }

        /// <summary>
        ///     Строит CAML выражение условного оператора.
        /// </summary>
        /// <param name="operator">
        ///     Оператор.
        /// </param>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <param name="isLookupId">
        ///     Является ли поле идентификатором подстановки?
        /// </param>
        /// <param name="type">
        ///     Тип поля.
        /// </param>
        /// <param name="value">
        ///     Значение.
        /// </param>
        /// <param name="isIncludeTimeValue">
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetComparisonOperators(
            string @operator,
            string fieldName,
            bool isLookupId,
            string type,
            string value,
            bool isIncludeTimeValue)
        {
            if (string.IsNullOrEmpty(@operator))
                throw new ArgumentNullException(nameof(@operator));

            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException(nameof(type));

            return string.Format(
                Tags.TAG,
                @operator,
                GetFieldRef(fieldName, isLookupId) + GetValue(type, value, isIncludeTimeValue));
        }

        /// <summary>
        ///     Построение оператора сравнения. 
        ///     Gets the comparison operator.
        /// </summary>
        /// <param name="operator">
        ///     Оператор.
        ///     The operator.
        /// </param>
        /// <param name="fieldId">
        ///     Идентификатор поля. 
        ///     The field identifier.
        /// </param>
        /// <param name="isLookupId">
        ///     Если установлено <c>true</c> [идентификатор подстановки].
        ///     if set to <c>true</c> [is lookup identifier].
        /// </param>
        /// <param name="type">
        ///     Тип.
        ///     The type.
        /// </param>
        /// <param name="value">
        ///     Значение.
        ///     The value.
        /// </param>
        /// <param name="isIncludeTimeValue">
        ///     Если установлено <c>true</c> [включение значения времени].
        ///     if set to <c>true</c> [is include time value].
        /// </param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// operator
        /// or
        /// type
        /// </exception>
        public static string GetComparisonOperators(
            string @operator,
            Guid fieldId,
            bool isLookupId,
            string type,
            string value,
            bool isIncludeTimeValue)
        {
            if (string.IsNullOrEmpty(@operator))
                throw new ArgumentNullException(nameof(@operator));

            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException(nameof(type));

            return string.Format(
                Tags.TAG,
                @operator,
                GetFieldRefID(fieldId, isLookupId) + GetValue(type, value, isIncludeTimeValue));
        }

        /// <summary>
        ///     Строит CAML выражение условного оператора.
        /// </summary>
        /// <param name="operator">
        ///     Оператор.
        /// </param>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <param name="isLookupId">
        /// </param>
        /// <param name="type">
        ///     Тип поля.
        /// </param>
        /// <param name="value">
        ///     Значение.
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetComparisonOperators(
            string @operator,
            string fieldName,
            bool isLookupId,
            string type,
            string value)
        {
            return GetComparisonOperators(@operator, fieldName, isLookupId, type, value, false);
        }

        /// <summary>
        ///     Построение оператора сравнения.
        ///     Gets the comparison operator.
        /// </summary>
        /// <param name="operator">
        ///     Оператор.
        ///     The operator.
        /// </param>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        ///     The field identifier.
        /// </param>
        /// <param name="isLookupId">
        ///     Если установлено <c>true</c> [идентификатор подстановки].
        ///     if set to <c>true</c> [is lookup identifier].
        /// </param>
        /// <param name="type">
        ///     Тип.
        ///     The type.
        /// </param>
        /// <param name="value">
        ///     Значение.
        ///     The value.
        /// </param>
        /// <returns></returns>
        public static string GetComparisonOperators(
            string @operator,
            Guid fieldId,
            bool isLookupId,
            string type,
            string value)
        {
            return GetComparisonOperators(@operator, fieldId, isLookupId, type, value, false);
        }

        /// <summary>
        ///     Построение разметки оператора ContainsText.
        ///     The get contains text.
        /// </summary>
        /// <param name="fieldName">
        ///      Наименование поля.
        ///     The field name.
        /// </param>
        /// <param name="value">
        ///     Значение.
        ///     The value.
        /// </param>
        /// <returns>
        ///     The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static string GetContainsText(string fieldName, string value)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            return !string.IsNullOrEmpty(value)
                ? GetComparisonOperators(ComparisonOperators.CONTAINS, fieldName, false, Types.TEXT, value)
                : GetIsNullOrEmpty(fieldName);
        }

        /// <summary>
        ///     Строит CAML выражение: значение поля равно указанному значению.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <param name="isLookupId">
        /// </param>
        /// <param name="type">
        ///     Тип поля.
        /// </param>
        /// <param name="value">
        ///     Значение.
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Eq")]
        public static string GetEq(string fieldName, bool isLookupId, string type, string value)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException(nameof(type));

            return GetComparisonOperators(ComparisonOperators.EQ, fieldName, isLookupId, type, value);
        }

        /// <summary>
        ///     Получает разметку оператора сравнения Eq.
        ///     Gets the eq.
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        ///     The field identifier.
        /// </param>
        /// <param name="isLookupId">
        ///     Если установлено <c>true</c> [идентификатор подстановки].
        ///     if set to <c>true</c> [is lookup identifier].
        /// </param>
        /// <param name="type">
        ///     Тип.
        ///     The type.
        /// </param>
        /// <param name="value">
        ///     Значение.
        ///     The value.
        /// </param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public static string GetEq(Guid fieldId, bool isLookupId, string type, string value)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException(nameof(type));

            return GetComparisonOperators(ComparisonOperators.EQ, fieldId, isLookupId, type, value);
        }

        /// <summary>
        ///     Получает разметку оператора сравнения Eq по типу boolean.
        ///     The get eq bool.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        ///     The field name.
        /// </param>
        /// <param name="value">
        ///     Значение.
        ///     The value.
        /// </param>
        /// <returns>
        ///     The <see cref="string"/>.
        /// </returns>
        public static string GetEqBool(string fieldName, bool value)
        {
            return GetEq(
                fieldName,
                false,
                Types.INTEGER,
                value ? 1.ToString(CultureInfo.CurrentCulture) : 0.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        ///     Строит CAML выражение: значение поля равно указанному целочисленному значению.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <param name="value">
        ///     Значение.
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetEqInteger(string fieldName, int value)
        {
            return GetEq(fieldName, false, Types.INTEGER, value.ToString());
        }

        /// <summary>
        ///     Строит CAML выражение: значение поля равно указанному целочисленному значению.
        ///     Gets the eq.
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        ///     The field identifier.
        /// </param>
        /// <param name="value">
        ///     Значение.
        ///     The value.
        /// </param>
        /// <returns></returns>
        public static string GetEqInteger(Guid fieldId, int value)
        {
            return GetEq(fieldId, false, Types.INTEGER, value.ToString());
        }

        /// <summary>
        ///     Строит CAML выражение: значение поля равно указанному значению.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <param name="value">
        ///     Значение.
        /// </param>
        /// <param name="isLookupId">
        ///     Сравнение по идентификатору.
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetEqLookup(string fieldName, SPFieldLookupValue value, bool isLookupId)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            string result;
            if (value != null)
            {
                var strValue = value.LookupValue;
                var valueType = Types.LOOKUP;
                if (isLookupId)
                {
                    valueType = Types.INTEGER;
                    strValue = value.LookupId.ToString();
                }

                result = string.Format(
                    Tags.TAG,
                    ComparisonOperators.EQ,
                    GetFieldRef(fieldName, isLookupId) + GetValue(valueType, strValue));
            }
            else
            {
                result = GetIsNullOrEmpty(fieldName);
            }

            return result;
        }

        /// <summary>
        ///     Строит CAML выражение: значение поля равно указанному значению подстановки.
        ///     Gets the eq.
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        ///     The field identifier.
        /// </param>
        /// <param name="value">
        ///     Значение.
        ///     The value.
        /// </param>
        /// <param name="isLookupId">
        ///     Если установлено <c>true</c> [идентификатор подстановки].
        ///     if set to <c>true</c> [is lookup identifier].
        /// </param>
        /// <returns></returns>
        public static string GetEqLookup(Guid fieldId, SPFieldLookupValue value, bool isLookupId)
        {
            string result;
            if (value != null)
            {
                var strValue = value.LookupValue;
                var valueType = Types.LOOKUP;
                if (isLookupId)
                {
                    valueType = Types.INTEGER;
                    strValue = value.LookupId.ToString();
                }

                result = string.Format(
                    Tags.TAG,
                    ComparisonOperators.EQ,
                    GetFieldRefID(fieldId, isLookupId) + GetValue(valueType, strValue));
            }
            else
            {
                result = GetIsNullOrEmpty(fieldId);
            }

            return result;
        }

        /// <summary>
        ///     Строит CAML выражение: значение поля равно указанному значению подстановки.
        ///     Сравнение происходит по идентификатору.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <param name="id">
        ///     Идентификатор.
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetEqLookup(string fieldName, int id)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            return GetEqLookup(fieldName, new SPFieldLookupValue(id, string.Empty), true);
        }

        /// <summary>
        ///     Строит CAML выражение: значение поля равно указанному значению.
        ///     Сравнение происходит по идентификатору.
        ///     Gets the eq.
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        ///     The field identifier.
        /// </param>
        /// <param name="id">
        ///     Идентификатор.
        ///     The identifier.
        /// </param>
        /// <returns></returns>
        public static string GetEqLookup(Guid fieldId, int id)
        {
            return GetEqLookup(fieldId, new SPFieldLookupValue(id, string.Empty), true);
        }

        /// <summary>
        ///     Строит CAML выражение: значение поля равно указанному строковому значению.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <param name="value">
        ///     Значение.
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetEqText(string fieldName, string value)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            return !string.IsNullOrEmpty(value)
                ? GetEq(fieldName, false, Types.TEXT, value)
                : GetIsNullOrEmpty(fieldName);
        }

        /// <summary>
        ///     Строит CAML выражение: значение поля равно указанному строковому значению.
        ///     Gets the eq text.
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        ///     The field identifier.
        /// </param>
        /// <param name="value">
        ///     Значение.
        ///     The value.
        /// </param>
        /// <returns></returns>
        public static string GetEqText(Guid fieldId, string value)
        {
            return !string.IsNullOrEmpty(value)
                ? GetEq(fieldId, false, Types.TEXT, value)
                : GetIsNullOrEmpty(fieldId);
        }

        /// <summary>
        ///     Строит тег FieldRef.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <param name="isLookupId">
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetFieldRef(string fieldName, bool isLookupId)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            var lookupId = string.Empty;
            if (isLookupId)
                lookupId = @" LookupId=""TRUE""";

            return string.Format(Tags.FIELD_REF_TAG, fieldName, lookupId);
        }

        /// <summary>
        ///     Строит тег FieldRef.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetFieldRef(string fieldName)
        {
            return GetFieldRef(fieldName, false);
        }

        /// <summary>
        ///     Построение тега FieldRef ID.
        ///     Gets the FieldRef ID.
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор.
        ///     The field identifier.
        /// </param>
        /// <returns></returns>
        public static string GetFieldRefID(Guid fieldId)
        {
            return GetFieldRefID(fieldId, false);
        }

        /// <summary>
        ///     Строит тег FieldRef.
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        /// </param>
        /// <param name="isLookupId"></param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetFieldRefID(Guid fieldId, bool isLookupId)
        {
            if (fieldId == Guid.Empty)
                throw new ArgumentException("fieldId");

            var lookupId = string.Empty;
            if (isLookupId)
                lookupId = @" LookupId=""TRUE""";

            return string.Format(Tags.FIELD_REF_ID_TAG, fieldId.ToString("B"), lookupId);
        }

        /// <summary>
        ///     Построение тега FieldRef ID, который может принимать нулевое значение.
        ///     Gets the FieldRef ID Nnullable="True".
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        ///     The field identifier.
        /// </param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">fieldId</exception>
        public static string GetFieldRefIDNullable(Guid fieldId)
        {
            if (fieldId == Guid.Empty)
                throw new ArgumentException("fieldId");

            return string.Format(Tags.FIELD_REF_ID_TAG_NULLABLE, fieldId.ToString("B"));
        }

        /// <summary>
        ///     Построение тега FieldRef ID, который может принимать нулевое значение.
        ///     Gets the FieldRef Name Nnullable="True".
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        ///     Name of the field.
        /// </param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">fieldName</exception>
        public static string GetFieldRefNullable(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            return string.Format(Tags.FIELD_REF_TAG_NULLABLE, fieldName);
        }

        /// <summary>
        ///     Построение разметки "меньше или равно значению" - Leq.
        ///     Get Leq.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        ///     The field name.
        /// </param>
        /// <param name="value">
        ///     Значение.
        ///     The value.
        /// </param>
        /// <param name="isIncludeTimeValue">
        ///     Если установлено <c>true</c> [включать значение времени].
        ///     if set to <c>true</c> [is include time value].
        ///     The is Include Time Value.
        /// </param>
        /// <returns>
        ///     The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static string GetGeqDateTime(string fieldName, DateTime value, bool isIncludeTimeValue)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            return GetComparisonOperators(
                ComparisonOperators.GEQ,
                fieldName,
                false,
                Types.DATE_TIME,
                SPUtility.CreateISO8601DateTimeFromSystemDateTime(value),
                isIncludeTimeValue);
        }

        /// <summary>
        ///     Построение разметки "больше значения" - Gt.
        ///     Get Gt.
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        ///     The field identifier.
        /// </param>
        /// <param name="value">
        ///     Значение.
        ///     The value.
        /// </param>
        /// <param name="isIncludeTimeValue">
        ///     Если установлено <c>true</c> [включать значение времени].
        ///     if set to <c>true</c> [is include time value].
        /// </param>
        /// <returns></returns>
        public static string GetGtDateTime(Guid fieldId, DateTime value, bool isIncludeTimeValue)
        {
            return GetComparisonOperators(
                ComparisonOperators.GT,
                fieldId,
                false,
                Types.DATE_TIME,
                SPUtility.CreateISO8601DateTimeFromSystemDateTime(value),
                isIncludeTimeValue);
        }

        /// <summary>
        ///     Построение разметки "больше значения" - Gt.
        ///     Get Gt.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        ///     Name of the field.
        /// </param>
        /// <param name="value">
        ///     Значение.
        ///     The value.
        /// </param>
        /// <param name="isIncludeTimeValue">
        ///     Если установлено <c>true</c> [включать значение времени].
        ///     if set to <c>true</c> [is include time value].
        /// </param>
        /// <returns></returns>
        public static string GetGtDateTime(string fieldName, DateTime value, bool isIncludeTimeValue)
        {
            return GetComparisonOperators(
                ComparisonOperators.GT,
                fieldName,
                false,
                Types.DATE_TIME,
                SPUtility.CreateISO8601DateTimeFromSystemDateTime(value),
                isIncludeTimeValue);
        }

        /// <summary>
        ///     Построение разметки вхождения в диапазон числовых значений.
        ///     Get In.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        ///     The field name.
        /// </param>
        /// <param name="numbers">
        ///     Числовые значения.
        ///     The numbers.
        /// </param>
        /// <returns>
        ///     The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static string GetInNumbers(string fieldName, List<int> numbers)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            if (numbers == null)
                throw new ArgumentNullException(nameof(numbers));

            if (numbers.Count > 499)
            {
                var numbersQ = new List<int>(numbers);
                var operands = new List<string>();

                while (numbersQ.Count > 0)
                {
                    var numbers2 = new List<int>();
                    var index = 0;
                    while (index < 499)
                    {
                        numbers2.Add(numbersQ[0]);
                        numbersQ.RemoveAt(0);
                        index++;

                        if (numbersQ.Count == 0)
                            break;
                    }

                    operands.Add(GetInNumbers(fieldName, numbers2));
                }

                return CamlChain(LogicalOperators.OR, operands);
            }

            var sb = new StringBuilder();
            sb.Append("<In>");
            sb.Append(GetFieldRef(fieldName, true));
            sb.Append("<Values>");
            foreach (var number in numbers)
            {
                sb.AppendFormat("<Value Type=\"Integer\">{0}</Value>", number);
            }

            sb.Append("</Values>");
            sb.Append("</In>");

            return sb.ToString();
        }

        /// <summary>
        ///     Построение разметки вхождения в диапазон числовых значений.
        ///     Get In.
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        ///     The field identifier.
        /// </param>
        /// <param name="numbers">
        ///     Числовые значения.
        ///     The numbers.
        /// </param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">numbers</exception>
        public static string GetInNumbers(Guid fieldId, List<int> numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException(nameof(numbers));

            if (numbers.Count > 499)
            {
                var numbersQ = new List<int>(numbers);
                var operands = new List<string>();

                while (numbersQ.Count > 0)
                {
                    var numbers2 = new List<int>();
                    var index = 0;
                    while (index < 499)
                    {
                        numbers2.Add(numbersQ[0]);
                        numbersQ.RemoveAt(0);
                        index++;

                        if (numbersQ.Count == 0)
                            break;
                    }

                    operands.Add(GetInNumbers(fieldId, numbers2));
                }

                return CamlChain(LogicalOperators.OR, operands);
            }

            var sb = new StringBuilder();
            sb.Append("<In>");
            sb.Append(GetFieldRefID(fieldId, true));
            sb.Append("<Values>");
            foreach (var number in numbers)
            {
                sb.AppendFormat("<Value Type=\"Integer\">{0}</Value>", number);
            }

            sb.Append("</Values>");
            sb.Append("</In>");

            return sb.ToString();
        }

        /// <summary>
        ///     Построение разметки вхождения в диапазон текстовых значений.
        ///     Get In.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        ///     The field name.
        /// </param>
        /// <param name="texts">
        ///     Текстовые значения.
        ///     The texts.
        /// </param>
        /// <returns>
        ///     The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static string GetInTexts(string fieldName, IEnumerable<string> texts)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            if (texts == null)
                throw new ArgumentNullException(nameof(texts));

            var textList = texts.Where(_ => !_.IsNullOrEmpty()).ToList();
            if (textList.Count == 0)
                throw new ArgumentException("textList.Count == 0", nameof(texts));

            if (textList.Count <= 499)
            {
                var sb = new StringBuilder();
                sb.Append("<In>");
                sb.Append(GetFieldRef(fieldName, true));
                sb.Append("<Values>");
                foreach (var text in textList)
                {
                    sb.AppendFormat("<Value Type=\"Text\">{0}</Value>", text);
                }

                sb.Append("</Values>");
                sb.Append("</In>");
                return sb.ToString();
            }

            var operands = new List<string>();
            while (textList.Count > 0)
            {
                var numbers2 = new List<string>();
                var index = 0;
                while (index < 499)
                {
                    numbers2.Add(textList[0]);
                    textList.RemoveAt(0);
                    index++;

                    if (textList.Count == 0)
                        break;
                }

                operands.Add(GetInTexts(fieldName, numbers2));
            }

            return CamlChain(LogicalOperators.OR, operands);
        }

        /// <summary>
        ///     Строит CAML выражение: проверка поля на null.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <param name="isLookupId"></param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetIsNotNull(string fieldName, bool isLookupId)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            return string.Format(Tags.TAG, ComparisonOperators.IS_NOT_NULL, GetFieldRef(fieldName, isLookupId));
        }

        /// <summary>
        ///     Строит CAML выражение: проверка поля на null.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetIsNull(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            return string.Format(Tags.TAG, ComparisonOperators.IS_NULL, GetFieldRef(fieldName));
        }

        /// <summary>
        ///     Построение разметки проверки поля на null.
        ///     Get IsNull.
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        ///     The field identifier.
        /// </param>
        /// <returns></returns>
        public static string GetIsNull(Guid fieldId)
        {
            return string.Format(Tags.TAG, ComparisonOperators.IS_NULL, GetFieldRefID(fieldId));
        }

        /// <summary>
        ///     Строит CAML выражение: проверка строкового поля на null или пустую строку.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetIsNullOrEmpty(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            return CamlChain(
                LogicalOperators.OR,
                GetIsNull(fieldName),
                string.Format(
                    Tags.TAG,
                    ComparisonOperators.EQ,
                    GetFieldRef(fieldName) + GetValue(Types.TEXT, string.Empty)));
        }

        /// <summary>
        ///     Строит CAML выражение: проверка строкового поля на null или пустую строку.
        ///     Get IsNull or Eq empty.
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        ///     The field identifier.
        /// </param>
        /// <returns></returns>
        public static string GetIsNullOrEmpty(Guid fieldId)
        {
            return CamlChain(
                LogicalOperators.OR,
                GetIsNull(fieldId),
                string.Format(
                    Tags.TAG,
                    ComparisonOperators.EQ,
                    GetFieldRefID(fieldId) + GetValue(Types.TEXT, string.Empty)));
        }

        /// <summary>
        ///     Строит CAML выражение: значение поля меньше или равно указанному значению.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <param name="value">
        ///     Значение.
        /// </param>
        /// <param name="isIncludeTimeValue">
        ///     Если установлено <c>true</c> [включать значение времени].
        ///     if set to <c>true</c> [is include time value].
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetLeqDateTime(string fieldName, DateTime value, bool isIncludeTimeValue)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            return GetComparisonOperators(
                ComparisonOperators.LEQ,
                fieldName,
                false,
                Types.DATE_TIME,
                SPUtility.CreateISO8601DateTimeFromSystemDateTime(value),
                isIncludeTimeValue);
        }

        /// <summary>
        ///     Построение разметки "меньше или равно значению" - Leq.
        ///     Get Leq.
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        ///     The field identifier.
        /// </param>
        /// <param name="value">
        ///     The value.</param>
        /// <param name="isIncludeTimeValue">
        ///     Если установлено <c>true</c> [включать значение времени].
        ///     if set to <c>true</c> [is include time value].
        /// </param>
        /// <returns></returns>
        public static string GetLeqDateTime(Guid fieldId, DateTime value, bool isIncludeTimeValue)
        {
            return GetComparisonOperators(
                ComparisonOperators.LEQ,
                fieldId,
                false,
                Types.DATE_TIME,
                SPUtility.CreateISO8601DateTimeFromSystemDateTime(value),
                isIncludeTimeValue);
        }

        /// <summary>
        ///     Построение разметки "меньше значения" - Lt.
        ///     Gets Lt.
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        ///     The field identifier.
        /// </param>
        /// <param name="value">
        ///     Значение.
        ///     The value.
        /// </param>
        /// <param name="isIncludeTimeValue">
        ///     Если установлено <c>true</c> [включать значение времени].
        ///     if set to <c>true</c> [is include time value].
        /// </param>
        /// <returns></returns>
        public static string GetLtDateTime(Guid fieldId, DateTime value, bool isIncludeTimeValue)
        {
            return GetComparisonOperators(
                ComparisonOperators.LT,
                fieldId,
                false,
                Types.DATE_TIME,
                SPUtility.CreateISO8601DateTimeFromSystemDateTime(value),
                isIncludeTimeValue);
        }

        /// <summary>
        ///     Построение разметки "меньше значения" - Lt.
        ///     Get Lt.
        /// </summary>
        /// <param name="fieldName">
        ///     Name of the field.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="isIncludeTimeValue">
        ///     Если установлено <c>true</c> [включать значение времени].
        ///     if set to <c>true</c> [is include time value].
        /// </param>
        /// <returns></returns>
        public static string GetLtDateTime(string fieldName, DateTime value, bool isIncludeTimeValue)
        {
            return GetComparisonOperators(
                ComparisonOperators.LT,
                fieldName,
                false,
                Types.DATE_TIME,
                SPUtility.CreateISO8601DateTimeFromSystemDateTime(value),
                isIncludeTimeValue);
        }

        /// <summary>
        ///     Строит CAML выражение: значение поля не равно указанному значению.
        ///     Get Neq.
        /// </summary>
        /// <param name="fieldName">
        ///     Название поля
        ///     The field name.
        /// </param>
        /// <param name="isLookupId">
        ///     Идентификатор подстановки.
        ///     The is lookup id.
        /// </param>
        /// <param name="type">
        ///     Тип.
        ///     The type.
        /// </param>
        /// <param name="value">
        ///     Значение.
        ///     The value.
        /// </param>
        /// <returns>
        ///     The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static string GetNeq(string fieldName, bool isLookupId, string type, string value)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException(nameof(type));

            return GetComparisonOperators(ComparisonOperators.NEQ, fieldName, isLookupId, type, value);
        }

        /// <summary>
        ///     Строит CAML выражение: значение поля не равно указанному значению.
        ///     Get Neq.
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        ///     The field identifier.
        /// </param>
        /// <param name="isLookupId">
        ///     Если установлено <c>true</c> [идентификатор подстановки].
        ///     if set to <c>true</c> [is lookup identifier].
        /// </param>
        /// <param name="type">
        ///     Тип.
        ///     The type.
        /// </param>
        /// <param name="value">
        ///     Значение.
        ///     The value.
        /// </param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public static string GetNeq(Guid fieldId, bool isLookupId, string type, string value)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException(nameof(type));

            return GetComparisonOperators(ComparisonOperators.NEQ, fieldId, isLookupId, type, value);
        }

        /// <summary>
        ///     Строит CAML выражение: значение поля не равно указанному значению подстановки.
        ///     Сравнение происходит по идентификатору.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <param name="id">
        ///     Идентификатор.
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetNeqLookup(string fieldName, int id)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            return GetNeqLookup(fieldName, new SPFieldLookupValue(id, string.Empty), true);
        }

        /// <summary>
        ///     Строит CAML выражение: значение поля не равно указанному значению подстановки.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        /// </param>
        /// <param name="value">
        ///     Значение.
        /// </param>
        /// <param name="isLookupId">
        ///     Сравнение по идентификатору.
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetNeqLookup(string fieldName, SPFieldLookupValue value, bool isLookupId)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            string result;
            if (value != null)
            {
                var strValue = value.LookupValue;
                var valueType = Types.LOOKUP;
                if (isLookupId)
                {
                    valueType = Types.INTEGER;
                    strValue = value.LookupId.ToString();
                }

                result = string.Format(
                    Tags.TAG,
                    ComparisonOperators.NEQ,
                    GetFieldRef(fieldName, isLookupId) + GetValue(valueType, strValue));
            }
            else
            {
                result = GetIsNullOrEmpty(fieldName);
            }

            return result;
        }

        /// <summary>
        ///     Строит CAML выражение: значение поля не равно указанному текстовому значению.
        ///     Get Neq.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        ///     The field name.
        /// </param>
        /// <param name="value">
        ///     Значение.
        ///     The value.
        /// </param>
        /// <returns>
        ///     The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static string GetNeqText(string fieldName, string value)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            return !string.IsNullOrEmpty(value)
                ? GetNeq(fieldName, false, Types.TEXT, value)
                : GetIsNullOrEmpty(fieldName);
        }

        /// <summary>
        ///     Строит CAML выражение: значение поля не равно указанному текстовому значению.
        ///     Get Neq.
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        ///     The field identifier.
        /// </param>
        /// <param name="value">
        ///     Значение.
        ///     The value.
        /// </param>
        /// <returns></returns>
        public static string GetNeqText(Guid fieldId, string value)
        {
            return !string.IsNullOrEmpty(value)
                ? GetNeq(fieldId, false, Types.TEXT, value)
                : GetIsNullOrEmpty(fieldId);
        }

        /// <summary>
        ///     Построение разметки невхождения в диапазон текстовых значений.
        ///     Get NotIncludes.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        ///     Name of the field.
        /// </param>
        /// <param name="isLookupId">
        ///     Если установлено <c>true</c> [идентификатор подстановки].
        ///     if set to <c>true</c> [is lookup identifier].
        /// </param>
        /// <param name="texts">
        ///     The texts.
        /// </param>
        /// <returns></returns>
        public static string GetNotIncludesTexts(string fieldName, bool isLookupId, IEnumerable<string> texts)
        {
            return GetNotIncludesTexts(fieldName, null, isLookupId, texts);
        }

        /// <summary>
        ///     Построение разметки невхождения в диапазон текстовых значений.
        ///     Get NotIncludes.
        /// </summary>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        ///     The field identifier.
        /// </param>
        /// <param name="isLookupId">
        ///     Если установлено <c>true</c> [идентификатор подстановки].
        ///     if set to <c>true</c> [is lookup identifier].
        /// </param>
        /// <param name="texts">
        ///     Диапазон текстовых значений.
        ///     The texts.
        /// </param>
        /// <returns></returns>
        public static string GetNotIncludesTexts(Guid fieldId, bool isLookupId, IEnumerable<string> texts)
        {
            return GetNotIncludesTexts(null, fieldId, isLookupId, texts);
        }

        /// <summary>
        ///     Построение разметки невхождения в диапазон текстовых значений.
        ///     Get NotIncludes.
        /// </summary>
        /// <param name="fieldName">
        ///     Наименование поля.
        ///     Name of the field.
        /// </param>
        /// <param name="fieldId">
        ///     Идентификатор поля.
        ///     The field identifier.
        /// </param>
        /// <param name="isLookupId">
        ///     Если установлено <c>true</c> [идентификатор подстановки].
        ///     if set to <c>true</c> [is lookup identifier].
        /// </param>
        /// <param name="texts">
        ///     Диапазон текстовых значений.
        ///     The texts.
        /// </param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// fieldName
        /// or
        /// texts
        /// </exception>
        /// <exception cref="System.ArgumentException">textList.Count == 0 - texts</exception>
        public static string GetNotIncludesTexts(string fieldName, Guid? fieldId, bool isLookupId, IEnumerable<string> texts)
        {
            if (string.IsNullOrEmpty(fieldName) && fieldId == null)
                throw new ArgumentNullException(nameof(fieldName));

            if (texts == null)
                throw new ArgumentNullException(nameof(texts));

            var textList = texts.Where(_ => !_.IsNullOrEmpty()).ToList();
            if (textList.Count == 0)
                throw new ArgumentException("textList.Count == 0", nameof(texts));

            if (textList.Count <= 499)
            {
                var sb = new StringBuilder();
                sb.Append("<NotIncludes>");
                if (fieldId != null)
                    sb.Append(GetFieldRefID(fieldId.Value, isLookupId));
                else
                    sb.Append(GetFieldRef(fieldName, isLookupId));


                foreach (var text in textList)
                {
                    sb.AppendFormat("<Value Type=\"Text\">{0}</Value>", text);
                }

                sb.Append("</NotIncludes>");
                return sb.ToString();
            }

            var operands = new List<string>();
            while (textList.Count > 0)
            {
                var numbers2 = new List<string>();
                var index = 0;
                while (index < 499)
                {
                    numbers2.Add(textList[0]);
                    textList.RemoveAt(0);
                    index++;

                    if (textList.Count == 0)
                        break;
                }

                operands.Add(GetNotIncludesTexts(fieldName, fieldId, isLookupId, numbers2));
            }

            return CamlChain(LogicalOperators.OR, operands);
        }

        /// <summary>
        ///     Строит запрос.
        ///     Get Query.
        /// </summary>
        /// <param name="where">
        ///     Разметка цепочки условных операторов.
        ///     The where.
        /// </param>
        /// <param name="orderBy">
        ///     Разметка сортировки.
        ///     The order by.
        /// </param>
        /// <returns>
        ///     The <see cref="string"/>.
        /// </returns>
        public static string GetQuery(string where, string orderBy)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(where))
                sb.Append(where);

            if (!string.IsNullOrEmpty(orderBy))
                sb.Append(orderBy);

            return string.Format(Tags.QUERY, sb);
        }

        /// <summary>
        ///     Строит тег Value.
        /// </summary>
        /// <param name="type">
        ///     Тип.
        /// </param>
        /// <param name="value">
        ///     Значение.
        /// </param>
        /// <param name="isIncludeTimeValue">
        ///     Если установлено <c>true</c> [включать значение времени].
        ///     if set to <c>true</c> [is include time value].
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetValue(string type, string value, bool isIncludeTimeValue)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException(nameof(type));

            var includeTimeValue = string.Empty;
            if (isIncludeTimeValue)
                includeTimeValue = @" IncludeTimeValue = ""TRUE""";

            return string.Format(Tags.VALUE_TAG, type, includeTimeValue, value);
        }

        /// <summary>
        ///     Строит тег Value.
        /// </summary>
        /// <param name="type">
        ///     Тип.
        /// </param>
        /// <param name="value">
        ///     Значение.
        /// </param>
        /// <returns>
        ///     CAML выражение.
        /// </returns>
        public static string GetValue(string type, string value)
        {
            return GetValue(type, value, false);
        }

        /// <summary>
        ///     Операторы сравнения.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ComparisonOperators
        {
            /// <summary>
            ///     BeginsWith.
            /// </summary>
            public const string BEGINS_WITH = "BeginsWith";

            /// <summary>
            ///     Contains.
            /// </summary>
            public const string CONTAINS = "Contains";

            /// <summary>
            ///     DateRangesOverlap.
            /// </summary>
            public const string DATE_RANGES_OVERLAP = "DateRangesOverlap";

            /// <summary>
            ///     Eq.
            /// </summary>
            public const string EQ = "Eq";

            /// <summary>
            ///     Geq.
            /// </summary>
            public const string GEQ = "Geq";

            /// <summary>
            ///     Gt.
            /// </summary>
            public const string GT = "Gt";

            /// <summary>
            ///     In.
            /// </summary>
            public const string IN = "In";

            /// <summary>
            ///     Includes.
            /// </summary>
            public const string INCLUDES = "Includes";

            /// <summary>
            ///     IsNotNull.
            /// </summary>
            public const string IS_NOT_NULL = "IsNotNull";

            /// <summary>
            ///     IsNull.
            /// </summary>
            public const string IS_NULL = "IsNull";

            /// <summary>
            ///     Leq.
            /// </summary>
            public const string LEQ = "Leq";

            /// <summary>
            ///     Lt.
            /// </summary>
            public const string LT = "Lt";

            /// <summary>
            ///     Neq.
            /// </summary>
            public const string NEQ = "Neq";

            /// <summary>
            ///     NotIncludes.
            /// </summary>
            public const string NOT_INCLUDES = "NotIncludes";
        }

        /// <summary>
        ///     Логические операторы.
        /// </summary>
        public static class LogicalOperators
        {
            /// <summary>
            ///     And.
            /// </summary>
            public const string AND = "And";

            /// <summary>
            ///     Or.
            /// </summary>
            public const string OR = "Or";
        }

        /// <summary>
        ///     Теги используемые в CAML выражениях.
        /// </summary>
        public static class Tags
        {
            /// <summary>
            /// <![CDATA[<FieldRef ID=""{0}""/>]]>.
            /// </summary>
            public const string FIELD_REF_ID_TAG = @"<FieldRef ID=""{0}""{1}/>";

            /// <summary>
            /// <![CDATA[<FieldRef ID=""{0}"" Nullable=""TRUE""/>]]>.
            /// </summary>
            public const string FIELD_REF_ID_TAG_NULLABLE = @"<FieldRef ID=""{0}"" Nullable=""TRUE""/>";

            /// <summary>
            /// <![CDATA[<FieldRef Name=""{0}"" Nullable=""TRUE""/>]]>.
            /// </summary>
            public const string FIELD_REF_TAG_NULLABLE = @"<FieldRef Name=""{0}"" Nullable=""TRUE""/>";

            /// <summary>
            /// <![CDATA[<FieldRef Name=""{0}""{1}/>]]>.
            /// </summary>
            public const string FIELD_REF_TAG = @"<FieldRef Name=""{0}""{1}/>";

            /// <summary>
            /// <![CDATA[<FieldRef Ascending =""{1}"" Name = ""{0}""/>]]>.
            /// </summary>
            public const string ORDER_BY_ELEMENT = @"<FieldRef Ascending=""{1}"" Name=""{0}""/>";

            /// <summary>
            /// <![CDATA[<FieldRef Ascending=""{1}"" ID=""{0}""/>]]>.
            /// </summary>
            public const string ORDER_BY_ELEMENT_ID = @"<FieldRef Ascending=""{1}"" ID=""{0}""/>";

            /// <summary>
            /// <![CDATA[<OrderBy>{0}</OrderBy>]]>.
            /// </summary>
            public const string ORDER_BY_OVERRIDE_TAG = @"<OrderBy Override=""TRUE"">{0}</OrderBy>";

            /// <summary>
            /// <![CDATA[<OrderBy Override="TRUE">{0}</OrderBy>]]>.
            /// </summary>
            public const string ORDER_BY_TAG = @"<OrderBy>{0}</OrderBy>";

            /// <summary>
            /// <![CDATA[<Query>{0}</Query>]]>.
            /// </summary>
            public const string QUERY = @"<Query>{0}</Query>";

            /// <summary>
            /// <![CDATA[<{0}>{1}</{0}>]]>.
            /// </summary>
            public const string TAG = @"<{0}>{1}</{0}>";

            /// <summary>
            /// <![CDATA[<Value Type=""{0}"">{1}</Value>]]>.
            /// </summary>
            public const string VALUE_TAG = @"<Value Type=""{0}""{1}>{2}</Value>";

            /// <summary>
            /// <![CDATA[<Where>{0}</Where>]]>.
            /// </summary>
            public const string WHERE = @"<Where>{0}</Where>";
        }

        /// <summary>
        ///     Типы <c>SharePoint</c>.
        /// </summary>
        public static class Types
        {
            /// <summary>
            ///     Boolean.
            /// </summary>
            public const string BOOLEAN = "Boolean";

            /// <summary>
            ///     ContentTypeId.
            /// </summary>
            public const string CONTENT_TYPE_ID = "ContentTypeId";

            /// <summary>
            ///     Counter.
            /// </summary>
            public const string COUNTER = "Counter";

            /// <summary>
            ///     Currency.
            /// </summary>
            public const string CURRENCY = "Currency";

            /// <summary>
            ///     <c>DateTime</c>.
            /// </summary>
            public const string DATE_TIME = "DateTime";

            /// <summary>
            ///     Guid.
            /// </summary>
            public const string GUID = "Guid";

            /// <summary>
            ///     Integer.
            /// </summary>
            public const string INTEGER = "Integer";

            /// <summary>
            ///     Lookup.
            /// </summary>
            public const string LOOKUP = "Lookup";

            /// <summary>
            ///     Note.
            /// </summary>
            public const string NOTE = "Note";

            /// <summary>
            ///     Number.
            /// </summary>
            public const string NUMBER = "Number";

            /// <summary>
            ///     Text.
            /// </summary>
            public const string TEXT = "Text";

            /// <summary>
            ///     User.
            /// </summary>
            public const string USER = "User";
        }
    }
}