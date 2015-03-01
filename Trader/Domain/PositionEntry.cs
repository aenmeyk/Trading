using System;

namespace Trader.Domain
{
    public class PositionEntry
    {
        public PositionEntry(string symbol, DateTime date, decimal quantity, decimal costBasis)
        {
            Symbol = symbol;
            Date = date;
            Quantity = quantity;
            CostBasis = costBasis;
        }

        public string Symbol { get; private set; }
        public DateTime Date { get; private set; }
        public decimal Quantity { get; set; }
        public decimal CostBasis { get; private set; }

        public decimal CostBasisPerShare
        {
            get { return CostBasis / Quantity; }
        }

        public decimal ShortTermGains
        {
            get
            {
                if (!IsShortTerm)
                {
                    return 0;
                }

                if (_profit <= 0)
                {
                    return 0;
                }

                return _profit;
            }
        }

        public decimal LongTermGains
        {
            get
            {
                if (IsShortTerm)
                {
                    return 0;
                }

                if (_profit <= 0)
                {
                    return 0;
                }

                return _profit;
            }
        }

        public decimal ShortTermLosses
        {
            get
            {
                if (!IsShortTerm)
                {
                    return 0;
                }

                if (_profit >= 0)
                {
                    return 0;
                }

                return _profit;
            }
        }

        public decimal LongTermLosses
        {
            get
            {
                if (IsShortTerm)
                {
                    return 0;
                }

                if (_profit >= 0)
                {
                    return 0;
                }

                return _profit;
            }
        }

        public decimal ShortTermProfitPerShare
        {
            get
            {
                if (!IsShortTerm)
                {
                    return 0;
                }

                return _profit / Quantity;
            }
        }

        public decimal LongTermProfitPerShare
        {
            get
            {
                if (IsShortTerm)
                {
                    return 0;
                }

                return _profit / Quantity;
            }
        }

        public bool IsShortTerm
        {
            get { return Date.AddYears(1) > Market.Today; }
        }

        private decimal _profit
        {
            get
            {
                var quote = Market.QuoteDictionary[Symbol];
                var currentValue = quote.SalePrice * Quantity;

                return currentValue - CostBasis;
            }
        }
    }
}
