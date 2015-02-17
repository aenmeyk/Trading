namespace HistoricalPriceExtractor
{
    public static class Constants
    {
        public const string YAHOO_QUOTE_URL = "http://real-chart.finance.yahoo.com/table.csv?s={0}&amp;a=00&amp;b=1&amp;c=1800&amp;d=00&amp;e=1&amp;f=2099&amp;g=d&amp;ignore=.csv";
        public const string GOOGLE_QUOTE_URL = "http://www.google.com/finance/historical?q={0}&startdate=Feb+16%2C+1970&enddate=Feb+15%2C+2025&output=csv";
        public const int REQUEST_DELAY_MILLISECONDS = 800;
    }
}
