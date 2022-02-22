namespace Universe.SharePoint.DataAccess.Test
{
    using Sp.DataAccess;

    public class UniverseSpTestContext : UniverseSpContext
    {
        public UniverseSpTestContext(string webUrl, string webLogin) : base(webUrl, webLogin)
        {
        }
    }
}