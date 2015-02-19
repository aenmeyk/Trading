using System;
using System.Collections.Generic;
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

        // None, All
        protected virtual string LoggingLevel { get { return "None"; } }
        public abstract IEnumerable<string> Symbols { get; }

        public void Initialize(DateTime startDate)
        {
            Account = new Account(Constants.OPENING_BALANCE, startDate, TaxRate, Spread, TradingFee);
            Account.LoggingLevel = LoggingLevel;
        }

        public void ExecuteStrategy(DateTime date, IEnumerable<Quote> quotes)
        {
            var applicableQuotes = quotes.Where(x => Symbols.Contains(x.Symbol));
            Account.PerformDailyActivities(date, applicableQuotes);
            ExecuteStrategyImplementation(date, applicableQuotes);
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

        protected abstract void ExecuteStrategyImplementation(DateTime date, IEnumerable<Quote> quotes);
    }
}
