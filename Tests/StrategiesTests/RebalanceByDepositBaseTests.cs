using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trader.Domain;
using Trader.Strategies;

namespace Tests.StrategiesTests
{

    [TestClass]
    public class RebalanceByDepositBaseTests : TestBase
    {
        private class Fake : RebalanceByDepositBase
        {
            private Dictionary<string, decimal> _allocations = new Dictionary<string, decimal>
            {
                { "C", 0.2M },
                { "D", 0.3M },
                { "E", 0.4M },
                { "F", 0.1M },
            };

            protected override Dictionary<string, decimal> Allocations
            {
                get { return _allocations; }
            }

            protected override string Name
            {
                get { return "Stub"; }
            }

            public Dictionary<string, decimal> CurrentAllocations
            {
                get { return Account.CurrentAllocations; }
            }

            public override void Initialize()
            {
                Account = new Account(0, 0);
                Account.AllowPartialholdings = true;
            }
            
            public void SetAllocations(Dictionary<string, decimal> allocations)
            {
                _allocations = allocations;
            }
        }

        [TestMethod]
        public void DepositCash_should_allocate_funds_correctly()
        {
            var fake = new Fake();
            fake.Initialize();
            fake.DepositCash(1000);
            fake.CurrentAllocations["C"].Should().Be(0.2M);
            fake.CurrentAllocations["D"].Should().Be(0.3M);
            fake.CurrentAllocations["E"].Should().Be(0.4M);
            fake.CurrentAllocations["F"].Should().Be(0.1M);

            fake.DepositCash(200);
            fake.CurrentAllocations["C"].Should().Be(0.2M);
            fake.CurrentAllocations["D"].Should().Be(0.3M);
            fake.CurrentAllocations["E"].Should().Be(0.4M);
            fake.CurrentAllocations["F"].Should().Be(0.1M);
        }

        [TestMethod]
        public void DepositCash_should_allocate_funds_to_stocks_below_desired_level()
        {
            var firstAllocations = new Dictionary<string, decimal>
            {
                { "C", 0.1M },
                { "D", 0.55M },
                { "E", 0.3M },
                { "F", 0.05M },
            };

            var secondAllocations = new Dictionary<string, decimal>
            {
                { "C", 0.25M },
                { "D", 0.25M },
                { "E", 0.25M },
                { "F", 0.25M },
            };

            var fake = new Fake();
            fake.Initialize();
            fake.SetAllocations(firstAllocations);
            fake.DepositCash(100);
            fake.CurrentAllocations["C"].Should().Be(0.1M);
            fake.CurrentAllocations["D"].Should().Be(0.55M);
            fake.CurrentAllocations["E"].Should().Be(0.3M);
            fake.CurrentAllocations["F"].Should().Be(0.05M);

            fake.SetAllocations(secondAllocations);
            fake.DepositCash(100);
            fake.CurrentAllocations["C"].Should().BeApproximately(0.2416667M, PRECISION);
            fake.CurrentAllocations["D"].Should().BeApproximately(0.2750000M, PRECISION);
            fake.CurrentAllocations["E"].Should().BeApproximately(0.2416667M, PRECISION);
            fake.CurrentAllocations["F"].Should().BeApproximately(0.2416667M, PRECISION);
        }
    }
}
