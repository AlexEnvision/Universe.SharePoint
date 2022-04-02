//  ╔═════════════════════════════════════════════════════════════════════════════════╗
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Framework                                             ║
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
//  ║   Copyright 2021 Universe.Framework                                             ║
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
using Universe.Sp.CQRS.Dal.Commands.Base;
using Universe.Sp.CQRS.Dal.Commands.CommandResults;
using Universe.Sp.CQRS.Extensions;
using Universe.Sp.DataAccess.Models;

namespace Universe.Sp.CQRS.Dal.Commands
{
    /// <summary>
    ///     Команда обновления множества сущностей
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TEntitySp"></typeparam>
    public class UpdateSpEntitiesCommand<TEntitySp> : BaseCommand
        where TEntitySp : EntitySp, new()
    {
        public virtual UpdateEntitiesResult Execute(IList<TEntitySp> entitiesDbs, bool systemUpdate = false, bool supplyReceivers = false)
        {
            if (entitiesDbs == null)
                throw new ArgumentNullException(nameof(entitiesDbs));

            if (entitiesDbs.Count == 0)
                return new UpdateEntitiesResult {
                    Ids = new List<int>(),
                    IsSuccessful = false
                };

            var setDb = SpCtx.Set<TEntitySp>();
            var entitiesSpsArray = entitiesDbs.ToArray();
            setDb.Update(entitiesSpsArray);

            setDb.SaveChanges(systemUpdate, supplyReceivers);

            var ids = entitiesSpsArray.Select(x => x.Id).ToList();
            return new UpdateEntitiesResult {
                Ids = ids,
                IsSuccessful = true
            };
        }
    }
}