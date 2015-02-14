using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HistoricalPriceExtractor
{
    public class TickerProvider
    {
        public IEnumerable<string> GetTickers()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "HistoricalPriceExtractor.Tickers.txt";
            string tickerString;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    tickerString = reader.ReadToEnd();
                }
            }

            var tickers = tickerString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return tickers;
        }
    }
}
