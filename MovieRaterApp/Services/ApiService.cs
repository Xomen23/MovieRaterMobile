using System.Net.Http.Json;
using System.Text.Json;
using MovieRaterApp.Models;

namespace MovieRaterApp.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/register", request, JsonOptions);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsStringAsync();
        }
        catch
        {
            return null;
        }
    }

    public async Task<User?> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/login", request, JsonOptions);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<User>(JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<User?> GetUserAsync(string username)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/users/me?username={Uri.EscapeDataString(username)}");
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
            var response = await _httpClient.GetAsync($"api/movies/search?query={Uri.EscapeDataString(query)}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ImdbSearchResponse>(JsonOptions);
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
            var response = await _httpClient.GetAsync($"api/movies/imdb/{Uri.EscapeDataString(imdbId)}");
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
            var response = await _httpClient.PostAsJsonAsync("api/movies/review", request, JsonOptions);
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
            var response = await _httpClient.PutAsJsonAsync($"api/movies/review/{Uri.EscapeDataString(reviewId)}", request, JsonOptions);
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
            var response = await _httpClient.DeleteAsync($"api/movies/review/{Uri.EscapeDataString(reviewId)}");
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
            var response = await _httpClient.GetAsync($"api/movies/{Uri.EscapeDataString(imdbId)}/reviews");
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
            var response = await _httpClient.GetAsync($"api/movies?userId={Uri.EscapeDataString(userId)}");
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
            var response = await _httpClient.GetAsync($"api/movies/{Uri.EscapeDataString(imdbId)}/average-rating");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<AverageRatingResponse>(JsonOptions);
        }
        catch
        {
            return null;
        }
    }
}
