using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace HistoricalPriceExtractor
{
    public class Extractor
    {
        private string _quoteUrl;
        private HttpClient _httpClient;

        public Extractor(string quoteUrl)
        {
            _quoteUrl = quoteUrl;
            _httpClient = new HttpClient();
        }

        public async Task<string> GetHistoricalPrices(string symbol)
        {
            var url = string.Format(Constants.QUOTE_URL, symbol);

            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
            var historicalPrices = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return historicalPrices;
        }
    }
}
