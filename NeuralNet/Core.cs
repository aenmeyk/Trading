using System;

namespace NeuralNet
{
    public static class Core
    {
        public static double[][] HiddenWeight;
        public static double[] OutputWeight;
        public static double[] HiddenBias;
        public static double OutputBias;

        static Core()
        {
            initializeHiddenWeightArray();
            initializeOutputWeightArray();
            initializeHiddenBiasArray();
            PopulateWeights();
        }

        public static void PopulateWeights()
        {
            PopulateHiddenWeights();
            PopulateOutputWeights();
        }

        private static void initializeHiddenWeightArray()
        {
            HiddenWeight = new double[NetworkSettings.HiddenNeuronCount][];

            for (int i = 0; i < NetworkSettings.HiddenNeuronCount; i++)
            {
                HiddenWeight[i] = new double[NetworkSettings.InputNeuronCount];
            }
        }

        private static void initializeOutputWeightArray()
        {
            OutputWeight = new double[NetworkSettings.HiddenNeuronCount];
        }

        private static void initializeHiddenBiasArray()
        {
            HiddenBias = new double[NetworkSettings.HiddenNeuronCount];
        }

        private static void PopulateHiddenWeights()
        {
            Random random = new Random();

            for (int neuron2 = 0; neuron2 < NetworkSettings.HiddenNeuronCount; neuron2++)
            {
                for (int neuron1 = 0; neuron1 < NetworkSettings.InputNeuronCount; neuron1++)
                {
                    HiddenWeight[neuron2][neuron1] = (random.NextDouble() * 2) - 1;
                }

                HiddenBias[neuron2] = (random.NextDouble() * 2) - 1;
            }
        }

        private static void PopulateOutputWeights()
        {
            Random random = new Random();

            for (int neuron = 0; neuron < NetworkSettings.HiddenNeuronCount; neuron++)
            {
                OutputWeight[neuron] = (random.NextDouble() * 2) - 1;
            }

            OutputBias = (random.NextDouble() * 2) - 1;
        }
    }
}
