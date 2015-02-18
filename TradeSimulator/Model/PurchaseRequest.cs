using Common.Models;

namespace TradeSimulator.Model
{
    public class PurchaseRequest
    {
        public Quote Quote { get; set; }
        public decimal Percent { get; set; }
    }
}
