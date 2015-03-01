using System;
using Common;
using Common.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trader.Domain;

namespace Tests
{
    // Tests:
    //  - TaxLotOptimizer - Test different long vs. short term gains/losses.

    [TestClass]
    public class AccountTests
    {
        public const decimal SHORT_TERM_TAX_RATE = 0.2M;
        public const decimal LONG_TERM_TAX_RATE = 0.1M;
        private const decimal PRECISION = 0.0000001M;

        private Stock _stockA = new Stock
        {
            Symbol = "A",
            Spread = 0.002M,
            SchwabOneSource = false,
            IsIndex = false
        };

        private Stock _stockB = new Stock
        {
            Symbol = "B",
            Spread = 0.001M,
            SchwabOneSource = false,
            IsIndex = false
        };

        private Account _account;

        [TestInitialize]
        public void Initialize()
        {
            Market.StockDictionary.Clear();
            Market.QuoteDictionary.Clear();

            Market.Today = new DateTime(2010, 1, 1);
            Market.StockDictionary.Add(_stockA.Symbol, _stockA);
            Market.StockDictionary.Add(_stockB.Symbol, _stockB);
            SetPrice(_stockA, 100);
            SetPrice(_stockB, 200);

            _account = new Account(SHORT_TERM_TAX_RATE, LONG_TERM_TAX_RATE);
        }

        [TestMethod]
        public void Account_buy_and_liquidate()
        {
            // Initialize Account
            _account.Value.Should().Be(0);

            // Make initial deposit
            var depositAmount = 1000M;
            _account.DepositCash(depositAmount);
            _account.Value.Should().Be(depositAmount);

            // Buy stock
            _account.Buy(_stockA.Symbol);
            _account.Value.Should().BeApproximately(987.0937126M, PRECISION);

            // Liquidate
            _account.Liquidate();
            _account.Value.Should().BeApproximately(982.5149701M, PRECISION);
        }

        [TestMethod]
        public void Account_buy_and_liquidate_two_stocks()
        {
            // Make initial deposit
            _account.DepositCash(1000M);

            // Buy first stock
            _account.Buy(_stockA.Symbol, 500);
            _account.Value.Should().BeApproximately(989.0897206M, PRECISION);

            _account.Buy(_stockB.Symbol, 500);
            _account.Value.Should().BeApproximately(979.1586017M, PRECISION);

            // Liquidate
            _account.Liquidate();
            _account.Value.Should().BeApproximately(969.0068813M, PRECISION);
        }

        private void SetPrice(Stock stock, decimal price)
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
