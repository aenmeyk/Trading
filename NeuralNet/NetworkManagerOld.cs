//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Threading.Tasks;
//using Common.ExtensionMethods;
//using Common.Models;
//using DataAccess.Repositories;

//namespace NeuralNet
//{
//    public class NetworkManagerOld
//    {
//        private HiddenWeightsRepository _hiddenWeightsRepository = new HiddenWeightsRepository();
//        private OutputWeightsRepository _outputWeightsRepository = new OutputWeightsRepository();
//        private HiddenBiasesRepository _hiddenBiasesRepository = new HiddenBiasesRepository();
//        private OutputBiasesRepository _outputBiasesRepository = new OutputBiasesRepository();

//        public void TrainNetwork(IEnumerable<NeuralNetworkItem> neuralNetworkItems)
//        {
//            SetNeuronCounts();
//            Core.InitializeRandomValues();

//            //for (var trial = 0; trial < ProcessingSettings.Trials; trial++)
//            Parallel.For(0, ProcessingSettings.Trials, trial =>
//            {
//                var shuffledNetworkItems = neuralNetworkItems.ToList();
//                shuffledNetworkItems.Shuffle();
//                var inputValues = ConvertToInputValues(shuffledNetworkItems);
//                var outputValues = ConvertToOutputValues(shuffledNetworkItems);
//                var network = new Network(inputValues, outputValues);

//                network.LearningRate = ProcessingSettings.LearningRate;
//                ProcessingSettings.LearningRate *= ProcessingSettings.LearningRateMultiplier;

//                for (int record = 0; record < network.InputValues.Count(); record++)
//                {
//                    NetworkOperations.RunHiddenLayer(network, record);
//                    NetworkOperations.RunOutputLayer(network, record);
//                    NetworkOperations.CalculateDelta(network, record);
//                    NetworkOperations.BackPropogate(network, record);
//                }
//             });
//            //}
//        }

//        public void TestNetwork(IEnumerable<NeuralNetworkItem> neuralNetworkItems)
//        {
//            var inputValues = ConvertToInputValues(neuralNetworkItems);
//            var network = new Network(inputValues);

//            for (int record = 0; record < network.InputValues.Count(); record++)
//            {
//                NetworkOperations.RunHiddenLayer(network, record);
//                NetworkOperations.RunOutputLayer(network, record);
//            }

//            foreach (var outputValue in network.OutputOutput)
//            {
//                Console.WriteLine(Math.Round(outputValue, 15));
//            }
//        }

//        public double Predict(NeuralNetworkItem neuralNetworkItem)
//        {
//            var inputValues = ConvertToInputValues(new[] { neuralNetworkItem });
//            var network = new Network(inputValues);

//            NetworkOperations.RunHiddenLayer(network, 0);
//            NetworkOperations.RunOutputLayer(network, 0);

//            return network.OutputOutput[0];
//        }

//        public void PersistNetworkValues(string symbol)
//        {
//            var hiddenWeights = new Collection<NeuronValue>();
//            var outputWeights = new Collection<NeuronValue>();
//            var hiddenBiases = new Collection<NeuronValue>();

//            for (int hiddenNeuron = 0; hiddenNeuron < NetworkSettings.HiddenNeuronCount; hiddenNeuron++)
//            {
//                for (int inputNeuron = 0; inputNeuron < NetworkSettings.InputNeuronCount; inputNeuron++)
//                {
//                    hiddenWeights.Add(new NeuronValue
//                    {
//                        Symbol = symbol,
//                        HiddenNeuronIndex = hiddenNeuron,
//                        InputNeuronIndex = inputNeuron,
//                        Value = Core.HiddenWeight[hiddenNeuron][inputNeuron]
//                    });
//                }

//                outputWeights.Add(new NeuronValue
//                {
//                    Symbol = symbol,
//                    HiddenNeuronIndex = hiddenNeuron,
//                    Value = Core.OutputWeight[hiddenNeuron]
//                });

//                hiddenBiases.Add(new NeuronValue
//                {
//                    Symbol = symbol,
//                    HiddenNeuronIndex = hiddenNeuron,
//                    Value = Core.HiddenBias[hiddenNeuron]
//                });
//            }

//            var outputBias = new NeuronValue
//            {
//                Symbol = symbol,
//                Value = Core.OutputBias
//            };

//            _hiddenWeightsRepository.DeleteForSymbol(symbol);
//            _outputWeightsRepository.DeleteForSymbol(symbol);
//            _hiddenBiasesRepository.DeleteForSymbol(symbol);
//            _outputBiasesRepository.DeleteForSymbol(symbol);

//            _hiddenWeightsRepository.InsertNeuronValues(hiddenWeights);
//            _outputWeightsRepository.InsertNeuronValues(outputWeights);
//            _hiddenBiasesRepository.InsertNeuronValues(hiddenBiases);
//            _outputBiasesRepository.InsertNeuronValues(outputBias);
//        }

//        public bool LoadNetworkValues(string symbol)
//        {
//            var hiddenWeights = _hiddenWeightsRepository.GetForSymbols<NeuronValue>(new[] { symbol });

//            if (!hiddenWeights.Any())
//            {
//                return false;
//            }

