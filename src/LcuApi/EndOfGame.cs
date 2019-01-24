using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace LcuApi
{
    public class EndOfGame
    {
        private readonly HttpClient _client;

        internal EndOfGame(HttpClient client)
        {
            this._client = client;
        }

        public async Task<StatsBlock> GetEndOfGameStats()
        {
            var res = await this._client.GetAsync("lol-end-of-game/v1/eog-stats-block");

            return JsonConvert.DeserializeObject<StatsBlock>(await res.Content.ReadAsStringAsync());
        }
        /*
         * {
              "accountId": 0,
              "basePoints": 0,
              "battleBoostIpEarned": 0,
              "boostIpEarned": 0,
              "boostXpEarned": 0,
              "causedEarlySurrender": true,
              "championId": 0,
              "coOpVsAiMinutesLeftToday": 0,
              "coOpVsAiMsecsUntilReset": 0,
              "completionBonusPoints": 0,
              "currentLevel": 0,
              "customMinutesLeftToday": 0,
              "customMsecsUntilReset": 0,
              "difficulty": "string",
              "earlySurrenderAccomplice": true,
              "elo": 0,
              "eloChange": 0,
              "experienceEarned": 0,
              "experienceTotal": 0,
              "firstWinBonus": 0,
              "gameEndedInEarlySurrender": true,
              "gameId": 0,
              "gameLength": 0,
              "gameMode": "string",
              "gameMutators": [
                "string"
              ],
              "gameType": "string",
              "globalBoostXpEarned": 0,
              "imbalancedTeamsNoPoints": true,
              "invalid": true,
              "ipEarned": 0,
              "ipTotal": 0,
              "leveledUp": true,
              "loyaltyBoostIpEarned": 0,
              "loyaltyBoostXpEarned": 0,
              "missionsXpEarned": 0,
              "myTeamStatus": "string",
              "newSpells": [
                0
              ],
              "nextLevelXp": 0,
              "odinBonusIp": 0,
              "partyRewardsBonusIpEarned": 0,
              "pointsPenalties": {},
              "preLevelUpExperienceTotal": 0,
              "preLevelUpNextLevelXp": 0,
              "previousLevel": 0,
              "previousXpTotal": 0,
              "queueBonusEarned": 0,
              "queueType": "string",
              "ranked": true,
              "reportGameId": 0,
              "rerollData": {
                "pointChangeFromChampionsOwned": 0,
                "pointChangeFromGameplay": 0,
                "pointsUntilNextReroll": 0,
                "pointsUsed": 0,
                "previousPoints": 0,
                "rerollCount": 0,
                "totalPoints": 0
              },
              "roomName": "string",
              "roomPassword": "string",
              "rpEarned": 0,
              "sendStatsToTournamentProvider": true,
              "skinId": 0,
              "skinIndex": 0,
              "summonerId": 0,
              "summonerName": "string",
              "talentPointsGained": 0,
              "teamBoost": {
                "availableSkins": [
                  0
                ],
                "ipReward": 0,
                "ipRewardForPurchaser": 0,
                "price": 0,
                "skinUnlockMode": "string",
                "summonerName": "string",
                "unlocked": true
              },
              "teamEarlySurrendered": true,
              "teams": [
                {
                  "championBans": [
                    0
                  ],
                  "fullId": "string",
                  "isBottomTeam": true,
                  "isPlayerTeam": true,
                  "isWinningTeam": true,
                  "memberStatusString": "string",
                  "name": "string",
                  "players": [
                    {
                      "botPlayer": true,
                      "championId": 0,
                      "detectedTeamPosition": "string",
                      "elo": 0,
                      "eloChange": 0,
                      "gameId": 0,
                      "isReportingDisabled": true,
                      "items": [
                        0
                      ],
                      "leaver": true,
                      "leaves": 0,
                      "level": 0,
                      "losses": 0,
                      "profileIconId": 0,
                      "selectedPosition": "string",
                      "skinIndex": 0,
                      "skinName": "string",
                      "spell1Id": 0,
                      "spell2Id": 0,
                      "stats": {},
                      "summonerId": 0,
                      "summonerName": "string",
                      "teamId": 0,
                      "userId": 0,
                      "wins": 0
                    }
                  ],
                  "stats": {},
                  "tag": "string",
                  "teamId": 0
                }
              ],
              "timeUntilNextFirstWinBonus": 0,
              "userId": 0
            }
         */

        public class StatsBlock
        {
            public long AccountId { get; set; }
            public long GameId { get; set; }
            public long SummonerId { get; set; }
        }
    }
}
