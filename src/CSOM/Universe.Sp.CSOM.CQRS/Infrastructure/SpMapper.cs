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
using System.Reflection;
using iSys.Chatbot.Tools.Caml;
using Microsoft.SharePoint.Client;
using Universe.Helpers.Extensions;
using Universe.Sp.CSOM.CQRS.Models.Filter;

namespace Universe.Sp.CSOM.CQRS.Infrastructure
{
    public class SpMapper
    {
        private List<PropertyInfo> GetProperties<TEntity>() where TEntity : class, new()
        {
            var type = typeof(TEntity);
            return GetProperties(type);
        }

        private List<PropertyInfo> GetProperties(Type incomingtype)
        {
            var type = incomingtype;

            var properties = type.GetProperties(
                BindingFlags.Public | BindingFlags.Instance
                                    | BindingFlags.GetProperty | BindingFlags.SetProperty);


            var q = properties.ToList();
            q = q.Where(a => a.PropertyType.Name != "SPListItem" && a.Name != "Id" && a.Name != "ListUrl" && a.Name != "owshiddenversion").ToList();

            return q.ToList();
        }

        public void Map<TEntity>(TEntity entitySp, ListItem item) where TEntity : class, new()
        {
            var properties = GetProperties<TEntity>();

            foreach (PropertyInfo propertyInfo in properties)
            {
                var name = propertyInfo.Name;
                var value = propertyInfo.GetValue(entitySp);

                item[name] = value;
            }
        }

        public TEntity ReverseMap<TEntity>(ListItem item, TEntity entitySp) where TEntity : class, new()
        {
            var properties = GetProperties<TEntity>();

            foreach (PropertyInfo propertyInfo in properties)
            {
                var resolvedValue = ResolveColumnValue(propertyInfo, item);
                propertyInfo.SetValue(entitySp, resolvedValue);
            }

            return entitySp;
        }

        public TEntity SafeReverseMap<TEntity>(ListItem item, TEntity entitySp) where TEntity : class, new()
        {
            var properties = GetProperties<TEntity>();

            foreach (PropertyInfo propertyInfo in properties)
            {
                var resolvedValue = SafeResolveColumnValue(propertyInfo, item);
                if (resolvedValue != null)
                {
                    propertyInfo.SetValue(entitySp, resolvedValue);
                }
            }

            return entitySp;
        }

        public TEntity SafeReverseMap<TEntity>(
            FieldMapContainer<TEntity> metaContainer,
            ListItem item,
            TEntity entitySp) where TEntity : class, new()
        {
            var properties = GetProperties<TEntity>();

            foreach (PropertyInfo propertyInfo in properties)
            {
                var resolvedValue = SafeResolveColumnValue(propertyInfo, item);
                if (resolvedValue != null)
                {
                    propertyInfo.SetValue(entitySp, resolvedValue);
                }
                else
                {
                    
                }
            }

            return entitySp;
        }

        private object SafeResolveColumnValue(PropertyInfo propertyInfo, ListItem item)
        {
            try
            {
                return ResolveColumnValue(propertyInfo, item);
            }
            catch (Exception ex)
            {
                //ignored
            }

            return null;
        }

        private object ResolveColumnValue(PropertyInfo propertyInfo, ListItem item)
        {
            var fieldName = propertyInfo.Name;
            var propertyTypeName = propertyInfo.PropertyType.Name;
            var propertyTypeNameForCompare = propertyTypeName.PrepareToCompare();

            if (propertyTypeNameForCompare == typeof(bool).Name.PrepareToCompare())
            {
                return item.GetBool(fieldName);
            }

            if (propertyTypeNameForCompare == typeof(int).Name.PrepareToCompare() ||
                propertyTypeNameForCompare == typeof(long).Name.PrepareToCompare())
            {
                return item.GetInt32(fieldName);
            }

            if (propertyTypeNameForCompare == typeof(int?).Name.PrepareToCompare() ||
                propertyTypeNameForCompare == typeof(long?).Name.PrepareToCompare())
            {
                return item.GetInt32Nullable(fieldName);
            }

            if (propertyTypeNameForCompare == typeof(double).Name.PrepareToCompare())
            {
                return item.GetDouble(fieldName);
            }

            if (propertyTypeNameForCompare == typeof(double?).Name.PrepareToCompare())
            {
                return item.GetDoubleNullable(fieldName);
            }

            if (propertyTypeNameForCompare == typeof(decimal).Name.PrepareToCompare())
            {
                return item.GetDecimal(fieldName);
            }

            if (propertyTypeNameForCompare == typeof(decimal?).Name.PrepareToCompare())
            {
                return item.GetDecimalNullable(fieldName);
            }

            if (propertyTypeNameForCompare == typeof(string).Name.PrepareToCompare())
            {
                return item.GetString(fieldName);
            }

            if (propertyTypeNameForCompare == typeof(DateTime).Name.PrepareToCompare())
            {
                return item.GetDateTime(fieldName);
            }

            if (propertyTypeNameForCompare == typeof(DateTime?).Name.PrepareToCompare())
            {
                return item.GetDateTimeNullable(fieldName);
            }

            if (propertyTypeNameForCompare == typeof(Guid).Name.PrepareToCompare() ||
                propertyTypeNameForCompare == typeof(Guid?).Name.PrepareToCompare())
            {
                return item.GetGuid(fieldName);
            }

            if (propertyTypeNameForCompare == typeof(FieldLookupValue).Name.PrepareToCompare())
            {
                return item.GetLookupValue(fieldName);
            }

            return item.GetValueByInternalName(propertyInfo.Name);
        }
    }
}