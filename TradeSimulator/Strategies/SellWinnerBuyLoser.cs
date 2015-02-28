using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Models;
using TradeSimulator.Model;

namespace TradeSimulator.Strategies
{
    public class SellWinnerBuyLoser : StrategyBase
    {
        private DateTime _lastDate = DateTime.MinValue;
        private Dictionary<string, PriceHistory> _previousQuotes = new Dictionary<string, PriceHistory>();

        protected override string Name
        {
            get { return "Sell Winner Buy Loser"; }
        }
        public override IEnumerable<string> Symbols
        {
            get
            {
                return new[]
                {
"EWG",
"EWU",
"EWM",
"EWO",
"EWS",
"EWN",
"EWK",
"EWD",
"EWL",
"EWQ",
"EWC",
"EWH",
"EWA",
"EWI",
"EWP",
                };
            }
        }

        protected override decimal Spread { get { return 0M; } }
        protected override decimal TradingFee { get { return 0M; } }
        protected override decimal TaxRate { get { return 0M; } }

        protected override void ExecuteStrategyImplementation(DateTime date)
        {
            if ((date - _lastDate).TotalDays >= 15)
            {
                var quotes = TodayQuotes.Values;
                var lowestGrowth = decimal.MaxValue;
                var selectedQuote = quotes.First();

                foreach (var symbol in Symbols)
                {
                    if (_previousQuotes.ContainsKey(symbol))
                    {
                        var prevoiusQuote = _previousQuotes[symbol];
                        var currentQuote = quotes.Single(x => x.Symbol == symbol);
                        var growth = currentQuote.AdjustedClosePrice / prevoiusQuote.AdjustedClosePrice;

                        if (growth < lowestGrowth)
                        {
                            selectedQuote = currentQuote;
                            lowestGrowth = growth;
                        }
                    }
                }

                var purchaseRequest = new PurchaseRequest { Quote = selectedQuote, Percent = 1 };
                Account.Buy(new[] { purchaseRequest });

                _lastDate = date;
                _previousQuotes = quotes.ToDictionary(x => x.Symbol);
            }
        }
    }
}
