﻿using System;
using System.Diagnostics;
using TradeSimulator.Strategies;

namespace TradeSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            //var strategyRunner = new StrategyRunner();
            //strategyRunner.Run();

            var sw = new Stopwatch();
            sw.Start();
            var neuralNetwork = new NeuralNetwork();
            neuralNetwork.Run();
            Console.WriteLine("Elapsed Seconds: {0}", sw.Elapsed.TotalSeconds);

            Console.WriteLine();
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
