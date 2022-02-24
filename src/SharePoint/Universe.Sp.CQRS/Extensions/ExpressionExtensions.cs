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
using System.Linq.Expressions;

namespace Universe.Sp.CQRS.Extensions
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public static class ExpressionExtensions
    {
        public static Expression<Func<TEntitySp, object>> CreateExpressionDbeUniversal<TEntitySp>(string propertyName)
            where TEntitySp : class
        {
            var dbEntityType = typeof(TEntitySp);
            var hasProp = dbEntityType.GetProperty(propertyName) != null;
            if (!hasProp)
                return null;

            var param = Expression.Parameter(dbEntityType, "e");
            Expression body = Expression.PropertyOrField(param, propertyName);

            var getterExpression = Expression.Lambda<Func<TEntitySp, object>>(Expression.TypeAs(body, typeof(object)), param);
            return getterExpression;
        }

        public static Expression<Func<TEntitySp, object>> CreateExpressionDbeWithChildUniversal<TEntitySp>(string propertyName, string childProperty)
            where TEntitySp : class
        {
            var dbEntityType = typeof(TEntitySp);
            var hasProp = dbEntityType.GetProperty(propertyName) != null;
            if (!hasProp)
                return null;

            var param = Expression.Parameter(dbEntityType, "e");
            Expression body = Expression.PropertyOrField(param, propertyName);
            Expression childbody = Expression.PropertyOrField(body, childProperty);

            var getterExpression = Expression.Lambda<Func<TEntitySp, object>>(Expression.TypeAs(childbody, typeof(object)), param);
            return getterExpression;
        }

        public static Expression<Func<TEntitySp, object>> CreateExpressionDbeWithChainPropsUniversal<TEntitySp>(List<string> propChain)
            where TEntitySp : class
        {
            var dbEntityType = typeof(TEntitySp);
            if (propChain == null || propChain.Count == 0)
                throw new ArgumentException("Не указана цепочка свойств!");

            var param = Expression.Parameter(dbEntityType, "e");
            var rootProp = propChain.FirstOrDefault();
            if (rootProp == null)
                throw new ArgumentException("Не найдено корневое свойство!");

            Expression body = Expression.PropertyOrField(param, rootProp);
            for (var index = 1; index < propChain.Count; index++)
            {
                var propertyName = propChain[index];
                var hasProp = dbEntityType.GetProperty(propertyName) != null;
                if (!hasProp)
                    return null;

                body = Expression.PropertyOrField(body, propertyName);
            }

            var getterExpression = Expression.Lambda<Func<TEntitySp, object>>(Expression.TypeAs(body, typeof(object)), param);
            return getterExpression;
        }

        public static Expression<Func<TEntitySp, object>> CreateExpressionDbeWithChainPropsAndChindProp
            <TEntitySp>(List<string> propChain, string childProperty)
            where TEntitySp : class
        {
            var dbEntityType = typeof(TEntitySp);
            if (propChain == null || propChain.Count == 0)
                throw new ArgumentException("Не указана цепочка свойств!");

            var param = Expression.Parameter(dbEntityType, "e");
            var rootProp = propChain.FirstOrDefault();
            if (rootProp == null)
                throw new ArgumentException("Не найдено корневое свойство!");

            Expression body = Expression.PropertyOrField(param, rootProp);
            for (var index = 1; index < propChain.Count; index++)
            {
                var propertyName = propChain[index];
                var hasProp = dbEntityType.GetProperty(propertyName) != null;
                if (!hasProp)
                    return null;

                body = Expression.PropertyOrField(body, propertyName);
            }

            Expression childbody = Expression.PropertyOrField(body, childProperty);

            var getterExpression = Expression.Lambda<Func<TEntitySp, object>>(Expression.TypeAs(childbody, typeof(object)), param);
            return getterExpression;
        }
     
        public static Expression<Func<T, object>> ConvertFunction<T>(LambdaExpression function)
        {
            ParameterExpression p = Expression.Parameter(typeof(T));
            return Expression.Lambda<Func<T, object>>(
                Expression.TypeAs(Expression.Invoke(function, Expression.Convert(p, typeof(T))), typeof(object)),
                p);
        }
    }
}