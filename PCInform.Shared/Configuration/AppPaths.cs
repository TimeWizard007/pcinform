namespace PCInform.Configuration;

public static class AppPaths
{
    public static string GlobalConfigDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PCInform");

    public static string ConfigFilePath => Path.Combine(GlobalConfigDirectory, "appsettings.json");

    public static string LogDirectory => Path.Combine(GlobalConfigDirectory, "Logs");

    public static string LogFilePath => Path.Combine(LogDirectory, "PCInform.log");

    public static string UserSettingsDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PCInform");

    public static string UserSettingsFilePath => Path.Combine(UserSettingsDirectory, "settings.json");
}
