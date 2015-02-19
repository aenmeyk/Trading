using System.Linq;
using System.Threading.Tasks;

namespace NeuralNet
{
    public static class NetworkManager
    {
        private static double[][] _trainingInputValues;
        private static double[] _trainingOutputValues;
        private static double[][] _testingInputValues;
        private static Network _trainingNetwork;
        private static Network _testingNetwork;

        public static void InitializeNetwork(double[][] trainingInputValues, double[] trainingOutputValues, double[][] testingInputValues)
        {
            NetworkSettings.InputNeurons = trainingInputValues[0].Count();
            NetworkSettings.HiddenNeurons = NetworkSettings.InputNeurons;

            _trainingInputValues = trainingInputValues;
            _trainingOutputValues = trainingOutputValues;
            _testingInputValues = testingInputValues;

            Core.PopulateWeights();
        }

        public static void TrainNetwork()
        {
            Parallel.For(0, ProcessingSettings.Trials, trial =>
            {
                _trainingNetwork = new Network(_trainingInputValues, _trainingOutputValues);
                _trainingNetwork.CurrentLearningRate = ProcessingSettings.StartLearningRate - (trial * ProcessingSettings.LearningRateMultiplier);

                for (int record = 0; record < _trainingNetwork.InputValues.Count(); record++)
                {
                    NetworkOperations.RunHiddenLayer(_trainingNetwork, record);
                    NetworkOperations.RunOutputLayer(_trainingNetwork, record);
                    NetworkOperations.CalculateDelta(_trainingNetwork, record);
                    NetworkOperations.BackPropogate(_trainingNetwork, record);
                }
            });
        }

        public static void TestNetwork()
        {
            _testingNetwork = new Network(_testingInputValues);

            for (int record = 0; record < _testingNetwork.InputValues.Count(); record++)
            {
                NetworkOperations.RunHiddenLayer(_testingNetwork, record);
                NetworkOperations.RunOutputLayer(_testingNetwork, record);
            }

            //foreach (var outputValue in _testingNetwork.OutputOutput)
            //{
            //    Console.WriteLine(Math.Round(outputValue, 4));
            //}
        }
    }
}
