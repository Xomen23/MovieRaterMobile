namespace MovieRaterApp.Services;

public static class ApiConstants
{
    /// <summary>
    /// LAN IP računara (ipconfig → Wi-Fi adapter).
    /// </summary>
    public const string DevPcLanIp = "192.168.0.30";

    public static IReadOnlyList<string> GetCandidateBaseUrls()
    {
#if ANDROID
        if (IsAndroidEmulator())
            return ["http://10.0.2.2:8080/"];

        // Wi-Fi prvo, zatim USB (adb reverse tcp:8080 tcp:8080).
        return
        [
            $"http://{DevPcLanIp}:8080/",
            "http://127.0.0.1:8080/"
        ];
#elif WINDOWS
        return ["http://localhost:8080/"];
#else
        return ["http://localhost:8080/"];
#endif
    }

#if ANDROID
    private static bool IsAndroidEmulator()
    {
        var fingerprint = Android.OS.Build.Fingerprint ?? string.Empty;
        var model = Android.OS.Build.Model ?? string.Empty;
        var manufacturer = Android.OS.Build.Manufacturer ?? string.Empty;
        var hardware = Android.OS.Build.Hardware ?? string.Empty;

        return fingerprint.Contains("generic", StringComparison.OrdinalIgnoreCase)
               || fingerprint.Contains("emulator", StringComparison.OrdinalIgnoreCase)
               || model.Contains("Emulator", StringComparison.OrdinalIgnoreCase)
               || model.Contains("sdk_gphone", StringComparison.OrdinalIgnoreCase)
               || manufacturer.Contains("Genymotion", StringComparison.OrdinalIgnoreCase)
               || hardware.Contains("goldfish", StringComparison.OrdinalIgnoreCase)
               || hardware.Contains("ranchu", StringComparison.OrdinalIgnoreCase);
    }
#endif
}
