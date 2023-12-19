using System.Text.Json.Serialization;

public record Value
{
   [JsonPropertyName("id")]
   public string Id { get; init; }

   [JsonPropertyName("name")]
   public string Name { get; init; }
}
