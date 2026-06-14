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
        var sections = new List<string>();
        AppendContactSection(sections, AppLanguage.Polish);
        AppendComputerSection(sections, data, AppLanguage.Polish);
        AppendUserSection(sections, data, AppLanguage.Polish);
        AppendAgentSection(sections, data, AppLanguage.Polish);
        return string.Join('\n', sections).TrimEnd() + '\n';
    }

    private static string FormatEnglishClipboard(SystemInfoData data)
    {
        var sections = new List<string>();
        AppendContactSection(sections, AppLanguage.English);
        AppendComputerSection(sections, data, AppLanguage.English);
        AppendUserSection(sections, data, AppLanguage.English);
        AppendAgentSection(sections, data, AppLanguage.English);
        return string.Join('\n', sections).TrimEnd() + '\n';
    }

    private static string FormatPolishReportEmail(SystemInfoData data)
    {
        var sections = new List<string>
        {
            """
            ---

            ## OPIS PROBLEMU

            Prosimy o krótki opis problemu:



            ---
            """
        };

        AppendContactSection(sections, AppLanguage.Polish, inline: true);
        AppendComputerSection(sections, data, AppLanguage.Polish, inline: true);
        AppendUserSection(sections, data, AppLanguage.Polish, inline: true);
        AppendAgentSection(sections, data, AppLanguage.Polish, inline: true);
        return string.Join('\n', sections).TrimEnd() + '\n';
    }

    private static string FormatEnglishReportEmail(SystemInfoData data)
    {
        var sections = new List<string>
        {
            """
            ---

            ## PROBLEM DESCRIPTION

            Please provide a short description of the issue:



            ---
            """
        };

        AppendContactSection(sections, AppLanguage.English, inline: true);
        AppendComputerSection(sections, data, AppLanguage.English, inline: true);
        AppendUserSection(sections, data, AppLanguage.English, inline: true);
        AppendAgentSection(sections, data, AppLanguage.English, inline: true);
        return string.Join('\n', sections).TrimEnd() + '\n';
    }

    private static void AppendContactSection(List<string> sections, AppLanguage language, bool inline = false)
    {
        var support = ConfigurationService.Current.Support;
        var lines = BuildContactLines(support, language);
        if (lines.Count == 0)
        {
            return;
        }

        if (inline)
        {
            sections.Add(language == AppLanguage.Polish ? "## KONTAKT" : "## CONTACT");
            sections.Add(string.Join('\n', lines));
            return;
        }

        sections.Add(language == AppLanguage.Polish
            ? "---------------------------------\nKONTAKT\n---------------------------------"
            : "---------------------------------\nCONTACT\n---------------------------------");
        sections.Add(string.Join('\n', lines));
    }

    private static List<string> BuildContactLines(SupportSettings support, AppLanguage language)
    {
        var lines = new List<string>();

        if (VisibilityHelper.IsEmailVisible(support))
        {
            lines.Add($"{LocalizationManager.EmailLabel} {support.EmailTo}");
        }

        if (VisibilityHelper.IsPhoneVisible(support))
        {
            lines.Add($"{LocalizationManager.HotlineLabel} {support.Phone}");
        }

        if (VisibilityHelper.IsMobilePhoneVisible(support))
        {
            lines.Add($"{LocalizationManager.MobilePhoneLabel} {support.MobilePhone}");
        }

        if (VisibilityHelper.IsWebsiteVisible(support))
        {
            lines.Add($"{LocalizationManager.WebsiteLabel} {support.WebsiteUrl}");
        }

        return lines;
    }

    private static void AppendComputerSection(
        List<string> sections,
        SystemInfoData data,
        AppLanguage language,
        bool inline = false)
    {
        var features = ConfigurationService.Current.Features;
        var lines = BuildComputerLines(data, features, language);
        if (lines.Count == 0)
        {
            return;
        }

        if (inline)
        {
            sections.Add(language == AppLanguage.Polish ? "## DANE KOMPUTERA" : "## COMPUTER DATA");
            sections.Add(string.Join('\n', lines));
            return;
        }

        sections.Add(language == AppLanguage.Polish
            ? "---------------------------------\nDANE KOMPUTERA\n---------------------------------"
            : "---------------------------------\nCOMPUTER DATA\n---------------------------------");
        sections.Add(string.Join('\n', lines));
    }

    private static List<string> BuildComputerLines(SystemInfoData data, FeatureSettings features, AppLanguage language)
    {
        var lines = new List<string>();

        if (features.ShowComputerName)
        {
            lines.Add($"{LocalizationManager.ComputerNameLabel} {data.ComputerName}");
        }

        if (features.ShowDomain)
        {
            lines.Add($"{LocalizationManager.DomainLabel} {data.Domain}");
        }

        if (features.ShowOperatingSystem)
        {
            lines.Add($"{LocalizationManager.OperatingSystemLabel} {data.OperatingSystem}");
        }

        if (features.ShowIpAddress)
        {
            lines.Add($"{LocalizationManager.IpAddressLabel} {data.IpAddress}");
        }

        if (features.ShowDnsServers)
        {
            lines.Add($"{LocalizationManager.DnsLabel} {data.DnsServers}");
        }

        if (features.ShowUptime)
        {
            lines.Add($"{LocalizationManager.UptimeLabel} {data.Uptime}");
        }

        if (features.ShowManufacturerModel)
        {
            lines.Add($"{LocalizationManager.ManufacturerLabel} {data.ManufacturerModel}");
        }

        if (features.ShowSerialNumber)
        {
            lines.Add($"{LocalizationManager.BiosSerialLabel} {data.BiosSerial}");
        }

        if (features.ShowDeviceType)
        {
            lines.Add($"{LocalizationManager.MachineTypeLabel} {data.MachineType}");
        }

        return lines;
    }

    private static void AppendUserSection(
        List<string> sections,
        SystemInfoData data,
        AppLanguage language,
        bool inline = false)
    {
        var features = ConfigurationService.Current.Features;
        var lines = BuildUserLines(data, features, language);
        if (lines.Count == 0)
        {
            return;
        }

        if (inline)
        {
            sections.Add(string.Join('\n', lines));
            return;
        }

        sections.Add(language == AppLanguage.Polish
            ? "---------------------------------\nTWOJE DANE\n---------------------------------"
            : "---------------------------------\nUSER DATA\n---------------------------------");
        sections.Add(string.Join('\n', lines));
    }

    private static List<string> BuildUserLines(SystemInfoData data, FeatureSettings features, AppLanguage language)
    {
        var lines = new List<string>();

        if (features.ShowUserLogin)
        {
            lines.Add(language == AppLanguage.Polish
                ? $"Login: {data.UserLogin}"
                : $"Login: {data.UserLogin}");
        }

        if (features.ShowDisplayName)
        {
            lines.Add(language == AppLanguage.Polish
                ? $"Nazwa użytkownika: {data.UserDisplayName}"
                : $"Display name: {data.UserDisplayName}");
        }

        return lines;
    }

    private static void AppendAgentSection(
        List<string> sections,
        SystemInfoData data,
        AppLanguage language,
        bool inline = false)
    {
        var features = ConfigurationService.Current.Features;
        if (!features.ShowTeamViewerSection && !features.IncludeAteraInReports)
        {
            return;
        }

        var lines = new List<string>();
        if (features.ShowTeamViewerSection)
        {
            lines.Add($"TeamViewer: {GetTeamViewerStatus(data, language)}");
        }

        if (features.IncludeAteraInReports)
        {
            lines.Add($"Atera: {GetAteraStatus(data, language)}");
        }

        if (lines.Count == 0)
        {
            return;
        }

        if (inline)
        {
            sections.Add(string.Join('\n', lines));
            return;
        }

        sections.Add(language == AppLanguage.Polish
            ? "---------------------------------\nAGENTY\n---------------------------------"
            : "---------------------------------\nAGENTS\n---------------------------------");
        sections.Add(string.Join('\n', lines));
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
