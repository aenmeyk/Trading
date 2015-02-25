using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common.Models;
using TradeSimulator.Model;

namespace TradeSimulator.Strategies
{
    public class RebalanceWithoutSelling : StrategyBase
    {
        protected override string Name
        {
            get { return "Rebalance Without Selling"; }
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

        //protected override decimal Spread { get { return 0M; } }
        //protected override decimal TradingFee { get { return 0M; } }
        //protected override decimal TaxRate { get { return 0M; } }

        protected override decimal Spread { get { return 0.000704704M; } }
        protected override decimal TradingFee { get { return 0M; } }
        protected override decimal TaxRate { get { return 0M; } }

        protected override void DepositCashImplementation(DateTime date, decimal amount)
        {
            var quotes = TodayQuotes.Values;

            if (quotes.Any())
            {
                var desiredPercent = 1.0M / Symbols.Count();
                var totalValue = Account.Portfolio.TotalValue + amount;
                var purchaseRequests = new Collection<PurchaseRequest>();

                //foreach (var position in Account.Portfolio.PositionDictionary.Values)
                foreach (var quote in quotes)
                {
                    var allocation = desiredPercent;

                    if (Account.Portfolio.PositionDictionary.ContainsKey(quote.Symbol))
                    {
                        var position = Account.Portfolio.PositionDictionary[quote.Symbol];
                        allocation = desiredPercent - (position.CurrentValue / totalValue);
                    }

                    if (allocation > 0)
                    {
                        var purchaseRequest = new PurchaseRequest { Quote = quote, Percent = allocation };
                        purchaseRequests.Add(purchaseRequest);
                    }
                }

                var totalAllocation = purchaseRequests.Sum(x => x.Percent);

                foreach (var purchaseRequest in purchaseRequests)
                {
                    purchaseRequest.Percent = purchaseRequest.Percent / totalAllocation;
                }

                Account.Buy(purchaseRequests);
            }
        }
    }
}
