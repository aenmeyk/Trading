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

        public string LoggingLevel { get; set; }

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

                    LogMessage("{0} \tBuy \t{1} \t{2} \t{3} \t{4} \t{5}"
                        , purchaseRequest.Quote.DateValue.ToShortDateString()
                        , purchaseRequest.Quote.Symbol
                        , Math.Round(purchaseRequest.Quote.AdjustedClosePrice, 2)
                        , Math.Round(position.Quantity, 2)
                        , Math.Round(position.CostBasis, 2)
                        , Math.Round(_totalValue, 2));
                }
            }
        }

        public void Liquidate(DateTime liquidationDate)
        {
            var positions = Portfolio.PositionDictionary.Values.ToList();

            foreach (var position in positions)
            {
                var saleValue = Portfolio.GetPositionSaleValue(position);
                _cashBalance += saleValue;
                _capitalGains += saleValue - position.CostBasis;
                Portfolio.PositionDictionary.Remove(position.Symbol);

                LogMessage("{0} \tSell \t{1} \t{2} \t{3} \t{4} \t{5} \t{6}"
                    , liquidationDate.ToShortDateString()
                    , position.Symbol
                    , Math.Round(position.CurrentPrice, 2)
                    , Math.Round(position.Quantity, 2)
                    , Math.Round(position.CurrentValue, 2)
                    , Math.Round(_totalValue, 2)
                    , Math.Round(_capitalGains, 2));
            }
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
            LogMessage("Value: {0}", _totalValue);
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

                LogMessage("Taxes: {0} \tAccountValue: {1}", Math.Round(_taxBalance, 2), Math.Round(_totalValue, 2));

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

        private void LogMessage(string message, params object[] args)
        {
            if (LoggingLevel == "All")
            {
                Debug.WriteLine(message, args);
            }
        }
    }
}
