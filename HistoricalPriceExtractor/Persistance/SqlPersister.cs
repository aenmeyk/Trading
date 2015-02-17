using DataAccess.Repositories;

namespace HistoricalPriceExtractor.Persistance
{
    public class SqlPersister : IPersister
    {
        private PriceHistoryRepositoryBase _repository;

        public SqlPersister(PriceHistoryRepositoryBase repository)
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
