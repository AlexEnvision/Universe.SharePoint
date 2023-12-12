using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Universe.Sp.CSOM.CQRS.Extensions
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public static class MetaInfoHelper
    {
        public static Dictionary<string, Expression<Func<TEntityDb, object>>> FieldMap<TEntityDb>(
            params KeyValuePair<string, Expression<Func<TEntityDb, object>>>[] records)
            where TEntityDb : class 
        {
            return records.ToDictionary(x => x.Key, x => x.Value);
        }

        public static KeyValuePair<string, Expression<Func<TEntityDb, object>>> MapRule
            <TEntityDb>(string key, Expression<Func<TEntityDb, object>> expressionRule)
            where TEntityDb : class 
        {
            return new KeyValuePair<string, Expression<Func<TEntityDb, object>>>(key, expressionRule);
        }
    }
}