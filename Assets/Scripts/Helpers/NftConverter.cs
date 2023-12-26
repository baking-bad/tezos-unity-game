using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Helpers
{
    public class NftConverter : JsonConverter<Nft>
    {
        public override Nft Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var nft = new Nft();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return nft;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propName = (reader.GetString() ?? "").ToLower();
                    reader.Read();

                    switch (propName)
                    {
                        case var _ when propName.Equals(nameof(Nft.Name).ToLower()):
                            nft.Name = reader.GetString()?.Trim();
                            break;
                        case var _ when propName.Equals(nameof(Nft.Description).ToLower()):
                            nft.Description = reader.GetString()?.Trim();
                            break;
                        case var _ when propName.Equals(nameof(Nft.ThumbnailUri).ToLower()):
                            nft.ThumbnailUri = reader.GetString()?.Trim();
                            break;
                        case var _ when propName.Equals(nameof(Nft.Type).ToLower()):
                            nft.Type = (Nft.NftType) Enum.Parse(typeof(Nft.NftType), reader.GetString() ?? string.Empty, true);
                            break;
                        case var _ when propName.Equals(nameof(Nft.GameParameters).ToLower()):
                            if (JsonDocument.TryParseValue(ref reader, out var jsonDoc))
                            {
                                var parameters = JsonNode.Parse(jsonDoc.RootElement.ToString())?.AsArray();

                                if (parameters == null) break;

                                nft.GameParameters = parameters.Deserialize<List<GameParameters>>(options);
                            }

                            break;
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Nft value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            // writer.WriteString(nameof(Nft.Name), value.Name);
            // writer.WriteString(nameof(Nft.Description), value.Description);
            // writer.WriteString(nameof(Nft.ThumbnailUri), value.ThumbnailUri);
            // writer.WriteString(nameof(Nft.Type), value.Type);
            // writer.WriteString(nameof(Nft.GameParameters), value.GameParameters);

            writer.WriteEndObject();
        }
    }
}
