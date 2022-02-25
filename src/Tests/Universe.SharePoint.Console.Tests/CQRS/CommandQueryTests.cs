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
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Universe.Diagnostic;
using Universe.Framework.ConsoleApp.Tests.CQRS.Base;
using Universe.Framework.ConsoleApp.Tests.Infrastructure;
using Universe.SharePoint.DataAccess.Test;
using Universe.SharePoint.DataAccess.Test.Models;
using Universe.Sp.CQRS.Dal.Commands;
using Universe.Sp.CQRS.Dal.Queries;
using Universe.Sp.CQRS.Infrastructure;
using Universe.Sp.CQRS.Models.Condition;
using Universe.Sp.CQRS.Models.Filter;
using Universe.Sp.CQRS.Models.Page;
using Universe.Sp.CQRS.Models.Req;
using Universe.Sp.CQRS.Models.Sort;

namespace Universe.Framework.ConsoleApp.Tests.CQRS
{
    /// <summary>
    ///     Тест запросов и команд.
    /// <author>Alex Envision</author>
    /// </summary>
    public class CommandQueryTests : BaseCommandQueryTests
    {
        public CommandQueryTests()
        {
            PrepareToStart();
        }

        private void PrepareToStart()
        {
            Console.WriteLine(@"Готовится запуск CommandQueryTests...");
        }

        public void CreateEntityQueryTest()
        {
            var container = UnityConfig.Container;

            var settings = new AppTestSettings();

            var scope = new UniverseSpScope<UniverseSpTestContext>(settings, container);

            using (var runningTimeWatcher = new RunningTimeWatcher())
            {
                var req = new TrainsetSp
                {
                    Name = "Trainset256",
                    Title = "Trainset256",
                    Created = DateTime.Now,
                    SetNumber = 256
                };

                Console.WriteLine(@"Сохранение данных в SP...");
                var result = scope.GetCommand<AddSpEntityCommand<TrainsetSp>>().Execute(
                    req
                );

                Console.WriteLine($@"Время выполнения запроса: {runningTimeWatcher.TakeRunningTime():G}");

                var resultSfy = JsonConvert.SerializeObject(result, Formatting.Indented);
                Console.WriteLine($@"Элемент списка {nameof(TrainsetSp)}: {Environment.NewLine}{resultSfy}");
            }
        }

        public void CreateEntitiesQueryTest()
        {
            var container = UnityConfig.Container;

            var settings = new AppTestSettings();

            var scope = new UniverseSpScope<UniverseSpTestContext>(settings, container);

            using (var runningTimeWatcher = new RunningTimeWatcher())
            {
                var req = new List<TrainsetSp>
                {
                    new TrainsetSp
                    {
                        Name = "Trainset257",
                        Title = "Trainset257",
                        Created = DateTime.Now,
                        SetNumber = 257
                    },
                    new TrainsetSp
                    {
                        Name = "Trainset258",
                        Title = "Trainset258",
                        Created = DateTime.Now,
                        SetNumber = 258
                    }
                };

                Console.WriteLine(@"Сохранение данных в SP...");
                var result = scope.GetCommand<AddSpEntitiesCommand<TrainsetSp>>().Execute(
                    req
                );

                Console.WriteLine($@"Время выполнения запроса: {runningTimeWatcher.TakeRunningTime():G}");

                var resultSfy = JsonConvert.SerializeObject(result, Formatting.Indented);
                Console.WriteLine($@"Элемент списка {nameof(TrainsetSp)}: {Environment.NewLine}{resultSfy}");
            }
        }

