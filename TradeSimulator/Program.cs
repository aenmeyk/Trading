using System;

namespace TradeSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            var strategyRunner = new StrategyRunner();
            strategyRunner.Run();

            Console.WriteLine();
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
