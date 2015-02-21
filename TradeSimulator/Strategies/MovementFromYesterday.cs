using System.Collections.Generic;

namespace TradeSimulator.Strategies
{
    public class MovementFromYesterday : MovementFromYesterdayBase
    {
        protected override string Name
        {
            get { return "Movement from yesterday - Select symbols"; }
        }

        protected override decimal TradingFee { get { return 0M; } }
        protected override decimal Spread { get { return 0M; } }
        protected override decimal TaxRate { get { return 0M; } }

        //protected override decimal TradingFee { get { return 8.95M; } }
        //protected override decimal Spread { get { return 0.00007549500M; } }
        //protected override decimal TaxRate { get { return 0.28M; } }


        public override IEnumerable<string> Symbols
        {
            get
            {
                return new[]
                {
                    "RSP",
                    "SCHX",
                    "SPLV",
                    "SCHB",
                    "SCHD",
                    "RPV",
                    "RPG",
                    "LOWC",
                    "PIN",
                    "PKW",
                    "SCHG",
                    "SCHH",
                    "SCHV",
                };
            }
        }
    }
}
