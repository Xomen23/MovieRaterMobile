using System.Text.Json.Serialization;

namespace MovieRaterApp.Models;

public class UpdatePasswordRequest
{
    [JsonPropertyName("currentPassword")]
    public string? CurrentPassword { get; set; }

    [JsonPropertyName("newPassword")]
    public string? NewPassword { get; set; }
}
