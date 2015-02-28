using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Models;
using TradeSimulator.Model;

namespace TradeSimulator.Strategies
{
    public abstract class StrategyBase
    {
        protected abstract string Name { get; }
        protected abstract decimal TaxRate { get; }
        protected abstract decimal Spread { get; }
        protected abstract decimal TradingFee { get; }
        protected Account Account { get; private set; }
        protected Dictionary<DateTime, IEnumerable<PriceHistory>> AllQuotes { get; private set; }
        protected Dictionary<string, PriceHistory> TodayQuotes { get; private set; }

        // None, All
        protected virtual string LoggingLevel { get { return "None"; } }
        protected virtual bool PrintRunningBalance { get { return false; } }
        protected virtual bool AllowPartialPurchases { get { return false; } }
        public abstract IEnumerable<string> Symbols { get; }

        public virtual void Initialize(DateTime startDate, Dictionary<DateTime, IEnumerable<PriceHistory>> allQuotes)
        {
            AllQuotes = allQuotes;
            Account = new Account(Constants.OPENING_BALANCE, startDate, TaxRate, Spread, TradingFee, AllowPartialPurchases);
            Account.LoggingLevel = LoggingLevel;
        }

        public void ExecuteStrategy(DateTime date)
        {
            ExecuteStrategyImplementation(date);

            if (PrintRunningBalance)
            {
                PrintAccountBalance(date);
            }
        }

        public void PrintAccountBalance(DateTime currentDate)
        {
            Debug.WriteLine("{0}\t{1}", currentDate.ToShortDateString(), Account.TotalValue);
        }

        public void PerformDailyActivities(DateTime date)
        {
            TodayQuotes = AllQuotes[date].Where(x => Symbols.Contains(x.Symbol)).ToDictionary(x => x.Symbol);
            Account.PerformDailyActivities(date, TodayQuotes.Values);
        }

        public void DepositCash(DateTime date, decimal amount)
        {
            Account.DepositCash(amount);
            DepositCashImplementation(date, amount);
        }

        public void PrintResult(DateTime currentDate)
        {
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine(Name);
            Console.WriteLine("--------");
            Account.PrintStatement(currentDate);
            Console.WriteLine("---------------------------------------------------");
        }

        protected virtual void ExecuteStrategyImplementation(DateTime date)
        {
            // Allow sub class to implement
        }

        protected virtual void DepositCashImplementation(DateTime date, decimal amount)
        {
            // Allow sub class to implement
        }
    }
}
