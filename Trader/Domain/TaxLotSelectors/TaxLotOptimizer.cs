using System;
using System.Collections.Generic;
using System.Linq;

namespace Trader.Domain.TaxLotSelectors
{
    public class TaxLotOptimizer
    {
        public SellTransaction SellPositionEntries(string symbol, decimal quantity, List<PositionEntry> positionEntries)
        {
            var quantityRemaining = quantity;
            var shortTermTaxableAmount = 0M;
            var longTermTaxableAmount = 0M;
            var isShortTermSale = false;
            var isLongTermSale = false;

            var orderedPositionEntries = positionEntries
                .OrderBy(x => x.ShortTermGains)
                .OrderBy(x => x.LongTermGains)
                .OrderBy(x => x.LongTermLosses)
                .OrderBy(x => x.ShortTermLosses);

            foreach (var positionEntry in orderedPositionEntries)
            {
                var quantityToRemove = Math.Min(quantity, positionEntry.Quantity);
                shortTermTaxableAmount += positionEntry.ShortTermProfitPerShare * quantityToRemove;
                longTermTaxableAmount += positionEntry.LongTermProfitPerShare * quantityToRemove;
                positionEntry.Quantity -= quantityToRemove;
                quantityRemaining -= quantityToRemove;

                if (positionEntry.IsShortTerm)
                {
                    isShortTermSale = true;
                }
                else
                {
                    isLongTermSale = true;
                }

                if (quantityRemaining == 0)
                {
                    break;
                }
            }

            var quote = Market.QuoteDictionary[symbol];
            var fee = quote.Stock.TradingFee;

            if (isShortTermSale)
            {
                shortTermTaxableAmount -= fee;
            }
            else if (isLongTermSale)
            {
                longTermTaxableAmount -= fee;
            }

            if (quantityRemaining != 0)
            {
                throw new Exception("Error trying to sell position entries. Remaining quantity is not zero.");
            }

            var sellTransaction = new SellTransaction(symbol, quantity, shortTermTaxableAmount, longTermTaxableAmount);
            positionEntries.RemoveAll(x => x.Quantity == 0);

            return sellTransaction;
        }
    }
}
