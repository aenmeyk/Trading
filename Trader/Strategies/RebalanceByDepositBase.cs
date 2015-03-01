using System.Collections.Generic;
using System.Linq;
using Trader.Domain;

namespace Trader.Strategies
{
    public abstract class RebalanceByDepositBase : StrategyBase
    {
        protected abstract Dictionary<string, decimal> Allocations { get; }

        public override IEnumerable<string> Symbols
        {
            get { return Allocations.Keys; }
        }

        public override void DepositCash(decimal amount)
        {
            Account.DepositCash(amount);
            var currentAccountValue = Account.Value;
            var desiredValues = new Dictionary<string, decimal>();
            var currentValues = Account.CurrentValues;

            foreach (var allocation in Allocations)
            {
                desiredValues.Add(allocation.Key, allocation.Value * currentAccountValue);

                if (!currentValues.ContainsKey(allocation.Key))
                {
                    currentValues.Add(allocation.Key, 0);
                }
            }

            var allocationsBelowDesiredValue = currentValues
                .Where(x => x.Value < desiredValues[x.Key]);

            var symbolsBelowDesiredValue = allocationsBelowDesiredValue
                .Select(x => x.Key);

            var totalPercentBelowDesired = Allocations
                .Where(x => symbolsBelowDesiredValue.Contains(x.Key))
                .Sum(x => x.Value);

            var totalAmountBelowDesired = allocationsBelowDesiredValue
                .Sum(x => x.Value);

            var totalFundsInPlay = Account.CashAvailableToTrade + totalAmountBelowDesired;

            foreach (var symbol in symbolsBelowDesiredValue)
            {
                var percentOfTotalBelowDesired = Allocations[symbol] / totalPercentBelowDesired;
                var desiredAmount = percentOfTotalBelowDesired * totalFundsInPlay;
                var currentAmount = currentValues[symbol];
                var amountToBuy = desiredAmount - currentAmount;

                Account.Buy(symbol, amountToBuy);
            }
        }
    }
}
