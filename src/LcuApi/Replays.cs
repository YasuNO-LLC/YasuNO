using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace LcuApi
{
    public class Replays
    {
        private readonly HttpClient _client;

        internal Replays(HttpClient client)
        {
            this._client = client;
        }

        public async Task<string> GetRoflsPath()
        {
            var res = await this._client.GetAsync("lol-replays/v1/rofls/path");

            return JsonConvert.DeserializeObject<string>(await res.Content.ReadAsStringAsync());
        }

        public async Task<bool> DownloadMatchReplay(long matchId)
        {
            var res = await this._client.PostAsync(
                          $"lol-replays/v1/rofls/{matchId}/download",
                          new StringContent(
                              "{\"componentType\":\"replay\"}",
                              Encoding.UTF8,
                              "application/json"
                          )
                      );

            return res.StatusCode == HttpStatusCode.NoContent;
        }

        public async Task<bool> WatchMatchReplay(long matchId)
        {
            var res = await this._client.PostAsync(
                          $"lol-replays/v1/rofls/{matchId}/watch",
                          new StringContent(
                              "{\"componentType\":\"replay\"}",
                              Encoding.UTF8,
                              "application/json"
                          )
                      );

            return res.StatusCode == HttpStatusCode.NoContent;
        }
    }
}
