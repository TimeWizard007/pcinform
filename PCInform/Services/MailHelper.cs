using System.Diagnostics;
using PCInform.Configuration;

namespace PCInform.Services;

internal static class MailHelper
{
    public static bool TryOpenHelpdeskEmail()
    {
        var support = ConfigurationService.Current.Support;
        return TryOpenEmail(support.EmailTo, cc: support.EmailCc, bcc: support.EmailBcc);
    }

    public static bool TryOpenReport(string subject, string body)
    {
        var support = ConfigurationService.Current.Support;
        return TryOpenEmail(support.EmailTo, subject, body, support.EmailCc, support.EmailBcc);
    }

    public static bool TryOpenEmail(
        string to,
        string? subject = null,
        string? body = null,
        string? cc = null,
        string? bcc = null)
    {
        if (string.IsNullOrWhiteSpace(to))
        {
            return false;
        }

        if (OutlookMailService.TryCreateDraft(to, subject ?? string.Empty, body ?? string.Empty, cc, bcc))
        {
            return true;
        }

        return TryOpenMailto(to, subject, body, cc, bcc);
    }

    private static bool TryOpenMailto(
        string to,
        string? subject,
        string? body,
        string? cc,
        string? bcc)
    {
        try
        {
            if (!IsMailtoSchemeRegistered())
            {
                return false;
            }

            var url = BuildMailtoUrl(to, subject, body, cc, bcc);
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

    private static string BuildMailtoUrl(
        string to,
        string? subject,
        string? body,
        string? cc,
        string? bcc)
    {
        var url = $"mailto:{Uri.EscapeDataString(to)}";
        var queryParts = new List<string>();

        if (!string.IsNullOrWhiteSpace(cc))
        {
            queryParts.Add($"cc={Uri.EscapeDataString(cc)}");
        }

        if (!string.IsNullOrWhiteSpace(bcc))
        {
            queryParts.Add($"bcc={Uri.EscapeDataString(bcc)}");
        }

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
