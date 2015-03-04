using System;
using Common.Models;

namespace Trader.Domain
{
    public class Quote
    {
        private decimal _adjustedClosePrice;

        public Quote(Stock stock, PriceHistory priceHistory)
        {
            Stock = stock;
            DateValue = priceHistory.DateValue;
            HighPrice = priceHistory.HighPrice;
            LowPrice = priceHistory.LowPrice;
            ClosePrice = priceHistory.ClosePrice;
            Volume = priceHistory.Volume;
            _adjustedClosePrice = priceHistory.AdjustedClosePrice;
        }

        public Stock Stock { get; private set; }
        public DateTime DateValue { get; private set; }
        public decimal HighPrice { get; private set; }
        public decimal LowPrice { get; private set; }
        public decimal ClosePrice { get; private set; }
        public long Volume { get; private set; }

        public decimal PurchasePrice
        {
            get { return _adjustedClosePrice + _spreadValue; }
        }

        public decimal SalePrice
        {
            get { return _adjustedClosePrice - _spreadValue; }
        }

        private decimal _spreadValue
        {
            get { return _adjustedClosePrice * (Stock.Spread / 2); }
        }
    }
}
