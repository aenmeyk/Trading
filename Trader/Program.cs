using System;
using System.Diagnostics;
using Trader.Strategies;

namespace Trader
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var strategyRunner = new StrategyRunner();
            strategyRunner.Run();

            Console.WriteLine();
            Console.WriteLine("Elapsed Seconds: {0}", stopwatch.Elapsed.TotalSeconds);
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
