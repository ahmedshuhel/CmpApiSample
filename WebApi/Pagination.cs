using System.Text.Json.Serialization;

public record Pagination
{
   [JsonPropertyName("next")]
   public string Next { get; init; }

   [JsonPropertyName("previous")]
   public string Previous { get; init; }
}
