using System.Text.Json.Serialization;

public record AssetsResponse
{
   [JsonPropertyName("data")]
   public required List<Data> Data { get; init; }

   [JsonPropertyName("pagination")]
   public required Pagination Pagination { get; init; }

   [JsonPropertyName("total_count")]
   public int TotalCount { get; init; }
}