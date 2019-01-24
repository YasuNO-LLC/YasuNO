using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace LcuApi
{
    public enum Position
    {
        TOP,
        JUNGLE,
        MID,
        BOTTOM,
        SUPPORT
    }

    public enum Tier
    {
        ALL,
        UNRANKED,
        IRON,
        BRONZE,
        SILVER,
        GOLD,
        PLATINUM,
        DIAMOND,
        MASTER,
        GRANDMASTER,
        CHALLENGER
    }

    public enum Queue
    {
        draft5,
        rank5flex,
        rank5solo,
        blind5,
        aram,
        blind3,
        rank3flex
    }

    public class CareerStats
    {
        private readonly HttpClient _client;

        internal CareerStats(HttpClient client)
        {
            this._client = client;
        }

        public async Task<ChampionQueueStatsDto> GetChampionAverage(
            int championId,
            Position position,
            Tier tier,
            Queue queue
        )
        {
            var res = await this._client.GetAsync(
                          $"lol-career-stats/v1/champion-averages/{championId}/{position}/{tier}/{queue}"
                      );

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<ChampionQueueStatsDto>(await res.Content.ReadAsStringAsync());
        }

        public async Task<SummonerGameStatsDto[]> GetSummonerGames(string puuid)
        {
            var res = await this._client.GetAsync($"lol-career-stats/v1/summoner-games/{puuid}");

            var ret = JsonConvert.DeserializeObject<SummonerGameStatsDto[]>(await res.Content.ReadAsStringAsync());

            foreach (var statsDto in ret)
            {
                statsDto.Stats = new CareerStatsSaneDto { CareerStats = statsDto.Stats.CareerStats };
            }

            return ret;
        }

        public class ChampionStatsDto
        {
            public double CsDiffAtLaningEnd { get; set; }
            public double CsPerMinute { get; set; }
            public double DamagePerDeath { get; set; }
            public double DamagePerGold { get; set; }
            public double DamageShare { get; set; }
            public double GoldDiffAtLaningEnd { get; set; }
            public double Kda { get; set; }
            public double KillConversionRatio { get; set; }
            public double KillParticipation { get; set; }
            public double ObjectiveControlRatio { get; set; }
            public double RoamDominanceScore { get; set; }
            public double UtilityScore { get; set; }
            public double VisionScorePerHour { get; set; }
        }

        public class ChampionQueueStatsDto
        {
            public int ChampionId { get; set; }
            public string Position { get; set; }
            public string QueueType { get; set; }
            public string RankTier { get; set; }
            public ChampionStatsDto Stats { get; set; }
        }

        public class CareerStatsJs
        {
            public int Assists { get; set; }
            public int CcScore { get; set; }
            public int ConvertedKillAndAssists { get; set; }
            public int CsAtLaningEnd { get; set; }
            public int CsDiffAtLaningEnd { get; set; }
            public int ScScore { get; set; }
            public int Damage { get; set; }
            public int DamageShieldedOnTeammates { get; set; }
            public int Deaths { get; set; }
            public int Defeat { get; set; }
            public int DoubleKills { get; set; }
            public int GamePlayed { get; set; }
            public int GoldAtLaningEnd { get; set; }
            public int GoldDiffAtLaningEnd { get; set; }
            public int GoldEarned { get; set; }
            public int HealsOnTeammates { get; set; }
            public int Kills { get; set; }
            public int ObjectiveTakenInvolved { get; set; }
            public int ParticipandId { get; set; }
            public int PentaKills { get; set; }
            public int Position { get; set; }
            public int QuadraKills { get; set; }
            public int RoamDominanceScore { get; set; }
            public int TeamDatamge { get; set; }
            public int TeamKills { get; set; }
            public int TeamObjectivesTaken { get; set; }
            public int TimePlayed { get; set; }
            public int TrippleKills { get; set; }
            public int Victory { get; set; }
            public int VisionScore { get; set; }
        }

        public interface ICareerStatsDto
        {
            CareerStatsJs CareerStats { get; set; }
        }

        public class CareerStatsDto : ICareerStatsDto
        {
            [JsonProperty("CareerStats.js")] public CareerStatsJs CareerStats { get; set; }
        }

        public class CareerStatsSaneDto : ICareerStatsDto
        {
            public CareerStatsJs CareerStats { get; set; }
        }

        public class SummonerGameStatsDto
        {
            public SummonerGameStatsDto(CareerStatsDto stats)
            {
                this.Stats = stats;
            }

            public int ChampionId { get; set; }
            public long GameId { get; set; }
            public int GamesCalculated { get; set; }
            public string Lane { get; set; }
            public string PlatformId { get; set; }
            public string QueueType { get; set; }
            public string Role { get; set; }
            public int Season { get; set; }
            public int TeamId { get; set; }
            public long Timestamp { get; set; }
            public ICareerStatsDto Stats { get; set; }
        }
    }
}
