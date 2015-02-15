using System;

namespace Common.Models
{
    public class Quote
    {
        public string Symbol { get; set; }
        public DateTime DateValue { get; set; }
        public decimal AdjustedClosePrice { get; set; }
        public long Volume { get; set; }
        public decimal Growth { get; set; }
    }
}
