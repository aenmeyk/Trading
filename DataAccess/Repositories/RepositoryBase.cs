using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace DataAccess.Repositories
{
    public abstract class RepositoryBase
    {
        protected string ConnectionString { get { return Constants.CONNECTION_STRING; } }
        protected abstract string TableName { get; }

        public IEnumerable<T> Get<T>()
        {
            var queryText = string.Format("SELECT * FROM {0}", TableName);

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                return sqlConnection.Query<T>(queryText);
            }
        }

        public void Truncate()
        {
            var queryText = string.Format("TRUNCATE TABLE {0}", TableName);

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                
                using(var sqlCommand = new SqlCommand(queryText, sqlConnection))
                {
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        protected void BulkInsert(DataTable dataTable)
        {
            using (var bulkCopy = new SqlBulkCopy(ConnectionString, SqlBulkCopyOptions.Default))
            {
                bulkCopy.DestinationTableName = TableName;
                bulkCopy.WriteToServer(dataTable);
            }
        }
    }
}
