using System.IO;

namespace HistoricalPriceExtractor.Persistance
{
    public interface IPersister
    {
        void Persist(string ticker, Stream priceStream);
    }
}
