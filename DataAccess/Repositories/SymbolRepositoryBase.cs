using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace DataAccess.Repositories
{
    public abstract class SymbolRepositoryBase : RepositoryBase
    {
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

        public IEnumerable<T> GetForSymbols<T>(IEnumerable<string> symbols)
        {
            var symbolText = string.Join(",", symbols.Select(x => string.Format("'{0}'", x)).ToArray());
            var queryText = string.Format("SELECT * FROM {0} WHERE Symbol IN ({1})", TableName, symbolText);

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                return sqlConnection.Query<T>(queryText);
            }
        }
    }
}
