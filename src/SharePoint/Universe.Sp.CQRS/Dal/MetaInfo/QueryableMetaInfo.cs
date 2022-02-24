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
using System.Linq.Expressions;

namespace Universe.Sp.CQRS.Dal.MetaInfo
{
    /// <summary>
    ///     The queryable meta info.
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TSource">
    /// </typeparam>
    public class QueryableMetaInfo<TSource> : BaseMetaInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryableMetaInfo{TSource}"/> class.
        /// </summary>
        /// <param name="entityName">
        /// The entity name.
        /// </param>
        public QueryableMetaInfo(string entityName)
            : base(entityName)
        {
        }

        /// <summary>
        /// Gets the mappin sortg dictionary.
        /// </summary>
        public Dictionary<string, Expression<Func<TSource, object>>> MappinSortDictionary { get; private set; }

        /// <summary>
        /// The build dictionary sort.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary{TKey,TValue}"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public Dictionary<string, Expression<Func<TSource, object>>> BuildDictionarySort()
        {
            var result = new Dictionary<string, Expression<Func<TSource, object>>>();
            foreach (var fieldMetaInfo in FieldsMetaInfo.ConvertAll(_ => (QueryableFieldMetaInfo<TSource>)_))
            {
                if (fieldMetaInfo.Sortable)
                    try
                    {
                        result.Add(fieldMetaInfo.Name, fieldMetaInfo.DbFieldSelector);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{fieldMetaInfo.Name} - {ex.Message}", ex);
                    }
            }

            return result;
        }

        /// <summary>
        /// The build meta info.
        /// </summary>
        /// <exception cref="Exception">
        /// </exception>
        public void BuildMetaInfo()
        {
            try
            {
                MappinSortDictionary = BuildDictionarySort();
                BuildGridViewColumnsExtent();
            }
            catch (Exception ex)
            {
                throw new Exception($"При инициализации метаописания для {EntityName}, произошла ошибка: {ex.Message}", ex);
            }
        }
    }
}