﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common.Models;
using TradeSimulator.Model;

namespace TradeSimulator.Strategies
{
    public class AlwaysAllocateDepositEvenly : StrategyBase
    {
        protected override string Name
        {
            get { return "Always Allocate Deposit Evenly"; }
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
                var purchaseRequests = new Collection<PurchaseRequest>();
                var percent = 1.0M / Symbols.Count();

                foreach (var quote in quotes)
                {
                    var purchaseRequest = new PurchaseRequest { Quote = quote, Percent = percent };
                    purchaseRequests.Add(purchaseRequest);
                }

                Account.Buy(purchaseRequests);
            }
        }
    }
}
