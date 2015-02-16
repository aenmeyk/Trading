using NeuralNet;

namespace TradeSimulator.Strategies
{
    public class NeuralNetwork
    {
        public void Run()
        {
            NetworkManager.InitializeNetwork();
            NetworkManager.TrainNetwork();
            NetworkManager.TestNetwork();
        }
    }
}
