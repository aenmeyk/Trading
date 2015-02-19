using System;

namespace NeuralNet
{
    public class NetworkOperations
    {
        public static void RunHiddenLayer(Network network, int record)
        {
            double currentNetValue = 0;

            for (int neuron = 0; neuron < NetworkSettings.HiddenNeuronCount; neuron++)
            {
                for (int k = 0; k < NetworkSettings.InputNeuronCount; k++)
                {
                    currentNetValue += network.InputValues[record][k] * Core.HiddenWeight[neuron][k];
                }

                currentNetValue += currentNetValue / (NetworkSettings.InputNeuronCount - 1.0);
                network.HiddenOutput[record][neuron] = TransferFunction(currentNetValue + Core.HiddenBias[neuron]);
            }
        }

        public static void RunOutputLayer(Network network, int record)
        {
            double currentNetValue = 0;

            for (int k = 0; k < NetworkSettings.HiddenNeuronCount; k++)
            {
                currentNetValue += network.HiddenOutput[record][k] * Core.OutputWeight[k];
            }

            network.OutputOutput[record] = TransferFunction(currentNetValue + Core.OutputBias);
        }

        public static void CalculateDelta(Network network, int record)
        {
            double currentErrorFactor = 0;

            //RUN OUTPUT LAYER
            currentErrorFactor = network.OutputValues[record] - network.OutputOutput[record];
            network.OutputDelta[record] = network.OutputOutput[record] * (1 - network.OutputOutput[record]) * currentErrorFactor;

            //RUN HIDDEN LAYER
            for (int neuron = 0; neuron < NetworkSettings.HiddenNeuronCount; neuron++)
            {
                currentErrorFactor = network.OutputDelta[record] * Core.OutputWeight[neuron];
                network.HiddenDelta[record][neuron] = network.HiddenOutput[record][neuron] * (1 - network.HiddenOutput[record][neuron]) * currentErrorFactor;
            }
        }

        public static void BackPropogate(Network network, int record)
        {
            //UPDATE BIAS AND WEIGHTS FOR HIDDEN NEURONS
            for (int neuron = 0; neuron < NetworkSettings.HiddenNeuronCount; neuron++)
            {
                for (int k = 0; k < NetworkSettings.InputNeuronCount; k++)
                {
                    Core.HiddenWeight[neuron][k] += network.CurrentLearningRate * network.InputValues[record][k] * network.HiddenDelta[record][neuron];
                }

                Core.HiddenBias[neuron] += network.CurrentLearningRate * network.HiddenDelta[record][neuron];
            }

            //UPDATE BIAS AND WEIGHTS FOR OUTPUT NEURONS
            for (int k = 0; k < NetworkSettings.HiddenNeuronCount; k++)
            {
                Core.OutputWeight[k] += network.CurrentLearningRate * network.HiddenOutput[record][k] * network.OutputDelta[record];
            }

            Core.OutputBias += network.CurrentLearningRate * network.OutputDelta[record];
        }

        public static double TransferFunction(double val)
        {
            //return transferFunction(val, 1);
            return 1 / (1 + Math.Exp(-val)); //SIGMOID
        }
    }

    //public static double transferFunction(double val, double slope)
    //{
    //    return 1 / (1 + Math.Exp(-val * slope)); //SIGMOID
    //    //return 1 / (1 + Math.Exp(-val)); //SIGMOID

    //    //double slope = 0.3;
    //    //return Math.Sign(val) * (1 - Math.Exp(-1 * (Math.Pow((val * slope), 2)))); //DOUBLE SIGMOID
    //    //return Math.Sign(val) * (1 - Math.Exp(-1 * (Math.Pow(val, 2)))); // DOUBLE SIGMOID
    // }}
}
