using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace LcuApi
{
    public class ChampSelect
    {
        private readonly HttpClient _client;

        internal ChampSelect(HttpClient client)
        {
            this._client = client;
        }

        public async Task<bool> PatchAction(int actionId, PatchActionDto patchAction)
        {
            var serialized = JsonConvert.SerializeObject(new { championId = patchAction.ChampionId });
            var res = await this._client.PatchAsync(
                          $"lol-champ-select/v1/session/actions/{actionId}",
                          new StringContent(serialized, Encoding.UTF8, "application/json")
                      );

            return res.StatusCode == HttpStatusCode.NoContent;
        }

        public async Task<bool> CompleteAction(int actionId)
        {
            var res = await this._client.PostAsync(
                          $"/lol-champ-select/v1/session/actions/{actionId}/complete",
                          new StringContent("{}", Encoding.UTF8, "application/json")
                      );

            return res.IsSuccessStatusCode;
        }

        public async Task<List<int>> PickableChampions()
        {
            var res = await this._client.GetAsync(
                          "/lol-champ-select/v1/pickable-champions",
                          CancellationToken.None
                      );

            return JsonConvert.DeserializeObject<PickableChampionsDto>(await res.Content.ReadAsStringAsync())
                              ?.ChampionIds;
        }

        public class PatchActionDto
        {
            [JsonProperty("actorCellId")] public int ActorCellId { get; set; }

            [JsonProperty("championId")] public int ChampionId { get; set; }

            [JsonProperty("completed")] public bool Completed { get; set; }

            [JsonProperty("id")] public int Id { get; set; }

            [JsonProperty("pickTurn")] public int PickTurn { get; set; }

            [JsonProperty("type")] public string ActionType { get; set; }
        }

        private class PickableChampionsDto
        {
            [JsonProperty("championIds")] public List<int> ChampionIds { get; set; }
        }
    }
}
