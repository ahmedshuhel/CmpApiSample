using System.Text.Json.Serialization;

public record Data
{
   [JsonPropertyName("id")]
   public string Id { get; init; }

   [JsonPropertyName("title")]
   public string Title { get; init; }

   [JsonPropertyName("type")]
   public string Type { get; init; }

   [JsonPropertyName("mime_type")]
   public string MimeType { get; init; }

   [JsonPropertyName("file_extension")]
   public string FileExtension { get; init; }

   [JsonPropertyName("created_at")]
   public string CreatedAt { get; init; }

   [JsonPropertyName("modified_at")]
   public string ModifiedAt { get; init; }

   [JsonPropertyName("folder_id")]
   public string FolderId { get; init; }

   [JsonPropertyName("file_location")]
   public string FileLocation { get; init; }

   [JsonPropertyName("content")]
   public Content Content { get; init; }

   [JsonPropertyName("labels")]
   public List<Label> Labels { get; init; }

   [JsonPropertyName("links")]
   public Links Links { get; init; }

   [JsonPropertyName("owner_organization_id")]
   public string OwnerOrganizationId { get; init; }

   [JsonPropertyName("thumbnail_url")]
   public string ThumbnailUrl { get; init; }
}
