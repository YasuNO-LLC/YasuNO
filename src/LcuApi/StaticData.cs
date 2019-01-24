using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace LcuApi
{
    public class StaticData
    {
        public static async Task<IEnumerable<int>> GetChampionIds()
        {
            string responseText;
            using (var client = new HttpClient())
            {
                var result =
                    await client.GetAsync("http://ddragon.leagueoflegends.com/cdn/8.22.1/data/en_US/champion.json");

                responseText = await result.Content.ReadAsStringAsync();
            }

            return StaticData.ParseOutChampionIds(responseText);
        }

        private static IEnumerable<int> ParseOutChampionIds(string jsonResponse)
        {
            dynamic obj = JObject.Parse(jsonResponse);

            foreach (var champ in obj.data)
            {
                var champId = champ.Value.key.Value;
                yield return int.Parse(champId);
            }
        }
    }
}
