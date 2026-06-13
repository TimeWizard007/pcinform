using PCInform.Configuration;
using PCInform.Localization;
using PCInform.Models;

namespace PCInform.Services;

internal static class ReportFormatter
{
    public static string FormatClipboard(SystemInfoData data, AppLanguage language) =>
        language == AppLanguage.Polish
            ? FormatPolishClipboard(data)
            : FormatEnglishClipboard(data);

    public static string FormatReportEmail(SystemInfoData data, AppLanguage language) =>
        language == AppLanguage.Polish
            ? FormatPolishReportEmail(data)
            : FormatEnglishReportEmail(data);

    private static string FormatPolishClipboard(SystemInfoData data)
    {
        var report =
$"""
---------------------------------
DANE KOMPUTERA
---------------------------------
Nazwa komputera: {data.ComputerName}
Domena: {data.Domain}
System operacyjny: {data.OperatingSystem}
Adres IP: {data.IpAddress}
DNS: {data.DnsServers}
Czas pracy: {data.Uptime}
Producent/model: {data.ManufacturerModel}
Numer seryjny: {data.BiosSerial}
Typ urządzenia: {data.MachineType}

---------------------------------
TWOJE DANE
---------------------------------
Login: {data.UserLogin}
Nazwa użytkownika: {data.UserDisplayName}
""";

        report += BuildAgentSection(data, AppLanguage.Polish);
        return report;
    }

    private static string FormatEnglishClipboard(SystemInfoData data)
    {
        var report =
$"""
---------------------------------
COMPUTER DATA
---------------------------------
Computer name: {data.ComputerName}
Domain: {data.Domain}
Operating system: {data.OperatingSystem}
IP address: {data.IpAddress}
DNS: {data.DnsServers}
Uptime: {data.Uptime}
Manufacturer/model: {data.ManufacturerModel}
Serial number: {data.BiosSerial}
Device type: {data.MachineType}

---------------------------------
USER DATA
---------------------------------
Login: {data.UserLogin}
Display name: {data.UserDisplayName}
""";

        report += BuildAgentSection(data, AppLanguage.English);
        return report;
    }

    private static string FormatPolishReportEmail(SystemInfoData data)
    {
        var report =
$"""
---

## OPIS PROBLEMU

Prosimy o krótki opis problemu:



---

## DANE KOMPUTERA

Nazwa komputera: {data.ComputerName}
Domena: {data.Domain}
System operacyjny: {data.OperatingSystem}
Adres IP: {data.IpAddress}
DNS: {data.DnsServers}
Czas pracy: {data.Uptime}
Producent/model: {data.ManufacturerModel}
Numer seryjny: {data.BiosSerial}
Typ urządzenia: {data.MachineType}

Login: {data.UserLogin}
Nazwa użytkownika: {data.UserDisplayName}
""";

        report += BuildInlineAgentLines(data, AppLanguage.Polish);
        return report;
    }

    private static string FormatEnglishReportEmail(SystemInfoData data)
    {
        var report =
$"""
---

## PROBLEM DESCRIPTION

Please provide a short description of the issue:



---

## COMPUTER DATA

Computer name: {data.ComputerName}
Domain: {data.Domain}
Operating system: {data.OperatingSystem}
IP address: {data.IpAddress}
DNS: {data.DnsServers}
Uptime: {data.Uptime}
Manufacturer/model: {data.ManufacturerModel}
Serial number: {data.BiosSerial}
Device type: {data.MachineType}

Login: {data.UserLogin}
Display name: {data.UserDisplayName}
""";

        report += BuildInlineAgentLines(data, AppLanguage.English);
        return report;
    }

    private static string BuildAgentSection(SystemInfoData data, AppLanguage language)
    {
        var features = ConfigurationService.Current.Features;
        if (!features.ShowTeamViewer && !features.IncludeAteraInReports)
        {
            return string.Empty;
        }

        var section =
            language == AppLanguage.Polish
                ? "\n\n---------------------------------\nAGENTY\n---------------------------------\n"
                : "\n\n---------------------------------\nAGENTS\n---------------------------------\n";

        var lines = new List<string>();
        if (features.ShowTeamViewer)
        {
            lines.Add(language == AppLanguage.Polish
                ? $"TeamViewer: {GetTeamViewerStatus(data, language)}"
                : $"TeamViewer: {GetTeamViewerStatus(data, language)}");
        }

        if (features.IncludeAteraInReports)
        {
            lines.Add(language == AppLanguage.Polish
                ? $"Atera: {GetAteraStatus(data, language)}"
                : $"Atera: {GetAteraStatus(data, language)}");
        }

        return lines.Count == 0 ? string.Empty : section + string.Join('\n', lines) + '\n';
    }

    private static string BuildInlineAgentLines(SystemInfoData data, AppLanguage language)
    {
        var features = ConfigurationService.Current.Features;
        var lines = new List<string>();

        if (features.ShowTeamViewer)
        {
            lines.Add($"TeamViewer: {GetTeamViewerStatus(data, language)}");
        }

        if (features.IncludeAteraInReports)
        {
            lines.Add($"Atera: {GetAteraStatus(data, language)}");
        }

        return lines.Count == 0 ? string.Empty : '\n' + string.Join('\n', lines) + '\n';
    }

    private static string GetTeamViewerStatus(SystemInfoData data, AppLanguage language) =>
        language == AppLanguage.Polish
            ? (data.TeamViewerInstalled ? "Zainstalowany" : "Nie zainstalowano")
            : (data.TeamViewerInstalled ? "Installed" : "Not installed");

    private static string GetAteraStatus(SystemInfoData data, AppLanguage language) =>
        language == AppLanguage.Polish
            ? (data.AteraInstalled ? "Zainstalowano" : "Nie zainstalowano")
            : (data.AteraInstalled ? "Installed" : "Not installed");
}
