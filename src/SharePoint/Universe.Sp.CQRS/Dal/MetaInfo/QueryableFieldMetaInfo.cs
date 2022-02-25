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
using System.Linq.Expressions;
using Universe.Sp.CQRS.Dal.Base;

namespace Universe.Sp.CQRS.Dal.MetaInfo
{
    /// <summary>
    ///     The queryable field meta info.
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TSource">
    /// </typeparam>
    public class QueryableFieldMetaInfo<TSource> : BaseFieldMetaInfo
    {
        /// <summary>
        /// The _data type.
        /// </summary>
        private Type _dataType;

        /// <summary>
        /// The _db field selector for extent func.
        /// </summary>
        private Func<TSource, object> _dbFieldSelectorForExtentFunc;

        /// <summary>
        /// Gets the data type.
        /// </summary>
        public override Type DataType => _dataType ?? (_dataType = DbFieldSelector.GetPropertyType());

        /// <summary>
        /// Gets or sets the db field selector.
        /// </summary>
        public Expression<Func<TSource, object>> DbFieldSelector { get; set; }

        /// <summary>
        /// Gets or sets the db field selector for extent.
        /// </summary>
        public Expression<Func<TSource, object>> DbFieldSelectorForExtent { get; set; }

        /// <summary>
        /// Gets the db field selector for extent func.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public Func<TSource, object> DbFieldSelectorForExtentFunc
        {
            get
            {
                if (_dbFieldSelectorForExtentFunc != null)
                    return _dbFieldSelectorForExtentFunc;

                if (DbFieldSelectorForExtent != null)
                    _dbFieldSelectorForExtentFunc = DbFieldSelectorForExtent.Compile();
                else if (DbFieldSelector != null)
                    _dbFieldSelectorForExtentFunc = DbFieldSelector.Compile();
                else
                    throw new InvalidOperationException("DbFieldSelectorForExtentFunc");

                return _dbFieldSelectorForExtentFunc;
            }
        }

        /// <summary>
        /// Gets or sets the field type.
        /// </summary>
        public override string FieldType
        {
            get
            {
                if (FieldTypeEnum != null)
                    return FieldTypeEnum.Value.ToString();

                return GetFieldType(DataType);
            }
        }
    }
}