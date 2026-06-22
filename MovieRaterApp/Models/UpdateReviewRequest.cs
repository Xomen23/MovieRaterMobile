using System.Text.Json.Serialization;

namespace MovieRaterApp.Models;

public class UpdateReviewRequest
{
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    [JsonPropertyName("rating")]
    public int? Rating { get; set; }

    [JsonPropertyName("reviewText")]
    public string? ReviewText { get; set; }
}
