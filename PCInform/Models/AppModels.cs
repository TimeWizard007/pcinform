namespace PCInform.Models;

internal sealed class AppSettings
{
    public ApplicationSettings Application { get; set; } = new();
    public SupportSettings Support { get; set; } = new();
    public FeatureSettings Features { get; set; } = new();
    public UpdateSettings Update { get; set; } = new();
}

internal sealed class ApplicationSettings
{
    public string Name { get; set; } = "PC Inform";
    public string WindowTitle { get; set; } = "PC Inform";
    public string BannerText { get; set; } = "Service Desk";
    public string DefaultLanguage { get; set; } = "pl";
    public string AccentColor { get; set; } = "#E87722";
    public string WebsiteUrl { get; set; } = string.Empty;
    public bool EnablePolish { get; set; } = true;
    public bool EnableEnglish { get; set; } = true;
}

internal sealed class SupportSettings
{
    public string CompanyName { get; set; } = "Your Company";
    public string EmailTo { get; set; } = "helpdesk@example.com";
    public string EmailCc { get; set; } = string.Empty;
    public string EmailBcc { get; set; } = string.Empty;
    public string EmailSubjectPrefixPl { get; set; } = "Pomoc";
    public string EmailSubjectPrefixEn { get; set; } = "Support request";
    public string Phone { get; set; } = string.Empty;
    public string MobilePhone { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public bool ShowCompanyName { get; set; } = true;
    public bool ShowEmail { get; set; } = true;
    public bool ShowPhone { get; set; } = true;
    public bool ShowMobilePhone { get; set; }
    public bool ShowWebsite { get; set; } = true;
}

internal sealed class FeatureSettings
{
    public bool ShowComputerName { get; set; } = true;
    public bool ShowDomain { get; set; } = true;
    public bool ShowOperatingSystem { get; set; } = true;
    public bool ShowIpAddress { get; set; } = true;
    public bool ShowDnsServers { get; set; } = true;
    public bool ShowUptime { get; set; } = true;
    public bool ShowManufacturerModel { get; set; } = true;
    public bool ShowSerialNumber { get; set; } = true;
    public bool ShowDeviceType { get; set; } = true;
    public bool ShowUserLogin { get; set; } = true;
    public bool ShowDisplayName { get; set; } = true;
    public bool ShowTeamViewerSection { get; set; }
    public bool ShowTeamViewer { get; set; }
    public bool AllowLaunchTeamViewer { get; set; }
    public bool DetectAtera { get; set; }
    public bool ShowAteraInGui { get; set; }
    public bool IncludeAteraInReports { get; set; }
    public bool CheckUpdates { get; set; }
}

internal sealed class UpdateSettings
{
    public bool Enabled { get; set; }
    public string VersionUrl { get; set; } = string.Empty;
}

internal sealed class RemoteVersionInfo
{
    public string Version { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public string Sha256 { get; set; } = string.Empty;
    public bool Mandatory { get; set; }
    public string ReleaseNotesPl { get; set; } = string.Empty;
    public string ReleaseNotesEn { get; set; } = string.Empty;
}

internal sealed class SystemInfoData
{
    public string ComputerName { get; init; } = string.Empty;
    public string Domain { get; init; } = string.Empty;
    public string OperatingSystem { get; init; } = string.Empty;
    public string IpAddress { get; init; } = string.Empty;
    public string DnsServers { get; init; } = string.Empty;
    public string Uptime { get; init; } = string.Empty;
    public string ManufacturerModel { get; init; } = string.Empty;
    public string BiosSerial { get; init; } = string.Empty;
    public string MachineType { get; init; } = string.Empty;
    public string UserLogin { get; init; } = string.Empty;
    public string UserDisplayName { get; init; } = string.Empty;
    public bool TeamViewerInstalled { get; init; }
    public string? TeamViewerPath { get; init; }
    public bool AteraInstalled { get; init; }
}
