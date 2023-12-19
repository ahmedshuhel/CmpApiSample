using System.Text.Json.Serialization;

public record Content
{
   [JsonPropertyName("type")]
   public string Type { get; init; }

   [JsonPropertyName("value")]
   public string Value { get; init; }
}
