using System;
using System.Collections.Generic;
using Common.Models;

namespace Trader.Domain
{
    public static class Market
    {
        static Market()
        {
            StockDictionary = new Dictionary<string, Stock>();
            QuoteDictionary = new Dictionary<string, Quote>();
        }

        public static DateTime Today { get; set; }
        public static IDictionary<string, Stock> StockDictionary { get; set; }
        public static IDictionary<string, Quote> QuoteDictionary { get; set; }

    }
}
