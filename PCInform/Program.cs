namespace PCInform;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Configuration.ConfigurationService.Initialize();
        UI.AppTheme.Initialize(Configuration.ConfigurationService.Current.Application.AccentColor);
        Localization.LocalizationManager.Initialize(Services.SettingsService.LoadInitialLanguage());
        Application.Run(new MainForm());
    }
}
