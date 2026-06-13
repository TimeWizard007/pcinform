using System.Diagnostics;
using PCInform.Configuration;

namespace PCInform.Services;

internal static class MailHelper
{
    public static bool TryOpenHelpdeskEmail()
    {
        return TryOpenEmail(ConfigurationService.Current.Support.Email);
    }

    public static bool TryOpenReport(string subject, string body)
    {
        return TryOpenEmail(ConfigurationService.Current.Support.Email, subject, body);
    }

    public static bool TryOpenEmail(string to, string? subject = null, string? body = null)
    {
        if (string.IsNullOrWhiteSpace(to))
        {
            return false;
        }

        if (OutlookMailService.TryCreateDraft(to, subject ?? string.Empty, body ?? string.Empty))
        {
            return true;
        }

        return TryOpenMailto(to, subject, body);
    }

    private static bool TryOpenMailto(string to, string? subject, string? body)
    {
        try
        {
            if (!IsMailtoSchemeRegistered())
            {
                return false;
            }

            var url = BuildMailtoUrl(to, subject, body);
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string BuildMailtoUrl(string to, string? subject, string? body)
    {
        var url = $"mailto:{Uri.EscapeDataString(to)}";
        var queryParts = new List<string>();

        if (!string.IsNullOrEmpty(subject))
        {
            queryParts.Add($"subject={Uri.EscapeDataString(subject)}");
        }

        if (!string.IsNullOrEmpty(body))
        {
            queryParts.Add($"body={Uri.EscapeDataString(body)}");
        }

        if (queryParts.Count > 0)
        {
            url += "?" + string.Join("&", queryParts);
        }

        return url;
    }

    private static bool IsMailtoSchemeRegistered()
    {
        try
        {
            using var key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"mailto\shell\open\command");
            var command = key?.GetValue(null)?.ToString();
            return !string.IsNullOrWhiteSpace(command);
        }
        catch
        {
            return false;
        }
    }
}
