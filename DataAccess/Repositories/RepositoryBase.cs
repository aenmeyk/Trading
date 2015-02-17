using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

namespace DataAccess.Repositories
{
    public abstract class RepositoryBase
    {
        protected string ConnectionString { get { return Constants.CONNECTION_STRING; } }
        protected abstract string TableName { get; }

        public void DeleteForSymbol(string symbol)
        {
            var commandText = string.Format("DELETE FROM {0} WHERE Symbol = '{1}'", TableName, symbol);

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = new SqlCommand(commandText, sqlConnection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<T> GetForSymbol<T>(string symbol)
        {
            var queryText = string.Format("SELECT * FROM {0} WHERE Symbol = @Symbol", TableName);

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                return sqlConnection.Query<T>(queryText, new { Symbol = symbol });
            }
        }

        public IEnumerable<T> Get<T>()
        {
            var queryText = string.Format("SELECT * FROM {0}", TableName);

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                return sqlConnection.Query<T>(queryText);
            }
        }
    }
}
