using Common.Models;

namespace TradeSimulator.Model
{
    public class PurchaseRequest
    {
        public PriceHistory Quote { get; set; }
        public decimal Percent { get; set; }
    }
}
