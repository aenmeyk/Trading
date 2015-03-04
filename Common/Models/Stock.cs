namespace Common.Models
{
    public class Stock
    {
        private decimal _spread;

        public string Symbol { get; set; }
        public decimal NetExpenseRatio { get; set; }
        public decimal Spread
        {
            get
            {
                if (GeneralSettings.IGNORE_SPREAD)
                {
                    return 0;
                }

                return _spread;
            }
            set
            {
                _spread = value;
            }
        }
        public bool SchwabOneSource { get; set; }
        public bool IsIndex { get; set; }
        public decimal TradingFee
        {
            get { return SchwabOneSource || IsIndex ? 0 : GeneralSettings.TRADING_FEE; }
        }
    }
}
