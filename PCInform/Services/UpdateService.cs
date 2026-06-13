using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using PCInform.Configuration;
using PCInform.Localization;
using PCInform.Models;

namespace PCInform.Services;

internal static class UpdateService
{
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(10) };

    public static async Task CheckForUpdatesAsync(IWin32Window? owner)
    {
        var config = ConfigurationService.Current;
        if (!config.Features.CheckUpdates || !config.Update.Enabled)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(config.Update.VersionUrl))
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

            if (string.IsNullOrWhiteSpace(remote.DownloadUrl))
            {
                return;
            }

            var language = LocalizationManager.CurrentLanguage;
            var releaseNotes = language == AppLanguage.Polish
                ? remote.ReleaseNotesPl
                : remote.ReleaseNotesEn;

            if (string.IsNullOrWhiteSpace(releaseNotes))
            {
                releaseNotes = language == AppLanguage.Polish
                    ? "Brak informacji o wydaniu."
                    : "No release notes provided.";
            }

            var message = LocalizationManager.UpdateAvailableMessage(remote.Version, releaseNotes);
            var result = ShowUpdateDialog(owner, message);

            if (result == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = remote.DownloadUrl,
                    UseShellExecute = true
                });
            }
        }
        catch
        {
            // Do not block startup when update check fails.
        }
    }

    private static DialogResult ShowUpdateDialog(IWin32Window? owner, string message)
    {
        if (owner is Control control && control.InvokeRequired)
        {
            return (DialogResult)control.Invoke(() => ShowUpdateDialog(owner, message))!;
        }

        return MessageBox.Show(
            owner,
            message,
            LocalizationManager.UpdateAvailableTitle,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Information);
    }
}
