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

using System.Linq.Expressions;

namespace Universe.Sp.CQRS.Dal.Base
{
    /// <summary>
    /// The one parameter rebinder.
    /// <author>Alex Envision</author>
    /// </summary>
    internal class OneParameterRebinder : ExpressionVisitor
    {
        /// <summary>
        /// The _new parameter.
        /// </summary>
        private readonly ParameterExpression _newParameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneParameterRebinder"/> class.
        /// </summary>
        /// <param name="newParameter">
        /// The new parameter.
        /// </param>
        private OneParameterRebinder(ParameterExpression newParameter)
        {
            _newParameter = newParameter;
        }

        /// <summary>
        /// The replace parameter.
        /// </summary>
        /// <param name="newParameter">
        /// The new parameter.
        /// </param>
        /// <param name="exp">
        /// The exp.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression ReplaceParameter(ParameterExpression newParameter, Expression exp)
        {
            return new OneParameterRebinder(newParameter).Visit(exp);
        }

        /// <summary>
        /// The visit parameter.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            return base.VisitParameter(_newParameter);
        }
    }
}