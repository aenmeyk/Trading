using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common.Models;
using TradeSimulator.Model;

namespace TradeSimulator.Strategies
{
    public class ActualAllocationBenchmark : StrategyBase
    {
        protected override string Name
        {
            get { return "Actual Allocation Benchmark"; }
        }

        public override IEnumerable<string> Symbols
        {
            get { return _allocations.Keys; }
        }

        protected override decimal Spread
        {
            //get { return 0M; }
            get { return 0.000579016666M; }
        }

        protected override decimal TradingFee
        {
            get { return 0M; }
        }

        protected override decimal TaxRate
        {
            //get { return 0M; }
            get { return 0.28M; }
        }

        private Dictionary<string, decimal> _allocations = new Dictionary<string, decimal>
        {
            { "SCHX", 0.40M },
            { "SCHM", 0.16M },
            { "SCHA", 0.11M },
            { "SCHF", 0.21M },
            { "SCHE", 0.05M },
            { "SCHH", 0.045M },
            { "VNQI", 0.025M }
        };

        protected override void ExecuteStrategyImplementation(DateTime date)
        {
            var quotes = TodayQuotes.Values;

            if (quotes.Any())
            {
                var purchaseRequests = new Collection<PurchaseRequest>();

                foreach (var quote in quotes)
                {
                    if (!Account.Portfolio.PositionDictionary.ContainsKey(quote.Symbol))
                    {
                        var percent = _allocations[quote.Symbol];
                        var purchaseRequest = new PurchaseRequest { Quote = quote, Percent = percent };
                        purchaseRequests.Add(purchaseRequest);
                    }
                }

                Account.Buy(purchaseRequests);
            }
        }
    }
}
