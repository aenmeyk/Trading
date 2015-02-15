using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using GenericParsing;

namespace HistoricalPriceExtractor.Persistance
{
    public class SqlPersister : IPersister, IDisposable
    {
        private bool disposed = false;
        private string _connectionString;
        private SqlConnection _sqlConnection;

        public SqlPersister(string connectionString)
        {
            _connectionString = connectionString;
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
            var commandText = string.Format("DELETE FROM [dbo].[PriceHistory] WHERE Symbol = '{0}'", symbol);

            using (var command = new SqlCommand(commandText, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        private void InsertHistoricalPrices(string symbol, string historicalPrices)
        {
            var reader = new StringReader(historicalPrices);

            using (var parser = new GenericParserAdapter(reader))
            {
                parser.SkipStartingDataRows = 1;
                var dataTable = parser.GetDataTable();
                var formattedDataTable = FormatDataTable(dataTable, symbol);

                using (var bulkCopy = new SqlBulkCopy(_connectionString, SqlBulkCopyOptions.Default))
                {
                    bulkCopy.DestinationTableName = "PriceHistory";
                    bulkCopy.WriteToServer(formattedDataTable);
                }
            }
        }

        private DataTable FormatDataTable(DataTable dataTable, string symbol)
        {
            var newDataTable = new DataTable();
            newDataTable.Columns.Add("DateValue", Type.GetType(typeName: "System.DateTime"));
            newDataTable.Columns.Add("OpenPrice", Type.GetType(typeName: "System.Decimal"));
            newDataTable.Columns.Add("HighPrice", Type.GetType(typeName: "System.Decimal"));
            newDataTable.Columns.Add("LowPrice", Type.GetType(typeName: "System.Decimal"));
            newDataTable.Columns.Add("ClosePrice", Type.GetType(typeName: "System.Decimal"));
            newDataTable.Columns.Add("Volume", Type.GetType(typeName: "System.Int32"));
            newDataTable.Columns.Add("AdjustedClosePrice", Type.GetType(typeName: "System.Decimal"));

            foreach (DataRow dataRow in dataTable.Rows)
            {
                newDataTable.Rows.Add(dataRow.ItemArray);
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
