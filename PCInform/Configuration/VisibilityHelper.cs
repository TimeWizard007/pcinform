using PCInform.Models;

namespace PCInform.Configuration;

internal static class VisibilityHelper
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

    public static bool HasAnyReportContent(FeatureSettings features, SupportSettings support) =>
        IsContactSectionVisible(support) ||
        HasAnyComputerField(features) ||
        HasAnyUserField(features) ||
        features.ShowTeamViewerSection ||
        features.IncludeAteraInReports;
}
