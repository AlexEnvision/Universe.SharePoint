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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Universe.Helpers.Extensions;
using Universe.Sp.CQRS.Dal.Base;
using Universe.Sp.CQRS.Dal.MetaInfo;
using Universe.Sp.CQRS.Models;

namespace Universe.Sp.CQRS.Extensions
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    internal static class EntitySpQueryExtensions
    {
        private static bool CanIgnore(PropertyInfo propDto) =>
            propDto.PropertyType == typeof(List<>) ||
            propDto.PropertyType == typeof(IList<>);

        /// <summary>
        /// Создает метаинформацию на основе типа
        /// </summary>
        /// <typeparam name="TEntitySp"></typeparam>
        public static QueryableMetaInfo<TEntitySp> CreateDbRequestMetaInfo<TEntitySp>(
            this QueryBuilder<TEntitySp> query,
            Dictionary<string, Expression<Func<TEntitySp, object>>> fieldMap,
            bool disablePropsMiSearch = false)
            where TEntitySp : class
        {
            QueryableMetaInfo<TEntitySp> metainfo = query.CreateQueryableMetaInfo(
                Activator.CreateInstance(typeof(TEntitySp)),
                typeof(TEntitySp).Name);

            var properties = typeof(TEntitySp)
                .GetProperties();

            var propertiesDtoEntity = properties.GroupBy(g => g.Name).ToDictionary(g => g.Key, g => g.ToList());

            //Регистрация кастомной метаинформации вне зависимости от полей входящей сущности,
            //а также регистрация ключей с маленькой буквы
            if (fieldMap != null && fieldMap.Count != 0)
                foreach (var kvp in fieldMap)
                {
                    var name = kvp.Key;
                    var field = kvp.Value;
                    metainfo.AddField(name, field, name);
                    metainfo.AddField(name.FirstLetterToLower(), field, name.FirstLetterToLower());
                }

            if (!disablePropsMiSearch)
                foreach (var kvp in propertiesDtoEntity)
                {
                    var propDtoTypeList = kvp.Value;
                    foreach (var propDto in propDtoTypeList)
                    {
                        var name = kvp.Key;

                        if (CanIgnore(propDto))
                            continue;

                        // Игнорирование базовах классов при совпадении имен
                        if (propDtoTypeList.Count > 1 && propDto.DeclaringType != typeof(TEntitySp))
                            continue;

                        if (fieldMap != null &&
                            fieldMap.TryGetValue(name, out var field))
                            continue;

                        var newExpression = ExpressionExtensions.CreateExpressionDbeUniversal<TEntitySp>(name);
                        if (newExpression != null)
                        {
                            metainfo.AddField(name, newExpression, name);
                            metainfo.AddField(name.FirstLetterToLower(), newExpression, name.FirstLetterToLower());
                        }
                    }
                }

            metainfo.BuildMetaInfo();
            return metainfo;
        }

        private static Expression<Func<object, object>> CreateExpression(Type entityType, string propertyName)
        {
            var param = Expression.Parameter(typeof(object), "e");
            Expression body = Expression.PropertyOrField(Expression.TypeAs(param, entityType), propertyName);
            var getterExpression = Expression.Lambda<Func<object, object>>(body, param);
            return getterExpression;
        }
    }
}