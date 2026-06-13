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
        var settings = TryLoadFromFile(AppPaths.ConfigFilePath)
                       ?? TryLoadFromFile(AppPaths.LocalConfigFilePath)
                       ?? CreateDefaultSettings();

        settings = MergeWithDefaults(settings);

        if (!File.Exists(AppPaths.ConfigFilePath))
        {
            TrySaveSettings(settings, AppPaths.ConfigFilePath);
        }

        return settings;
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
            return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions);
        }
        catch
        {
            return null;
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
            // Ignore config write failures.
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

        settings.Support.CompanyName = NullIfWhiteSpace(settings.Support.CompanyName) ?? defaults.Support.CompanyName;
        settings.Support.Email = settings.Support.Email?.Trim() ?? defaults.Support.Email;
        settings.Support.Phone = settings.Support.Phone?.Trim() ?? defaults.Support.Phone;
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
            AccentColor = "#E87722"
        },
        Support = new SupportSettings
        {
            CompanyName = "Your Company",
            Email = "helpdesk@example.com",
            Phone = "+48 000 000 000",
            EmailSubjectPrefixPl = "Pomoc",
            EmailSubjectPrefixEn = "Support request"
        },
        Features = new FeatureSettings
        {
            ShowTeamViewer = true,
            AllowLaunchTeamViewer = true,
            DetectAtera = true,
            ShowAteraInGui = false,
            IncludeAteraInReports = true,
            CheckUpdates = true
        },
        Update = new UpdateSettings
        {
            Enabled = true,
            VersionUrl = "https://example.com/pcinform/latest/version.json"
        }
    };
}
