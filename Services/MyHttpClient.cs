using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace BlazorApp.Services
{
    // Clases de modelo
    public class MyHttpClient
    {
        private readonly HttpClient _httpClient;
        private const string apiUrl = "https://g.tenor.com/v1/search?q=excited&key=LIVDSRZULELA&limit=8";

        public MyHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<MediaItem>> GetMediaItemsAsync()
        {
            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonConvert.DeserializeObject<RootObject>(responseData);
            return jsonObject.Results
                .SelectMany(r => r.Media)
                .SelectMany(m => m.Values) // Obtener todas las propiedades bajo 'media'
                .Select(v => new MediaItem
                {
                    Preview = v["preview"]?.ToString(), // Obtener el valor de 'preview'
                    Url = v["url"]?.ToString() // Obtener el valor de 'url'
                })
                .ToList();
        }
    }

    public class MediaItem
    {
        public string Preview { get; set; }
        public string Url { get; set; }
    }

    public class RootObject
    {
        public List<ResultItem> Results { get; set; }
    }

    public class ResultItem
    {
        public List<Dictionary<string, JToken>> Media { get; set; } // Cambiar el tipo a Dictionary<string, JToken>
    }

}
