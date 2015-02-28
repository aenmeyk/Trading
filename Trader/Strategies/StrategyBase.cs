using System;
using System.Collections.Generic;
using Common;
using Trader.Domain;

namespace Trader.Strategies
{
    public abstract class StrategyBase
    {
        protected abstract string Name { get; }
        protected Account Account { get; private set; }

        public virtual void Initialize()
        {
            Account = new Account(GeneralSettings.SHORT_TERM_TAX_RATE, GeneralSettings.LONG_TERM_TAX_RATE);
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

        protected abstract void ExecuteStrategyImplementation();

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
