using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Models;

namespace TradeSimulator.Model
{
    public class Account
    {
        private decimal _openingBalance;
        private decimal _cashBalance;
        private decimal _capitalGains;
        private decimal _taxRate;
        private DateTime _accountOpenDate;

        // Year and amount of taxes paid
        private Dictionary<int, decimal> _taxPayments = new Dictionary<int, decimal>();

        private decimal _taxBalance
        {
            get { return _capitalGains * _taxRate; }
        }

        private decimal _totalValue
        {
            get { return Portfolio.TotalValue + _cashBalance - _taxBalance; }
        }

        public Account(decimal openingBalance, DateTime accountOpenDate, decimal taxRate, decimal spread, decimal tradingFee)
        {
            _openingBalance = openingBalance;
            _cashBalance = openingBalance;
            _accountOpenDate = accountOpenDate;
            _taxRate = taxRate;
            Portfolio = new Portfolio(spread, tradingFee);
        }

        public Portfolio Portfolio { get; private set; }

        public void Buy(IEnumerable<PurchaseRequest> purchaseRequests)
        {
            if (!purchaseRequests.Any())
            {
                return;
            }

            var purchaseRequestCount = purchaseRequests.Count();
            var totalAvailableCash = _cashBalance;

            foreach (var purchaseRequest in purchaseRequests)
            {
                var cashAvailableForPurchase = totalAvailableCash * purchaseRequest.Percent;
                var position = Portfolio.AddPosition(purchaseRequest, cashAvailableForPurchase);

                if (position != null)
                {
                    _cashBalance -= position.CostBasis;
                }

                //Debug.WriteLine("{0} \tBuy \t{1} \t{2} \t{3}"
                //    , purchaseRequest.Quote.DateValue.ToShortDateString()
                //    , purchaseRequest.Quote.Symbol
                //    , position.CostBasis
                //    , _totalValue);
            }
        }

        public void Liquidate(DateTime liquidationDate)
        {
            //Debug.WriteLine("{0} \tSell \t{1} \t{2} \t{3}"
            //    , liquidationDate.ToShortDateString()
            //    , "all"
            //    , Portfolio.TotalValue
            //    , _totalValue);

            _cashBalance += Portfolio.TotalValue;
            _capitalGains += Portfolio.TotalValue - Portfolio.CostBasis;
            Portfolio.Liquidate(liquidationDate);
        }

        public void PerformDailyActivities(DateTime date, IEnumerable<Quote> quotes)
        {
            Portfolio.UpdatePrices(quotes, date);
            PayTaxes(date);
        }

        public void PrintStatement(DateTime currentDate)
        {
            var annualGrowth = GetAnnualGrowth(currentDate);

            Console.WriteLine("Opening Balance: {0}", Math.Round(_openingBalance, 2));
            Console.WriteLine("Closing Balance: {0}", Math.Round(_totalValue, 2));
            Console.WriteLine("Total Growth: {0}%", Math.Round(((_totalValue / _openingBalance) - 1) * 100, 2));
            Console.WriteLine("Annual Growth: {0}%", Math.Round(annualGrowth, 2));
        }

        public void PrintCurrentPosition()
        {
            Debug.WriteLine("Value: {0}", _totalValue);
        }

        private void PayTaxes(DateTime date)
        {
            var previousYear = date.Year - 1;

            // If we haven't paid taxes for the previous year.
            if (!_taxPayments.ContainsKey(previousYear))
            {
                if (_capitalGains != 0)
                {
                    _cashBalance -= _taxBalance;
                }

                _taxPayments.Add(previousYear, _taxBalance);
                _capitalGains = 0;
            }
        }

        private double GetAnnualGrowth(DateTime currentDate)
        {
            var days = (currentDate - _accountOpenDate).TotalDays;
            var years = days / 365.25;
            var annualGrowth = Math.Pow((double)_totalValue / (double)_openingBalance, 1 / years) - 1;

            return annualGrowth * 100;
        }
    }

    //public class Account2
    //{
    //    private string _name;
    //    private decimal _openingBalance;
    //    private decimal _capitalGains = 0;
    //    private DateTime _firstTransactionDate = DateTime.MaxValue;
    //    private DateTime _lastTransactionDate = DateTime.MinValue;
    //    private DateTime _currentTransactionDate = DateTime.MinValue;

    //    private decimal taxBalance
    //    {
    //        get { return _capitalGains * Constants.TAX_RATE; }
    //    }

