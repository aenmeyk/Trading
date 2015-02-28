using System;

namespace Common
{
    public static class GeneralSettings
    {
        public const decimal OPENING_BALANCE = 653.73M;
        public const decimal CONTRIBUTION = 0M;

        public static DateTime START_DATE = new DateTime(1996, 4, 1);
        public static DateTime END_DATE = new DateTime(2016, 12, 31);

        public const decimal TRADING_FEE = 8.95M;
        public const decimal SHORT_TERM_TAX_RATE = 0.28M;
        public const decimal LONG_TERM_TAX_RATE = 0.15M;
    }

    public static class CommonConstants
    {
        public const string PERSISTANCE_PATH = @"D:\Work\Trading\HistoricalPrices\{0}.csv";
    }
}
