using System;
using System.Threading;
using Common;
using DataAccess.Repositories;
using HistoricalPriceExtractor.Persistance;

namespace HistoricalPriceExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            var symbolProvider = new SymbolProvider();
            var yahooExtractor = new Extractor(Constants.YAHOO_QUOTE_URL);
            var googleExtractor = new Extractor(Constants.GOOGLE_QUOTE_URL);
            var filePersister = new FilePersister(CommonConstants.PERSISTANCE_PATH);
            var yahooPersister = new SqlPersister(new PriceHistoryYahooRepository());
            var googlePersister = new SqlPersister(new PriceHistoryGoogleRepository());
            var symbols = symbolProvider.GetSymbols();

            foreach (var symbol in symbols)
            {
                Console.WriteLine("Getting historical prices for: {0}", symbol);

                try
                {
                    var yahooPrices = yahooExtractor.GetHistoricalPrices(symbol).Result;
                    yahooPersister.Persist(symbol, yahooPrices);
                    filePersister.Persist(symbol, yahooPrices);
                }
                catch
                {
                    Console.WriteLine("Yahoo Bad Symbol: {0}", symbol);
                }

                try
                {
                    var googlePrices = googleExtractor.GetHistoricalPrices(symbol).Result;
                    googlePersister.Persist(symbol, googlePrices);
                }
                catch
                {
                    Console.WriteLine("Google Bad Symbol: {0}", symbol);
                }

                Thread.Sleep(Constants.REQUEST_DELAY_MILLISECONDS);
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
