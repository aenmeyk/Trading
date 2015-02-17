namespace DataAccess.Repositories
{
    public class HistoricalPricesGoogleRepository : HistoricalPricesRepositoryBase
    {
        protected override string TableName
        {
            get { return "PriceHistoryGoogle"; }
        }
    }
}
