namespace DataAccess.Repositories
{
    public class PriceHistoryYahooRepository : PriceHistoryRepositoryBase
    {
        protected override string TableName
        {
            get { return "dbo.PriceHistory"; }
        }
    }
}
