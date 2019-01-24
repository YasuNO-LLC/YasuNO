using System.Net.Http;
using System.Threading.Tasks;

namespace LcuApi
{
    public class Builtin
    {
        private readonly HttpClient _client;

        internal Builtin(HttpClient client)
        {
            this._client = client;
        }

        public async Task<bool> Subscribe(string eventName)
        {
            var res = await this._client.PostAsync("/Subscribe", new StringContent(""));

            return res.IsSuccessStatusCode;
        }
    }
}
