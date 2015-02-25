using System;

namespace NeuralNet
{
    public class Core
    {
        private int _inputNeuronCount;
        private int _hiddenNeuronCount;

        public double[][] HiddenWeight;
        public double[] OutputWeight;
        public double[] HiddenBias;
        public double OutputBias;

        public Core()
        {
            _inputNeuronCount = NetworkSettings.InputNeuronCount;
            _hiddenNeuronCount = NetworkSettings.HiddenNeuronCount;
        }

        public void InitializeArrays()
        {
            InitializeHiddenWeightArray();
            InitializeOutputWeightArray();
            InitializeHiddenBiasArray();
        }

        public void InitializeRandomValues()
        {
            PopulateHiddenWeights();
            PopulateOutputWeights();
        }

        private void InitializeHiddenWeightArray()
        {
            HiddenWeight = new double[_hiddenNeuronCount][];

            for (int i = 0; i < _hiddenNeuronCount; i++)
            {
                HiddenWeight[i] = new double[_inputNeuronCount];
            }
        }

        private void InitializeOutputWeightArray()
        {
            OutputWeight = new double[_hiddenNeuronCount];
        }

        private void InitializeHiddenBiasArray()
        {
            HiddenBias = new double[_hiddenNeuronCount];
        }

        private void PopulateHiddenWeights()
        {
            var random = new Random();

            for (int neuron2 = 0; neuron2 < _hiddenNeuronCount; neuron2++)
            {
                for (int neuron1 = 0; neuron1 < _inputNeuronCount; neuron1++)
                {
                    HiddenWeight[neuron2][neuron1] = (random.NextDouble() * 2) - 1;
                }

                HiddenBias[neuron2] = (random.NextDouble() * 2) - 1;
            }
        }

        private void PopulateOutputWeights()
        {
            var random = new Random();

            for (int neuron = 0; neuron < _hiddenNeuronCount; neuron++)
            {
                OutputWeight[neuron] = (random.NextDouble() * 2) - 1;
            }

            OutputBias = (random.NextDouble() * 2) - 1;
        }
    }
}
