namespace PCInform.Configuration;

internal static class AppPaths
{
    /// <summary>
    /// Machine-wide configuration directory (%PROGRAMDATA%\PCInform).
    /// </summary>
    public static string GlobalConfigDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PCInform");

    /// <summary>
    /// Machine-wide appsettings.json (%PROGRAMDATA%\PCInform\appsettings.json).
    /// </summary>
    public static string ConfigFilePath => Path.Combine(GlobalConfigDirectory, "appsettings.json");

    /// <summary>
    /// Per-user preferences only (language).
    /// </summary>
    public static string UserSettingsDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PCInform");

    public static string UserSettingsFilePath => Path.Combine(UserSettingsDirectory, "settings.json");
}
