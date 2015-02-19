using System;

namespace NeuralNet
{
    public static class NetworkSettings
    {
        public static int InputNeurons = -1;
        public static int HiddenNeurons = -1;
    }

    public static class ProcessingSettings
    {
        public static DateTime TestPeriodStartDate = new DateTime(2015, 1, 1);
        public static DateTime TestPeriodEndDate = new DateTime(2020, 12, 31);
        public static bool ForwardDates = true;
        public static double StartLearningRate = 0.2;
        public static double EndLearningRate = 0.0001;
        public static int Trials = 100;
        public static int ThresholdDivisor = 0;
        public static int FixedTopSecurities = 1;
        public static double FixedThreshold = 0;

        public static double LearningRateMultiplier;
    }

    public static class TestSettings
    {
        public static DateTime TrainStartDate { get; set; }
        public static DateTime TrainEndDate { get; set; }
        public static DateTime TestStartDate { get; set; }
        public static DateTime TestEndDate { get; set; }
    }
}
