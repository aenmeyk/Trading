using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Models;
using DataAccess.Repositories;
using NeuralNet;

namespace TradeSimulator.Strategies
{
    public class NeuralNetwork
    {
        private NeuralNetworkRepository _repository;

        public NeuralNetwork()
        {
            _repository = new NeuralNetworkRepository();
        }

        public void Run()
        {
            var data = _repository.Get<NeuralNetworkItem>();
            var trainingData = data.Where(x => x.DateValue < ProcessingSettings.TestPeriodStartDate);
            var testingData = data.Where(x => x.DateValue >= ProcessingSettings.TestPeriodStartDate);
            var trainingInputValues = ConvertToInputValues(trainingData);
            var trainingOutputValues = ConvertToOutputValues(trainingData);
            var testingInputValues = ConvertToInputValues(testingData, printOutput: true);

            NetworkManager.InitializeNetwork(trainingInputValues, trainingOutputValues, testingInputValues);
            NetworkManager.TrainNetwork();
            NetworkManager.TestNetwork();
        }

        private double[][] ConvertToInputValues(IEnumerable<NeuralNetworkItem> neuralNetworkItems, bool printOutput = false)
        {
            var result = new double[neuralNetworkItems.Count()][];
            var i = 0;

            foreach (var item in neuralNetworkItems)
            {
                result[i] = new double[22];
                result[i][0] = item.PriceChange;
                result[i][1] = item.VolumeChange;
                result[i][2] = item.PriceOverMovingAvg2;
                result[i][3] = item.PriceOverMovingAvg4;
                result[i][4] = item.PriceOverMovingAvg8;
                result[i][5] = item.PriceOverMovingAvg16;
                result[i][6] = item.PriceOverMovingAvg32;
                result[i][7] = item.VolumeOverMovingAvg2;
                result[i][8] = item.VolumeOverMovingAvg4;
                result[i][9] = item.VolumeOverMovingAvg8;
                result[i][10] = item.VolumeOverMovingAvg16;
                result[i][11] = item.VolumeOverMovingAvg32;
                result[i][12] = item.PriceRsd32;
                result[i][13] = item.PriceRsd64;
                result[i][14] = item.PriceRsd128;
                result[i][15] = item.PriceRsd256;
                result[i][16] = item.PriceRsd512;
                result[i][17] = item.VolumeRsd32;
                result[i][18] = item.VolumeRsd64;
                result[i][19] = item.VolumeRsd128;
                result[i][20] = item.VolumeRsd256;
                result[i][21] = item.VolumeRsd512;

                if (printOutput)
                {
                    Debug.WriteLine(string.Format("{0}\t{1}\t{2}", item.Symbol, item.DateValue.ToShortDateString(), item.FuturePriceChange1));
                }

                i++;
            }

            return result;
        }

        private double[] ConvertToOutputValues(IEnumerable<NeuralNetworkItem> neuralNetworkItems)
        {
            var result = new double[neuralNetworkItems.Count()];
            var i = 0;

            foreach (var item in neuralNetworkItems)
            {
                result[i] = item.FuturePriceChange1;
                i++;
            }

            return result;
        }
    }
}
