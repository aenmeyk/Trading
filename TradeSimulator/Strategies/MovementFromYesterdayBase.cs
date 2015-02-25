using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Models;
using TradeSimulator.Model;

namespace TradeSimulator.Strategies
{
    public abstract class MovementFromYesterdayBase : StrategyBase
    {
        private DateTime _lastDate = DateTime.MinValue;
        private Dictionary<string, Quote> _previousDayQuotes;

        protected override decimal TradingFee
        {
            get { return 8.95M; }
        }

        protected override decimal TaxRate
        {
            get { return 0.28M; }
        }


        protected override void ExecuteStrategyImplementation(DateTime date)
        {
            if ((date - _lastDate).TotalDays >= 3)
            {
                var quotes = TodayQuotes.Values;
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

                if (!Account.Portfolio.PositionDictionary.ContainsKey(selectedQuote.Symbol))
                {
                    var purchaseRequest = new PurchaseRequest { Quote = selectedQuote, Percent = 1 };
                    Account.Liquidate(date);
                    Account.Buy(new[] { purchaseRequest });
                }

                _lastDate = date;
            }

            _previousDayQuotes = TodayQuotes;
        }
    }
}
