using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using GenericParsing;

namespace HistoricalPriceExtractor.Persistance
{
    public class SqlPersister : IPersister, IDisposable
    {
        private bool disposed = false;
        private string _connectionString;
        private string _tableName;
        private SqlConnection _sqlConnection;

        public SqlPersister(string connectionString, string tableName)
        {
            _connectionString = connectionString;
            _tableName = tableName;
            _sqlConnection = new SqlConnection(connectionString);
            _sqlConnection.Open();
        }

        public void Persist(string symbol, string historicalPrices)
        {
            DeleteExistingHistoricalPrices(symbol);
            InsertHistoricalPrices(symbol, historicalPrices);
        }

        private void DeleteExistingHistoricalPrices(string symbol)
        {
            var commandText = string.Format("DELETE FROM {0} WHERE Symbol = '{1}'", _tableName, symbol);

            using (var command = new SqlCommand(commandText, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        private void InsertHistoricalPrices(string symbol, string historicalPrices)
        {
            historicalPrices = historicalPrices.Replace(",-", ",");
            var reader = new StringReader(historicalPrices);

            using (var parser = new GenericParserAdapter(reader))
            {
                parser.SkipStartingDataRows = 1;
                var dataTable = parser.GetDataTable();
                var formattedDataTable = FormatDataTable(dataTable, symbol);

                using (var bulkCopy = new SqlBulkCopy(_connectionString, SqlBulkCopyOptions.Default))
                {
                    bulkCopy.DestinationTableName = _tableName;
                    bulkCopy.WriteToServer(formattedDataTable);
                }
            }
        }

        private DataTable FormatDataTable(DataTable dataTable, string symbol)
        {
            var newDataTable = new DataTable
            {
                Columns =
                {
                    new DataColumn("DateValue", typeof(DateTime)),
                    new DataColumn("OpenPrice", typeof(decimal)) { AllowDBNull = true},
                    new DataColumn("HighPrice", typeof(decimal)) { AllowDBNull = true},
                    new DataColumn("LowPrice", typeof(decimal)) { AllowDBNull = true},
                    new DataColumn("ClosePrice", typeof(decimal)) { AllowDBNull = true},
                    new DataColumn("Volume", typeof(long)) { AllowDBNull = true},
                    new DataColumn("AdjustedClosePrice", typeof(decimal)) { AllowDBNull = true},
                }
            };

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var items = new Collection<object>();

                foreach(var item in dataRow.ItemArray)
                {
                    var itemToAdd = string.IsNullOrWhiteSpace(item.ToString())
                        ? DBNull.Value
                        : item;

                    items.Add(itemToAdd);
                }

                newDataTable.Rows.Add(items.ToArray());
            }

            var symbolColumn = newDataTable.Columns.Add("Symbol", Type.GetType(typeName: "System.String"));

            foreach (DataRow dataRow in newDataTable.Rows)
            {
                dataRow[symbolColumn] = symbol;
            }

            newDataTable.Columns["DateValue"].SetOrdinal(0);
            newDataTable.Columns["Symbol"].SetOrdinal(1);
            newDataTable.Columns["OpenPrice"].SetOrdinal(2);
            newDataTable.Columns["HighPrice"].SetOrdinal(3);
            newDataTable.Columns["LowPrice"].SetOrdinal(4);
            newDataTable.Columns["ClosePrice"].SetOrdinal(5);
            newDataTable.Columns["AdjustedClosePrice"].SetOrdinal(6);
            newDataTable.Columns["Volume"].SetOrdinal(7);

            return newDataTable;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _sqlConnection.Dispose();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
