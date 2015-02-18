using System;
using System.Collections.Generic;
using System.Linq;
using Common.Models;
using TradeSimulator.Model;

namespace TradeSimulator.Strategies
{
    public class SP500Benchmark : StrategyBase
    {
        protected override string Name
        {
            get { return "S&P 500 Benchmark"; }
        }

        public override IEnumerable<string> Symbols
        {
            get { return new[] { "^GSPC" }; }
        }

        protected override decimal Spread
        {
            get { return 0M; }
        }

        protected override decimal TradingFee
        {
            get { return 0M; }
        }

        protected override decimal TaxRate
        {
            get { return 0M; }
        }

        protected override void ExecuteStrategyImplementation(DateTime date, IEnumerable<Quote> quotes)
        {
            var selectedQuote = quotes.SingleOrDefault();

            if (selectedQuote != null && !Account.Portfolio.Positions.Any())
            {
                var purchaseRequest = new PurchaseRequest { Quote = selectedQuote, Percent = 1 };
                Account.Buy(new[] { purchaseRequest });
            }
        }
    }
}
