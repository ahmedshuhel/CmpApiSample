using System.Text.Json.Serialization;

public record Label
{
   [JsonPropertyName("group")]
   public Group Group { get; init; }

   [JsonPropertyName("values")]
   public List<Value> Values { get; init; }
}
