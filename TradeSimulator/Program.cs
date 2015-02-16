using System;
using Common;
using TradeSimulator.Strategies;

namespace TradeSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            //var strategy = new MovementFromYesterday(CommonConstants.CONNECTION_STRING);
            var strategy = new NeuralNetwork();
            strategy.Run();

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
