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
using System.Linq;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Universe.Sp.Common.Workflows.Entities;
using Universe.Sp.Common.Workflows.Entities.Result;

namespace Universe.Sp.Common.Workflows
{
    using Diagnostic.Logger;

    /// <summary>
    ///     Расширения для работы с рабочими процессами.
    ///     Extension workflow.
    /// <author>Alex Envision</author>
    /// </summary>
    public static class WorkflowsExt
    {
        /// <summary>
        /// The fill association data.
        /// </summary>
        /// <param name="wfData">
        /// The wf data.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FillAssociationData(string wfData, params KeyValuePair<string, string>[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return wfData;

            var document = new XmlDocument();
            document.LoadXml(wfData);
            foreach (var keyValuePair in parameters)
            {
                var node = document.SelectSingleNode("//Data/" + keyValuePair.Key);
                if (node == null)
                {
                    var newChild = document.CreateElement(keyValuePair.Key);
                    newChild.InnerText = keyValuePair.Value;
                    document.FirstChild.AppendChild(newChild);
                }
                else
                {
                    node.InnerText = keyValuePair.Value;
                }
            }

            return document.OuterXml;
        }

        /// <summary>
        ///     Запуск рабочего процесса.
        ///     Starts the workflow.
        /// </summary>
        /// <param name="parameters">
        ///     Параметры запуска.
        ///     The parameters.
        /// </param>
        /// <param name="log">
        ///     Лог.
        ///     The logger.
        /// </param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// parameters
        /// or
        /// log
        /// </exception>
        /// <exception cref="SPException">
        /// ApplicationPages_StartWorkflow_WorkflowDisabled
        /// or
        /// SPException_WorkflowAlreadyRunning
        /// </exception>
        public static void StartWorkflow(StartWorkflowParameters parameters, IUniverseLogger log)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            using (var site = new SPSite(parameters.SiteId))
            using (var web = site.OpenWeb(parameters.WebId))
            using (var manager = site.WorkflowManager)
            {
                var list = web.Lists[parameters.ListId];
                var listItem = list.GetItemById(parameters.ListItemId);

                var association =
                    list.WorkflowAssociations.Cast<SPWorkflowAssociation>().FirstOrDefault(_ => _.Name == parameters.AssociationName);
                if (association == null)
                {
                    list.WorkflowAssociations.UpdateAssociationsToLatestVersion();
                    association =
                        list.WorkflowAssociations.Cast<SPWorkflowAssociation>().FirstOrDefault(_ => _.Name == parameters.AssociationName);
                }

                if (association == null)
                    throw new SPException($"Не найдена SPWorkflowAssociation {parameters.AssociationName}");

                if (!association.Enabled)
                    throw new SPException("ApplicationPages_StartWorkflow_WorkflowDisabled");

                var eventData = association.AssociationData;
                if (parameters.EventData?.Length > 0)
                    eventData = FillAssociationData(association.AssociationData, parameters.EventData);

                var allowUnsafeUpdates = web.AllowUnsafeUpdates;
                try
                {
                    log.Warning(
                        $@"{parameters.AssociationName} {parameters.ListItemId} eventData: {eventData} SPWorkflowRunOptions: {
                            parameters.RunOptions
                            }");
                    web.AllowUnsafeUpdates = true;
                    manager.StartWorkflow(listItem, association, eventData, parameters.RunOptions);
                    log.Warning($@"{parameters.AssociationName} {parameters.ListItemId} StartWorkflow завершено ");
                }
                catch (SPException exception)
                {
                    log.Error(exception, exception.Message);
                    if (exception.ErrorCode == -2130575205)
                        throw new SPException("SPException_WorkflowAlreadyRunning", exception);

                    throw;
                }
                finally
                {
                    web.AllowUnsafeUpdates = allowUnsafeUpdates;
                }
            }
        }

        /// <summary>
        ///     Запуск рабочего процесса уровня узла.
        ///     Starts the workflow on web.
        /// </summary>
        /// <param name="parameters">
        ///     Параметры запуска.
        ///     The parameters.
        /// </param>
        /// <param name="log">
        ///     Лог.
        ///     The logger.
        /// </param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// parameters
        /// or
        /// log
        /// </exception>
        public static void StartWorkflowOnWeb(StartWebWorkflowParameters parameters, IUniverseLogger log)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            SPSecurity.RunWithElevatedPrivileges(
                () => {
                    using (var site = new SPSite(parameters.SiteId))
                    using (var web = site.OpenWeb(parameters.WebId))
                    using (var manager = site.WorkflowManager)
                    {
                        var allowUpdates = false;
                        try
                        {
                            allowUpdates = web.AllowUnsafeUpdates;
                            web.AllowUnsafeUpdates = true;

                            var association =
                                web.WorkflowAssociations.Cast<SPWorkflowAssociation>()
                                    .FirstOrDefault(_ => _.Name == parameters.AssociationName);
                            if (association == null)
                            {
                                web.WorkflowAssociations.UpdateAssociationsToLatestVersion();
                                association =
                                    web.WorkflowAssociations.Cast<SPWorkflowAssociation>()
                                        .FirstOrDefault(_ => _.Name == parameters.AssociationName);
                            }

                            if (association == null)
                                throw new SPException($"Не найдена SPWorkflowAssociation {parameters.AssociationName}");

                            if (!association.Enabled)
                                throw new SPException("ApplicationPages_StartWorkflow_WorkflowDisabled");

                            var eventData = association.AssociationData;
                            if (parameters.EventData?.Length > 0)
                                eventData = FillAssociationData(association.AssociationData, parameters.EventData);

                            try
                            {
                                log.Warning(
                                    $@"{parameters.AssociationName} {parameters.WebId} eventData: {eventData} AutoStart");
                                manager.StartWorkflow(web, association, eventData, parameters.RunOptions);
                                log.Warning($@"{parameters.AssociationName} {parameters.WebId} StartWorkflow завершено");
                            }
                            catch (SPException exception)
                            {
                                log.Error(exception, exception.Message);
                                if (exception.ErrorCode == -2130575205)
                                    throw new SPException("SPException_WorkflowAlreadyRunning", exception);

                                throw;
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex, ex.Message);
                        }
                        finally
                        {
                            web.AllowUnsafeUpdates = allowUpdates;
                        }
                    }
                });
        }

        /// <summary>
        ///     Запуск рабочего процесса уровня узла.
        ///     Starts the workflow on web.
        /// </summary>
        /// <param name="parameters">
        ///     Параметры запуска.
        ///     The parameters.
        /// </param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// parameters
        /// or
        /// log
        /// </exception>
        public static List<StartWorkflowLogResult> StartWorkflowOnWeb(StartWebWorkflowParameters parameters)
        {
            List<StartWorkflowLogResult> logItems = new List<StartWorkflowLogResult>();

            if (parameters == null)
            {
                logItems.Add(
                    new StartWorkflowLogResult
                    {
                        ExceptionInfo = new ArgumentNullException(nameof(parameters)),
                        Message = nameof(parameters),
                        LevelCode = StartWorkflowLevelCode.Error
                    });
                return logItems;
            }

            SPSecurity.RunWithElevatedPrivileges(
                () => {
                    using (var site = new SPSite(parameters.SiteId))
                    using (var web = site.OpenWeb(parameters.WebId))
                    using (var manager = site.WorkflowManager)
                    {
                        var allowUpdates = false;
                        try
                        {
                            allowUpdates = web.AllowUnsafeUpdates;
                            web.AllowUnsafeUpdates = true;

                            var association =
                                web.WorkflowAssociations.Cast<SPWorkflowAssociation>()
                                    .FirstOrDefault(_ => _.Name == parameters.AssociationName);
                            if (association == null)
                            {
                                web.WorkflowAssociations.UpdateAssociationsToLatestVersion();
                                association =
                                    web.WorkflowAssociations.Cast<SPWorkflowAssociation>()
                                        .FirstOrDefault(_ => _.Name == parameters.AssociationName);
                            }

                            if (association == null)
                                throw new SPException($"Не найдена SPWorkflowAssociation {parameters.AssociationName}");

                            if (!association.Enabled)
                                throw new SPException("ApplicationPages_StartWorkflow_WorkflowDisabled");

                            var eventData = association.AssociationData;
                            if (parameters.EventData?.Length > 0)
                                eventData = FillAssociationData(association.AssociationData, parameters.EventData);

                            try
                            {
                                logItems.Add(
                                    new StartWorkflowLogResult
                                    {
                                        Message = $@"{parameters.AssociationName} {parameters.WebId} eventData: {eventData} AutoStart",
                                        LevelCode = StartWorkflowLevelCode.Warn
                                    });

                                manager.StartWorkflow(web, association, eventData, parameters.RunOptions);

                                logItems.Add(
                                    new StartWorkflowLogResult
                                    {
                                        Message = $@"{parameters.AssociationName} {parameters.WebId} StartWorkflow завершено ",
                                        LevelCode = StartWorkflowLevelCode.Warn
                                    });
                            }
                            catch (SPException exception)
                            {
                                logItems.Add(
                                    new StartWorkflowLogResult
                                    {
                                        ExceptionInfo = exception,
                                        Message = exception.Message,
                                        LevelCode = StartWorkflowLevelCode.Error
                                    });

                                if (exception.ErrorCode == -2130575205)
                                    throw new SPException("SPException_WorkflowAlreadyRunning", exception);

                                throw;
                            }
                        }
                        catch (Exception ex)
                        {
                            logItems.Add(
                                new StartWorkflowLogResult
                                {
                                    ExceptionInfo = ex,
                                    Message = ex.Message,
                                    LevelCode = StartWorkflowLevelCode.Error
                                });
                        }
                        finally
                        {
                            web.AllowUnsafeUpdates = allowUpdates;
                        }
                    }
                });

            return logItems;
        }
    }
}