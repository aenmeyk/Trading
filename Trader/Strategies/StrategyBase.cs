using System;
using System.Collections.Generic;
using Common;
using Trader.Domain;

namespace Trader.Strategies
{
    public abstract class StrategyBase
    {
        protected abstract string Name { get; }
        protected Account Account { get; set; }

        public virtual void Initialize()
        {
            Account = new Account(GeneralSettings.SHORT_TERM_TAX_RATE, GeneralSettings.LONG_TERM_TAX_RATE);
        }

        public virtual void Close()
        {
            Account.Liquidate();
        }

        public virtual IEnumerable<string> Symbols
        {
            get { return new string[0]; }
        }

        public virtual void DepositCash(decimal amount)
        {
            Account.DepositCash(amount);
        }

        public void ExecuteStrategy()
        {
            ExecuteStrategyImplementation();
        }

        protected virtual void ExecuteStrategyImplementation()
        {
            // Allow sub class to override.
        }

        public virtual void PayTaxes()
        {
            Account.PayTaxes();
        }

        public void PrintResult()
        {
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine(Name);
            Console.WriteLine("--------");
            Account.PrintStatement();
            Console.WriteLine("---------------------------------------------------");
        }
    }
}
