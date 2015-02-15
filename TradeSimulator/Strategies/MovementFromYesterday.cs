using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Common.Models;
using Dapper;

namespace TradeSimulator.Strategies
{
    public class MovementFromYesterday
    {
        //private IEnumerable<string> _symbols = new[] { "SCHX", "SCHM", "SCHA", "SCHF", "XLG", "WMCR", "DEF", "SCHE", "FEU", "MDD", "BIK", "PAF" };
        private IEnumerable<string> _symbols = new[] {
"ACIM",
"BIK",
"CQQQ",
"CSM",
"CVY",
"CWI",
"DEF",
"DGRE",
"DGRS",
"DGRW",
"DNL",
"DRW",
"DWAS",
"DWX",
"EDIV",
"EDOG",
"EEB",
"EELV",
"EMBB",
"EWEM",
"EWRS",
"EWX",
"FEU",
"FNDA",
"FNDB",
"FNDC",
"FNDE",
"FNDF",
"FNDX",
"FRN",
"GAF",
"GAL",
"GMF",
"GML",
"GXC",
"HAO",
"HGI",
"IBLN",
"IDLV",
"IDOG",
"IHDG",
"INKM",
"JPP",
"KNOW",
"LOWC",
"MDD",
"MDYG",
"MDYV",
"NOBL",
"PAF",
"PDP",
"PID",
"PIE",
"PIN",
"PIZ",
"PKW",
"PXMC",
"PXSV",
"QAUS",
"QCAN",
"QDEU",
"QESP",
"QGBR",
"QJPN",
"QKOR",
"QMEX",
"QQQE",
"QTWN",
"RFG",
"RFV",
"RLY",
"RPG",
"RPV",
"RSCO",
"RSP",
"RWO",
"RZG",
"RZV",
"SCHA",
"SCHB",
"SCHC",
"SCHD",
"SCHE",
"SCHF",
"SCHG",
"SCHH",
"SCHM",
"SCHV",
"SCHX",
"SDOG",
"SLYG",
"SLYV",
"SPHB",
"SPLV",
"SYE",
"SYG",
"SYV",
"TOLZ",
"VSPY",
"WDIV",
"WMCR",
"XLG",
"YAO" };

        private string _connectionString;
        private Dictionary<string, IEnumerable<Quote>> _quotes = new Dictionary<string, IEnumerable<Quote>>();
        private Account _account = new Account(Constants.OPENING_BALANCE);
        IEnumerable<Quote> _sp500Quotes;

        // The simulation will run based on the date range for this symbol
        private string _masterSymbol;

        public MovementFromYesterday(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Run()
        {
            GetQuotes();
            RunStrategy();
        }

        private void GetQuotes()
        {
            var queryText = "SELECT Symbol, DateValue, AdjustedClosePrice, Volume FROM dbo.PriceHistory WHERE Symbol = @Symbol";
            var currentMinDate = DateTime.MaxValue;

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                _sp500Quotes = sqlConnection.Query<Quote>(queryText, new { Symbol = "^GSPC" });

                foreach (var symbol in _symbols)
                {
                    var quotes = sqlConnection.Query<Quote>(queryText, new { Symbol = symbol });
                    _quotes.Add(symbol, quotes);

                    var minDate = quotes.Min(x => x.DateValue);

                    if (minDate < currentMinDate)
                    {
                        currentMinDate = minDate;
                        _masterSymbol = symbol;
                    }
                }
            }
        }

        private void RunStrategy()
        {
            var dateRange = _quotes[_masterSymbol]
                .Select(x => x.DateValue)
                .Where(x => x >= Constants.START_DATE && x <= Constants.END_DATE)
                .OrderBy(x => x);

            DateTime? previousDate = null;
            var currentQuotes = new Collection<Quote>();

            foreach (var currentDate in dateRange)
            {
                if (previousDate != null)
                {
                    foreach (var quotes in _quotes.Values)
                    {
                        var previousQuote = quotes.SingleOrDefault(x => x.DateValue == previousDate);

                        if (previousQuote != null)
                        {
                            var previousPrice = previousQuote.AdjustedClosePrice;
                            var quote = quotes.SingleOrDefault(x => x.DateValue == currentDate);

                            if (quote != null)
                            {
                                quote.Growth = quote.AdjustedClosePrice / previousPrice;
                                currentQuotes.Add(quote);
                            }
                        }
                    }

                    Trade(currentQuotes);
                    Debug.WriteLine("{0}\t{1}\t{2}", currentDate.ToShortDateString(), _account.CurrentSymbol, _account.TotalBalance);
                }

                currentQuotes.Clear();
                previousDate = currentDate;
            }

            PrintResults(dateRange);
        }

        private void Trade(Collection<Quote> currentQuotes)
        {
            var newQuote = currentQuotes.First(x => x.Growth == currentQuotes.Min(y => y.Growth));

            // Only change the trade if we are moving to a different stock.
            if (newQuote.Symbol != _account.CurrentSymbol)
            {
                if (_account.CurrentStockQuantity > 0)
                {
                    var existingStockPrice = currentQuotes.Single(x => x.Symbol == _account.CurrentSymbol).AdjustedClosePrice;
                    _account.Sell(existingStockPrice);
                }

                _account.Buy(newQuote.Symbol, newQuote.AdjustedClosePrice);
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
    }
}
