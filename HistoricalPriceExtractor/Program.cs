using System;
using System.Threading;
using Common;
using HistoricalPriceExtractor.Persistance;

namespace HistoricalPriceExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            var symbolProvider = new SymbolProvider();
            var extractor = new Extractor(Constants.QUOTE_URL);
            var filePersister = new FilePersister(CommonConstants.PERSISTANCE_PATH);
            var sqlPersister = new SqlPersister(CommonConstants.CONNECTION_STRING);
            var symbols = symbolProvider.GetSymbols();

            foreach(var symbol in symbols)
            {
                Console.WriteLine("Getting historical prices for: {0}", symbol);
                var priceStream = extractor.GetHistoricalPrices(symbol).Result;
                filePersister.Persist(symbol, priceStream);
                sqlPersister.Persist(symbol, priceStream);
                Thread.Sleep(Constants.REQUEST_DELAY_MILLISECONDS);
            }

            sqlPersister.Dispose();
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
