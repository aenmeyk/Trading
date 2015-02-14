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

        public void Persist(string ticker, Stream priceStream)
        {
            var path = string.Format(_path, ticker);

            using (var fileStream = File.Create(path))
            {
                priceStream.Seek(0, SeekOrigin.Begin);
                priceStream.CopyTo(fileStream);
            }
        }
    }
}
