using System.Text.Json.Serialization;

public record TokenResponse
{
   [JsonPropertyName("access_token")]
   public required string AccessToken { get; init; }

   [JsonPropertyName("refresh_token")]
   public required string RefreshToken { get; init; }

   [JsonPropertyName("id_token")]
   public required string IdToken { get; init; }

   [JsonPropertyName("expires_in")]
   public int ExpiresIn { get; init; }

   [JsonPropertyName("token_type")]
   public required string TokenType { get; init; }
}