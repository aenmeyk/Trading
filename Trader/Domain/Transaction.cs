using System;
using Common.Models;

namespace Trader.Domain
{
    public abstract class Transaction
    {
        public Transaction(string symbol, decimal quantity)
        {
            Quote = Market.QuoteDictionary[symbol];
            Stock = Quote.Stock;
            Fees = Quote.Stock.TradingFee;
        }

        public Quote Quote { get; private set; }
        public Stock Stock { get; private set; }
        public decimal Quantity { get; protected set; }
        public decimal Price { get; protected set; }
        public decimal Fees { get; private set; }
        public abstract TransactionType TransactionType { get; }
    }

    public class BuyTransaction : Transaction
    {
        public BuyTransaction(string symbol, decimal quantity)
            : base(symbol, quantity)
        {
            Price = Quote.PurchasePrice;
            Quantity = quantity;
        }

        public decimal CostBasis
        {
            get { return (Quantity * Price) + Fees; }
        }

        public decimal CostBasisPerShare
        {
            get { return CostBasis / Quantity; }
        }

        public override TransactionType TransactionType
        {
            get { return TransactionType.Buy; }
        }
    }

    public class SellTransaction : Transaction
    {
        public SellTransaction(string symbol, decimal quantity, decimal shortTermTaxableAmount, decimal longTermTaxableAmount)
            : base(symbol, quantity)
        {
            Price = Quote.SalePrice;
            Quantity = quantity;
            ShortTermTaxableAmount = shortTermTaxableAmount;
            LongTermTaxableAmount = longTermTaxableAmount;
        }

        public decimal ShortTermTaxableAmount { get; private set; }
        public decimal LongTermTaxableAmount { get; private set; }

        public decimal TotalCashReturned
        {
            get { return (Quantity * Price) - Fees; }
        }

        public override TransactionType TransactionType
        {
            get { return TransactionType.Sell; }
        }
    }
}
