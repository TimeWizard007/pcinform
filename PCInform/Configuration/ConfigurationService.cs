using System.Text.Json;
using System.Text.Json.Serialization;
using PCInform.Models;

namespace PCInform.Configuration;

internal static class ConfigurationService
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
        Current = LoadSettings();
    }

    public static AppSettings LoadSettings()
    {
        AppSettings settings;

        if (File.Exists(AppPaths.ConfigFilePath))
        {
            settings = TryLoadFromFile(AppPaths.ConfigFilePath) ?? CreateDefaultSettings();
        }
        else
        {
            settings = CreateDefaultSettings();
            TrySaveSettings(settings, AppPaths.ConfigFilePath);
        }

        return MergeWithDefaults(settings);
    }

    private static AppSettings? TryLoadFromFile(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var json = File.ReadAllText(path);
            var settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions);
            if (settings is not null)
            {
                ApplyLegacySupportFields(json, settings);
            }

            return settings;
        }
        catch
        {
            return null;
        }
    }

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

    private static void TrySaveSettings(AppSettings settings, string path)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            var json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(path, json);
        }
        catch
        {
            // Ignore config write failures (e.g. insufficient permissions).
        }
    }

    private static AppSettings MergeWithDefaults(AppSettings settings)
    {
        var defaults = CreateDefaultSettings();

        settings.Application ??= new ApplicationSettings();
        settings.Support ??= new SupportSettings();
        settings.Features ??= new FeatureSettings();
        settings.Update ??= new UpdateSettings();

        settings.Application.Name = NullIfWhiteSpace(settings.Application.Name) ?? defaults.Application.Name;
        settings.Application.WindowTitle = NullIfWhiteSpace(settings.Application.WindowTitle) ?? defaults.Application.WindowTitle;
        settings.Application.BannerText = NullIfWhiteSpace(settings.Application.BannerText) ?? defaults.Application.BannerText;
        settings.Application.DefaultLanguage = NullIfWhiteSpace(settings.Application.DefaultLanguage) ?? defaults.Application.DefaultLanguage;
        settings.Application.AccentColor = NullIfWhiteSpace(settings.Application.AccentColor) ?? defaults.Application.AccentColor;
        settings.Application.WebsiteUrl = settings.Application.WebsiteUrl?.Trim() ?? defaults.Application.WebsiteUrl;
        NormalizeLanguageSettings(settings.Application);

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

    private static void NormalizeLanguageSettings(ApplicationSettings application)
    {
        if (!application.EnablePolish && !application.EnableEnglish)
        {
            application.EnablePolish = true;
            application.EnableEnglish = false;
        }
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
            CheckUpdates = false
        },
        Update = new UpdateSettings
        {
            Enabled = false,
            VersionUrl = string.Empty
        }
    };
}
