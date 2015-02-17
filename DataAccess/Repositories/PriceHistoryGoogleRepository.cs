namespace DataAccess.Repositories
{
    public class PriceHistoryGoogleRepository : PriceHistoryRepositoryBase
    {
        protected override string TableName
        {
            get { return "dbo.PriceHistoryGoogle"; }
        }
    }
}
