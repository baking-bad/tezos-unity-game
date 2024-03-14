#nullable enable
using System.Text.Json.Serialization;
using UnityEngine;

namespace Api.Models
{
    [SerializeField]
    public class PlayerStats
    {
        [JsonPropertyName("games_played")]
        public int GamesPlayed { get; set; }
        [JsonPropertyName("bosses_killed")]
        public int BossesKilled { get; set; }
        [JsonPropertyName("best_score")]
        public int BestScore { get; set; }
        [JsonPropertyName("mobs_killed")]
        public int MobsKilled { get; set; }
        [JsonPropertyName("shots_fired")]
        public int ShotsFired { get; set; }
        [JsonPropertyName("favourite_weapon")]
        public string? FavouriteWeapon { get; set; }
    }
}
