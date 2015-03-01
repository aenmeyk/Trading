using System;
using System.Linq;

namespace Trader.Domain
{
    public class Account
    {
        private Portfolio _portfolio = new Portfolio();
        private decimal _cashValue = 0;
        private decimal _shortTermTaxableAmount = 0M;
        private decimal _longTermTaxableAmount = 0M;
        private decimal _shortTermTaxRate;
        private decimal _longTermTaxRate;

        public Account(decimal shortTermTaxRate, decimal longTermTaxRate)
        {
            _shortTermTaxRate = shortTermTaxRate;
            _longTermTaxRate = longTermTaxRate;
        }

        private decimal _cash
        {
            get { return _cashValue; }
            set
            {
                if (value < -0.001M)
                {
                    throw new Exception("Cash balance below zero.");
                }

                _cashValue = value;
            }
        }

        private decimal _cashAvailableToTrade
        {
            get { return _cashValue - _taxesDue; }
        }

        private decimal _taxesDue
        {
            get
            {
                var shortTermTaxDue = _shortTermTaxableAmount * _shortTermTaxRate;
                var longTermTaxDue = _longTermTaxableAmount * _longTermTaxRate;
                var totalTaxDue = shortTermTaxDue + longTermTaxDue;

                return totalTaxDue;
            }
        }

        public decimal Value
        {
            get { return _portfolio.Value + _cash - _taxesDue; }
        }

        public void DepositCash(decimal amount)
        {
            _cash += amount;
        }

        public void Buy(string symbol)
        {
            var value = _cashAvailableToTrade;
            Buy(symbol, value);
        }

        public void Buy(string symbol, decimal value)
        {
            var quote = Market.QuoteDictionary[symbol];
            var quantity = (value - quote.Stock.TradingFee) / quote.PurchasePrice;

            if (quantity > 0)
            {
                var buyTransaction = _portfolio.Buy(symbol, quantity);
                _cash -= buyTransaction.CostBasis;
            }
        }

        public void Sell(string symbol, decimal quantity)
        {
            var sellTransaction = _portfolio.Sell(symbol, quantity);
            HandleSellTransaction(sellTransaction);
        }

        public void SellAll(string symbol)
        {
            var sellTransaction = _portfolio.SellAll(symbol);
            HandleSellTransaction(sellTransaction);
        }

        public void Liquidate()
        {
            var sellTransactions = _portfolio.Liquidate();

            foreach (var sellTransaction in sellTransactions)
            {
                HandleSellTransaction(sellTransaction);
            }
        }

        public void PayTaxes()
        {
            _cashValue -= _taxesDue;
            _shortTermTaxableAmount = 0;
            _longTermTaxableAmount = 0;
        }

        public void PrintStatement()
        {
            //var annualGrowth = GetAnnualGrowth(currentDate);

            //Console.WriteLine("Opening Balance: {0,12:n}", _openingBalance);
            Console.WriteLine("Closing Value: {0,12:n}", Value);
            //Console.WriteLine("Total Growth:    {0,14:p}", (TotalValue / _openingBalance) - 1);
            //Console.WriteLine("Annual Growth:   {0,14:p}", annualGrowth);
        }

        private void HandleSellTransaction(SellTransaction sellTransaction)
        {
            _shortTermTaxableAmount += sellTransaction.ShortTermTaxableAmount;
            _longTermTaxableAmount += sellTransaction.LongTermTaxableAmount;
            _cash += sellTransaction.TotalCashReturned;
        }
    }
}
