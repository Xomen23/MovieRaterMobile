using System.Text.Json.Serialization;

namespace MovieRaterApp.Models;

public class AverageRatingResponse
{
    [JsonPropertyName("imdbId")]
    public string? ImdbId { get; set; }

    [JsonPropertyName("averageRating")]
    public double AverageRating { get; set; }

    [JsonPropertyName("reviewCount")]
    public int ReviewCount { get; set; }
}
