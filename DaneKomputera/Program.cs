namespace DaneKomputera;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        var language = Services.SettingsService.LoadLanguage();
        Localization.LocalizationManager.Initialize(language);
        Application.Run(new MainForm());
    }
}
