using DataAccess.Repositories;

namespace HistoricalPriceExtractor.Persistance
{
    public class SqlPersister : IPersister
    {
        private HistoricalPricesRepositoryBase _repository;

        public SqlPersister(HistoricalPricesRepositoryBase repository)
        {
            _repository = repository;
        }

        public void Persist(string symbol, string historicalPrices)
        {
            _repository.DeleteForSymbol(symbol);
            _repository.InsertHistoricalPrices(symbol, historicalPrices);
        }
    }
}
