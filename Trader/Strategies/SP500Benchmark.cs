using System;
using System.Collections.Generic;
using System.Linq;
using Trader.Domain;

namespace Trader.Strategies
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

        public override void Initialize()
        {
            Account = new Account(0, 0);
            Account.AllowPartialholdings = true;
        }

        protected override void ExecuteStrategyImplementation()
        {
            var symbol = Symbols.Single();
            Account.Buy(symbol);
        }
    }
}
