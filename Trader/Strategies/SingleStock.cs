using System;
using System.Collections.Generic;
using System.Linq;
using Trader.Domain;

namespace Trader.Strategies
{
    public class SingleStock : StrategyBase
    {
        private string _symbol;
        public SingleStock(string symbol)
        {
            _symbol = symbol;
        }

        public override string Name
        {
            get { return _symbol; }
        }

        public override IEnumerable<string> Symbols
        {
            get { return new[] { _symbol }; }
        }

        public override void Initialize()
        {
            Account = new Account(0, 0);
            Account.AllowPartialholdings = true;
        }

        protected override void ExecuteStrategyImplementation()
        {
            var symbol = AvailableSymbols.Single();
            Account.Buy(symbol);
        }
    }
}
