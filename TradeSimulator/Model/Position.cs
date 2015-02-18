using System;

namespace TradeSimulator.Model
{
    public class Position
    {
        public Position(string symbol, decimal quantity, decimal purchasePrice, decimal transactionFee, DateTime purchaseDate)
        {
            Symbol = symbol;
            Quantity = quantity;
            PurchasePrice = purchasePrice;
            CurrentPrice = purchasePrice;
            PurchaseDate = purchaseDate;
            CostBasis = (quantity * purchasePrice) + transactionFee;
        }

        public string Symbol { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal PurchasePrice { get; private set; }
        public decimal CurrentPrice { get; set; }
        public DateTime PurchaseDate { get; private set; }
        public decimal CostBasis { get; private set; }
        public decimal CurrentValue
        {
            get { return Quantity * CurrentPrice; }
        }
    }
}
