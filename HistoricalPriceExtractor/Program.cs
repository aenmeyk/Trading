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
            //  var yahooExtractor = new Extractor(Constants.YAHOO_QUOTE_URL);
            var googleExtractor = new Extractor(Constants.GOOGLE_QUOTE_URL);
            var filePersister = new FilePersister(CommonConstants.PERSISTANCE_PATH);
            // var yahooPersister = new SqlPersister(CommonConstants.CONNECTION_STRING, "PriceHistory");
            var googlePersister = new SqlPersister(CommonConstants.CONNECTION_STRING, "PriceHistoryGoogle");
            var symbols = symbolProvider.GetSymbols();

            foreach (var symbol in symbols)
            {
                try
                {
                    Console.WriteLine("Getting historical prices for: {0}", symbol);
                    // var yahooPrices = yahooExtractor.GetHistoricalPrices(symbol).Result;
                    var googlePrices = googleExtractor.GetHistoricalPrices(symbol).Result;
                    // filePersister.Persist(symbol, yahooPrices);
                    //yahooPersister.Persist(symbol, yahooPrices);
                    googlePersister.Persist(symbol, googlePrices);
                }
                catch
                {
                    Console.WriteLine("Bad Symbol: {0}", symbol);
                }

                Thread.Sleep(Constants.REQUEST_DELAY_MILLISECONDS);
            }

            //yahooPersister.Dispose();
            googlePersister.Dispose();
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