//            var outputWeights = _outputWeightsRepository.GetForSymbols<NeuronValue>(new[] { symbol });
//            var hiddenBiases = _hiddenBiasesRepository.GetForSymbols<NeuronValue>(new[] { symbol });
//            var outputBiases = _outputBiasesRepository.GetForSymbols<NeuronValue>(new[] { symbol });

//            SetNeuronCounts();

//            Core.HiddenWeight = new double[NetworkSettings.HiddenNeuronCount][];
//            Core.OutputWeight = new double[NetworkSettings.HiddenNeuronCount];
//            Core.HiddenBias = new double[NetworkSettings.HiddenNeuronCount];
//            Core.OutputBias = outputBiases.Single().Value;

//            for (int i = 0; i < NetworkSettings.HiddenNeuronCount; i++)
//            {
//                Core.HiddenWeight[i] = new double[NetworkSettings.HiddenNeuronCount];
//            }

//            foreach (var neuronValue in hiddenWeights)
//            {
//                Core.HiddenWeight[neuronValue.HiddenNeuronIndex][neuronValue.InputNeuronIndex] = neuronValue.Value;
//            }

//            foreach (var neuronValue in outputWeights)
//            {
//                Core.OutputWeight[neuronValue.HiddenNeuronIndex] = neuronValue.Value;
//            }

//            foreach (var neuronValue in hiddenBiases)
//            {
//                Core.HiddenBias[neuronValue.HiddenNeuronIndex] = neuronValue.Value;
//            }

//            return true;
//        }

//        private void SetNeuronCounts()
//        {
//            NetworkSettings.InputNeuronCount = INPUT_NEURON_COUNT;
//            NetworkSettings.HiddenNeuronCount = INPUT_NEURON_COUNT * 2;
//        }

//        private int INPUT_NEURON_COUNT = 22;

//        private double[][] ConvertToInputValues(IEnumerable<NeuralNetworkItem> neuralNetworkItems, bool printOutput = false)
//        {
//            var result = new double[neuralNetworkItems.Count()][];
//            var i = 0;

//            foreach (var item in neuralNetworkItems)
//            {
//                var k = 0;
//                result[i] = new double[INPUT_NEURON_COUNT];
//                result[i][k++] = Amplify(item.PriceChange);
//                result[i][k++] = TransferFunction(item.VolumeChange);
//                result[i][k++] = Amplify(item.PriceOverMovingAvg2);
//                result[i][k++] = Amplify(item.PriceOverMovingAvg4);
//                result[i][k++] = Amplify(item.PriceOverMovingAvg8);
//                result[i][k++] = Amplify(item.PriceOverMovingAvg16);
//                result[i][k++] = Amplify(item.PriceOverMovingAvg32);
//                result[i][k++] = TransferFunction(item.VolumeOverMovingAvg2);
//                result[i][k++] = TransferFunction(item.VolumeOverMovingAvg4);
//                result[i][k++] = TransferFunction(item.VolumeOverMovingAvg8);
//                result[i][k++] = TransferFunction(item.VolumeOverMovingAvg16);
//                result[i][k++] = TransferFunction(item.VolumeOverMovingAvg32);
//                result[i][k++] = item.PriceRsd32;
//                result[i][k++] = item.PriceRsd64;
//                result[i][k++] = item.PriceRsd128;
//                result[i][k++] = item.PriceRsd256;
//                result[i][k++] = item.PriceRsd512;
//                result[i][k++] = TransferFunction(item.VolumeRsd32);
//                result[i][k++] = TransferFunction(item.VolumeRsd64);
//                result[i][k++] = TransferFunction(item.VolumeRsd128);
//                result[i][k++] = TransferFunction(item.VolumeRsd256);
//                result[i][k++] = TransferFunction(item.VolumeRsd512);

//                i++;
//            }

//            //foreach (var item in neuralNetworkItems)
//            //{
//            //    result[i] = new double[11];
//            //    result[i][0] = item.PriceChange;
//            //    result[i][1] = item.PriceOverMovingAvg2;
//            //    result[i][2] = item.PriceOverMovingAvg4;
//            //    result[i][3] = item.PriceOverMovingAvg8;
//            //    result[i][4] = item.PriceOverMovingAvg16;
//            //    result[i][5] = item.PriceOverMovingAvg32;
//            //    result[i][6] = item.PriceRsd32;
//            //    result[i][7] = item.PriceRsd64;
//            //    result[i][8] = item.PriceRsd128;
//            //    result[i][9] = item.PriceRsd256;
//            //    result[i][10] = item.PriceRsd512;

//            //    i++;
//            //}

//            return result;
//        }

//        private double[] ConvertToOutputValues(IEnumerable<NeuralNetworkItem> neuralNetworkItems)
//        {
//            var result = new double[neuralNetworkItems.Count()];
//            var i = 0;

//            foreach (var item in neuralNetworkItems)
//            {
//                result[i] = Amplify(item.FuturePriceChange1); // > 1 ? 1 : 0;
//                i++;
//            }

//            return result;
//        }

//        public static double TransferFunction(double val)
//        {
//            return 1 / (1 + Math.Exp(-val));
//        }

//        public static double Amplify(double val)
//        {
//            return 1 / (1 + Math.Exp(-(val - 1) * 100));
//        }
//    }
//}
