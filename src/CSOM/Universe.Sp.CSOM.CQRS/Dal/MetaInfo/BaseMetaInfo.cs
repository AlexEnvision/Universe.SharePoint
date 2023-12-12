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

using System.Collections.Generic;
using System.Linq;

namespace Universe.Sp.CSOM.CQRS.Dal.MetaInfo
{
    /// <summary>
    /// The base meta info.
    /// </summary>
    public class BaseMetaInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMetaInfo"/> class.
        /// </summary>
        /// <param name="entityName">
        /// The entity name.
        /// </param>
        protected BaseMetaInfo(string entityName)
        {
            EntityName = entityName;
            FieldsMetaInfo = new List<BaseFieldMetaInfo>();
        }

        /// <summary>
        /// Gets or sets the entity name.
        /// </summary>
        public string EntityName { get; protected set; }

        /// <summary>
        /// Gets or sets the fields meta info.
        /// </summary>
        public List<BaseFieldMetaInfo> FieldsMetaInfo { get; protected set; }

        /// <summary>
        /// Gets or sets the grid view columns extent.
        /// </summary>
        public List<BaseFieldMetaInfo> GridViewColumnsExtent { get; protected set; }


        /// <summary>
        /// The build grid view columns extent.
        /// </summary>
        protected void BuildGridViewColumnsExtent()
        {
            GridViewColumnsExtent = FieldsMetaInfo.Where(_ => _.Name.StartsWith("Extent.") && _.CanBeVisible).ToList();
        }
    }
}