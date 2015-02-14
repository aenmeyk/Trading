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

        public async Task<Stream> GetHistoricalPrices(string ticker)
        {
            var url = string.Format(Constants.QUOTE_URL, ticker);

            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
            var priceStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            return priceStream;
        }
    }
}
