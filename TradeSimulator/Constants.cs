using System;

namespace TradeSimulator
{
    public static class Constants
    {
        public const decimal OPENING_BALANCE = 10000M;
        public const decimal CONTRIBUTION = 1000M;
        public static DateTime START_DATE = new DateTime(1996, 4, 1);
        public static DateTime END_DATE = new DateTime(2016, 12, 31);

        //public static DateTime START_DATE = new DateTime(2009, 11, 3);
        //public static DateTime END_DATE = new DateTime(2012, 12, 31);

        //public static int UNSETTLED_FUNDS_DAYS = 1;
    }
}
