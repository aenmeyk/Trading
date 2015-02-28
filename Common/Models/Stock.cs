namespace Common.Models
{
    public class Stock
    {
        public string Symbol { get; set; }
        public decimal NetExpenseRatio { get; set; }
        public decimal Spread { get; set; }
        public bool SchwabOneSource { get; set; }
        public bool IsIndex { get; set; }
        public decimal TradingFee
        {
            get { return SchwabOneSource || IsIndex ? 0 : GeneralSettings.TRADING_FEE; }
        }
    }
}
