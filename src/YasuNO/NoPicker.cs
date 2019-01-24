using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;

using LcuApi;

using Newtonsoft.Json;

namespace Yasuno
{
    internal class NoPicker : IDisposable
    {
        private const string HoverEvent = "OnJsonApiEvent_lol-champ-select_v1_session";
        private const int Cancer = 157;

        private static readonly Random Random = new Random();

        private readonly Client _client;
        private readonly IDisposable _subscription;

        public NoPicker(Client client)
        {
            this._client = client ?? throw new ArgumentNullException(nameof(client));

            this._subscription =
                this._client
                    .GetEventObservable(NoPicker.HoverEvent)
                    .Where(obj => obj != null)
                    .Select(JsonConvert.SerializeObject)
                    .Select(JsonConvert.DeserializeObject<LobbyStateUpdate>)
                    .Where(state => state?.Data?.Actions != null)
                    .SelectMany(state => state.Data.Actions)
                    .SelectMany(actions => actions)
                    .Where(action => action.ChampionId == NoPicker.Cancer && action.ActionType.Equals("pick", StringComparison.InvariantCultureIgnoreCase))
                    .Subscribe(this.OnCancerHovered);
        }

        public void Dispose()
        {
            this._subscription.Dispose();
        }

        private async void OnCancerHovered(ChampSelect.PatchActionDto action)
        {
            Debugger.Log(0, "", $"Cancer found!! {JsonConvert.SerializeObject(action)}\n");
            if (action.Completed)
            {
                // dodge
            }
            else
            {
                var pickable = await this._client.ChampSelect.PickableChampions();
                if (pickable == null)
                {
                    return;
                }

                pickable.Remove(NoPicker.Cancer);

                var ind = NoPicker.Random.Next(pickable.Count);

                var champId = pickable[ind];

                action.ChampionId = champId;

                Debugger.Log(0, "", $"trying to pick champion {champId}\n");
                await Task.Delay(100);
                await this._client.ChampSelect.PatchAction(action.Id, action);
                await this._client.ChampSelect.CompleteAction(action.Id);
            }
        }

        private class LobbyState
        {
            [JsonProperty("actions")] public List<List<ChampSelect.PatchActionDto>> Actions { get; set; }
        }

        private class LobbyStateUpdate
        {
            [JsonProperty("data")] public LobbyState Data { get; set; }
        }
    }
}
