using System;
using System.Collections.Generic;
using System.Linq;
using Trader.Domain.TaxLotSelectors;

namespace Trader.Domain
{
    public class Position
    {
        private TaxLotOptimizer _taxLotOptimizer = new TaxLotOptimizer();
        private List<PositionEntry> _positionEntries = new List<PositionEntry>();

        public Position(string symbol)
        {
            Symbol = symbol;
        }

        public string Symbol { get; private set; }

        public decimal Quantity
        {
            get { return _positionEntries.Sum(x => x.Quantity); }
        }

        public decimal CurrentValue
        {
            get { return Market.QuoteDictionary[Symbol].AdjustedClosePrice * Quantity; }
        }

        public decimal CostBasisPerShare
        {
            get { return CostBasis / Quantity; }
        }

        public decimal CostBasis
        {
            get { return _positionEntries.Sum(x => x.CostBasis); }
        }

        public void AddStock(BuyTransaction transaction)
        {
            var positionEntry = new PositionEntry(Symbol, transaction.Quote.DateValue, transaction.Quantity, transaction.CostBasis);
            _positionEntries.Add(positionEntry);
        }

        public SellTransaction RemoveStock()
        {
            var quantity = _positionEntries.Sum(x => x.Quantity);
            return RemoveStock(quantity);
        }

        public SellTransaction RemoveStock(decimal quantity)
        {
            if (quantity > Quantity)
            {
                throw new Exception("Trying to remove more stock than is available");
            }

            var sellTransaction = _taxLotOptimizer.SellPositionEntries(Symbol, quantity, _positionEntries);

            return sellTransaction;
        }
    }
}
