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
using System.Linq;
using Universe.Sp.CSOM.CQRS.Dal.Commands.Base;
using Universe.Sp.CSOM.CQRS.Dal.Commands.CommandResults;
using Universe.Sp.CSOM.CQRS.Extensions;
using Universe.Sp.CSOM.DataAccess.Models;

namespace Universe.Sp.CSOM.CQRS.Dal.Commands
{
    /// <summary>
    ///     Комманда добавления множества сущностей
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TEntitySp"></typeparam>
    public class AddSpEntitiesCommand<TEntitySp> : BaseCommand
        where TEntitySp : EntitySp, new()
    {
        public virtual AddEntitiesResult Execute(IList<TEntitySp> entitiesSp, bool systemUpdate = false, bool supplyReceivers = false)
        {
            if (entitiesSp == null)
                throw new ArgumentNullException(nameof(entitiesSp));

            if (entitiesSp.Count == 0)
                return new AddEntitiesResult {
                    Ids = new List<int>(),
                    IsSuccessful = false
                };

            var setSp = SpCtx.Set<TEntitySp>();
            setSp.AddRange(entitiesSp);

            setSp.SaveChanges(systemUpdate, supplyReceivers);

            var ids = entitiesSp.Select(x => x.Id).ToList();
            return new AddEntitiesResult {
                Ids = ids,
                IsSuccessful = true
            };
        }
    }
}