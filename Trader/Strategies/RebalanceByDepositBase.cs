using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            var deficits = currentValues
                .Where(x => x.Value < desiredValues[x.Key])
                .Select(x => new { Symbol = x.Key, Deficit = desiredValues[x.Key] - x.Value })
                .OrderByDescending(x => x.Deficit);

            var symbolsInPlay = new Collection<string>();
            var amountsToBuy = new Dictionary<string, decimal>();
            var nextItemIndex = 1;
            var cashAvailableToTrade = Account.CashAvailableToTrade;

            foreach (var deficit in deficits)
            {
                symbolsInPlay.Add(deficit.Symbol);
                var currentItemDeficit = deficit.Deficit;
                var amountToBuy = 0M;

                if (nextItemIndex < deficits.Count())
                {
                    var nextItemDeficit = deficits.ElementAt(nextItemIndex).Deficit;
                    amountToBuy = Math.Min(cashAvailableToTrade / symbolsInPlay.Count(), currentItemDeficit - nextItemDeficit);
                }
                else
                {
                    amountToBuy = cashAvailableToTrade / symbolsInPlay.Count();
                }

                foreach (var symbol in symbolsInPlay)
                {
                    if (amountsToBuy.ContainsKey(symbol))
                    {
                        amountsToBuy[symbol] += amountToBuy;
                    }
                    else
                    {
                        amountsToBuy.Add(symbol, amountToBuy);
                    }
                }

                cashAvailableToTrade -= amountToBuy * symbolsInPlay.Count();
                nextItemIndex++;
            }

            foreach (var amountToBuy in amountsToBuy)
            {
                Account.Buy(amountToBuy.Key, amountToBuy.Value);
            }
        }
    }
}
