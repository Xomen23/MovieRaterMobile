namespace MovieRaterApp.Services;

public class ApiConnectionResolver
{
    private string? _activeBaseUrl;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public string? ActiveBaseUrl => _activeBaseUrl;

    public async Task<string> ResolveAsync(CancellationToken cancellationToken = default)
    {
        if (_activeBaseUrl is not null)
            return _activeBaseUrl;

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_activeBaseUrl is not null)
                return _activeBaseUrl;

            foreach (var candidate in ApiConstants.GetCandidateBaseUrls())
            {
                if (await ProbeAsync(candidate, cancellationToken))
                {
                    _activeBaseUrl = candidate;
                    return candidate;
                }
            }

            return ApiConstants.GetCandidateBaseUrls()[0];
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Invalidate() => _activeBaseUrl = null;

    private static async Task<bool> ProbeAsync(string baseUrl, CancellationToken cancellationToken)
    {
        try
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(3)
            };
            using var response = await client.GetAsync("api/health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
