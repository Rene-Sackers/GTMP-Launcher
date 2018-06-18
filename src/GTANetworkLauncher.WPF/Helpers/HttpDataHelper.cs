using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GrandTheftMultiplayer.Launcher.Helpers
{
    public class HttpDataHelper
    {
        public static async Task<T> GetDataFromUrl<T>(string url) where T : class
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url);

                    return response.IsSuccessStatusCode
                        ? JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync())
                        : null;
                }
            }
            catch
            {
                // ignore
            }

            return default(T);
        }
    }
}
