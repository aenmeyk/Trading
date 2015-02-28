namespace DataAccess.Repositories
{
    public class StockRepository : SymbolRepositoryBase
    {
        protected override string TableName
        {
            get { return "dbo.Stock"; }
        }
    }
}
