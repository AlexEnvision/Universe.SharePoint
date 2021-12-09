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
using Universe.Diagnostic;
using Universe.Sp.Common.BatchProcess.Entities;

namespace Universe.Sp.Common.BatchProcess
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class BatchSpProcessResult
    {
        public BatchSpCommandResults Results { get; set; }
        public string ResultsAsText { get; set; }

        public void LogResult(EventLogger log)
        {
            foreach (var result in Results)
            {
                try
                {
                    if (result.Code == "0")
                    {
                        log.Info(
                            result.ID > 0
                                ? $"Создан элемент с ID={result.ID}. ID операции {result.AttrID}"
                                : $"Сохранён элемент. ID операции {result.AttrID}");
                    }
                    else
                    {
                        throw new Exception(
                            $"Ошибка cоздания/сохранения элемента c ID={result.ID}. ID операции {result.AttrID}. Текст ошибки {result.ErrorText}");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, ex.Message);
                }               
            }
        }
    }
}