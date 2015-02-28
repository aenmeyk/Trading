using System;

namespace Common.Models
{
    public class PriceHistory
    {
        public string Symbol { get; set; }
        public DateTime DateValue { get; set; }
        public decimal HighPrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal AdjustedClosePrice { get; set; }
        public long Volume { get; set; }
    }
}
