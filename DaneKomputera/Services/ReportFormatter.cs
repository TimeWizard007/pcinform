using DaneKomputera.Localization;
using DaneKomputera.Models;

namespace DaneKomputera.Services;

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
        return
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

---------------------------------
TEAMVIEWER
---------------------------------
Status: {GetTeamViewerStatus(data, AppLanguage.Polish)}
Atera: {GetAteraStatus(data, AppLanguage.Polish)}
""";
    }

    private static string FormatEnglishClipboard(SystemInfoData data)
    {
        return
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

---------------------------------
TEAMVIEWER
---------------------------------
Status: {GetTeamViewerStatus(data, AppLanguage.English)}
Atera: {GetAteraStatus(data, AppLanguage.English)}
""";
    }

    private static string FormatPolishReportEmail(SystemInfoData data)
    {
        return
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

TeamViewer: {GetTeamViewerStatus(data, AppLanguage.Polish)}
Atera: {GetAteraStatus(data, AppLanguage.Polish)}
""";
    }

    private static string FormatEnglishReportEmail(SystemInfoData data)
    {
        return
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

TeamViewer: {GetTeamViewerStatus(data, AppLanguage.English)}
Atera: {GetAteraStatus(data, AppLanguage.English)}
""";
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
