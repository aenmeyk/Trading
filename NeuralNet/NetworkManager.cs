using System;
using System.Diagnostics;
using System.Linq;

namespace NeuralNet
{
    public static class NetworkManager
    {
        private static Network _trainingNetwork ;
        private static Network _testingNetwork;

        public static void InitializeNetwork(double[][] trainingInputValues, double[] trainingOutputValues, double[][] testingInputValues)
        {
            NetworkSettings.InputNeurons = trainingInputValues[0].Count();
            NetworkSettings.HiddenNeurons = NetworkSettings.InputNeurons;

            _trainingNetwork = new Network(trainingInputValues, trainingOutputValues);
            _testingNetwork = new Network(testingInputValues);

            Core.PopulateWeights();
        }

        public static void TrainNetwork()
        {
            for (int trial = 0; trial < ProcessingSettings.Trials; trial++)
            {
                _trainingNetwork.CurrentLearningRate = ProcessingSettings.StartLearningRate - (trial * ProcessingSettings.LearningRateMultiplier);

                for (int record = 0; record < _trainingNetwork.InputValues.Count(); record++)
                {
                    NetworkOperations.RunHiddenLayer(_trainingNetwork, record);
                    NetworkOperations.RunOutputLayer(_trainingNetwork, record);
                    NetworkOperations.CalculateDelta(_trainingNetwork, record);
                    NetworkOperations.BackPropogate(_trainingNetwork, record);
                }
            }
        }

        public static void TestNetwork()
        {
            for (int record = 0; record < _testingNetwork.InputValues.Count(); record++)
            {
                NetworkOperations.RunHiddenLayer(_testingNetwork, record);
                NetworkOperations.RunOutputLayer(_testingNetwork, record);
            }

            foreach (var outputValue in _testingNetwork.OutputOutput)
            {
                Debug.WriteLine(outputValue);
            }
        }
    }
}