        public void ReadEntitiesQueryTest()
        {
            var container = UnityConfig.Container;

            var settings = new AppTestSettings();

            var scope = new UniverseSpScope<UniverseSpTestContext>(settings, container);

            using (var runningTimeWatcher = new RunningTimeWatcher())
            {
                var req = new GetSpEntitiesReq
                {
                    FieldMapContainer = new FieldMapContainer<TrainsetSp>
                    {
                        FieldMap = new Dictionary<string, Expression<Func<TrainsetSp, object>>>
                        {
                            { "Number", x => x.SetNumber }
                        }
                    },
                    Filters = new List<ConditionConfiguration>
                    {
                        new OrConfiguration
                        {
                            Operands = new List<ConditionConfiguration>
                            {
                                new ContainsConfiguration
                                {
                                    LeftOperand = new FieldArgumentConfiguration
                                    {
                                        Field = new FieldConfiguration
                                        {
                                            SpFieldName = "Name",
                                        }
                                    },
                                    RightOperand = new ValueArgumentConfiguration
                                    {
                                        Expression = "9"
                                    }
                                },
                                new ContainsConfiguration
                                {
                                    LeftOperand = new FieldArgumentConfiguration
                                    {
                                        Field = new FieldConfiguration
                                        {
                                            SpFieldName = "Name",
                                        }
                                    },
                                    RightOperand = new ValueArgumentConfiguration
                                    {
                                        Expression = "1"
                                    }
                                },
                                new EqConfiguration
                                {
                                    LeftOperand = new FieldArgumentConfiguration
                                    {
                                        Field = new FieldConfiguration
                                        {
                                            SpFieldName = "Number",
                                        }
                                    },
                                    RightOperand = new ValueArgumentConfiguration
                                    {
                                        Expression = "1"
                                    }
                                }
                            }
                        }
                    },
                    Sorting = new List<SortConfiguration>
                    {
                        new SortConfiguration
                        {
                            Field = "Name",
                            Direction = SortDirection.Desc
                        }
                    },
                    Paging = new Paging
                    {
                        CountOnPage = 30,
                        PageIndex = 1
                    }
                    //SpQuery = SpQueryExt.ItemsQuery(
                    //    where: CamlHelper.GetCamlWhere(
                    //        CamlHelper.CamlChain(
                    //            CamlHelper.LogicalOperators.OR,
                    //            CamlHelper.CamlChain(
                    //                CamlHelper.LogicalOperators.AND,
                    //                CamlHelper.GetEqText(
                    //                    "Name",
                    //                    "Trainset001")
                    //            ))),
                    //    viewFields: CamlHelper.BuildFieldsRef(
                    //        "ID",
                    //        "Title",
                    //        "Name"),
                    //    rowLimit: 2000
                    //)
                };

                Console.WriteLine(@"Чтение данных из SP...");
                var result = scope.GetQuery<GetSpEntitiesQuery<TrainsetSp>>().Execute(
                    req
                );

                Console.WriteLine($@"Время выполнения запроса: {runningTimeWatcher.TakeRunningTime():G}");

                var resultSfy = JsonConvert.SerializeObject(result, Formatting.Indented);
                Console.WriteLine($@"Элемент списка {nameof(TrainsetSp)}: {Environment.NewLine}{resultSfy}");
            }
        }

        public void UpdateEntitiesQueryTest()
        {
            var container = UnityConfig.Container;

            var settings = new AppTestSettings();

            var scope = new UniverseSpScope<UniverseSpTestContext>(settings, container);

            using (var runningTimeWatcher = new RunningTimeWatcher())
            {
                var req = new GetSpEntitiesReq
                {
                    FieldMapContainer = new FieldMapContainer<TrainsetSp>
                    {
                        FieldMap = new Dictionary<string, Expression<Func<TrainsetSp, object>>>
                        {
                            { "Number", x => x.SetNumber }
                        }
                    },
                    Filters = new List<ConditionConfiguration>
                    {
                        new AndConfiguration
                        {
                            Operands = new List<ConditionConfiguration>
                            {
                                new ContainsConfiguration
                                {
                                    LeftOperand = new FieldArgumentConfiguration
                                    {
                                        Field = new FieldConfiguration
                                        {
                                            SpFieldName = "Name",
                                        }
                                    },
                                    RightOperand = new ValueArgumentConfiguration
                                    {
                                        Expression = "9"
                                    }
                                },
                                new ContainsConfiguration
                                {
                                    LeftOperand = new FieldArgumentConfiguration
                                    {
                                        Field = new FieldConfiguration
                                        {
                                            SpFieldName = "Name",
                                        }
                                    },
                                    RightOperand = new ValueArgumentConfiguration
                                    {
                                        Expression = "1"
                                    }
                                }
                            }
                        }
                    },
                    Sorting = new List<SortConfiguration>
                    {
                        new SortConfiguration
                        {
                            Field = "Name",
                            Direction = SortDirection.Desc
                        }
                    },
                    Paging = new Paging
                    {
                        CountOnPage = 30,
                        PageIndex = 1
                    }
                    //SpQuery = SpQueryExt.ItemsQuery(
                    //    where: CamlHelper.GetCamlWhere(
                    //        CamlHelper.CamlChain(
                    //            CamlHelper.LogicalOperators.OR,
                    //            CamlHelper.CamlChain(
                    //                CamlHelper.LogicalOperators.AND,
                    //                CamlHelper.GetEqText(
                    //                    "Name",
                    //                    "Trainset001")
                    //            ))),
                    //    viewFields: CamlHelper.BuildFieldsRef(
                    //        "ID",
                    //        "Title",
                    //        "Name"),
                    //    rowLimit: 2000
                    //)
                };

                Console.WriteLine(@"Чтение данных из SP...");
                var result = scope.GetQuery<GetSpEntitiesQuery<TrainsetSp>>().Execute(
                    req
                );

                Console.WriteLine($@"Время выполнения запроса: {runningTimeWatcher.TakeRunningTime():G}");

                var resultSfy = JsonConvert.SerializeObject(result, Formatting.Indented);
                Console.WriteLine($@"Элемент списка {nameof(TrainsetSp)}: {Environment.NewLine}{resultSfy}");
            }
        }
    }
}