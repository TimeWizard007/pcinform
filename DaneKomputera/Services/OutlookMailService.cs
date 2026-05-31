using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace DaneKomputera.Services;

internal static class OutlookMailService
{
    private const int OlMailItem = 0;
    private const int OlFolderInbox = 6;

    private const string MapiProfilesRegistryPath =
        @"Software\Microsoft\Windows NT\CurrentVersion\Windows Messaging Subsystem\Profiles";

    public static bool TryCreateDraft(string to, string subject, string body)
    {
        if (!HasUsableMapiProfile())
        {
            return false;
        }

        var outlookWasRunning = IsOutlookProcessRunning();
        object? outlookApp = null;
        object? mailItem = null;
        var startedByApplication = false;

        try
        {
            var outlookType = Type.GetTypeFromProgID("Outlook.Application");
            if (outlookType is null)
            {
                return false;
            }

            outlookApp = Activator.CreateInstance(outlookType);
            if (outlookApp is null)
            {
                return false;
            }

            startedByApplication = !outlookWasRunning;

            dynamic outlook = outlookApp;
            if (!TryInitializeMapiSession(outlook))
            {
                TryCloseOutlookStartedByApplication(outlook, startedByApplication);
                return false;
            }

            mailItem = outlook.CreateItem(OlMailItem);
            if (mailItem is null)
            {
                TryCloseOutlookStartedByApplication(outlook, startedByApplication);
                return false;
            }

            dynamic mail = mailItem;
            mail.To = to;
            mail.Subject = subject;
            mail.Body = body;
            mail.Display(false);
            return true;
        }
        catch (COMException)
        {
            if (outlookApp is not null)
            {
                TryCloseOutlookStartedByApplication((dynamic)outlookApp, startedByApplication);
            }

            return false;
        }
        catch (InvalidCastException)
        {
            if (outlookApp is not null)
            {
                TryCloseOutlookStartedByApplication((dynamic)outlookApp, startedByApplication);
            }

            return false;
        }
        catch (Exception)
        {
            if (outlookApp is not null)
            {
                TryCloseOutlookStartedByApplication((dynamic)outlookApp, startedByApplication);
            }

            return false;
        }
        finally
        {
            if (mailItem is not null)
            {
                Marshal.FinalReleaseComObject(mailItem);
            }

            if (outlookApp is not null)
            {
                Marshal.FinalReleaseComObject(outlookApp);
            }
        }
    }

    private static bool HasUsableMapiProfile()
    {
        try
        {
            using var profilesKey = Registry.CurrentUser.OpenSubKey(MapiProfilesRegistryPath);
            if (profilesKey is null)
            {
                return false;
            }

            using var orderKey = profilesKey.OpenSubKey("Order");
            if (orderKey is null)
            {
                return false;
            }

            foreach (var valueName in orderKey.GetValueNames())
            {
                var profileName = orderKey.GetValue(valueName)?.ToString();
                if (string.IsNullOrWhiteSpace(profileName))
                {
                    continue;
                }

                using var profileKey = profilesKey.OpenSubKey(profileName);
                if (profileKey is null)
                {
                    continue;
                }

                if (ProfileHasConfiguredServices(profileKey))
                {
                    return true;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    private static bool ProfileHasConfiguredServices(RegistryKey profileKey)
    {
        foreach (var subKeyName in profileKey.GetSubKeyNames())
        {
            if (string.Equals(subKeyName, "Calendar Style", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            using var serviceKey = profileKey.OpenSubKey(subKeyName);
            if (serviceKey is null)
            {
                continue;
            }

            var guid = serviceKey.GetValue("GUID")?.ToString();
            var dllPath = serviceKey.GetValue("DLLPath")?.ToString();
            if (!string.IsNullOrWhiteSpace(guid) || !string.IsNullOrWhiteSpace(dllPath))
            {
                return true;
            }
        }

        return profileKey.GetSubKeyNames().Length > 0;
    }

    private static bool TryInitializeMapiSession(dynamic outlook)
    {
        try
        {
            dynamic mapi = outlook.GetNamespace("MAPI");
            mapi.Logon(Type.Missing, Type.Missing, false, false);

            dynamic session = outlook.Session;
            if (session is null)
            {
                return false;
            }

            dynamic accounts = session.Accounts;
            if (accounts is not null && accounts.Count > 0)
            {
                return true;
            }

            dynamic inbox = session.GetDefaultFolder(OlFolderInbox);
            return inbox is not null;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsOutlookProcessRunning()
    {
        try
        {
            return Process.GetProcessesByName("OUTLOOK").Length > 0;
        }
        catch
        {
            return false;
        }
    }

    private static void TryCloseOutlookStartedByApplication(dynamic outlook, bool startedByApplication)
    {
        if (!startedByApplication)
        {
            return;
        }

        try
        {
            outlook.Quit();
        }
        catch
        {
            // Ignore shutdown failures.
        }
    }
}
