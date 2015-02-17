namespace DataAccess.Repositories
{
    public class HistoricalPricesYahooRepository : HistoricalPricesRepositoryBase
    {
        protected override string TableName
        {
            get { return "PriceHistory"; }
        }
    }
}
