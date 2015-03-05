using System;
using Trader.Domain;

namespace Trader.Strategies
{
    public class BuyLoser : StrategyBase
    {
        private DateTime _previousDate = DateTime.MinValue;
        private string _currentSymbol = string.Empty;

        public override string Name
        {
            get { return "Buy Loser"; }
        }

        protected override void ExecuteStrategyImplementation()
        {
            if((Market.Today - _previousDate) < TimeSpan.FromDays(1))
            {
                return;
            }

            var loser = string.Empty;
            var smallestGain = decimal.MaxValue;
            var previousQuotes = Market.HistoricalQuoteDictionary.DaysBack(1);

            //var previousQuotes = Market.HistoricalQuoteDictionary.ContainsKey(_previousDate)
            //    ? Market.HistoricalQuoteDictionary[_previousDate]
            //    : Market.QuoteDictionary;

            foreach (var symbol in AvailableSymbols)
            {
                var currentPrice = Market.QuoteDictionary[symbol].SalePrice;
                var previousPrice = previousQuotes.ContainsKey(symbol)
                    ? previousQuotes[symbol].PurchasePrice
                    : currentPrice;

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
