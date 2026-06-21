namespace PCInform;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Configuration.AppDiagnosticLog.Initialize();
        Configuration.AppDiagnosticLog.Write("PC Inform process started");

        ApplicationConfiguration.Initialize();
        Configuration.ConfigurationService.Initialize();
        UI.AppTheme.Initialize(Configuration.ConfigurationService.Current.Application.AccentColor);
        Localization.LocalizationManager.Initialize(Services.SettingsService.LoadInitialLanguage());
        Application.Run(new MainForm());
    }
}
