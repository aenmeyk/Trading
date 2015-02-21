using System;

namespace NeuralNet
{
    internal static class NetworkSettings
    {
        public static int InputNeuronCount = -1;
        public static int HiddenNeuronCount = -1;
    }

    internal static class ProcessingSettings
    {
        //public static DateTime TestPeriodStartDate = new DateTime(2015, 1, 1);
        //public static DateTime TestPeriodEndDate = new DateTime(2020, 12, 31);
        public static double LearningRate = 0.15;
        //public static double EndLearningRate = 0.0001;
        public static int Trials = 80;
        public static double LearningRateMultiplier = 1;
    }

    internal static class TestSettings
    {
        public static DateTime TrainStartDate { get; set; }
        public static DateTime TrainEndDate { get; set; }
        public static DateTime TestStartDate { get; set; }
        public static DateTime TestEndDate { get; set; }
    }
}
