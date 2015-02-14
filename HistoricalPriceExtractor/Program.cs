using System;
using System.Threading;
using HistoricalPriceExtractor.Persistance;

namespace HistoricalPriceExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            var tickerProvider = new TickerProvider();
            var extractor = new Extractor(Constants.QUOTE_URL);
            var persister = new FilePersister(Constants.PERSISTANCE_PATH);
            var tickers = tickerProvider.GetTickers();

            foreach(var ticker in tickers)
            {
                Console.WriteLine("Getting historical prices for: {0}", ticker);
                var priceStream = extractor.GetHistoricalPrices(ticker).Result;
                persister.Persist(ticker, priceStream);
                Thread.Sleep(Constants.REQUEST_DELAY_MILLISECONDS);
            }

            Console.WriteLine("Done");
        }
    }
}
