using System;
using System.Collections.Generic;
using System.Linq;
using Common.Models;
using TradeSimulator.Model;

namespace TradeSimulator.Strategies
{
    public abstract class MovementFromYesterdayBase : StrategyBase
    {
        private Dictionary<string, Quote> _previousDayQuotes;

        protected override decimal TradingFee
        {
            get { return 8.95M; }
        }

        protected override decimal TaxRate
        {
            get { return 0.28M; }
        }

        protected override void ExecuteStrategyImplementation(DateTime date, IEnumerable<Quote> quotes)
        {
            var lowestGrowth = decimal.MaxValue;
            var selectedQuote = quotes.First();

            if (_previousDayQuotes != null && _previousDayQuotes.Count() > 0)
            {
                foreach (var quote in quotes)
                {
                    if (_previousDayQuotes.ContainsKey(quote.Symbol))
                    {
                        var previousDayQuote = _previousDayQuotes[quote.Symbol];
                        var growth = quote.AdjustedClosePrice / previousDayQuote.AdjustedClosePrice;

                        if (growth < lowestGrowth)
                        {
                            selectedQuote = quote;
                            lowestGrowth = growth;
                        }
                    }
                }
            }

            if (!Account.Portfolio.Positions.ContainsKey(selectedQuote.Symbol))
            {
                var purchaseRequest = new PurchaseRequest { Quote = selectedQuote, Percent = 1 };
                Account.Liquidate(date);
                Account.Buy(new[] { purchaseRequest });
            }

            _previousDayQuotes = quotes.ToDictionary(x => x.Symbol);
        }
    }
}
