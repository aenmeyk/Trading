using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trader.Domain;

namespace Tests.DomainTests
{
    [TestClass]
    public class PortfolioTests : TestBase
    {
        [TestMethod]
        public void CurrentAllocations_should_return_current_allocations()
        {
            StockA.Spread = 0;
            StockB.Spread = 0;
            var portfolio = new Portfolio();
            portfolio.Buy(StockA.Symbol, 75);
            portfolio.Buy(StockB.Symbol, 25);

            var allocations = portfolio.CurrentAllocations;

            allocations[StockA.Symbol].Should().Be(0.6M);
            allocations[StockB.Symbol].Should().Be(0.4M);
        }

        [TestMethod]
        public void CurrentValues_should_return_current_allocations()
        {
            StockA.Spread = 0;
            StockB.Spread = 0;
            var portfolio = new Portfolio();
            portfolio.Buy(StockA.Symbol, 75);
            portfolio.Buy(StockB.Symbol, 25);

            var allocations = portfolio.CurrentValues;

            allocations[StockA.Symbol].Should().Be(7500M);
            allocations[StockB.Symbol].Should().Be(5000M);
        }
    }
}
