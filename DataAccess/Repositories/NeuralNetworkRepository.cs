namespace DataAccess.Repositories
{
    public class NeuralNetworkRepository : RepositoryBase
    {
        protected override string TableName
        {
            get { return "nn.NeuralNetwork"; }
        }
    }
}
