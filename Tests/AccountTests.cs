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

            // Buy second stock
            _account.Buy(_stockB.Symbol, 500);
            _account.Value.Should().BeApproximately(979.1586017M, PRECISION);

            // Liquidate
            _account.Liquidate();
            _account.Value.Should().BeApproximately(969.0068813M, PRECISION);
        }

        [TestMethod]
        public void Account_buy_and_sell_two_stocks()
        {
            // Buy stocks
            _account.DepositCash(1000M);
            _account.Buy(_stockA.Symbol, 500);
            _account.Buy(_stockB.Symbol, 500);

            // Sell first stock
            _account.SellAll(_stockA.Symbol);
            _account.Value.Should().BeApproximately(974.1806576M, PRECISION);

            // Sell second stock
            _account.SellAll(_stockB.Symbol);
            _account.Value.Should().BeApproximately(969.0068813M, PRECISION);
        }

        [TestMethod]
        public void Account_buy_and_liquidate_many_purchases_of_same_stock()
        {
            // Buy initial stock
            _account.DepositCash(1000M);
            _account.Buy(_stockA.Symbol, 400);

            // Buy second batch
            SetPrice(_stockA, 150);
            _account.Buy(_stockA.Symbol, 400);
            _account.Value.Should().BeApproximately(1173.7223054M, PRECISION);

            // Buy third batch
            SetPrice(_stockA, 300);
            _account.Buy(_stockA.Symbol, 200);
            _account.Value.Should().BeApproximately(2137.7319361M, PRECISION);

            // Update stock price
            SetPrice(_stockA, 250);
            _account.Value.Should().BeApproximately(1781.4432801M, PRECISION);

            // Liquidate stock
            _account.Liquidate();
            _account.Value.Should().BeApproximately(1617.9946241M, PRECISION);
        }

        [TestMethod]
        public void Account_buy_and_liquidate_long_term()
        {
            // Buy initial stock
            _account.DepositCash(1000M);
            _account.Buy(_stockA.Symbol);

            // Update price
            Market.Today = Market.Today.Add(new TimeSpan(366, 0, 0, 0));
            SetPrice(_stockA, 150);

            // Liquidate stock
            _account.Liquidate();
            _account.Value.Should().BeApproximately(1424.5215120M, PRECISION);
        }

        [TestMethod]
        public void Account_buy_and_sell_in_stages()
        {
            // Buy initial stock
            _account.DepositCash(1000M);

            // Buy batch 1
            SetPrice(_stockA, 10);
            _account.Buy(_stockA.Symbol, 200);

            // Buy batch 2
            Market.Today = Market.Today.Add(new TimeSpan(100, 0, 0, 0));
            SetPrice(_stockA, 15);
            _account.Buy(_stockA.Symbol, 200);

            // Buy batch 3
            Market.Today = Market.Today.Add(new TimeSpan(100, 0, 0, 0));
            SetPrice(_stockA, 20);
            _account.Buy(_stockA.Symbol, 200);

            // Buy batch 4
            Market.Today = Market.Today.Add(new TimeSpan(100, 0, 0, 0));
            SetPrice(_stockA, 25);
            _account.Buy(_stockA.Symbol, 200);

            // Buy batch 5
            Market.Today = Market.Today.Add(new TimeSpan(100, 0, 0, 0));
            SetPrice(_stockA, 30);
            _account.Buy(_stockA.Symbol, 200);

            SetPrice(_stockA, 35);
            _account.Value.Should().BeApproximately(1931.4163523M, PRECISION);

            // Sell batch 1
            _account.Sell(_stockA.Symbol, 5);
            _account.Value.Should().BeApproximately(1911.1410526M, PRECISION);

            // Sell batch 2
            _account.Sell(_stockA.Symbol, 10);
            _account.Value.Should().BeApproximately(1878.6454533M, PRECISION);

            // Sell batch 3
            _account.Sell(_stockA.Symbol, 10);
            _account.Value.Should().BeApproximately(1857.4379264M, PRECISION);

            // Sell batch 4
            _account.Sell(_stockA.Symbol, 10);
            _account.Value.Should().BeApproximately(1831.2618590M, PRECISION);

            // Sell batch 5
            _account.Sell(_stockA.Symbol, 10);
            _account.Value.Should().BeApproximately(1793.6638254M, PRECISION);

            // Sell batch 6
            _account.Sell(_stockA.Symbol, 10);
            _account.Value.Should().BeApproximately(1748.1120274M, PRECISION);

            // Sell batch 7
            _account.SellAll(_stockA.Symbol);
            _account.Value.Should().BeApproximately(1739.8236457M, PRECISION);
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
