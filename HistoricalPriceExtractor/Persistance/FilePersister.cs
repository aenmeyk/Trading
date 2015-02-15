using System.IO;

namespace HistoricalPriceExtractor.Persistance
{
    public class FilePersister : IPersister
    {
        private string _path;

        public FilePersister(string path)
        {
            _path = path;
        }

        public void Persist(string symbol, string historicalPrices)
        {
            var path = string.Format(_path, symbol);
            File.WriteAllText(path, historicalPrices);
        }
    }
}
