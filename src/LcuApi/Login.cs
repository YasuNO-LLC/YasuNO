using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace LcuApi
{
    public class Login
    {
        private readonly HttpClient _client;

        internal Login(HttpClient client)
        {
            this._client = client;
        }

        public async Task<SessionDto> GetSession()
        {
            var res = await this._client.GetAsync("lol-login/v1/session");

            return JsonConvert.DeserializeObject<SessionDto>(await res.Content.ReadAsStringAsync());
        }

        public class SessionDto
        {
            public int AccountId { get; set; }
            public bool Connected { get; set; }
            public string Puuid { get; set; }
            public int SummonerId { get; set; }
            public string Username { get; set; }
        }
    }
}
