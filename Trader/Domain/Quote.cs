using System;
using Common.Models;

namespace Trader.Domain
{
    public class Quote
    {
        public Quote(Stock stock, PriceHistory priceHistory)
        {
            Stock = stock;
            DateValue = priceHistory.DateValue;
            HighPrice = priceHistory.HighPrice;
            LowPrice = priceHistory.LowPrice;
            ClosePrice = priceHistory.ClosePrice;
            AdjustedClosePrice = priceHistory.AdjustedClosePrice;
            Volume = priceHistory.Volume;
        }

        public Stock Stock { get; private set; }
        public DateTime DateValue { get; private set; }
        public decimal HighPrice { get; private set; }
        public decimal LowPrice { get; private set; }
        public decimal ClosePrice { get; private set; }
        public decimal AdjustedClosePrice { get; private set; }
        public long Volume { get; private set; }
    }
}
