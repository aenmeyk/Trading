﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common.Models;

namespace TradeSimulator.Model
{
    public class Portfolio
    {
        private decimal _spread;
        private decimal _tradingFee;
        private DateTime _lastPortfolioUpdateDate = DateTime.MinValue;
        private bool _allowPartialPurchases;

        public Portfolio(decimal spread, decimal tradingFee, bool allowPartialPurchases)
        {
            _spread = spread;
            _tradingFee = tradingFee;
            _allowPartialPurchases = allowPartialPurchases;
            PositionDictionary = new Dictionary<string, Position>();
        }

        public Dictionary<string, Position> PositionDictionary { get; private set; }

        public decimal TotalValue
        {
            get
            {
                var portfolioValue = 0M;

                foreach (var position in PositionDictionary.Values)
                {
                    //var spreadMultiplier = 1 - GetSpread(position.Symbol);
                    //var tradingFee = GetTradingFee(position.Symbol);
                    //portfolioValue += (position.CurrentValue * spreadMultiplier) - tradingFee;
                    portfolioValue += position.CurrentValue;
                }

                return portfolioValue;
            }
        }

        public decimal CostBasis
        {
            get { return PositionDictionary.Sum(x => x.Value.CostBasis); }
        }

        public Position AddPosition(PurchaseRequest purchaseRequest, decimal cashAvailableForPurchase)
        {
            var tradingFee = GetTradingFee(purchaseRequest.Quote.Symbol);
            var spreadMultiplier = 1 + GetSpread(purchaseRequest.Quote.Symbol);
            var purchasePrice = purchaseRequest.Quote.AdjustedClosePrice * spreadMultiplier;
            var quantity = _allowPartialPurchases
                ? (cashAvailableForPurchase - tradingFee) / purchasePrice
                : (int)Math.Floor((cashAvailableForPurchase - tradingFee) / purchasePrice);

            if (quantity > 0)
            {
                var symbol = purchaseRequest.Quote.Symbol;

                if (!PositionDictionary.ContainsKey(symbol))
                {
                    PositionDictionary.Add(symbol, new Position(symbol));
                }

                var position = PositionDictionary[symbol];
                position.AddStock(quantity, purchasePrice, tradingFee);

                return position;
            }

            return null;
        }

        public void UpdatePrices(IEnumerable<PriceHistory> quotes, DateTime date)
        {
            foreach (var quote in quotes)
            {
                if (PositionDictionary.ContainsKey(quote.Symbol))
                {
                    var position = PositionDictionary[quote.Symbol];
                    position.CurrentPrice = quote.AdjustedClosePrice;
                }
            }

            _lastPortfolioUpdateDate = date;
        }

        public decimal GetPositionSaleValue(Position position)
        {
            var spreadMultiplier = 1 - GetSpread(position.Symbol);
            var tradingFee = GetTradingFee(position.Symbol);

            return (position.CurrentValue * spreadMultiplier) - tradingFee;
        }

        public void Liquidate(DateTime liquidationDate)
        {
            if (_lastPortfolioUpdateDate != liquidationDate)
            {
                throw new Exception("Portfolio prices have note been updated.");
            }

            PositionDictionary.Clear();
        }

        private decimal GetSpread(string symbol)
        {
            return _spread;
        }

        private decimal GetTradingFee(string symbol)
        {
            return _tradingFee;
        }
    }
}
