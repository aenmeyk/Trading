using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common.Models;
using TradeSimulator.Model;

namespace TradeSimulator.Strategies
{
    public class EvenAllocation : StrategyBase
    {
        protected override string Name
        {
            get { return "Even Allocation Benchmark"; }
        }

        protected override bool PrintRunningBalance { get { return false; } }

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
            var quotes = TodayQuotes.Values;

            if (quotes.Any())
            {
                var purchaseRequests = new Collection<PurchaseRequest>();
                var percent = 1.0M / Symbols.Count();

                foreach (var quote in quotes)
                {
                    var purchaseRequest = new PurchaseRequest { Quote = quote, Percent = percent };
                    purchaseRequests.Add(purchaseRequest);
                }

                Account.Liquidate(date);
                Account.Buy(purchaseRequests);
            }
        }
    }
}
