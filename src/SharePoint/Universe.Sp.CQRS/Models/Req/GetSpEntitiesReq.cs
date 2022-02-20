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

using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.JsonUtilities;
using Universe.Sp.CQRS.Models.Condition;
using Universe.Sp.CQRS.Models.Filter;
using Universe.Sp.CQRS.Models.Page;
using Universe.Sp.CQRS.Models.Sort;

namespace Universe.Sp.CQRS.Models.Req
{
    /// <summary>
    ///     Модель запроса сущностей.
    /// <author>Alex Envision</author>
    /// </summary>
    public class GetSpEntitiesReq
    {
        public GetSpEntitiesReq()
        {
            Filters = new List<ConditionConfiguration>();
            Paging = new Paging();
            Sorting = new List<SortConfiguration>();
        }

        public List<ConditionConfiguration> Filters { get; set; }

        public Paging Paging { get; set; }

        public List<SortConfiguration> Sorting { get; set; }

        public long Id { get; set; }

        [JsonIgnore]
        public IFieldMapContainer FieldMapContainer { get; set; }

        private bool _isAllWithPaging;

        public bool IsAllWithPaging
        {
            get
            {
                if (Paging == null || Paging.CountOnPage == 0)
                    return true;

                return _isAllWithPaging;
            }
            set => _isAllWithPaging = value;
        }

        [JsonIgnore]
        public SPQuery SpQuery { get; set; }
    }
}
