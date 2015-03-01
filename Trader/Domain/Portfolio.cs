using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Trader.Domain
{
    public class Portfolio
    {
        private ICollection<Transaction> _transactions = new Collection<Transaction>();
        private IDictionary<string, Position> _positionDictionary = new Dictionary<string, Position>();

        public decimal Value
        {
            get { return _positionDictionary.Values.Sum(x => x.CurrentValue); }
        }

        public decimal Profit
        {
            get { return _positionDictionary.Values.Sum(x => x.CurrentValue - x.CostBasis); }
        }

        public Dictionary<string, decimal> CurrentAllocations
        {
            get
            {
                var currentAllocations = new Dictionary<string, decimal>();
                var currentTotalValue = Value;

                foreach (var position in _positionDictionary.Values)
                {
                    var percent = position.CurrentValue / currentTotalValue;
                    currentAllocations.Add(position.Symbol, percent);
                }

                return currentAllocations;
            }
        }

        public Dictionary<string, decimal> CurrentValues
        {
            get
            {
                return _positionDictionary
                    .Values
                    .ToDictionary(x => x.Symbol, x => x.CurrentValue);
            }
        }

        public BuyTransaction Buy(string symbol, decimal quantity)
        {
            var transaction = new BuyTransaction(symbol, quantity);
            _transactions.Add(transaction);

            Position position;

            if (_positionDictionary.ContainsKey(symbol))
            {
                position = _positionDictionary[symbol];
            }
            else
            {
                position = new Position(symbol);
                _positionDictionary.Add(symbol, position);
            }

            position.AddStock(transaction);

            return transaction;
        }

        public SellTransaction Sell(string symbol, decimal quantity)
        {
            var position = _positionDictionary[symbol];
            var transaction = position.RemoveStock(quantity);
            _transactions.Add(transaction);

            return transaction;
        }

        public SellTransaction SellAll(string symbol)
        {
            var position = _positionDictionary[symbol];
            var transaction = position.RemoveStock();
            _transactions.Add(transaction);
            _positionDictionary.Remove(symbol);

            return transaction;
        }

        public IEnumerable<SellTransaction> Liquidate()
        {
            var currentPositions = _positionDictionary.Values.ToList();

            foreach (var position in currentPositions)
            {
                // TODO: This will likely cause an error when trying to remove while iterating.
                var transaction = SellAll(position.Symbol);

                yield return transaction;
            }
        }
    }
}
