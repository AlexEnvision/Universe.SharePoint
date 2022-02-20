using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Universe.Sp.Common.Caml;
using Universe.Sp.CQRS.Models.Filter;

namespace Universe.Sp.CQRS.Models
{
    public class QueryBuilder<T> where T: class
    {
        public SPQuery SpQuery { get; set; }

        public string CamlWhere { get; set; }

        public QueryBuilder<T> WhereByFilters(List<CamlChainRule> filters)
        {
            var chains = filters.Select(x => x.Chain).ToArray();

            CamlWhere = CamlHelper.GetCamlWhere(CamlHelper.CamlChain(
                CamlHelper.LogicalOperators.OR,
                chains));

            return this;
        }
    }
}