using System.Text.Json.Serialization;

public record Group
{
   [JsonPropertyName("id")]
   public string Id { get; init; }

   [JsonPropertyName("name")]
   public string Name { get; init; }
}
