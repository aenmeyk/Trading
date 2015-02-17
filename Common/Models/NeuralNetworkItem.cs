using System;

namespace Common.Models
{
    public class NeuralNetworkItem
    {
        public string Symbol { get; set; }
        public DateTime DateValue { get; set; }
        public double PriceChange { get; set; }
        public double VolumeChange { get; set; }
        public double PriceOverMovingAvg2 { get; set; }
        public double PriceOverMovingAvg4 { get; set; }
        public double PriceOverMovingAvg8 { get; set; }
        public double PriceOverMovingAvg16 { get; set; }
        public double PriceOverMovingAvg32 { get; set; }
        public double VolumeOverMovingAvg2 { get; set; }
        public double VolumeOverMovingAvg4 { get; set; }
        public double VolumeOverMovingAvg8 { get; set; }
        public double VolumeOverMovingAvg16 { get; set; }
        public double VolumeOverMovingAvg32 { get; set; }
        public double PriceRsd32 { get; set; }
        public double PriceRsd64 { get; set; }
        public double PriceRsd128 { get; set; }
        public double PriceRsd256 { get; set; }
        public double PriceRsd512 { get; set; }
        public double VolumeRsd32 { get; set; }
        public double VolumeRsd64 { get; set; }
        public double VolumeRsd128 { get; set; }
        public double VolumeRsd256 { get; set; }
        public double VolumeRsd512 { get; set; }
        public double FuturePriceChange1 { get; set; }
        public double FuturePriceChange2 { get; set; }
        public double FuturePriceChange3 { get; set; }
        public double FuturePriceChange5 { get; set; }
        public double FuturePriceChange10 { get; set; }
    }
}
