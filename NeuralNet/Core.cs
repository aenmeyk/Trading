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
            initializeOutputBiasArray();

            PopulateWeights();
        }

        public static void PopulateWeights()
        {
            PopulateHiddenWeights();
            PopulateOutputWeights();
        }

        private static void initializeHiddenWeightArray()
        {
            HiddenWeight = new double[NetworkSettings.HiddenNeurons][];

            for (int i = 0; i < NetworkSettings.HiddenNeurons; i++)
            {
                HiddenWeight[i] = new double[NetworkSettings.InputNeurons];
            }
        }

        private static void initializeOutputWeightArray()
        {
            OutputWeight = new double[NetworkSettings.HiddenNeurons];
        }

        private static void initializeHiddenBiasArray()
        {
            HiddenBias = new double[NetworkSettings.HiddenNeurons];
        }

        private static void initializeOutputBiasArray()
        {
        }

        private static void PopulateHiddenWeights()
        {
            Random random = new Random();

            for (int neuron2 = 0; neuron2 < NetworkSettings.HiddenNeurons; neuron2++)
            {
                for (int neuron1 = 0; neuron1 < NetworkSettings.InputNeurons; neuron1++)
                {
                    HiddenWeight[neuron2][neuron1] = (random.NextDouble() * 2) - 1;
                }

                HiddenBias[neuron2] = (random.NextDouble() * 2) - 1;
            }
        }

        private static void PopulateOutputWeights()
        {
            Random random = new Random();

            for (int neuron = 0; neuron < NetworkSettings.HiddenNeurons; neuron++)
            {
                OutputWeight[neuron] = (random.NextDouble() * 2) - 1;
            }

            OutputBias = (random.NextDouble() * 2) - 1;
        }
    }
}
