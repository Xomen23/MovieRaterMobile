using System.Text.Json.Serialization;

namespace MovieRaterApp.Models;

// Polja mapirana prema backend ImdbTitleResponse.java (imdbapi.dev title detalji).
// Ako API promeni format, proveriti uzivo: GET /api/movies/imdb/{imdbId}
public class ImdbTitleResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("primaryTitle")]
    public string? PrimaryTitle { get; set; }

    [JsonPropertyName("startYear")]
    public int? StartYear { get; set; }

    [JsonPropertyName("plot")]
    public string? Plot { get; set; }

    [JsonPropertyName("primaryImage")]
    public ImdbPrimaryImage? PrimaryImage { get; set; }
}
