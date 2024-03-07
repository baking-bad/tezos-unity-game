using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

namespace Api.Models
{
    [SerializeField]
    public class GameSession
    {
        [JsonPropertyName("game_id")]
        public string GameId { get; set; }
        [JsonPropertyName("game_drop")]
        public IEnumerable<GameDrop> GameDrop { get; set; }
    }

    [SerializeField]
    public class GameDrop
    {
        [JsonPropertyName("boss")]
        public int Boss { get; set; }
        [JsonPropertyName("token")]
        public int Token { get; set; }
    }

    [SerializeField]
    public class Reward
    {
        [JsonPropertyName("token_id")]
        public int TokenId { get; set; }
        [JsonPropertyName("amount")]
        public int Amount { get; set; }
    }
    
    [SerializeField]
    public class ClaimRewardResponse
    {
        [JsonPropertyName("tokens_transfered")]
        public int TokensTransfered { get; set; }
        [JsonPropertyName("operation_hash")]
        public string OperationHash { get; set; }
    }
}
