using System.Linq;
using Trader.Domain;

namespace Trader.Strategies
{
    public class EvenAllocation : StrategyBase
    {
        public override string Name
        {
            get { return "Even Allocation"; }
        }

        public override void DepositCash(decimal amount)
        {
            Account.DepositCash(amount);
            var cashAvailableToTrade = Account.CashAvailableToTrade;
            var symbolCount = AvailableSymbols.Count();

            foreach (var symbol in AvailableSymbols)
            {
                var amountToBuy = cashAvailableToTrade / symbolCount;
                Account.Buy(symbol, amountToBuy);
            }
        }
    }
}
