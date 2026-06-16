using System.Text.Json;
using System.Text.Json.Serialization;
using PCInform.Models;

namespace PCInform.Configuration;

public static class ConfigurationService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static AppSettings Current { get; private set; } = CreateDefaultSettings();

    public static void Initialize()
    {
        AppDiagnosticLog.Write("Config upgrade started");
        Current = LoadSettings();
        AppDiagnosticLog.Write("Config upgrade completed");
    }

    public static AppSettings LoadSettings()
    {
        if (File.Exists(AppPaths.ConfigFilePath))
        {
            return LoadFromFile(AppPaths.ConfigFilePath);
        }

        var defaults = CreateDefaultSettings();
        TrySaveSettings(defaults, AppPaths.ConfigFilePath);
        return defaults;
    }

    public static AppSettings LoadFromFile(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return CreateDefaultSettings();
            }

            var json = File.ReadAllText(path);
            var settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? CreateDefaultSettings();
            ApplyLegacySupportFields(json, settings);
            ApplyReportUpgrade(json, settings);
            var addedFields = ApplySchemaUpgrade(json, settings);
            var merged = MergeWithDefaults(settings);

            if (addedFields.Count > 0)
            {
                AppDiagnosticLog.Write($"Missing fields detected: {string.Join(", ", addedFields)}");
                if (IsGlobalConfigPath(path))
                {
                    TrySaveSettings(merged, path);
                    AppDiagnosticLog.Write("Upgraded config saved to ProgramData");
                }
            }

            return merged;
        }
        catch
        {
            return CreateDefaultSettings();
        }
    }

    public static void SaveToFile(string path, AppSettings settings)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var merged = MergeWithDefaults(CloneSettings(settings));
        var json = JsonSerializer.Serialize(merged, JsonOptions);
        File.WriteAllText(path, json);
    }

    private static bool IsGlobalConfigPath(string path) =>
        string.Equals(Path.GetFullPath(path), Path.GetFullPath(AppPaths.ConfigFilePath), StringComparison.OrdinalIgnoreCase);

    private static void TrySaveSettings(AppSettings settings, string path)
    {
        try
        {
            SaveToFile(path, settings);
        }
        catch
        {
            // Ignore config write failures (e.g. insufficient permissions).
        }
    }

    private static AppSettings CloneSettings(AppSettings settings) => new()
    {
        Application = new ApplicationSettings
        {
            Name = settings.Application.Name,
            WindowTitle = settings.Application.WindowTitle,
            BannerText = settings.Application.BannerText,
            DefaultLanguage = settings.Application.DefaultLanguage,
            AccentColor = settings.Application.AccentColor,
            WebsiteUrl = settings.Application.WebsiteUrl,
            EnablePolish = settings.Application.EnablePolish,
            EnableEnglish = settings.Application.EnableEnglish
        },
        Support = new SupportSettings
        {
            CompanyName = settings.Support.CompanyName,
            EmailTo = settings.Support.EmailTo,
            EmailCc = settings.Support.EmailCc,
            EmailBcc = settings.Support.EmailBcc,
            EmailSubjectPrefixPl = settings.Support.EmailSubjectPrefixPl,
            EmailSubjectPrefixEn = settings.Support.EmailSubjectPrefixEn,
            Phone = settings.Support.Phone,
            MobilePhone = settings.Support.MobilePhone,
            WebsiteUrl = settings.Support.WebsiteUrl,
            ShowCompanyName = settings.Support.ShowCompanyName,
            ShowEmail = settings.Support.ShowEmail,
            ShowPhone = settings.Support.ShowPhone,
            ShowMobilePhone = settings.Support.ShowMobilePhone,
            ShowWebsite = settings.Support.ShowWebsite
        },
        Features = new FeatureSettings
        {
            ShowComputerName = settings.Features.ShowComputerName,
            ShowDomain = settings.Features.ShowDomain,
            ShowOperatingSystem = settings.Features.ShowOperatingSystem,
            ShowIpAddress = settings.Features.ShowIpAddress,
            ShowDnsServers = settings.Features.ShowDnsServers,
            ShowUptime = settings.Features.ShowUptime,
            ShowManufacturerModel = settings.Features.ShowManufacturerModel,
            ShowSerialNumber = settings.Features.ShowSerialNumber,
            ShowDeviceType = settings.Features.ShowDeviceType,
            ShowUserLogin = settings.Features.ShowUserLogin,
            ShowDisplayName = settings.Features.ShowDisplayName,
            ShowTeamViewerSection = settings.Features.ShowTeamViewerSection,
            ShowTeamViewer = settings.Features.ShowTeamViewer,
            AllowLaunchTeamViewer = settings.Features.AllowLaunchTeamViewer,
            DetectAtera = settings.Features.DetectAtera,
            ShowAteraInGui = settings.Features.ShowAteraInGui,
            IncludeAteraInReports = settings.Features.IncludeAteraInReports,
            CheckUpdates = settings.Features.CheckUpdates,
            ShowNetworkStatus = settings.Features.ShowNetworkStatus
        },
        Report = CloneReportSettings(settings.Report),
        Update = new UpdateSettings
        {
            Enabled = settings.Update.Enabled,
            VersionUrl = settings.Update.VersionUrl,
            ShowFooterIndicator = settings.Update.ShowFooterIndicator
        }
    };

    private static ReportSettings CloneReportSettings(ReportSettings report) => new()
    {
        IncludeComputerName = report.IncludeComputerName,
        IncludeDomain = report.IncludeDomain,
        IncludeOperatingSystem = report.IncludeOperatingSystem,
        IncludeIpAddress = report.IncludeIpAddress,
        IncludeDnsServers = report.IncludeDnsServers,
        IncludeUptime = report.IncludeUptime,
        IncludeManufacturerModel = report.IncludeManufacturerModel,
        IncludeSerialNumber = report.IncludeSerialNumber,
        IncludeDeviceType = report.IncludeDeviceType,
        IncludeUserLogin = report.IncludeUserLogin,
        IncludeDisplayName = report.IncludeDisplayName,
        IncludeTeamViewer = report.IncludeTeamViewer,
        IncludeAtera = report.IncludeAtera,
        IncludeNetworkStatus = report.IncludeNetworkStatus
    };

    private static void ApplyLegacySupportFields(string json, AppSettings settings)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            if (!document.RootElement.TryGetProperty("support", out var supportElement))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(settings.Support.EmailTo) &&
                supportElement.TryGetProperty("email", out var legacyEmail))
            {
                settings.Support.EmailTo = legacyEmail.GetString()?.Trim() ?? string.Empty;
            }
        }
        catch
        {
            // Ignore legacy migration failures.
        }
    }

    private static void ApplyReportUpgrade(string json, AppSettings settings)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.TryGetProperty("report", out _))
            {
                settings.Report ??= CreateDefaultSettings().Report;
                return;
            }

            var features = settings.Features ?? new FeatureSettings();
            settings.Report = new ReportSettings
            {
                IncludeComputerName = features.ShowComputerName,
                IncludeDomain = features.ShowDomain,
                IncludeOperatingSystem = features.ShowOperatingSystem,
                IncludeIpAddress = features.ShowIpAddress,
                IncludeDnsServers = features.ShowDnsServers,
                IncludeUptime = features.ShowUptime,
                IncludeManufacturerModel = features.ShowManufacturerModel,
                IncludeSerialNumber = features.ShowSerialNumber,
                IncludeDeviceType = features.ShowDeviceType,
                IncludeUserLogin = features.ShowUserLogin,
                IncludeDisplayName = features.ShowDisplayName,
                IncludeTeamViewer = features.ShowTeamViewer,
                IncludeAtera = features.IncludeAteraInReports,
                IncludeNetworkStatus = features.ShowNetworkStatus
            };
        }
        catch
        {
            settings.Report ??= CreateDefaultSettings().Report;
        }
    }

    private static List<string> ApplySchemaUpgrade(string json, AppSettings settings)
    {
        var addedFields = new List<string>();

        try
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            settings.Features ??= new FeatureSettings();
            settings.Report ??= CreateDefaultSettings().Report;
            settings.Update ??= new UpdateSettings();

            if (root.TryGetProperty("features", out var featuresElement))
            {
                if (!HasJsonProperty(featuresElement, "showNetworkStatus"))
                {
                    settings.Features.ShowNetworkStatus = true;
                    addedFields.Add("features.showNetworkStatus");
                }
            }
            else
            {
                settings.Features.ShowNetworkStatus = true;
                addedFields.Add("features.showNetworkStatus");
            }

            if (root.TryGetProperty("report", out var reportElement))
            {
                if (!HasJsonProperty(reportElement, "includeNetworkStatus"))
                {
                    settings.Report.IncludeNetworkStatus = true;
                    addedFields.Add("report.includeNetworkStatus");
                }
            }
            else
            {
                settings.Report.IncludeNetworkStatus = true;
                addedFields.Add("report.includeNetworkStatus");
            }

            if (root.TryGetProperty("update", out var updateElement))
            {
                if (!HasJsonProperty(updateElement, "showFooterIndicator"))
                {
                    settings.Update.ShowFooterIndicator = true;
                    addedFields.Add("update.showFooterIndicator");
                }
            }
            else
            {
                settings.Update.ShowFooterIndicator = true;
                addedFields.Add("update.showFooterIndicator");
            }
        }
        catch
        {
            // Ignore schema upgrade failures.
        }

        return addedFields;
    }

    private static bool HasJsonProperty(JsonElement element, string propertyName)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public static AppSettings MergeWithDefaults(AppSettings settings)
    {
        var defaults = CreateDefaultSettings();

        settings.Application ??= new ApplicationSettings();
        settings.Support ??= new SupportSettings();
        settings.Features ??= new FeatureSettings();
        settings.Report ??= CloneReportSettings(defaults.Report);
        settings.Update ??= new UpdateSettings();

        settings.Application.Name = NullIfWhiteSpace(settings.Application.Name) ?? defaults.Application.Name;
        settings.Application.WindowTitle = NullIfWhiteSpace(settings.Application.WindowTitle) ?? defaults.Application.WindowTitle;
        settings.Application.BannerText = NullIfWhiteSpace(settings.Application.BannerText) ?? defaults.Application.BannerText;
        settings.Application.DefaultLanguage = NullIfWhiteSpace(settings.Application.DefaultLanguage) ?? defaults.Application.DefaultLanguage;
        settings.Application.AccentColor = NullIfWhiteSpace(settings.Application.AccentColor) ?? defaults.Application.AccentColor;
        settings.Application.WebsiteUrl = settings.Application.WebsiteUrl?.Trim() ?? defaults.Application.WebsiteUrl;

        settings.Support.CompanyName = NullIfWhiteSpace(settings.Support.CompanyName) ?? defaults.Support.CompanyName;
        settings.Support.EmailTo = settings.Support.EmailTo?.Trim() ?? defaults.Support.EmailTo;
        settings.Support.EmailCc = settings.Support.EmailCc?.Trim() ?? defaults.Support.EmailCc;
        settings.Support.EmailBcc = settings.Support.EmailBcc?.Trim() ?? defaults.Support.EmailBcc;
        settings.Support.Phone = settings.Support.Phone?.Trim() ?? defaults.Support.Phone;
        settings.Support.MobilePhone = settings.Support.MobilePhone?.Trim() ?? defaults.Support.MobilePhone;
        settings.Support.WebsiteUrl = settings.Support.WebsiteUrl?.Trim() ?? defaults.Support.WebsiteUrl;
        settings.Support.EmailSubjectPrefixPl = NullIfWhiteSpace(settings.Support.EmailSubjectPrefixPl) ?? defaults.Support.EmailSubjectPrefixPl;
        settings.Support.EmailSubjectPrefixEn = NullIfWhiteSpace(settings.Support.EmailSubjectPrefixEn) ?? defaults.Support.EmailSubjectPrefixEn;

        settings.Update.VersionUrl = settings.Update.VersionUrl?.Trim() ?? defaults.Update.VersionUrl;

        return settings;
    }

    private static string? NullIfWhiteSpace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public static AppSettings CreateDefaultSettings() => new()
    {
        Application = new ApplicationSettings
        {
            Name = "PC Inform",
            WindowTitle = "PC Inform",
            BannerText = "Service Desk",
            DefaultLanguage = "pl",
            AccentColor = "#E87722",
            WebsiteUrl = "https://example.com",
            EnablePolish = true,
            EnableEnglish = true
        },
        Support = new SupportSettings
        {
            CompanyName = "Your Company",
            EmailTo = "helpdesk@example.com",
            EmailCc = string.Empty,
            EmailBcc = string.Empty,
            Phone = "+48 22 123 45 67",
            MobilePhone = string.Empty,
            WebsiteUrl = "https://example.com",
            EmailSubjectPrefixPl = "Pomoc",
            EmailSubjectPrefixEn = "Support request",
            ShowCompanyName = true,
            ShowEmail = true,
            ShowPhone = true,
            ShowMobilePhone = false,
            ShowWebsite = true
        },
        Features = new FeatureSettings
        {
            ShowComputerName = true,
            ShowDomain = true,
            ShowOperatingSystem = true,
            ShowIpAddress = true,
            ShowDnsServers = true,
            ShowUptime = true,
            ShowManufacturerModel = true,
            ShowSerialNumber = true,
            ShowDeviceType = true,
            ShowUserLogin = true,
            ShowDisplayName = true,
            ShowTeamViewerSection = false,
            ShowTeamViewer = false,
            AllowLaunchTeamViewer = false,
            DetectAtera = false,
            ShowAteraInGui = false,
            IncludeAteraInReports = false,
            CheckUpdates = false,
            ShowNetworkStatus = true
        },
        Report = new ReportSettings
        {
            IncludeComputerName = true,
            IncludeDomain = true,
            IncludeOperatingSystem = true,
            IncludeIpAddress = true,
            IncludeDnsServers = true,
            IncludeUptime = true,
            IncludeManufacturerModel = true,
            IncludeSerialNumber = true,
            IncludeDeviceType = true,
            IncludeUserLogin = true,
            IncludeDisplayName = true,
            IncludeTeamViewer = false,
            IncludeAtera = false,
            IncludeNetworkStatus = true
        },
        Update = new UpdateSettings
        {
            Enabled = false,
            VersionUrl = string.Empty,
            ShowFooterIndicator = true
        }
    };
}
