using System.Collections.Generic;

namespace Trader.Strategies
{
    public class ActualFixedAllocation : FixedAllocationBase
    {
        protected override Dictionary<string, decimal> Allocations
        {
            get
            {
                return new Dictionary<string, decimal>
                {
                    { "SCHX", 0.40M },
                    { "SCHM", 0.16M },
                    { "SCHA", 0.11M },
                    { "SCHF", 0.21M },
                    { "SCHE", 0.05M },
                    { "SCHH", 0.045M },
                    { "VNQI", 0.025M }
                };
            }
        }

        public override string Name
        {
            get { return "Actual Fixed Allocation"; }
        }
    }
}
