using System;

namespace TradeSimulator
{
    public class Account
    {
        public Account(decimal openingBalance)
        {
            CashBalance = openingBalance;
        }

        public decimal CashBalance { get; private set; }
        public decimal StockBalance { get; private set; }
        public string CurrentSymbol { get; private set; }
        public int CurrentStockQuantity { get; private set; }

        public decimal TotalBalance
        {
            get { return CashBalance + StockBalance; }
        }

        public void Buy(string symbol, decimal price)
        {
            if(CurrentStockQuantity != 0)
            {
                throw new Exception("Account already owns stock");
            }

            CurrentSymbol = symbol;
            CurrentStockQuantity = (int)Math.Floor(CashBalance / price);
           // CurrentStockQuantity = CashBalance / price;
            StockBalance = CurrentStockQuantity * price;
            CashBalance = CashBalance - StockBalance;
        }

        public void Sell(decimal price)
        {
            if (CurrentStockQuantity == 0)
            {
                throw new Exception("No stock to sell");
            }

            CurrentSymbol = string.Empty;
            CashBalance += CurrentStockQuantity * price;
            StockBalance = 0;
            CurrentStockQuantity = 0;
        }
    }
}
