using System;
using Common.Models;
using Trader.Domain;

namespace Tests
{
    public abstract class TestBase
    {
        protected const decimal PRECISION = 0.0000001M;

        public TestBase()
        {
            Market.StockDictionary.Clear();
            Market.QuoteDictionary.Clear();

            Market.Today = new DateTime(2010, 1, 1);
            Market.StockDictionary.Add(StockA.Symbol, StockA);
            Market.StockDictionary.Add(StockB.Symbol, StockB);
            Market.StockDictionary.Add(StockC.Symbol, StockC);
            Market.StockDictionary.Add(StockD.Symbol, StockD);
            Market.StockDictionary.Add(StockE.Symbol, StockE);
            Market.StockDictionary.Add(StockF.Symbol, StockF);
            SetPrice(StockA, 100);
            SetPrice(StockB, 200);
            SetPrice(StockC, 10);
            SetPrice(StockD, 15);
            SetPrice(StockE, 20);
            SetPrice(StockF, 25);
        }

        protected Stock StockA = new Stock
        {
            Symbol = "A",
            Spread = 0.002M,
            SchwabOneSource = false,
            IsIndex = false
        };

        protected Stock StockB = new Stock
        {
            Symbol = "B",
            Spread = 0.001M,
            SchwabOneSource = false,
            IsIndex = false
        };

        protected Stock StockC = new Stock
        {
            Symbol = "C",
            Spread = 0M,
            SchwabOneSource = true
        };

        protected Stock StockD = new Stock
        {
            Symbol = "D",
            Spread = 0M,
            SchwabOneSource = true
        };

        protected Stock StockE = new Stock
        {
            Symbol = "E",
            Spread = 0M,
            SchwabOneSource = true
        };

        protected Stock StockF = new Stock
        {
            Symbol = "F",
            Spread = 0M,
            SchwabOneSource = true
        };

        protected void SetPrice(Stock stock, decimal price)
        {
            var priceHistory = new PriceHistory
            {
                Symbol = stock.Symbol,
                DateValue = Market.Today,
                AdjustedClosePrice = price
            };

            Market.QuoteDictionary[stock.Symbol] = new Quote(stock, priceHistory);
        }
    }
}
