namespace PCInform.Models;

public sealed class AppSettings
{
    public ApplicationSettings Application { get; set; } = new();
    public SupportSettings Support { get; set; } = new();
    public FeatureSettings Features { get; set; } = new();
    public ReportSettings Report { get; set; } = new();
    public UpdateSettings Update { get; set; } = new();
}

public sealed class ApplicationSettings
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

public sealed class SupportSettings
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

public sealed class FeatureSettings
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

public sealed class ReportSettings
{
    public bool IncludeComputerName { get; set; } = true;
    public bool IncludeDomain { get; set; } = true;
    public bool IncludeOperatingSystem { get; set; } = true;
    public bool IncludeIpAddress { get; set; } = true;
    public bool IncludeDnsServers { get; set; } = true;
    public bool IncludeUptime { get; set; } = true;
    public bool IncludeManufacturerModel { get; set; } = true;
    public bool IncludeSerialNumber { get; set; } = true;
    public bool IncludeDeviceType { get; set; } = true;
    public bool IncludeUserLogin { get; set; } = true;
    public bool IncludeDisplayName { get; set; } = true;
    public bool IncludeTeamViewer { get; set; }
    public bool IncludeAtera { get; set; }
}

public sealed class UpdateSettings
{
    public bool Enabled { get; set; }
    public string VersionUrl { get; set; } = string.Empty;
    public bool ShowFooterIndicator { get; set; } = true;
}
