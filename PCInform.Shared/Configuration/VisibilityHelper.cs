using PCInform.Models;

namespace PCInform.Configuration;

public static class VisibilityHelper
{
    public static bool IsEmailVisible(SupportSettings support) =>
        support.ShowEmail && !string.IsNullOrWhiteSpace(support.EmailTo);

    public static bool IsPhoneVisible(SupportSettings support) =>
        support.ShowPhone && !string.IsNullOrWhiteSpace(support.Phone);

    public static bool IsMobilePhoneVisible(SupportSettings support) =>
        support.ShowMobilePhone && !string.IsNullOrWhiteSpace(support.MobilePhone);

    public static bool IsWebsiteVisible(SupportSettings support) =>
        support.ShowWebsite && !string.IsNullOrWhiteSpace(support.WebsiteUrl);

    public static bool IsContactSectionVisible(SupportSettings support) =>
        IsEmailVisible(support) ||
        IsPhoneVisible(support) ||
        IsMobilePhoneVisible(support) ||
        IsWebsiteVisible(support);

    public static bool HasAnyComputerField(FeatureSettings features) =>
        features.ShowComputerName ||
        features.ShowDomain ||
        features.ShowOperatingSystem ||
        features.ShowIpAddress ||
        features.ShowDnsServers ||
        features.ShowUptime ||
        features.ShowManufacturerModel ||
        features.ShowSerialNumber ||
        features.ShowDeviceType;

    public static bool HasAnyUserField(FeatureSettings features) =>
        features.ShowUserLogin || features.ShowDisplayName;

    public static bool HasAnyComputerReportField(ReportSettings report) =>
        report.IncludeComputerName ||
        report.IncludeDomain ||
        report.IncludeOperatingSystem ||
        report.IncludeIpAddress ||
        report.IncludeDnsServers ||
        report.IncludeUptime ||
        report.IncludeManufacturerModel ||
        report.IncludeSerialNumber ||
        report.IncludeDeviceType ||
        report.IncludeNetworkStatus;

    public static bool HasAnyUserReportField(ReportSettings report) =>
        report.IncludeUserLogin || report.IncludeDisplayName;

    public static bool HasAnyReportContent(ReportSettings report, SupportSettings support) =>
        HasAnyComputerReportField(report) ||
        HasAnyUserReportField(report) ||
        report.IncludeTeamViewer ||
        report.IncludeAtera;
}
