namespace Universe.Sp.CQRS.Extensions
{
    using DataAccess;
    using DataAccess.Models;
    using Microsoft.SharePoint;

    public static class SpContextExtensions
    {
        public static SPList Set<TEntitySp>(this IUniverseSpContext ctx) where TEntitySp : EntitySp, new()
        {
            var listUrl = new TEntitySp().ListUrl;
            var list = ctx.Web.GetList(listUrl);

            return list;
        }
    }
}