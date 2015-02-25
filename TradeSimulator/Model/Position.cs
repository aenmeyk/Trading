namespace TradeSimulator.Model
{
    public class Position
    {
        public Position(string symbol)
        {
            Symbol = symbol;
        }

        public string Symbol { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal PurchasePrice { get; private set; }
        public decimal CurrentPrice { get; set; }
        public decimal CostBasis { get; private set; }
        public decimal CurrentValue
        {
            get { return Quantity * CurrentPrice; }
        }

        public void AddStock(decimal quantity, decimal purchasePrice, decimal transactionFee)
        {
            PurchasePrice = ((PurchasePrice * Quantity) + (purchasePrice * quantity)) / (Quantity + quantity);
            Quantity += quantity;
            CurrentPrice = purchasePrice;
            CostBasis += (quantity * purchasePrice) + transactionFee;
        }
    }
}
