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
        var report = ConfigurationService.Current.Report;
        var lines = BuildComputerLines(data, report, language);
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

    private static List<string> BuildComputerLines(SystemInfoData data, ReportSettings report, AppLanguage language)
    {
        var lines = new List<string>();

        if (report.IncludeComputerName)
        {
            lines.Add($"{LocalizationManager.ComputerNameLabel} {data.ComputerName}");
        }

        if (report.IncludeDomain)
        {
            lines.Add($"{LocalizationManager.DomainLabel} {data.Domain}");
        }

        if (report.IncludeOperatingSystem)
        {
            lines.Add($"{LocalizationManager.OperatingSystemLabel} {data.OperatingSystem}");
        }

        if (report.IncludeIpAddress)
        {
            lines.Add($"{LocalizationManager.IpAddressLabel} {data.IpAddress}");
        }

        if (report.IncludeDnsServers)
        {
            lines.Add($"{LocalizationManager.DnsLabel} {data.DnsServers}");
        }

        if (report.IncludeUptime)
        {
            lines.Add($"{LocalizationManager.UptimeLabel} {data.Uptime}");
        }

        if (report.IncludeManufacturerModel)
        {
            lines.Add($"{LocalizationManager.ManufacturerLabel} {data.ManufacturerModel}");
        }

        if (report.IncludeSerialNumber)
        {
            lines.Add($"{LocalizationManager.BiosSerialLabel} {data.BiosSerial}");
        }

        if (report.IncludeDeviceType)
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
        var report = ConfigurationService.Current.Report;
        var lines = BuildUserLines(data, report, language);
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

    private static List<string> BuildUserLines(SystemInfoData data, ReportSettings report, AppLanguage language)
    {
        var lines = new List<string>();

        if (report.IncludeUserLogin)
        {
            lines.Add(language == AppLanguage.Polish
                ? $"Login: {data.UserLogin}"
                : $"Login: {data.UserLogin}");
        }

        if (report.IncludeDisplayName)
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
        var report = ConfigurationService.Current.Report;
        if (!report.IncludeTeamViewer && !report.IncludeAtera)
        {
            return;
        }

        var lines = new List<string>();
        if (report.IncludeTeamViewer)
        {
            lines.Add($"TeamViewer: {GetTeamViewerStatus(data, language)}");
        }

        if (report.IncludeAtera)
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
