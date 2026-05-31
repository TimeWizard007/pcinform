using System.Text.Json;
using DaneKomputera.Localization;

namespace DaneKomputera.Services;

internal static class SettingsService
{
    private static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "DaneKomputera");

    private static readonly string SettingsFilePath = Path.Combine(SettingsDirectory, "settings.json");

    public static AppLanguage LoadLanguage()
    {
        try
        {
            if (!File.Exists(SettingsFilePath))
            {
                return AppLanguage.Polish;
            }

            var json = File.ReadAllText(SettingsFilePath);
            var settings = JsonSerializer.Deserialize<UserSettings>(json);
            return settings?.Language?.ToLowerInvariant() switch
            {
                "en" or "english" => AppLanguage.English,
                "pl" or "polish" or "polski" => AppLanguage.Polish,
                _ => AppLanguage.Polish
            };
        }
        catch
        {
            return AppLanguage.Polish;
        }
    }

    public static void SaveLanguage(AppLanguage language)
    {
        try
        {
            Directory.CreateDirectory(SettingsDirectory);
            var settings = new UserSettings
            {
                Language = language == AppLanguage.English ? "en" : "pl"
            };
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFilePath, json);
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
