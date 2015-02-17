using System.Data.SqlClient;

namespace DataAccess.Repositories
{
    public abstract class RepositoryBase
    {
        protected string ConnectionString { get { return Constants.CONNECTION_STRING; } }
        protected abstract string TableName { get; }

        public void DeleteForSymbol(string symbol)
        {
            var commandText = string.Format("DELETE FROM dbo.{0} WHERE Symbol = '{1}'", TableName, symbol);

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = new SqlCommand(commandText, sqlConnection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
