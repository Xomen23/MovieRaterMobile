using System.Text.Json.Serialization;

namespace MovieRaterApp.Models;

public class ImdbSearchResponse
{
    [JsonPropertyName("titles")]
    public List<ImdbTitle>? Titles { get; set; }
}
