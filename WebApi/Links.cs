using System.Text.Json.Serialization;

public record Links
{
   [JsonPropertyName("self")]
   public string Self { get; init; }
}
