namespace DataAccess.Repositories
{
    public class NeuralNetworkRepository : SymbolRepositoryBase
    {
        protected override string TableName
        {
            get { return "nn.NeuralNetwork"; }
        }
    }
}
