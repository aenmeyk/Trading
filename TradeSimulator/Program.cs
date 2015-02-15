using System;
using Common;
using TradeSimulator.Strategies;

namespace TradeSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.BufferHeight = 2000;
            var strategy = new MovementFromYesterday(CommonConstants.CONNECTION_STRING);
            strategy.Run();

            Console.ReadLine();
        }
    }
}
