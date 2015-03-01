using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Models;
using DataAccess.Repositories;
using Trader.Domain;

namespace Trader.Strategies
{
    public class StrategyRunner
    {
        private StockRepository _stockRepository = new StockRepository();
        private PriceHistoryRepositoryBase _priceHistoryRepository = new PriceHistoryYahooRepository();

        private IEnumerable<StrategyBase> _strategies = new StrategyBase[]
        {
            new SP500Benchmark()
        };

        public void Run()
        {
            GetStock();
            var priceHistoryDictionary = GetPriceHistoryDictionary();
            var startDate = priceHistoryDictionary.Min(x => x.Key);
            var endDate = priceHistoryDictionary.Max(x => x.Key);
            var mostRecentYearTaxesPaid = startDate.Year;

            foreach (var strategy in _strategies)
            {
                strategy.Initialize();
                strategy.DepositCash(GeneralSettings.OPENING_BALANCE);
            }

            foreach (var date in priceHistoryDictionary.Keys.OrderBy(x => x))
            {
                UpdateMarket(date, priceHistoryDictionary[date]);
                var endOfMonth = new DateTime(Market.Today.Year, Market.Today.Month, DateTime.DaysInMonth(Market.Today.Year, Market.Today.Month));
                var endOfYear = new DateTime(Market.Today.Year, 12, 31);

                if (Market.Today.Year != mostRecentYearTaxesPaid)
                {
                    foreach (var strategy in _strategies)
                    {
                        strategy.PayTaxes();
                    }

                    mostRecentYearTaxesPaid = Market.Today.Year;
                }

                if (Market.Today.Day == 15 || Market.Today == endOfMonth)
                {
                    foreach (var strategy in _strategies)
                    {
                        strategy.DepositCash(GeneralSettings.CONTRIBUTION);
                    }
                }

                foreach (var strategy in _strategies)
                {
                    strategy.ExecuteStrategy();
                }
            }

            foreach (var strategy in _strategies)
            {
                strategy.Close();
                strategy.PrintResult();
            }
        }

        private void GetStock()
        {
            var stocks = _stockRepository.Get<Stock>();
            Market.StockDictionary = stocks.ToDictionary(x => x.Symbol);
        }

        private Dictionary<DateTime, IEnumerable<PriceHistory>> GetPriceHistoryDictionary()
        {
            var symbols = _strategies.SelectMany(x => x.Symbols);
            var quotes = _priceHistoryRepository.GetForSymbolsAndDateRange<PriceHistory>(symbols, GeneralSettings.START_DATE, GeneralSettings.END_DATE);
            var groupByDate = quotes
                .GroupBy(x => x.DateValue)
                .ToDictionary(g => g.Key, g => g.Select(x => x));

            return groupByDate;
        }

        private void UpdateMarket(DateTime date, IEnumerable<PriceHistory> priceHistories)
        {
            Market.QuoteDictionary.Clear();

            foreach (var priceHistory in priceHistories)
            {
                var symbol = priceHistory.Symbol;
                var stock = Market.StockDictionary[symbol];
                var quote = new Quote(stock, priceHistory);
                Market.QuoteDictionary.Add(symbol, quote);
            }

            Market.Today = date;
        }
    }
}
