namespace HistoricalPriceExtractor.Persistance
{
    public interface IPersister
    {
        void Persist(string symbol, string historicalPrices);
    }
}
