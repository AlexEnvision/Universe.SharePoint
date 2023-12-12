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

namespace Universe.Sp.CSOM.CQRS.Models.Page
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class Paging
    {
        private const int DefaultCountOnPageValue = 30;

        /// <summary>
        /// The _page index.
        /// </summary>
        private int _pageIndex = 1;

        /// <summary>
        /// Gets or sets the all count.
        /// </summary>
        public int AllCount { get; set; }

        private int _countOnPage;

        /// <summary>
        /// Gets or sets the count on page.
        /// </summary>
        public int CountOnPage
        {
            get
            {
                if (_countOnPage != 0)
                    return _countOnPage;

                return DefaultCountOnPageValue;
            }
            set => _countOnPage = value;
        }

        /// <summary>
        /// Gets the end row.
        /// </summary>
        public int EndRow => StartRow - 1 + CountOnPage;

        /// <summary>
        /// Gets the end row plus one.
        /// </summary>
        public int EndRowPlusOne => EndRow + 1;

        /// <summary>
        /// Gets or sets the go to page index.
        /// </summary>
        public int? GoToPageIndex { get; set; }

        /// <summary>
        /// Gets the page count.
        /// </summary>
        public int PageCount => CountOnPage != 0 ? (int)Math.Ceiling(AllCount / (decimal)CountOnPage) : (int)Math.Ceiling(AllCount / 1.0);

        /// <summary>
        /// Gets or sets the page index.
        /// </summary>
        public int PageIndex
        {
            get => _pageIndex;

            set => _pageIndex = value < 1 ? 1 : value;
        }

        /// <summary>
        /// SP list item position
        /// </summary>
        public int? Position { get; set; }

        /// <summary>
        /// Gets the start row.
        /// </summary>
        public int StartRow
        {
            get
            {
                var pi = PageIndex - 1;
                if (pi < 0)
                    pi = 0;

                return CountOnPage * pi + 1;
            }
        }
    }
}