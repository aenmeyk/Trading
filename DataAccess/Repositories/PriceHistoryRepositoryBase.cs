using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Common.ExtensionMethods;
using Dapper;
using GenericParsing;

namespace DataAccess.Repositories
{
    public abstract class PriceHistoryRepositoryBase : SymbolRepositoryBase
    {
        public void PersistHistoricalPrices(string symbol, string historicalPrices)
        {
            DeleteForSymbol(symbol);
            InsertHistoricalPrices(symbol, historicalPrices);
        }

        public IEnumerable<T> GetForSymbolsAndDateRange<T>(IEnumerable<string> symbols, DateTime startDate, DateTime endDate)
        {
            const string sqlStructure = @"
                SELECT Symbol, DateValue, AdjustedClosePrice, Volume 
                FROM {0}
                WHERE Symbol IN({1}) 
                    AND DateValue BETWEEN '{2}' AND '{3}' 
                ORDER BY DateValue";

            var symbolText = string.Join(",", symbols.Select(x => string.Format("'{0}'", x)).ToArray());
            var queryText = string.Format(sqlStructure
                , TableName
                , symbolText
                , startDate.ToSqlString()
                , endDate.ToSqlString());

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                return sqlConnection.Query<T>(queryText);
            }
        }

        public void InsertHistoricalPrices(string symbol, string historicalPrices)
        {
            historicalPrices = historicalPrices.Replace(",-", ",");
            var reader = new StringReader(historicalPrices);

            using (var parser = new GenericParserAdapter(reader))
            {
                parser.SkipStartingDataRows = 1;
                var dataTable = parser.GetDataTable();
                var formattedDataTable = FormatDataTable(dataTable, symbol);

                BulkInsert(formattedDataTable);
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

                foreach (var item in dataRow.ItemArray)
                {
                    var itemToAdd = string.IsNullOrWhiteSpace(item.ToString())
                        ? DBNull.Value
                        : item;

                    items.Add(itemToAdd);
                }

                newDataTable.Rows.Add(items.ToArray());
            }

            var symbolColumn = newDataTable.Columns.Add("Symbol", typeof(string));

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
    }
}
