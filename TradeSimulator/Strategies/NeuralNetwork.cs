using System;
using System.Collections.Generic;
using System.Linq;
using Common.Models;
using DataAccess.Repositories;
using NeuralNet;

namespace TradeSimulator.Strategies
{
    public class NeuralNetwork : StrategyBase
    {
        private NeuralNetworkRepository _repository;

        public override IEnumerable<string> Symbols
        {
            get
            {
                return new[]
                {
                    "SCHX",
                    "SCHM",
                    "SCHA",
                    "SCHF",
                    "SCHE",
                    "SCHH",
                    "VNQI",
                };
            }
        }

        protected override string Name
        {
            get { return "Neural Network"; }
        }

        protected override decimal Spread
        {
            get { return 0M; }
        }

        protected override decimal TradingFee
        {
            get { return 0M; }
        }

        protected override decimal TaxRate
        {
            get { return 0M; }
        }

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

            //var trainingInputValues = new[]
            //{
            //    new[] { 1.0, 3.0, 3.0 },
            //    new[] { 4.0, 9.0, 4.0 },
            //    new[] { 5.0, 4.0, 7.0 },
            //    new[] { 3.0, 1.0, 1.0 },
            //    new[] { 1.0, 2.0, 2.0 },
            //    new[] { 3.0, 8.0, 1.0 },
            //    new[] { 5.0, 7.0, 4.0 },
            //    new[] { 2.0, 3.0, 1.0 },
            //    new[] { 5.0, 2.0, 8.0 },
            //    new[] { 2.0, 9.0, 9.0 }
            //};

            //var trainingOutputValues = new[]
            //{
            //    0.0,
            //    1,
            //    1,
            //    0,
            //    0,
            //    1,
            //    1,
            //    0,
            //    1,
            //    1
            //};

            //var testingInputValues = new[]
            //{
            //    new[] { 7.0, 4.0, 2.0 },
            //    new[] { 3.0, 1.0, 1.0 },
            //    new[] { 4.0, 2.0, 2.0 },
            //    new[] { 5.0, 9.0, 5.0 },
            //    new[] { 9.0, 9.0, 9.0 },
            //    new[] { 3.0, 8.0, 1.0 },
            //    new[] { 5.0, 7.0, 4.0 },
            //    new[] { 2.0, 3.0, 1.0 },
            //    new[] { 5.0, 2.0, 8.0 },
            //    new[] { 2.0, 9.0, 9.0 }
            //};

            NetworkManager.InitializeNetwork(trainingInputValues, trainingOutputValues, testingInputValues);
            NetworkManager.TrainNetwork();
            NetworkManager.TestNetwork();
        }

        protected override void ExecuteStrategyImplementation(DateTime date, IEnumerable<Quote> quotes)
        {
            throw new NotImplementedException();
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
