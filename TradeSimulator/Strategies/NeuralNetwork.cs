using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static readonly object locker = new object();
        private NeuralNetworkRepository _repository;
        private NetworkManager _networkManager = new NetworkManager();
        private Dictionary<string, Core> _cores = new Dictionary<string, Core>();
        private DateTime lastDate = DateTime.MinValue;

        public override IEnumerable<string> Symbols
        {
            get
            {
                return new[]
                {
"EWG",
"EWU",
"EWQ",
"EWC",
                    "EWH",
                    "EWA",
                    "EWI",
                    //"EWP",
                    //"EWK",
                    //"EWD",
                    //"EWL",
                    //"EWM",
                    //"EWO",
                    //"EWS",
                    //"EWN",
                };
            }
        }

        protected override string Name
        {
            get { return "Neural Network"; }
        }

        protected override string LoggingLevel { get { return "All"; } }
        protected override bool PrintRunningBalance { get { return false; } }


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

        public override void Initialize(DateTime startDate, Dictionary<DateTime, IEnumerable<PriceHistory>> allQuotes)
        {
            base.Initialize(startDate, allQuotes);
            var neuralNetworkItems = _repository.GetForSymbols<NeuralNetworkItem>(Symbols);
            var trainingData = neuralNetworkItems.Where(x => x.DateValue >= new DateTime(1996, 4, 1) && x.DateValue < startDate);

            //Parallel.ForEach(Symbols, symbol =>
            foreach (var symbol in Symbols)
            {
                var core = new Core();
                TrainNetwork(symbol, core, trainingData);
                _networkManager.LoadNetworkValues(symbol, core);

                lock (locker)
                {
                    _cores.Add(symbol, core);
                }
             };
            //});
        }

    private void TrainNetwork(string symbol, Core core, IEnumerable<NeuralNetworkItem> trainingData)
        {
            if (trainingData.Any())
            {
                core.InitializeArrays();
                core.InitializeRandomValues();
                _networkManager.TrainNetwork(symbol, core, trainingData);
                _networkManager.PersistCoreValues(symbol, core);
            }
        }

        protected override void ExecuteStrategyImplementation(DateTime date)
        {
            if ((date - lastDate).TotalDays >= 10)
            {
                var quotes = TodayQuotes.Values;
                PriceHistory selectedQuote = null;
                var highestPrediction = double.MinValue;
                var neuralNetworkItems = _repository.GetForSymbolsAndDate<NeuralNetworkItem>(quotes.Select(x => x.Symbol), date);

                if (neuralNetworkItems.Any())
                {
                    //Parallel.ForEach(quotes, quote =>
                    foreach(var quote in quotes)
                    {
                        var core = _cores[quote.Symbol];
                        var prediction = _networkManager.Predict(core, neuralNetworkItems);

                        lock (locker)
                        {
                            if (prediction >= highestPrediction)
                            {
                                highestPrediction = prediction;
                                selectedQuote = quote;
                            }
                        }
                    };

                    if (selectedQuote != null && !Account.Portfolio.PositionDictionary.ContainsKey(selectedQuote.Symbol))
                    {
                        var purchaseRequest = new PurchaseRequest { Quote = selectedQuote, Percent = 1 };
                        Account.Liquidate(date);
                        Account.Buy(new[] { purchaseRequest });
                    }
                }

                lastDate = date;
            }
        }
    }
}
