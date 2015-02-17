using System;
using Common;
using TradeSimulator.Strategies;

namespace TradeSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            var strategy = new MovementFromYesterday();
            // var strategy = new NeuralNetwork();
            strategy.Run();

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
