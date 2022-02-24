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
using Universe.Sp.CQRS.Models.Filter;

namespace Universe.Sp.CQRS.Dal.MetaInfo
{
    /// <summary>
    /// The base field meta info.
    /// <author>Alex Envision</author>
    /// </summary>
    public abstract class BaseFieldMetaInfo
    {
        /// <summary>
        /// The _can be visible.
        /// </summary>
        private bool? _canBeVisible;

        /// <summary>
        /// The _filter title.
        /// </summary>
        private string _filterTitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFieldMetaInfo"/> class.
        /// </summary>
        protected BaseFieldMetaInfo()
        {
            AlwaysSelect = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether always select.
        /// </summary>
        public bool AlwaysSelect { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether displayed.
        /// </summary>
        public bool CanBeVisible
        {
            get => _canBeVisible ?? VisibleDefault;

            set => _canBeVisible = value;
        }

        /// <summary>
        /// Gets the data type.
        /// </summary>
        public abstract Type DataType { get; }

        /// <summary>
        /// Gets the field type.
        /// </summary>
        public abstract string FieldType { get; }

        /// <summary>
        /// Gets the field type.
        /// </summary>
        public FieldTypes? FieldTypeEnum { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether filterable.
        /// </summary>
        public virtual bool Filterable { get; set; }

        /// <summary>
        /// Gets or sets the filter title.
        /// </summary>
        public string FilterTitle
        {
            get => _filterTitle ?? Title;

            set => _filterTitle = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether is filterhierarchy.
        /// </summary>
        public bool IsFilterHierarchy { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether sortable.
        /// </summary>
        public virtual bool Sortable { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether default view.
        /// </summary>
        public bool VisibleDefault { get; set; }

        /// <summary>
        /// The get field type.
        /// </summary>
        /// <param name="dataType">
        /// The data type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetFieldType(Type dataType)
        {
            if (DataType == typeof(int) || DataType == typeof(int?)
                || DataType == typeof(long) || DataType == typeof(long?)
                || DataType == typeof(short) || DataType == typeof(short?))
                return FieldTypes.Int.ToString();

            if (DataType == typeof(decimal) || DataType == typeof(decimal?)
                || DataType == typeof(double) || DataType == typeof(double?)
                || DataType == typeof(float) || DataType == typeof(float?))
                return FieldTypes.Number.ToString();

            if (DataType == typeof(DateTimeOffset) || DataType == typeof(DateTimeOffset?))
                return FieldTypes.DateTimeOffset.ToString();

            if (DataType == typeof(bool) || DataType == typeof(bool?))
                return FieldTypes.Bool.ToString();

            return FieldTypes.String.ToString();
        }
    }
}