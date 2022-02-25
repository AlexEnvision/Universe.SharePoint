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

using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Universe.Sp.DataAccess.Models;
using Universe.Types.Collection;

namespace Universe.Sp.CQRS.Infrastructure
{
    /// <summary>
    ///     SetSp представляет собой набор всех сущностей в контексте или которые могут быть запрошены из
    ///     SharePoint. Объекты SpSet создаются из SpContext с помощью метода SpContext.Set.
    /// <author>Alex Envision</author>
    /// </summary>
    public class SetSp<TEntitySp> where TEntitySp : class, IEntitySp, new()
    {
        public SPList SpList { get; set; }

        private readonly SpMapper _mapper;

        private MatList<TEntitySp> _buffer;

        public SetSp()
        {
            _mapper = new SpMapper();
            _buffer = new MatList<TEntitySp>();
        }

        public TEntitySp Add(TEntitySp entitySp)
        {
            entitySp.ListItem = SpList.AddItem();
            _mapper.Map(entitySp, entitySp.ListItem);
            _buffer += entitySp;

            return entitySp;
        }

        public void Update(params TEntitySp[] entitiesSp)
        {
            foreach (var entitySp in entitiesSp)
            {
                entitySp.ListItem = SpList.AddItem();
                _mapper.Map(entitySp, entitySp.ListItem);
                _buffer += entitySp;
            }
        }

        public IEnumerable<TEntitySp> AddRange(IEnumerable<TEntitySp> entitiesSp)
        {
            var entitySps = entitiesSp.ToList();
            foreach (var entitySp in entitySps)
            {
                entitySp.ListItem = SpList.AddItem();
                _mapper.Map(entitySp, entitySp.ListItem);
                _buffer += entitySp;
            }

            return entitySps;
        }

        public TEntitySp Remove(TEntitySp entitySp)
        {
            entitySp.ListItem = SpList.AddItem();
            _buffer -= entitySp;
            entitySp.ListItem.Delete();

            return entitySp;
        }

        public IEnumerable<TEntitySp> RemoveRange(IEnumerable<TEntitySp> entitySps)
        {
            var removeRange = entitySps.ToList();
            foreach (var entitySp in removeRange)
            {
                entitySp.ListItem = SpList.AddItem();
                _mapper.Map(entitySp, entitySp.ListItem);
                _buffer += entitySp;
            }

            return removeRange;
        }

        public void SaveChanges()
        {
            foreach (var entitySp in _buffer)
            {
                entitySp.ListItem.Update();
                entitySp.Id = entitySp.ListItem.ID;
            }
        }
    }
}
