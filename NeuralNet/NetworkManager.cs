using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using DataAccess.Repositories;

namespace NeuralNet
{
    public class NetworkManager
    {
        private double[][] _trainingInputValues;
        private double[] _trainingOutputValues;
        private double[][] _testingInputValues;
        private Network _trainingNetwork;
        private Network _testingNetwork;

        private HiddenWeightsRepository _hiddenWeightsRepository = new HiddenWeightsRepository();
        private OutputWeightsRepository _outputWeightsRepository = new OutputWeightsRepository();
        private HiddenBiasesRepository _hiddenBiasesRepository = new HiddenBiasesRepository();
        private OutputBiasesRepository _outputBiasesRepository = new OutputBiasesRepository();

        public NetworkManager(double[][] trainingInputValues, double[] trainingOutputValues, double[][] testingInputValues)
        {
            NetworkSettings.InputNeuronCount = trainingInputValues[0].Count();
            NetworkSettings.HiddenNeuronCount = NetworkSettings.InputNeuronCount;

            _trainingInputValues = trainingInputValues;
            _trainingOutputValues = trainingOutputValues;
            _testingInputValues = testingInputValues;

            Core.PopulateWeights();
        }

        public void TrainNetwork()
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

        public void TestNetwork()
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

        public void PersistNetworkValues()
        {
            var hiddenWeights = new Collection<NeuronValue>();
            var outputWeights = new Collection<NeuronValue>();
            var hiddenBiases = new Collection<NeuronValue>();

            for (int hiddenNeuron = 0; hiddenNeuron < NetworkSettings.HiddenNeuronCount; hiddenNeuron++)
            {
                for (int inputNeuron = 0; inputNeuron < NetworkSettings.InputNeuronCount; inputNeuron++)
                {
                    hiddenWeights.Add(new NeuronValue
                    {
                        HiddenNeuronIndex = hiddenNeuron,
                        InputNeuronIndex = inputNeuron,
                        Value = Core.HiddenWeight[hiddenNeuron][inputNeuron]
                    });
                }

                outputWeights.Add(new NeuronValue
                {
                    HiddenNeuronIndex = hiddenNeuron,
                    Value = Core.OutputWeight[hiddenNeuron]
                });

                hiddenBiases.Add(new NeuronValue
                {
                    HiddenNeuronIndex = hiddenNeuron,
                    Value = Core.HiddenBias[hiddenNeuron]
                });
            }

            var outputBias = new NeuronValue { Value = Core.OutputBias };

            _hiddenWeightsRepository.InsertNeuronValues(hiddenWeights);
            _outputWeightsRepository.InsertNeuronValues(outputWeights);
            _hiddenBiasesRepository.InsertNeuronValues(hiddenBiases);
            _outputBiasesRepository.InsertNeuronValues(outputBias);
        }
    }
}
