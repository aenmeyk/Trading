using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using DataAccess.Repositories;
using NeuralNet;
using TradeSimulator.Model;

namespace TradeSimulator.Strategies
{
    public class NeuralNetwork : StrategyBase
    {
        private NeuralNetworkRepository _repository;
        private NetworkManager _networkManager = new NetworkManager();

        public override IEnumerable<string> Symbols
        {
            get
            {
                return new[]
                {
                    "RSP",
                    "SCHX",
                    "SPLV",
                    "SCHB",
                    "SCHD",
                    "RPV",
                    "RPG",
                    "LOWC",
                    "PIN",
                    "PKW",
                    "SCHG",
                    "SCHH",
                    "SCHV",
                };
            }
        }

        //public override IEnumerable<string> Symbols
        //{
        //    get
        //    {
        //        return new[]
        //        {
        //            "ACIM",
        //            "BIK",
        //            "CSM",
        //            "CVY",
        //            "CWI",
        //            "DEF",
        //            "DGRE",
        //            "DGRS",
        //            "DGRW",
        //            "DNL",
        //            "DRW",
        //            "DWAS",
        //            "DWX",
        //            "EDIV",
        //            "EDOG",
        //            "EEB",
        //            "EELV",
        //            "EMBB",
        //            "EWEM",
        //            "EWRS",
        //            "EWX",
        //            "FEU",
        //            "FNDA",
        //            "FNDB",
        //            "FNDC",
        //            "FNDE",
        //            "FNDF",
        //            "FNDX",
        //            "GAL",
        //            "GMF",
        //            "GML",
        //            "GXC",
        //            "HGI",
        //            "IBLN",
        //            "IDLV",
        //            "IDOG",
        //            "IHDG",
        //            "INKM",
        //            "JPP",
        //            "KNOW",
        //            "MDD",
        //            "MDYG",
        //            "MDYV",
        //            "NOBL",
        //            "PAF",
        //            "PDP",
        //            "PID",
        //            "PIE",
        //            "PIN",
        //            "PIZ",
        //            "PKW",
        //            "PXMC",
        //            "PXSV",
        //            "QAUS",
        //            "QCAN",
        //            "QDEU",
        //            "QESP",
        //            "QGBR",
        //            "QJPN",
        //            "QKOR",
        //            "QMEX",
        //            "QQQE",
        //            "QTWN",
        //            "RFG",
        //            "RFV",
        //            "RPG",
        //            "RPV",
        //            "RSCO",
        //            "RSP",
        //            "RWO",
        //            "RZG",
        //            "RZV",
        //            "SCHA",
        //            "SCHB",
        //            "SCHC",
        //            "SCHD",
        //            "SCHE",
        //            "SCHF",
        //            "SCHG",
        //            "SCHH",
        //            "SCHM",
        //            "SCHV",
        //            "SCHX",
        //            "SDOG",
        //            "SLYG",
        //            "SLYV",
        //            "SPHB",
        //            "SPLV",
        //            "SYE",
        //            "SYG",
        //            "SYV",
        //            "TOLZ",
        //            "VSPY",
        //            "WDIV",
        //            "WMCR",
        //            "XLG",
        //            "YAO"
        //        };
        //    }
        //}

        protected override string Name
        {
            get { return "Neural Network"; }
        }

        protected override decimal Spread { get { return 0M; } }
        protected override decimal TaxRate { get { return 0M; } }
        protected override decimal TradingFee { get { return 0M; } }

        //protected override decimal Spread { get { return 0.000409683076M; } }
        //protected override decimal TaxRate { get { return 0.28M; } }
        //protected override decimal TradingFee { get { return 8.95M; } }

        public NeuralNetwork()
        {
            _repository = new NeuralNetworkRepository();
        }

        public override void Initialize(DateTime startDate)
        {
            base.Initialize(startDate);

            foreach (var symbol in Symbols)
            {
                var neuralNetworkItems = _repository.GetForSymbols<NeuralNetworkItem>(new[] { symbol });
                var trainingData = neuralNetworkItems.Where(x => x.Symbol == symbol && x.DateValue < startDate);

                if (trainingData.Any())
                {
                    _networkManager.TrainNetwork(trainingData);
                    _networkManager.PersistNetworkValues(symbol);
                }
            }
        }

        private static readonly object locker = new object();

        DateTime lastDate = DateTime.MinValue;

        protected override void ExecuteStrategyImplementation(DateTime date, IEnumerable<Quote> quotes)
        {
            if ((date - lastDate).TotalDays >= 0)
            {
                Quote selectedQuote = null;
                var highestPrediction = double.MinValue;

                foreach (var quote in quotes)
                //Parallel.ForEach(quotes, quote =>
                {
                    var loadSuccessful = _networkManager.LoadNetworkValues(quote.Symbol);

                    if (loadSuccessful)
                    {
                        var neuralNetworkItems = _repository.GetForSymbolAndDate<NeuralNetworkItem>(quote.Symbol, date);
                        var neuralNetworkItem = neuralNetworkItems.SingleOrDefault(x => x.DateValue == date);

                        if (neuralNetworkItem != null)
                        {
                            var prediction = _networkManager.Predict(neuralNetworkItem);

                            lock (locker)
                            {
                                if (prediction >= highestPrediction)
                                {
                                    highestPrediction = prediction;
                                    selectedQuote = quote;
                                }
                            }
                        }
                    }
                //});
                };

                if (selectedQuote != null && !Account.Portfolio.PositionDictionary.ContainsKey(selectedQuote.Symbol))
                {
                    var purchaseRequest = new PurchaseRequest { Quote = selectedQuote, Percent = 1 };
                    Account.Liquidate(date);
                    Account.Buy(new[] { purchaseRequest });
                }

                lastDate = date;
            }
        }
    }
}
