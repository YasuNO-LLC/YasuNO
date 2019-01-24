using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace LcuApi
{
    public class Summoner
    {
        private readonly HttpClient _client;

        internal Summoner(HttpClient client)
        {
            this._client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<SummonerDto> CurrentSummoner()
        {
            var res = await this._client.GetAsync("/lol-summoner/v1/current-summoner");

            if (res.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            var serialized = await res.Content.ReadAsStringAsync();
            var summoner = JsonConvert.DeserializeObject<SummonerDto>(serialized);

            return summoner;
        }

        public class SummonerDto { }
    }
}
