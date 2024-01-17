using System.Text.Json.Serialization;
using UnityEngine;

namespace Api.Models
{
    [SerializeField]
    public class SignPayload
    {
        [JsonPropertyName("payload")] 
        public string Payload { get; set; }
    }
}
