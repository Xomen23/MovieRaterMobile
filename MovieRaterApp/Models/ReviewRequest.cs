using System.Text.Json.Serialization;

namespace MovieRaterApp.Models;

public class ReviewRequest
{
    [JsonPropertyName("review")]
    public Review? Review { get; set; }

    [JsonPropertyName("movie")]
    public Movie? Movie { get; set; }
}