    //    public Account2(string name, decimal openingBalance)
    //    {
    //        _name = name;
    //        _openingBalance = openingBalance;
    //        CashBalance = openingBalance;
    //    }

    //    public DateTime TransactionDate
    //    {
    //        get
    //        {
    //            return _currentTransactionDate;
    //        }
    //        private set
    //        {
    //            _currentTransactionDate = value;

    //            if (_currentTransactionDate < _firstTransactionDate)
    //            {
    //                _firstTransactionDate = _currentTransactionDate;
    //            }

    //            if (_currentTransactionDate > _lastTransactionDate)
    //            {
    //                _lastTransactionDate = _currentTransactionDate;
    //            }
    //        }
    //    }

    //    public decimal CashBalance { get; private set; }
    //    public decimal PurchasePrice { get; private set; }
    //    public string CurrentSymbol { get; private set; }
    //    public decimal CurrentStockQuantity { get; private set; }

    //    public decimal TotalBalance
    //    {
    //        get { return CashBalance + (PurchasePrice * CurrentStockQuantity) - taxBalance; }
    //    }

    //    public void Buy(Quote quote)
    //    {
    //        if (CurrentStockQuantity != 0)
    //        {
    //            throw new Exception("Account already owns stock");
    //        }

    //        // If it is a new year, pay the taxes.
    //        if (TransactionDate.Year != quote.DateValue.Year)
    //        {
    //            if (_capitalGains > 0)
    //            {
    //                CashBalance -= taxBalance;
    //            }

    //            _capitalGains = 0;
    //        }

    //        var spreadMultiplier = 1 + Constants.AVG_SPREAD;
    //        PurchasePrice = quote.AdjustedClosePrice * spreadMultiplier;

    //        if ((CashBalance - Constants.TRADING_FEE) / PurchasePrice > 0)
    //        {
    //            CurrentSymbol = quote.Symbol;
    //            CurrentStockQuantity = (int)Math.Floor((CashBalance - Constants.TRADING_FEE) / PurchasePrice);
    //            CashBalance = CashBalance - (PurchasePrice * CurrentStockQuantity) - Constants.TRADING_FEE;
    //            TransactionDate = quote.DateValue;
    //        }
    //    }

    //    public void Sell(Quote quote)
    //    {
    //        if (CurrentStockQuantity == 0)
    //        {
    //            throw new Exception("No stock to sell");
    //        }

    //        var salePrice = quote != null
    //           ? quote.AdjustedClosePrice
    //           : PurchasePrice;

    //        // Reduce the sale price by the spread
    //        var spreadMultiplier = 1 - Constants.AVG_SPREAD;
    //        salePrice = salePrice * spreadMultiplier;
    //        var purchaseValue = CurrentStockQuantity * PurchasePrice;
    //        var saleValue = CurrentStockQuantity * salePrice;

    //        _capitalGains += saleValue - purchaseValue - (Constants.TRADING_FEE * 2);
    //        CashBalance += saleValue - Constants.TRADING_FEE;

    //        PurchasePrice = 0;
    //        CurrentStockQuantity = 0;
    //        CurrentSymbol = string.Empty;
    //        TransactionDate = quote.DateValue;
    //    }

    //    private double GetAnnualGrowth(decimal openingBalance, decimal closingBalance, double years)
    //    {
    //        var annualGrowth = Math.Pow((double)closingBalance / (double)openingBalance, 1 / years) - 1;
    //        var annualGrowthPercent = annualGrowth * 100;

    //        return Math.Round(annualGrowthPercent, 2);
    //    }

    //    public void ShowStatement()
    //    {
    //        var days = (_lastTransactionDate - _firstTransactionDate).TotalDays;
    //        var years = days / 365.25;
    //        var annualGrowth = GetAnnualGrowth(_openingBalance, TotalBalance, years);

    //        Console.WriteLine("---------------------------------------------------");
    //        Console.WriteLine(_name);
    //        Console.WriteLine("--------");
    //        Console.WriteLine("Opening Balance: {0}", Math.Round(_openingBalance, 2));
    //        Console.WriteLine("Closing Balance: {0}", Math.Round(TotalBalance, 2));
    //        Console.WriteLine("Annual Growth: {0}%", annualGrowth);
    //        Console.WriteLine("---------------------------------------------------");
    //        Console.WriteLine();
    //    }
    //}
}
