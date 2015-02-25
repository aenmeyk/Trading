using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Models;
using TradeSimulator.Model;

namespace TradeSimulator.Strategies
{
    public class BuyLoser : StrategyBase
    {
        private DateTime _previousPurchaseDate = DateTime.MinValue;

        protected override string Name
        {
            get { return "Buy Loser"; }
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

        //protected override decimal Spread { get { return 0M; } }
        //protected override decimal TradingFee { get { return 0M; } }
        //protected override decimal TaxRate { get { return 0M; } }

        protected override decimal Spread { get { return 0.000704704M; } }
        protected override decimal TradingFee { get { return 0M; } }
        protected override decimal TaxRate { get { return 0M; } }

        protected override void DepositCashImplementation(DateTime date, decimal amount)
        {
            var quotes = TodayQuotes.Values;
            var lowestGrowth = decimal.MaxValue;
            var selectedQuote = quotes.First();

            foreach (var symbol in Symbols)
            {
                if (AllQuotes.ContainsKey(_previousPurchaseDate))
                {
                    var previousQuotes = AllQuotes[_previousPurchaseDate];
                    var prevoiusQuote = previousQuotes.FirstOrDefault(x => x.Symbol == symbol);

                    if (prevoiusQuote != null)
                    {
                        var currentQuote = quotes.Single(x => x.Symbol == symbol);
                        var growth = currentQuote.AdjustedClosePrice / prevoiusQuote.AdjustedClosePrice;

                        if (growth < lowestGrowth)
                        {
                            selectedQuote = currentQuote;
                            lowestGrowth = growth;
                        }
                    }
                }
            }

            var purchaseRequest = new PurchaseRequest { Quote = selectedQuote, Percent = 1 };
            Account.Buy(new[] { purchaseRequest });

            _previousPurchaseDate = date;
        }
    }
}
