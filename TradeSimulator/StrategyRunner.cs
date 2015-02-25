using System;
using System.Collections.Generic;
using System.Linq;
using Common.Models;
using DataAccess.Repositories;
using TradeSimulator.Strategies;

namespace TradeSimulator
{
    public class StrategyRunner
    {
        private PriceHistoryRepositoryBase _repository;

        private IEnumerable<StrategyBase> _strategies = new StrategyBase[]
        {
            new SP500Benchmark(),
            //new ActualAllocationBenchmark(),
            //new MovementFromYesterday(),
            //new MovementFromYesterdayAllSymbols(),
            new EvenAllocation(),
            new RebalanceAtDeposit(),
            new RebalanceWithoutSelling(),
            new AlwaysAllocateDepositEvenly(),
            new BuyLoser(),
            //new NeuralNetwork(),
       };

        public StrategyRunner()
        {
            _repository = new PriceHistoryYahooRepository();
        }

        public void Run()
        {
            var quoteDictionary = GetQuotes();
            var startDate = quoteDictionary.Min(x => x.Key);
            var endDate = quoteDictionary.Max(x => x.Key);

            foreach (var strategy in _strategies)
            {
                strategy.Initialize(startDate, quoteDictionary);
            }

            foreach (var date in quoteDictionary.Keys.OrderBy(x => x))
            {
                var endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

                foreach (var strategy in _strategies)
                {
                    strategy.PerformDailyActivities(date);
                }

                if(date.Day == 15 || date == endOfMonth)
                {
                    foreach (var strategy in _strategies)
                    {
                        strategy.DepositCash(date, Constants.CONTRIBUTION);
                    }
                }

                foreach (var strategy in _strategies)
                {
                    strategy.ExecuteStrategy(date);
                }
            }

            foreach (var strategy in _strategies)
            {
                strategy.PrintResult(endDate);
            }
        }

        private Dictionary<DateTime, IEnumerable<Quote>> GetQuotes()
        {
            var symbols = _strategies.SelectMany(x => x.Symbols);
            var currentMinDate = DateTime.MaxValue;
            var quotes = _repository.GetForSymbolsAndDateRange<Quote>(symbols, Constants.START_DATE, Constants.END_DATE);
            var groupByDate = quotes
                .GroupBy(x => x.DateValue)
                .ToDictionary(g => g.Key, g => g.Select(x => x));

            return groupByDate;
        }
    }
}
