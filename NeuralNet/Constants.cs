using System;

namespace NeuralNet
{
    public static class NetworkSettings
    {
        public static int InputNeurons = 3;
        public static int HiddenNeurons = 2;
        public static int PredictionDaysAhead = 1;
    }

    public static class ProcessingSettings
    {
        public static DateTime TestPeriodStartDate = new DateTime(1990, 1, 20);
        public static DateTime TestPeriodEndDate = new DateTime(2009, 10, 29);
        public static bool ForwardDates = true;
        public static double StartLearningRate = 0.2;
        public static double EndLearningRate = 0.0001;
        public static int Trials = 1000;
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
