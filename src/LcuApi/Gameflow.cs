using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace LcuApi
{
    public enum GameflowState
    {
        None,
        Lobby,
        ChampSelect,
        InProgress,
        WaitingForStates,
        EndOfGame
    }

    public class Gameflow
    {
        private readonly HttpClient _client;

        internal Gameflow(HttpClient client)
        {
            this._client = client;
        }

        public async Task<GameflowState> GetCurrentState()
        {
            var res = await this._client.GetAsync("lol-gameflow/v1/gameflow-phase");

            var gameflowStr = await res.Content.ReadAsStringAsync();

            if (!Enum.TryParse(JsonConvert.DeserializeObject<string>(gameflowStr), out GameflowState state))
            {
                Debugger.Log(0, string.Empty, $"invalid valid for enum: {gameflowStr}");
                return GameflowState.None;
            }

            return state;
        }
    }
}
