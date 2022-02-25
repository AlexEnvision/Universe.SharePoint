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
using Universe.Sp.CQRS.Dal.Commands.Base;
using Universe.Sp.CQRS.Dal.Commands.CommandResults;
using Universe.Sp.CQRS.Extensions;
using Universe.Sp.DataAccess.Models;

namespace Universe.Sp.CQRS.Dal.Commands
{
    /// <summary>
    ///     Комманда добавления сущности
    /// <author>Alex Envision</author>
    /// </summary>
    public class AddSpEntityCommand<TEntitySp> : BaseCommand
        where TEntitySp : EntitySp, new()
    {
        protected TEntitySp CreatedEntity { get; set; }

        public virtual AddEntityResult Execute(TEntitySp entitySp)
        {
            if (entitySp == null)
                throw new ArgumentNullException(nameof(entitySp));

            var setSp = SpCtx.Set<TEntitySp>();
            setSp.Add(entitySp);
            setSp.SaveChanges();
            
            var id = entitySp.Id;
            CreatedEntity = entitySp;

            return new AddEntityResult
            {
                Id = id,
                IsSuccessful = true
            };
        }

        public virtual AddEntityResult Undo()
        {
            var createdEntitySp = this.CreatedEntity;

            if (createdEntitySp == null)
                throw new ArgumentNullException(nameof(createdEntitySp));

            var setDb = SpCtx.Set<TEntitySp>();
            setDb.Remove(createdEntitySp);

            var id = createdEntitySp.Id;
            return new AddEntityResult
            {
                Id = id,
                IsSuccessful = true
            };
        }
    }
}