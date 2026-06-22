using System.Text.Json.Serialization;

namespace MovieRaterApp.Models;

public class ImdbPrimaryImage
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
