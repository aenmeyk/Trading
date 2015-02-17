using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Common.Models;
using DataAccess.Repositories;

namespace TradeSimulator.Strategies
{
    public class MovementFromYesterday
    {
        //private IEnumerable<string> _symbols = new[] { "SCHX", "SCHM", "SCHA", "SCHF", "XLG", "WMCR", "DEF", "SCHE", "FEU", "MDD", "BIK", "PAF" };
        private IEnumerable<string> _symbols = new[] {
"SCHX",
"SCHM",
"SCHA",
"SCHF",
"SCHE",
"SCHH",
"SCHG",
"SCHV",
"PIN",
"GXC",
"GMF",
"FEU"
};

        private HistoricalPricesRepositoryBase _repository;
        private Dictionary<string, Dictionary<DateTime, Quote>> _quotes = new Dictionary<string, Dictionary<DateTime, Quote>>();
        private Account _account = new Account(Constants.OPENING_BALANCE);
        IEnumerable<Quote> _sp500Quotes;

        // The simulation will run based on the date range for this symbol
        private string _masterSymbol;

        public MovementFromYesterday()
        {
            _repository = new HistoricalPricesYahooRepository();
        }

        public void Run()
        {
            GetQuotes();
            RunStrategy();
        }

        private void GetQuotes()
        {
            var queryText = "SELECT Symbol, DateValue, HighPrice, LowPrice, ClosePrice, AdjustedClosePrice, Volume FROM dbo.PriceHistory WHERE Symbol = @Symbol";
            var currentMinDate = DateTime.MaxValue;

                _sp500Quotes = _repository.GetForSymbol<Quote>("^GSPC");

                foreach (var symbol in _symbols)
                {
                    var quotes = _repository.GetForSymbol<Quote>(symbol);
                    var quoteDictionary = quotes.ToDictionary(x => x.DateValue);
                    _quotes.Add(symbol, quoteDictionary);

                    var minDate = quotes.Min(x => x.DateValue);

                    if (minDate < currentMinDate)
                    {
                        currentMinDate = minDate;
                        _masterSymbol = symbol;
                    }
                }
        }

        private void RunStrategy()
        {
            var dateRange = _quotes[_masterSymbol]
                .Select(x => x.Key)
                .Where(x => x >= Constants.START_DATE && x <= Constants.END_DATE)
                .OrderBy(x => x);

            DateTime previousDate = DateTime.MinValue;
            var currentQuotes = new Collection<Quote>();

            foreach (var currentDate in dateRange)
            {
                if ((currentDate - _account.TransactionDate).TotalDays >= Constants.UNSETTLED_FUNDS_DAYS && previousDate != DateTime.MinValue)
                {
                    foreach (var quotes in _quotes.Values)
                    {
                        if (quotes.ContainsKey(previousDate))
                        {
                            var previousQuote = quotes[previousDate];
                            var previousPrice = previousQuote.AdjustedClosePrice;
                            var quote = quotes[currentDate];

                            if (quote != null)
                            {
                                quote.Growth = quote.AdjustedClosePrice / previousPrice;
                                currentQuotes.Add(quote);
                            }
                        }
                    }

                    if (currentQuotes.Count > 0)
                    {
                        Trade(currentQuotes);
                        currentQuotes.Clear();
                    }
                }

                Debug.WriteLine("{0}\t{1}\t{2}", currentDate.ToShortDateString(), _account.CurrentSymbol, _account.TotalBalance);
                previousDate = currentDate;//.AddDays(-Constants.UNSETTLED_FUNDS_DAYS);
            }

            PrintResults(dateRange);
        }

        private void Trade(Collection<Quote> currentQuotes)
        {
            var quoteToBuy = currentQuotes.First(x => x.Growth == currentQuotes.Min(y => y.Growth));

            var existingQuote = currentQuotes.FirstOrDefault(x => x.Symbol == _account.CurrentSymbol);

            if (existingQuote != null && existingQuote.Growth <= quoteToBuy.Growth * 1.01M)
            {
                quoteToBuy = existingQuote;
            }

            // Only change the trade if we are moving to a different stock.
            if (quoteToBuy.Symbol != _account.CurrentSymbol)
            {
                if (_account.CurrentStockQuantity > 0)
                {
                    var quoteForCurrentStock = currentQuotes.SingleOrDefault(x => x.Symbol == _account.CurrentSymbol);

                    var currentStockPrice = quoteForCurrentStock != null
                        ? quoteForCurrentStock.AdjustedClosePrice
                        : _account.PurchasePrice;

                    _account.Sell(currentStockPrice);
                }

                _account.Buy(quoteToBuy);
            }
        }

        private void PrintResults(IEnumerable<DateTime> dateRange)
        {
            var startDate = dateRange.Min();
            var endDate = dateRange.Max();
            var days = (endDate - startDate).TotalDays;
            var years = days / 365.25;
            var annualGrowth = GetAnnualGrowth(Constants.OPENING_BALANCE, _account.TotalBalance, years);

            Console.WriteLine("Start Date: {0}", startDate.ToShortDateString());
            Console.WriteLine("End Date: {0}", endDate.ToShortDateString());
            Console.WriteLine("Balance: {0}", Math.Round(_account.TotalBalance, 2));
            Console.WriteLine("Annual Growth: {0}%", annualGrowth);

            var sp500StartPrice = _sp500Quotes.Single(x => x.DateValue == startDate).AdjustedClosePrice;
            var sp500EndPrice = _sp500Quotes.Single(x => x.DateValue == endDate).AdjustedClosePrice;
            var sp500AnnualGrowth = GetAnnualGrowth(sp500StartPrice, sp500EndPrice, years);

            Console.WriteLine();
            Console.WriteLine("S&P 500 Annual Growth: {0}%", sp500AnnualGrowth);
        }

        private double GetAnnualGrowth(decimal openingBalance, decimal closingBalance, double years)
        {
            var annualGrowth = Math.Pow((double)closingBalance / (double)openingBalance, 1 / years) - 1;
            var annualGrowthPercent = annualGrowth * 100;

            return Math.Round(annualGrowthPercent, 2);
        }

        private DateTime _startTime = DateTime.Now;
        private DateTime _lastTime = DateTime.Now;

        private void RecordTime([CallerLineNumber] int lineNumber = 0)
        {
            Debug.WriteLine("{0}: {1}\t\t{2}", lineNumber, (DateTime.Now - _startTime).TotalMilliseconds, (DateTime.Now - _lastTime).TotalMilliseconds);
            _lastTime = DateTime.Now;
        }
    }
}
