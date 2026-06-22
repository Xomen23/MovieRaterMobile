using System.Text.Json.Serialization;

namespace MovieRaterApp.Models;

public class UpdateUsernameRequest
{
    [JsonPropertyName("newUsername")]
    public string? NewUsername { get; set; }
}
