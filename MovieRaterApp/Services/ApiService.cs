using System.Net.Http.Json;
using System.Text.Json;
using MovieRaterApp.Models;

namespace MovieRaterApp.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ApiConnectionResolver _resolver;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService(HttpClient httpClient, ApiConnectionResolver resolver)
    {
        _httpClient = httpClient;
        _resolver = resolver;
    }

    public string BaseUrl =>
        _resolver.ActiveBaseUrl
        ?? _httpClient.BaseAddress?.ToString()
        ?? string.Join(" → ", ApiConstants.GetCandidateBaseUrls());

    public async Task EnsureConnectionAsync() => await EnsureBaseUrlAsync();

    public async Task<(string? UserId, string? ErrorMessage)> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await SendWithFallbackAsync(() =>
                _httpClient.PostAsJsonAsync("api/users/register", request, JsonOptions));
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return (body.Trim('"'), null);

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                return (null, string.IsNullOrWhiteSpace(body) ? "Korisničko ime već postoji." : body);

            return (null, $"Server je vratio grešku ({(int)response.StatusCode}).");
        }
        catch (TaskCanceledException)
        {
            return (null, $"Zahtev je istekao ({BaseUrl}). Proveri backend i Wi-Fi/USB konekciju.");
        }
        catch (HttpRequestException)
        {
            return (null, $"Nema konekcije ({BaseUrl}). Wi-Fi: firewall port 8080. USB: adb reverse tcp:8080 tcp:8080.");
        }
        catch (Exception)
        {
            return (null, "Registracija nije uspela. Proveri konekciju sa serverom.");
        }
    }

    public async Task<User?> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await SendWithFallbackAsync(() =>
                _httpClient.PostAsJsonAsync("api/users/login", request, JsonOptions));
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<User>(JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<(User? User, string? ErrorMessage)> UpdateUsernameAsync(string userId, string newUsername)
    {
        try
        {
            var response = await SendWithFallbackAsync(() =>
                _httpClient.PutAsJsonAsync(
                    $"api/users/{Uri.EscapeDataString(userId)}/username",
                    new UpdateUsernameRequest { NewUsername = newUsername },
                    JsonOptions));

            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<User>(JsonOptions), null);

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                return (null, "Korisničko ime je već zauzeto.");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return (null, "Korisnik nije pronađen.");

            return (null, "Promena korisničkog imena nije uspela.");
        }
        catch (TaskCanceledException)
        {
            return (null, "Zahtev je istekao. Proveri konekciju sa serverom.");
        }
        catch (HttpRequestException)
        {
            return (null, "Nema konekcije sa serverom.");
        }
        catch
        {
            return (null, "Promena korisničkog imena nije uspela.");
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdatePasswordAsync(
        string userId, string currentPassword, string newPassword)
    {
        try
        {
            var response = await SendWithFallbackAsync(() =>
                _httpClient.PutAsJsonAsync(
                    $"api/users/{Uri.EscapeDataString(userId)}/password",
                    new UpdatePasswordRequest
                    {
                        CurrentPassword = currentPassword,
                        NewPassword = newPassword
                    },
                    JsonOptions));

            if (response.IsSuccessStatusCode)
                return (true, null);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return (false, "Trenutna lozinka nije ispravna.");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return (false, "Korisnik nije pronađen.");

            return (false, "Promena lozinke nije uspela.");
        }
        catch (TaskCanceledException)
        {
            return (false, "Zahtev je istekao. Proveri konekciju sa serverom.");
        }
        catch (HttpRequestException)
        {
            return (false, "Nema konekcije sa serverom.");
        }
        catch
        {
            return (false, "Promena lozinke nije uspela.");
        }
    }

    public async Task<User?> GetUserAsync(string username)
    {
        try
        {
            var response = await SendWithFallbackAsync(() =>
                _httpClient.GetAsync($"api/users/me?username={Uri.EscapeDataString(username)}"));
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<User>(JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ImdbSearchResponse?> SearchMoviesAsync(string query)
    {
        try
        {
            var response = await SendWithFallbackAsync(() =>
                _httpClient.GetAsync($"api/movies/search?query={Uri.EscapeDataString(query)}"));
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"IMDB search raw JSON: {json}");
#endif
            return JsonSerializer.Deserialize<ImdbSearchResponse>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ImdbTitleResponse?> GetMovieDetailsAsync(string imdbId)
    {
        try
        {
            var response = await SendWithFallbackAsync(() =>
                _httpClient.GetAsync($"api/movies/imdb/{Uri.EscapeDataString(imdbId)}"));
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ImdbTitleResponse>(JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> PostReviewAsync(ReviewRequest request)
    {
        try
        {
            var response = await SendWithFallbackAsync(() =>
                _httpClient.PostAsJsonAsync("api/movies/review", request, JsonOptions));
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsStringAsync();
        }
        catch
        {
            return null;
        }
    }

    public async Task<Review?> UpdateReviewAsync(string reviewId, UpdateReviewRequest request)
    {
        try
        {
            var response = await SendWithFallbackAsync(() =>
                _httpClient.PutAsJsonAsync($"api/movies/review/{Uri.EscapeDataString(reviewId)}", request, JsonOptions));
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<Review>(JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DeleteReviewAsync(string reviewId)
    {
        try
        {
            var response = await SendWithFallbackAsync(() =>
                _httpClient.DeleteAsync($"api/movies/review/{Uri.EscapeDataString(reviewId)}"));
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<Review>?> GetMovieReviewsAsync(string imdbId)
    {
        try
        {
            var response = await SendWithFallbackAsync(() =>
                _httpClient.GetAsync($"api/movies/{Uri.EscapeDataString(imdbId)}/reviews"));
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<List<Review>>(JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<Review>?> GetUserReviewsAsync(string userId)
    {
        try
        {
            var response = await SendWithFallbackAsync(() =>
                _httpClient.GetAsync($"api/movies?userId={Uri.EscapeDataString(userId)}"));
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<List<Review>>(JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<AverageRatingResponse?> GetAverageRatingAsync(string imdbId)
    {
        try
        {
            var response = await SendWithFallbackAsync(() =>
                _httpClient.GetAsync($"api/movies/{Uri.EscapeDataString(imdbId)}/average-rating"));
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<AverageRatingResponse>(JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    private async Task EnsureBaseUrlAsync()
    {
        var url = await _resolver.ResolveAsync();
        if (_httpClient.BaseAddress?.ToString() != url)
            _httpClient.BaseAddress = new Uri(url);
    }

    private async Task<HttpResponseMessage> SendWithFallbackAsync(Func<Task<HttpResponseMessage>> send)
    {
        Exception? lastError = null;

        for (var attempt = 0; attempt < 2; attempt++)
        {
            await EnsureBaseUrlAsync();
            try
            {
                return await send();
            }
            catch (Exception ex) when (IsConnectionError(ex))
            {
                lastError = ex;
                _resolver.Invalidate();
            }
        }

        throw lastError ?? new HttpRequestException("Nema konekcije sa serverom.");
    }

    private static bool IsConnectionError(Exception ex) =>
        ex is HttpRequestException or TaskCanceledException;
}
