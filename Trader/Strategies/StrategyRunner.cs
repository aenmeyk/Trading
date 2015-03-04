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
            new SP500Benchmark(),
            //new ActualAllocationRebalanceByDeposit(),
            //new ActualFixedAllocation(),
            new BuyLoser()
        };

        public void Run()
        {
            GetStock();
            GetPriceHistoryDictionary();
            var startDate = Market.HistoricalQuoteDictionary.Min(x => x.Key);
            var endDate = Market.HistoricalQuoteDictionary.Max(x => x.Key);
            var mostRecentYearTaxesPaid = startDate.Year;
            Market.QuoteDictionary = Market.HistoricalQuoteDictionary[startDate];
            Market.Today = startDate;

            foreach (var strategy in _strategies)
            {
                strategy.Initialize();
                strategy.DepositCash(GeneralSettings.OPENING_BALANCE);
            }

            foreach (var date in Market.HistoricalQuoteDictionary.Keys.OrderBy(x => x))
            {
                Market.QuoteDictionary = Market.HistoricalQuoteDictionary[date];
                Market.Today = date;
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

        private void GetPriceHistoryDictionary()
        {
            var symbols = _strategies.SelectMany(x => x.Symbols);
            var quotes = _priceHistoryRepository.GetForSymbolsAndDateRange<PriceHistory>(symbols, GeneralSettings.START_DATE, GeneralSettings.END_DATE);
            var groupByDate = quotes
                .GroupBy(x => x.DateValue)
                .ToDictionary(g => g.Key, g => g.Select(x => x));

            foreach (var dateItem in groupByDate)
            {
                var quoteDictionary = new Dictionary<string, Quote>();

                foreach (var priceHistory in dateItem.Value)
                {
                    var symbol = priceHistory.Symbol;
                    var stock = Market.StockDictionary[symbol];
                    var quote = new Quote(stock, priceHistory);

                    quoteDictionary.Add(symbol, quote);
                }

                Market.HistoricalQuoteDictionary.Add(dateItem.Key, quoteDictionary);
            }
        }
    }
}
