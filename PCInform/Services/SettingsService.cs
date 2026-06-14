using System.Text.Json;
using PCInform.Configuration;
using PCInform.Localization;

namespace PCInform.Services;

internal static class SettingsService
{
    public static AppLanguage LoadLanguage()
    {
        try
        {
            if (!File.Exists(AppPaths.UserSettingsFilePath))
            {
                return LocalizationManager.ResolveLanguage(ConfigurationService.Current.Application.DefaultLanguage);
            }

            var json = File.ReadAllText(AppPaths.UserSettingsFilePath);
            var settings = JsonSerializer.Deserialize<UserSettings>(json);
            return LocalizationManager.ResolveLanguage(settings?.Language);
        }
        catch
        {
            return LocalizationManager.ResolveLanguage(ConfigurationService.Current.Application.DefaultLanguage);
        }
    }

    public static AppLanguage LoadInitialLanguage()
    {
        var preferred = LoadLanguage();
        return LocalizationManager.ResolveLanguage(preferred);
    }

    public static void SaveLanguage(AppLanguage language)
    {
        try
        {
            Directory.CreateDirectory(AppPaths.UserSettingsDirectory);
            var settings = new UserSettings
            {
                Language = language == AppLanguage.English ? "en" : "pl"
            };
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(AppPaths.UserSettingsFilePath, json);
        }
        catch
        {
            // Ignore settings write failures.
        }
    }

    private sealed class UserSettings
    {
        public string Language { get; set; } = "pl";
    }
}
