using System;

namespace TradeSimulator
{
    public static class Constants
    {
        public const decimal OPENING_BALANCE = 1430.62M;
        public static DateTime START_DATE = new DateTime(2007, 1, 17);
        public static DateTime END_DATE = new DateTime(2018, 10, 23);
        public static decimal TAX_RATE = 0.28M;
        public static decimal AVG_SPREAD = 0.0005M;
        public static int UNSETTLED_FUNDS_DAYS = 1;
        public static decimal TRADING_FEE = 0M;
    }
}
