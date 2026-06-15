using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using PCInform.Configuration;
using PCInform.Localization;
using PCInform.Models;

namespace PCInform.Services;

internal sealed class UpdateCheckResult
{
    public required string RemoteVersion { get; init; }
    public string? DownloadUrl { get; init; }
}

internal static class UpdateService
{
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(10) };

    public static UpdateCheckResult? LastResult { get; private set; }

    public static bool IsUpdateAvailable => LastResult is not null;

    public static async Task CheckForUpdatesAsync()
    {
        LastResult = null;

        var config = ConfigurationService.Current;
        if (!config.Update.Enabled || string.IsNullOrWhiteSpace(config.Update.VersionUrl))
        {
            return;
        }

        try
        {
            var json = await HttpClient.GetStringAsync(config.Update.VersionUrl).ConfigureAwait(false);
            var remote = JsonSerializer.Deserialize<RemoteVersionInfo>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (remote is null || string.IsNullOrWhiteSpace(remote.Version))
            {
                return;
            }

            if (!Version.TryParse(AppInfoService.Version, out var currentVersion) ||
                !Version.TryParse(remote.Version, out var remoteVersion))
            {
                return;
            }

            if (remoteVersion <= currentVersion)
            {
                return;
            }

            LastResult = new UpdateCheckResult
            {
                RemoteVersion = remote.Version.Trim(),
                DownloadUrl = string.IsNullOrWhiteSpace(remote.DownloadUrl) ? null : remote.DownloadUrl.Trim()
            };
        }
        catch
        {
            // Do not block startup when update check fails.
        }
    }

    public static void OpenDownloadPage()
    {
        var url = LastResult?.DownloadUrl;
        if (string.IsNullOrWhiteSpace(url))
        {
            url = LocalizationManager.AboutGitHubUrl;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch
        {
            // Ignore browser launch failures.
        }
    }
}
