using System;
using Trader.Domain;

namespace Trader.Strategies
{
    public class BuyLoser : StrategyBase
    {
        private DateTime _previousDate = DateTime.MinValue;
        private string _currentSymbol = string.Empty;

        protected override string Name
        {
            get { return "Buy Loser"; }
        }

        public override void Initialize()
        {
            Account = new Account(0, 0);
        }

        protected override void ExecuteStrategyImplementation()
        {
            if((Market.Today - _previousDate) < TimeSpan.FromDays(25))
            {
                return;
            }

            var loser = string.Empty;
            var smallestGain = decimal.MaxValue;
            var previousQuotes = Market.HistoricalQuoteDictionary.DaysBack(2);

            //var previousQuotes = Market.HistoricalQuoteDictionary.ContainsKey(_previousDate)
            //    ? Market.HistoricalQuoteDictionary[_previousDate]
            //    : Market.QuoteDictionary;

            foreach (var symbol in Symbols)
            {
                var previousPrice = previousQuotes[symbol].PurchasePrice;
                var currentPrice = Market.QuoteDictionary[symbol].SalePrice;
                var gain = currentPrice / previousPrice;

                if (gain < smallestGain)
                {
                    smallestGain = gain;
                    loser = symbol;
                }
            }

            if(_currentSymbol != loser)
            {
                Account.Liquidate();
                Account.Buy(loser);
            }

            _previousDate = Market.Today;
        }
    }
}
