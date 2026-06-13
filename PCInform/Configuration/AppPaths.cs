namespace PCInform.Configuration;

internal static class AppPaths
{
    public static string ApplicationDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PCInform");

    public static string ConfigFilePath => Path.Combine(ApplicationDirectory, "appsettings.json");

    public static string UserSettingsDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PCInform");

    public static string UserSettingsFilePath => Path.Combine(UserSettingsDirectory, "settings.json");

    public static string LocalConfigFilePath =>
        Path.Combine(AppContext.BaseDirectory, "appsettings.json");
}
