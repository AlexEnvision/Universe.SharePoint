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
using System.Linq;
using System.Net;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WorkflowServices;
using Universe.Diagnostic;
using Universe.Sp.Common.CSOM.Models;
using Universe.Sp.Common.CSOM.Models.Result;

namespace Universe.Sp.Common.CSOM.Workflows
{
    using Diagnostic.Logger;

    /// <summary>
    ///     Расширения для работы с рабочими процессами на клиентской модели SharePoint.
    ///     Extension workflow on CSOM.
    /// <author>Alex Envision</author>
    /// </summary>
    public static class WorkflowsCsomExt
    {
        /// <summary>
        ///     Запуск рабочего процесса.
        ///     Starts the workflow.
        /// </summary>
        /// <param name="parameters">
        ///     Параметры запуска.
        ///     The parameters.
        /// </param>
        /// <returns></returns>
        public static List<StartCsomWorkflowLogResult> StartWorkflow(StartCsomWorkflowParameters parameters)
        {
            List<StartCsomWorkflowLogResult> logItems = new List<StartCsomWorkflowLogResult>();

            var loginId = parameters.LoginId;
            var password = parameters.SecurePassword;

            var ctx = new ClientContext(parameters.WebUrl);
            ctx.Credentials = new SharePointOnlineCredentials(loginId, password);

            var workflowServicesManager = new WorkflowServicesManager(ctx, ctx.Web);
            //var workflowInteropService = workflowServicesManager.GetWorkflowInteropService();
            var workflowSubscriptionService = workflowServicesManager.GetWorkflowSubscriptionService();
            var workflowDeploymentService = workflowServicesManager.GetWorkflowDeploymentService();
            var workflowInstanceService = workflowServicesManager.GetWorkflowInstanceService();

            var publishedWorkflowDefinitions = workflowDeploymentService.EnumerateDefinitions(true);
            ctx.Load(publishedWorkflowDefinitions);
            ctx.ExecuteQuery();

            var def = from defs in publishedWorkflowDefinitions
                      where defs.DisplayName == parameters.WorkflowName
                      select defs;

            WorkflowDefinition workflow = def.FirstOrDefault();

            if (workflow != null)
            {
                // get all workflow associations
                var workflowAssociations = workflowSubscriptionService.EnumerateSubscriptionsByDefinition(workflow.Id);
                ctx.Load(workflowAssociations);
                ctx.ExecuteQuery();

                // find the first association
                var firstWorkflowAssociation = workflowAssociations.First();

                // start the workflow
                var startParameters = parameters.EventData;

                var list = ctx.Web.GetList(parameters.ListUrl);
                var listItem = list.GetItemById(parameters.ListItemId);

                // Retrieve all items in the ListItemCollection from List.GetItems(Query).
                ctx.Load(listItem);
                ctx.ExecuteQuery();

                logItems.Add(new StartCsomWorkflowLogResult
                {
                    Message = "Starting workflow for item: " + listItem.Id,
                    LevelCode = StartCsomWorkflowLevelCode.Info
                });
                workflowInstanceService.StartWorkflowOnListItem(firstWorkflowAssociation, listItem.Id, startParameters);
                ctx.ExecuteQuery();
            }

            return logItems;
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
        public static List<StartCsomWorkflowLogResult> StartWorkflowOnWeb(StartCsomWebWorkflowParameters parameters)
        {
            List<StartCsomWorkflowLogResult> logItems = new List<StartCsomWorkflowLogResult>();

            var loginId = parameters.LoginId;
            var password = parameters.SecurePassword;

            var ctx = new ClientContext(parameters.WebUrl);
            ctx.Credentials = new SharePointOnlineCredentials(loginId, password);

            var workflowServicesManager = new WorkflowServicesManager(ctx, ctx.Web);
            //var workflowInteropService = workflowServicesManager.GetWorkflowInteropService();
            var workflowSubscriptionService = workflowServicesManager.GetWorkflowSubscriptionService();
            var workflowDeploymentService = workflowServicesManager.GetWorkflowDeploymentService();
            var workflowInstanceService = workflowServicesManager.GetWorkflowInstanceService();

            var publishedWorkflowDefinitions = workflowDeploymentService.EnumerateDefinitions(true);
            ctx.Load(publishedWorkflowDefinitions);
            ctx.ExecuteQuery();

            var def = from defs in publishedWorkflowDefinitions
                      where defs.DisplayName == parameters.WorkflowName
                      select defs;

            var workflow = def.FirstOrDefault();

            if (workflow != null)
            {
                // get all workflow associations
                var workflowAssociations = workflowSubscriptionService.EnumerateSubscriptionsByDefinition(workflow.Id);
                ctx.Load(workflowAssociations);
                ctx.ExecuteQuery();

                // find the first association
                var firstWorkflowAssociation = workflowAssociations.First();

                // start the workflow
                var startParameters = parameters.EventData;

                logItems.Add(new StartCsomWorkflowLogResult
                {
                    Message = "Starting workflow " + parameters.WorkflowName,
                    LevelCode = StartCsomWorkflowLevelCode.Info
                });
                workflowInstanceService.StartWorkflow(firstWorkflowAssociation, startParameters);
                ctx.ExecuteQuery();
            }

            return logItems;
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
        public static void StartWorkflow(StartCsomWorkflowParameters parameters, IUniverseLogger log)
        {
            var ctx = new ClientContext(parameters.WebUrl);

            var loginId = parameters.LoginId;
            var password = parameters.SecurePassword;

            ctx.Credentials = new SharePointOnlineCredentials(loginId, password);

            var workflowServicesManager = new WorkflowServicesManager(ctx, ctx.Web);
            //var workflowInteropService = workflowServicesManager.GetWorkflowInteropService();
            var workflowSubscriptionService = workflowServicesManager.GetWorkflowSubscriptionService();
            var workflowDeploymentService = workflowServicesManager.GetWorkflowDeploymentService();
            var workflowInstanceService = workflowServicesManager.GetWorkflowInstanceService();

            var publishedWorkflowDefinitions = workflowDeploymentService.EnumerateDefinitions(true);
            ctx.Load(publishedWorkflowDefinitions);
            ctx.ExecuteQuery();

            var def = from defs in publishedWorkflowDefinitions
                      where defs.DisplayName == parameters.WorkflowName
                      select defs;

            WorkflowDefinition workflow = def.FirstOrDefault();

            if (workflow != null)
            {
                // get all workflow associations
                var workflowAssociations = workflowSubscriptionService.EnumerateSubscriptionsByDefinition(workflow.Id);
                ctx.Load(workflowAssociations);
                ctx.ExecuteQuery();

                // find the first association
                var firstWorkflowAssociation = workflowAssociations.First();

                // start the workflow
                var startParameters = parameters.EventData;

                var list = ctx.Web.GetList(parameters.ListUrl);
                var listItem = list.GetItemById(parameters.ListItemId);

                // Retrieve all items in the ListItemCollection from List.GetItems(Query).
                ctx.Load(listItem);
                ctx.ExecuteQuery();

                log.Info("Starting workflow for item: " + listItem.Id);

                workflowInstanceService.StartWorkflowOnListItem(firstWorkflowAssociation, listItem.Id, startParameters);
                ctx.ExecuteQuery();
            }
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
        public static void StartWorkflowOnWeb(StartCsomWebWorkflowParameters parameters, IUniverseLogger log)
        {
            var ctx = new ClientContext(parameters.WebUrl);
            ctx.Credentials = new NetworkCredential(parameters.LoginId, parameters.Password);

            var workflowServicesManager = new WorkflowServicesManager(ctx, ctx.Web);
            ctx.Load(workflowServicesManager);
            ctx.ExecuteQuery();

            var workflowInteropService = workflowServicesManager.GetWorkflowInteropService();
            var workflowSubscriptionService = workflowServicesManager.GetWorkflowSubscriptionService();
            var workflowDeploymentService = workflowServicesManager.GetWorkflowDeploymentService();
            var workflowInstanceService = workflowServicesManager.GetWorkflowInstanceService();

            ctx.Load(workflowInteropService);
            ctx.Load(workflowSubscriptionService);
            ctx.Load(workflowDeploymentService);
            ctx.Load(workflowInstanceService);
            ctx.ExecuteQuery();

            var siteWorkflows = workflowSubscriptionService.EnumerateSubscriptions();
            ctx.Load(siteWorkflows);
            ctx.ExecuteQuery();

            var publishedWorkflowDefinitions = workflowDeploymentService.EnumerateDefinitions(true);
            ctx.Load(publishedWorkflowDefinitions);
            ctx.ExecuteQuery();

            //var subscriptions = workflowSubscriptionService.EnumerateSubscriptions();      
            //ctx.Load(subscriptions);
            //ctx.ExecuteQuery();

            var def = from defs in publishedWorkflowDefinitions
                where defs.DisplayName == parameters.WorkflowName
                select defs;

            var workflow = def.FirstOrDefault();

            if (workflow != null)
            {
                // find the first association
                //var association = subscriptions.FirstOrDefault(sub => sub.Name == parameters.WorkflowName);

                //get all workflow associations
                var workflowAssociations = workflowSubscriptionService.EnumerateSubscriptionsByDefinition(workflow.Id);
                ctx.Load(workflowAssociations);
                ctx.ExecuteQuery();

                // find the first association
                var firstWorkflowAssociation = workflowAssociations.First();

                // start the workflow
                var startParameters = parameters.EventData;

                log.Info("Starting workflow " + parameters.WorkflowName);
                workflowInstanceService.StartWorkflow(firstWorkflowAssociation, startParameters);
                ctx.ExecuteQuery();
            }
        }
    }
}