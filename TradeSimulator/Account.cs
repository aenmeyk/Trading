using System;
using Common.Models;

namespace TradeSimulator
{
    public class Account
    {
        private decimal capitalGains = 0;

        private decimal taxBalance
        {
            get { return capitalGains * Constants.TAX_RATE; }
        }

        public Account(decimal openingBalance)
        {
            CashBalance = openingBalance;
            TransactionDate = DateTime.MinValue;
        }

        public DateTime TransactionDate { get; private set; }
        public decimal CashBalance { get; private set; }
        public decimal PurchasePrice { get; private set; }
        public string CurrentSymbol { get; private set; }
        public decimal CurrentStockQuantity { get; private set; }

        public decimal TotalBalance
        {
            get { return CashBalance + (PurchasePrice * CurrentStockQuantity) - taxBalance; }
        }

        public void Buy(Quote quote)
        {
            if (CurrentStockQuantity != 0)
            {
                throw new Exception("Account already owns stock");
            }

            // If it is a new year, pay the taxes.
            if (TransactionDate.Year != quote.DateValue.Year)
            {
                if (capitalGains > 0)
                {
                    CashBalance -= taxBalance;
                }

                capitalGains = 0;
            }

            var spreadMultiplier = 1 + Constants.AVG_SPREAD;
            PurchasePrice = quote.AdjustedClosePrice * spreadMultiplier;

            if ((CashBalance - Constants.TRADING_FEE) / PurchasePrice > 0)
            {
                CurrentSymbol = quote.Symbol;
                CurrentStockQuantity = (int)Math.Floor((CashBalance - Constants.TRADING_FEE) / PurchasePrice);
                CashBalance = CashBalance - (PurchasePrice * CurrentStockQuantity) - Constants.TRADING_FEE;
                TransactionDate = quote.DateValue;
            }
        }

        public void Sell(decimal salePrice)
        {
            if (CurrentStockQuantity == 0)
            {
                throw new Exception("No stock to sell");
            }

            // Reduce the sale price by the spread
            var spreadMultiplier = 1 - Constants.AVG_SPREAD;
            salePrice = salePrice * spreadMultiplier;
            var purchaseValue = CurrentStockQuantity * PurchasePrice;
            var saleValue = CurrentStockQuantity * salePrice;

            capitalGains += saleValue - purchaseValue - (Constants.TRADING_FEE * 2);
            CashBalance += saleValue - Constants.TRADING_FEE;

            PurchasePrice = 0;
            CurrentStockQuantity = 0;
            CurrentSymbol = string.Empty;
        }
    }
}
