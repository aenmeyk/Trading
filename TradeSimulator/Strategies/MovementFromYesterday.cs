using System.Collections.Generic;

namespace TradeSimulator.Strategies
{
    public class MovementFromYesterday : MovementFromYesterdayBase
    {
        protected override string Name
        {
            get { return "Movement from yesterday - Select symbols"; }
        }

        protected override decimal Spread
        {
            get { return 0.000764670833M; }
        }

        public override IEnumerable<string> Symbols
        {
            get
            {
                return new[]
                {
                    "SCHX",
                    "SCHM",
                    "SCHA",
                    "SCHF",
                    "SCHE",
                    "SCHH",
                    "SCHG",
                    "SCHV",
                    "PIN",
                    "GXC",
                    "GMF",
                    "FEU"
                };
            }
        }
    }
}
