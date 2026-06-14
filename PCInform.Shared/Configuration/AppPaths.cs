namespace PCInform.Configuration;

public static class AppPaths
{
    public static string GlobalConfigDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PCInform");

    public static string ConfigFilePath => Path.Combine(GlobalConfigDirectory, "appsettings.json");

    public static string UserSettingsDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PCInform");

    public static string UserSettingsFilePath => Path.Combine(UserSettingsDirectory, "settings.json");
}
