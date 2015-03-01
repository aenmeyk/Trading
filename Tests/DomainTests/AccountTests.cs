using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trader.Domain;

namespace Tests.DomainTests
{
    [TestClass]
    public class AccountTests : TestBase
    {
        public const decimal SHORT_TERM_TAX_RATE = 0.2M;
        public const decimal LONG_TERM_TAX_RATE = 0.1M;

        private Account _account;

        [TestInitialize]
        public void Initialize()
        {
            _account = new Account(SHORT_TERM_TAX_RATE, LONG_TERM_TAX_RATE);
            _account.AllowPartialholdings = true;
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
            _account.Buy(StockA.Symbol);
            _account.Value.Should().BeApproximately(987.0937126M, PRECISION);

            // Liquidate
            _account.Liquidate();
            _account.Value.Should().BeApproximately(982.5149701M, PRECISION);
        }

        [TestMethod]
        public void Account_buy_and_liquidate_SchwabOneSource()
        {
            // Initialize Account
            _account.Value.Should().Be(0);

            // Make initial deposit
            var depositAmount = 1000M;
            _account.DepositCash(depositAmount);
            _account.Value.Should().Be(depositAmount);

            // Buy stock
            _account.Buy(StockA.Symbol);
            _account.Value.Should().BeApproximately(987.0937126M, PRECISION);

            // Liquidate
            _account.Liquidate();
            _account.Value.Should().BeApproximately(982.5149701M, PRECISION);
        }

        [TestMethod]
        public void Account_buy_and_liquidate_whole_shares()
        {
            // Initialize Account
            _account.AllowPartialholdings = false;

            // Make initial deposit
            var depositAmount = 950M;
            _account.DepositCash(depositAmount);
            _account.Buy(StockA.Symbol);
            _account.Value.Should().BeApproximately(937.4500000M, PRECISION);

            // Liquidate
            _account.Liquidate();
            _account.Value.Should().BeApproximately(932.8000000M, PRECISION);
        }

        [TestMethod]
        public void Account_buy_and_liquidate_two_stocks()
        {
            StockA.SchwabOneSource = true;

            // Buy stock
            _account.DepositCash(1000M);
            _account.Buy(StockA.Symbol);
            _account.Value.Should().BeApproximately(996.0079840M, PRECISION);

            // Liquidate
            _account.Liquidate();
            _account.Value.Should().BeApproximately(996.8063872M, PRECISION);
        }

        [TestMethod]
        public void Account_buy_and_sell_two_stocks()
        {
            // Buy stocks
            _account.DepositCash(1000M);
            _account.Buy(StockA.Symbol, 500);
            _account.Buy(StockB.Symbol, 500);

            // Sell first stock
            _account.SellAll(StockA.Symbol);
            _account.Value.Should().BeApproximately(974.1806576M, PRECISION);

            // Sell second stock
            _account.SellAll(StockB.Symbol);
            _account.Value.Should().BeApproximately(969.0068813M, PRECISION);
        }

        [TestMethod]
        public void Account_buy_and_sell_value()
        {
            // Buy stocks
            _account.DepositCash(1000M);
            _account.Buy(StockA.Symbol, 500);
            _account.Buy(StockB.Symbol, 500);

            // Sell first stock
            _account.Sell(StockA.Symbol, 300);
            _account.Value.Should().BeApproximately(973.2971106M, PRECISION);

            // Sell second stock
            _account.Sell(StockB.Symbol, 200);
            _account.Value.Should().BeApproximately(966.9114263M, PRECISION);
        }

        [TestMethod]
        public void Account_buy_deposit_buy()
        {
            // Buy first batch
            _account.DepositCash(1000M);
            _account.Buy(StockA.Symbol);

            // Update stock price
            SetPrice(StockA, 150);

            // Buy second batch
            _account.DepositCash(1000M);
            _account.Buy(StockA.Symbol);
            _account.Value.Should().BeApproximately(2467.7342814M, PRECISION);
        }

        [TestMethod]
        public void Account_buy_liquidate_and_pay_taxes()
        {
            // Buy stock
            var depositAmount = 1000M;
            _account.DepositCash(depositAmount);
            _account.Buy(StockA.Symbol);

            // Update stock price
            SetPrice(StockA, 150);

            // Liquidate
            _account.Liquidate();

            // Pay taxes
            _account.PayTaxes();
            _account.Value.Should().BeApproximately(1377.3524551M, PRECISION);
            _account.TaxesPaid.Should().BeApproximately(94.3381138M, PRECISION);
        }

        [TestMethod]
        public void Account_buy_and_liquidate_many_purchases_of_same_stock()
        {
            // Buy initial stock
            _account.DepositCash(1000M);
            _account.Buy(StockA.Symbol, 400);

            // Buy second batch
            SetPrice(StockA, 150);
            _account.Buy(StockA.Symbol, 400);
            _account.Value.Should().BeApproximately(1173.7223054M, PRECISION);

            // Buy third batch
            SetPrice(StockA, 300);
            _account.Buy(StockA.Symbol, 200);
            _account.Value.Should().BeApproximately(2137.7319361M, PRECISION);

            // Update stock price
            SetPrice(StockA, 250);
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
            _account.Buy(StockA.Symbol);

            // Update price
            Market.Today = Market.Today.Add(new TimeSpan(366, 0, 0, 0));
            SetPrice(StockA, 150);

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
            SetPrice(StockA, 10);
            _account.Buy(StockA.Symbol, 200);

            // Buy batch 2
            Market.Today = Market.Today.Add(new TimeSpan(100, 0, 0, 0));
            SetPrice(StockA, 15);
            _account.Buy(StockA.Symbol, 200);

            // Buy batch 3
            Market.Today = Market.Today.Add(new TimeSpan(100, 0, 0, 0));
            SetPrice(StockA, 20);
            _account.Buy(StockA.Symbol, 200);

            // Buy batch 4
            Market.Today = Market.Today.Add(new TimeSpan(100, 0, 0, 0));
            SetPrice(StockA, 25);
            _account.Buy(StockA.Symbol, 200);

            // Buy batch 5
            Market.Today = Market.Today.Add(new TimeSpan(100, 0, 0, 0));
            SetPrice(StockA, 30);
            _account.Buy(StockA.Symbol, 200);

            SetPrice(StockA, 35);
            _account.Value.Should().BeApproximately(1931.4163523M, PRECISION);

            // Sell batch 1
            _account.SellQuantity(StockA.Symbol, 5);
            _account.Value.Should().BeApproximately(1911.1410526M, PRECISION);

            // Sell batch 2
            _account.SellQuantity(StockA.Symbol, 10);
            _account.Value.Should().BeApproximately(1878.6454533M, PRECISION);

            // Sell batch 3
            _account.SellQuantity(StockA.Symbol, 10);
            _account.Value.Should().BeApproximately(1857.4379264M, PRECISION);

            // Sell batch 4
            _account.SellQuantity(StockA.Symbol, 10);
            _account.Value.Should().BeApproximately(1831.2618590M, PRECISION);

            // Sell batch 5
            _account.SellQuantity(StockA.Symbol, 10);
            _account.Value.Should().BeApproximately(1793.6638254M, PRECISION);

            // Sell batch 6
            _account.SellQuantity(StockA.Symbol, 10);
            _account.Value.Should().BeApproximately(1748.1120274M, PRECISION);

            // Sell batch 7
            _account.SellAll(StockA.Symbol);
            _account.Value.Should().BeApproximately(1739.8236457M, PRECISION);
        }
    }
}
