using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Trader.Domain;

namespace Trader.Strategies
{
    public abstract class StrategyBase
    {
        public abstract string Name { get; }
        public Account Account { get; protected set; }

        public virtual void Initialize()
        {
            //Account = new Account(0, 0);
            Account = new Account(GeneralSettings.SHORT_TERM_TAX_RATE, GeneralSettings.LONG_TERM_TAX_RATE);
        }

        public virtual void Close()
        {
            Account.Liquidate();
        }

        public virtual IEnumerable<string> Symbols
        {
            get
            {
                return new[]
                {
"XPH",
"MDY",
"EWO",
"FBT",
"EWM",
"EWA",
"IBB",
"PJP",
"EEH",
"EWZ",
"BBH",
"DOD",
"SIJ",
                };
            }
        }

        protected virtual IEnumerable<string> AvailableSymbols
        {
            get { return Symbols.Where(x => Market.QuoteDictionary.Keys.Contains(x)); }
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
