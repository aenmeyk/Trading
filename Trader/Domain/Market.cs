using System;
using System.Collections.Generic;
using System.Linq;
using Common.Models;

namespace Trader.Domain
{
    public static class Market
    {
        static Market()
        {
            StockDictionary = new Dictionary<string, Stock>();
            QuoteDictionary = new Dictionary<string, Quote>();
            HistoricalQuoteDictionary = new Dictionary<DateTime, IDictionary<string, Quote>>();
        }

        public static DateTime Today { get; set; }
        public static IDictionary<string, Stock> StockDictionary { get; set; }
        public static IDictionary<string, Quote> QuoteDictionary { get; set; }
        public static IDictionary<DateTime, IDictionary<string, Quote>> HistoricalQuoteDictionary { get; set; }

        public static IDictionary<string, Quote> DaysBack(this IDictionary<DateTime, IDictionary<string, Quote>> value, int daysBack)
        {
            var date = value.Keys
                .Where(x => x < Today)
                .OrderByDescending(x => x)
                .Take(daysBack)
                .LastOrDefault();

            if (date > default(DateTime))
            {
                return HistoricalQuoteDictionary[date];
            }

            return HistoricalQuoteDictionary[Today];
        }
    }
}
