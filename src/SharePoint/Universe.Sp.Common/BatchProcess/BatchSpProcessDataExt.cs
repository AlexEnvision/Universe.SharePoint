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
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Universe.Sp.Common.BatchProcess.Entities;
using Universe.Sp.Common.BatchProcess.Entities.Base;

namespace Universe.Sp.Common.BatchProcess
{
    /// <summary>
    ///     Расширения для масс-обновления элементов
    /// <author>Alex Envision</author>
    /// </summary>
    public static class BatchSpProcessDataExt
    {
        private static readonly string _lastBatchCommand = "Universe.Sp.Common.BatchProcess.LastBatchCommand";

        /// <summary>
        ///     Масс-обновление элементов списка
        /// </summary>
        /// <typeparam name="TSpo">Splistitem/Spitem Proxy Object</typeparam>
        /// <param name="web">Узел</param>
        /// <param name="batchItems">Коллекция объектов Sharepoint Object</param>
        /// <param name="onError">Действие с элементом при ошибке во время выполнения Масс-обновления</param>
        /// <returns></returns>
        public static BatchSpProcessResult MassUpdate<TSpo>(this SPWeb web, IEnumerable<TSpo> batchItems,
            BatchSpOnErrorEnum onError = BatchSpOnErrorEnum.Continue) where TSpo : ISpo
        {
            if (web == null)
                throw new ArgumentNullException(nameof(web));

            if (batchItems == null)
                throw new ArgumentNullException(nameof(batchItems));

            if (!batchItems.Any())
                return new BatchSpProcessResult {ResultsAsText = "Отсутствуют эленты для обновления"};

            var methods = batchItems.Aggregate(string.Empty,
                (current, item) => current + item.GetBatchSaveCommand());

            var batch = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                        <ows:Batch OnError=""{0}"">{1}</ows:Batch>", onError, methods);

            CallContext.SetData(_lastBatchCommand, batch);

            var result = web.ProcessBatchData(batch);

            var results = Deserialize<BatchSpCommandResults>(result);
            var batchResult = new BatchSpProcessResult {Results = results, ResultsAsText = result};
            return batchResult;
        }

        /// <summary>
        ///     Масс-обновление элементов списка
        /// </summary>
        /// <typeparam name="TSpo">Splistitem/Spitem Proxy Object</typeparam>
        /// <param name="web">Узел</param>
        /// <param name="list">Список</param>
        /// <param name="batchItems">Коллекция объектов Sharepoint Object</param>
        /// <param name="onError">Действие с элементом при ошибке во время выполнения Масс-обновления</param>
        /// <returns></returns>
        public static BatchSpProcessResult MassUpdate<TSpo>(this SPWeb web,
            SPList list,
            IEnumerable<TSpo> batchItems,
            BatchSpOnErrorEnum onError = BatchSpOnErrorEnum.Continue) where TSpo : ISpo
        {
            if (web == null)
                throw new ArgumentNullException(nameof(web));

            if (batchItems == null)
                throw new ArgumentNullException(nameof(batchItems));

            if (!batchItems.Any())
                return new BatchSpProcessResult {ResultsAsText = "Отсутствуют эленты для обновления"};

            var methods = batchItems.Aggregate(string.Empty,
                (current, item) => current + item.GetBatchSaveCommand(list.ID));

            var batch = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                        <ows:Batch OnError=""{0}"">{1}</ows:Batch>", onError, methods);
            var result = web.ProcessBatchData(batch);

            var results = Deserialize<BatchSpCommandResults>(result);
            var batchResult = new BatchSpProcessResult {Results = results, ResultsAsText = result};
            return batchResult;
        }

        /// <summary>
        ///     Масс-обновление элементов списка
        /// </summary>
        /// <typeparam name="TSpo">Splistitem/Spitem Proxy Object</typeparam>
        /// <param name="web">Узел</param>
        /// <param name="batchItems">Коллекция объектов Sharepoint Object</param>
        /// <param name="onError">Действие с элементом при ошибке во время выполнения Масс-обновления</param>
        /// <returns></returns>
        public static BatchSpProcessResult MassUpdate<TSpo>(
            this SPWeb web,
            IEnumerable<TablePartItemContainer<TSpo>> batchItems,
            BatchSpOnErrorEnum onError = BatchSpOnErrorEnum.Continue) where TSpo : ISpo
        {
            if (web == null)
                throw new ArgumentNullException(nameof(web));

            if (batchItems == null)
                throw new ArgumentNullException(nameof(batchItems));

            if (!batchItems.Any())
                return new BatchSpProcessResult {ResultsAsText = "Отсутствуют эленты для обновления"};

            var methods = batchItems.Aggregate(string.Empty,
                (current, item) => current + item.GetBatchSaveCommand());

            var batch = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                        <ows:Batch OnError=""{0}"">{1}</ows:Batch>", onError, methods);
            var result = web.ProcessBatchData(batch);

            var results = Deserialize<BatchSpCommandResults>(result);
            var batchResult = new BatchSpProcessResult {Results = results, ResultsAsText = result};
            return batchResult;
        }

        /// <summary>
        ///     Масс-обновление элементов списка
        /// </summary>
        /// <typeparam name="TSpo">Splistitem/Spitem Proxy Object</typeparam>
        /// <param name="web">Узел</param>
        /// <param name="list">Список</param>
        /// <param name="batchItems">Коллекция объектов Sharepoint Object</param>
        /// <param name="onError">Действие с элементом при ошибке во время выполнения Масс-обновления</param>
        /// <returns></returns>
        public static BatchSpProcessResult MassUpdate<TSpo>(this SPWeb web,
            SPList list,
            IEnumerable<TablePartItemContainer<TSpo>> batchItems,
            BatchSpOnErrorEnum onError = BatchSpOnErrorEnum.Continue) where TSpo : ISpo
        {
            if (web == null)
                throw new ArgumentNullException(nameof(web));

            if (batchItems == null)
                throw new ArgumentNullException(nameof(batchItems));

            if (!batchItems.Any())
                return new BatchSpProcessResult {ResultsAsText = "Отсутствуют эленты для обновления"};

            var methods = batchItems.Aggregate(string.Empty,
                (current, item) => current + item.GetBatchSaveCommand(list.ID));

            var batch = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
                        <ows:Batch OnError=""{0}"">{1}</ows:Batch>", onError, methods);
            var result = web.ProcessBatchData(batch);

            var results = Deserialize<BatchSpCommandResults>(result);
            var batchResult = new BatchSpProcessResult {Results = results, ResultsAsText = result};
            return batchResult;
        }

        private static TEntity Deserialize<TEntity>(string objectsAsString) where TEntity : class
        {
            var encoding = Encoding.UTF8;
            var likeFile = $@"<?xml version=""1.0"" encoding=""UTF-8""?>{
                    objectsAsString
                        .Replace("<Results",
                            @"<Results xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""")
                }";
            var bytes = encoding.GetBytes(likeFile);
            using (Stream stream = new MemoryStream(bytes))
            {
                var type = typeof(TEntity);
                var serializer = new XmlSerializer(type);

                var fileBody = (TEntity) serializer.Deserialize(stream);
                return fileBody;
            }
        }

        /// <summary>
        ///     Получает последнюю Batch команду
        /// </summary>
        /// <returns></returns>
        public static string GetLastBatchCommand()
        {
            return CallContext.GetData(_lastBatchCommand) as string;
        }
    }
}