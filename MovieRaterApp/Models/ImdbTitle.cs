using System.Text.Json.Serialization;

namespace MovieRaterApp.Models;

// Polja mapirana prema backend ImdbTitle.java (imdbapi.dev search odgovor).
// Ako API promeni format, proveriti uzivo: GET /api/movies/search?query=...
public class ImdbTitle
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("originalTitle")]
    public string? OriginalTitle { get; set; }

    [JsonPropertyName("startYear")]
    public int? StartYear { get; set; }

    [JsonPropertyName("primaryImage")]
    public ImdbPrimaryImage? PrimaryImage { get; set; }
}
