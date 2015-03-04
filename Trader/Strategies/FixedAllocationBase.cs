using System.Collections.Generic;
using Trader.Domain;

namespace Trader.Strategies
{
    public abstract class FixedAllocationBase : StrategyBase
    {
        protected abstract Dictionary<string, decimal> Allocations { get; }

        public override IEnumerable<string> Symbols
        {
            get { return Allocations.Keys; }
        }

        public override void DepositCash(decimal amount)
        {
            Account.DepositCash(amount);
            var cashAvailableToTrade = Account.CashAvailableToTrade;

            foreach (var allocation in Allocations)
            {
                var amountToBuy = cashAvailableToTrade * allocation.Value;
                Account.Buy(allocation.Key, amountToBuy);
            }
        }
    }
}
