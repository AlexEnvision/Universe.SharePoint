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
using System.Linq.Expressions;
using System.Reflection;
using Universe.Sp.CSOM.CQRS.Dal.Base;

namespace Universe.Sp.CSOM.CQRS.Dal.Base
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public static class DataAccessExtensions
    {
        /// <summary>
        /// The aggregate predicates.
        /// </summary>
        /// <param name="predicates">
        /// The predicates.
        /// </param>
        /// <param name="andOr">
        /// The and or.
        /// </param>
        /// <typeparam name="TSource">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression<Func<TSource, bool>> AggregatePredicates<TSource>(
            List<Expression<Func<TSource, bool>>> predicates,
            Func<Expression, Expression, Expression> andOr)
        {
            if (predicates.Count == 1)
                return predicates[0];

            var expressions = new List<Expression>();
            foreach (var expression in predicates)
            {
                var secondBody = OneParameterRebinder.ReplaceParameter(predicates[0].Parameters.Single(), expression.Body);
                expressions.Add(secondBody);
            }

            var body = expressions.Aggregate(andOr);
            var resultExp = Expression.Lambda<Func<TSource, bool>>(body, predicates[0].Parameters);
            return resultExp;
        }

        /// <summary>
        /// The get member expression.
        /// </summary>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="MemberExpression"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public static MemberExpression GetMemberExpression<T>(this Expression<Func<T, object>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var expression = selector.Body;

            if (expression is MemberExpression memberExpr)
                return memberExpr;

            if (expression is UnaryExpression unaryExpression)
            {
                expression = unaryExpression.Operand;
                memberExpr = expression as MemberExpression;
                if (memberExpr != null)
                    return memberExpr;
            }

            return null;
        }

        /// <summary>
        /// The get custom expression.
        /// </summary>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public static Expression GetCustomExpression<T>(this Expression<Func<T, object>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var expression = selector.Body;
            return expression;
        }

        /// <summary>
        /// The get property type.
        /// </summary>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static Type GetPropertyType<T>(this Expression<Func<T, object>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var expression = selector.Body;

            Type result = null;

            while (true)
            {
                var memberExpr = expression as MemberExpression;
                if (memberExpr == null)
                    if (expression is UnaryExpression unaryExpression)
                    {
                        expression = unaryExpression.Operand;
                        continue;
                    }

                if (memberExpr == null)
                    break;

                result = ((PropertyInfo)memberExpr.Member).PropertyType;
                break;
            }

            // По-умолчание используется строковое значение
            return result ?? typeof(string);
        }
    }
}