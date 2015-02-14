namespace HistoricalPriceExtractor
{
    public static class Constants
    {
        public static string QUOTE_URL = "http://real-chart.finance.yahoo.com/table.csv?s={0}&amp;a=00&amp;b=1&amp;c=1800&amp;d=00&amp;e=1&amp;f=2099&amp;g=d&amp;ignore=.csv";
        public static string PERSISTANCE_PATH = @"D:\Work\Trading\HistoricalPrices\{0}.csv";
        public static int REQUEST_DELAY_MILLISECONDS = 1000;
    }
}
