using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HistoricalPriceExtractor
{
    public class SymbolProvider
    {
        public IEnumerable<string> GetSymbols()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "HistoricalPriceExtractor.Symbols.txt";
            string symbolString;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    symbolString = reader.ReadToEnd();
                }
            }

            var symbols = symbolString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return symbols;
        }
    }
}
