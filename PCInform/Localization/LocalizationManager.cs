using PCInform.Configuration;

namespace PCInform.Localization;

internal static class LocalizationManager
{
    public static AppLanguage CurrentLanguage { get; private set; } = AppLanguage.Polish;

    public static event EventHandler? LanguageChanged;

    public static void Initialize(AppLanguage language)
    {
        CurrentLanguage = ResolveLanguage(language);
    }

    public static void SetLanguage(AppLanguage language)
    {
        language = ResolveLanguage(language);
        if (CurrentLanguage == language)
        {
            return;
        }

        CurrentLanguage = language;
        Services.SettingsService.SaveLanguage(language);
        LanguageChanged?.Invoke(null, EventArgs.Empty);
    }

    public static bool IsLanguageSwitchVisible =>
        ConfigurationService.Current.Application.EnablePolish &&
        ConfigurationService.Current.Application.EnableEnglish;

    public static AppLanguage ResolveLanguage(AppLanguage preferred)
    {
        var application = ConfigurationService.Current.Application;
        if (!application.EnablePolish && !application.EnableEnglish)
        {
            return AppLanguage.Polish;
        }

        if (application.EnablePolish && !application.EnableEnglish)
        {
            return AppLanguage.Polish;
        }

        if (!application.EnablePolish && application.EnableEnglish)
        {
            return AppLanguage.English;
        }

        if (preferred == AppLanguage.English && application.EnableEnglish)
        {
            return AppLanguage.English;
        }

        if (preferred == AppLanguage.Polish && application.EnablePolish)
        {
            return AppLanguage.Polish;
        }

        return application.EnablePolish ? AppLanguage.Polish : AppLanguage.English;
    }

    public static AppLanguage ResolveLanguage(string? code) =>
        ResolveLanguage(ParseLanguage(code));

    public static AppLanguage ParseLanguage(string? code) => code?.ToLowerInvariant() switch
    {
        "en" or "english" => AppLanguage.English,
        _ => AppLanguage.Polish
    };

    public static string NoData => CurrentLanguage == AppLanguage.Polish ? "brak danych" : "no data";
    public static string LoadingText => CurrentLanguage == AppLanguage.Polish ? "Ładowanie..." : "Loading...";

    public static string WindowTitle => ConfigurationService.Current.Application.WindowTitle;
    public static string BannerTitle => ConfigurationService.Current.Application.BannerText;

    public static string ContactSection
    {
        get
        {
            var support = ConfigurationService.Current.Support;
            if (!support.ShowCompanyName || string.IsNullOrWhiteSpace(support.CompanyName))
            {
                return CurrentLanguage == AppLanguage.Polish ? "Kontakt" : "Contact";
            }

            return CurrentLanguage == AppLanguage.Polish
                ? $"Kontakt — {support.CompanyName}"
                : $"Contact — {support.CompanyName}";
        }
    }

    public static string EmailLabel => CurrentLanguage == AppLanguage.Polish ? "E-mail:" : "Email:";
    public static string HotlineLabel => CurrentLanguage == AppLanguage.Polish ? "Infolinia:" : "Phone:";
    public static string MobilePhoneLabel => CurrentLanguage == AppLanguage.Polish ? "Telefon komórkowy:" : "Mobile phone:";
    public static string WebsiteLabel => CurrentLanguage == AppLanguage.Polish ? "Strona WWW:" : "Website:";

    public static string ComputerDataSection => CurrentLanguage == AppLanguage.Polish
        ? "Dane komputera"
        : "Computer data";

    public static string ComputerNameLabel => CurrentLanguage == AppLanguage.Polish
        ? "Nazwa komputera:"
        : "Computer name:";

    public static string DomainLabel => CurrentLanguage == AppLanguage.Polish
        ? "Domena:"
        : "Active Directory domain:";

    public static string OperatingSystemLabel => CurrentLanguage == AppLanguage.Polish
        ? "System:"
        : "Operating system:";

    public static string IpAddressLabel => CurrentLanguage == AppLanguage.Polish ? "Adres IP:" : "IP address:";
    public static string DnsLabel => CurrentLanguage == AppLanguage.Polish ? "DNS:" : "DNS servers:";
    public static string UptimeLabel => CurrentLanguage == AppLanguage.Polish ? "Czas pracy:" : "Uptime:";

    public static string ManufacturerLabel => CurrentLanguage == AppLanguage.Polish
        ? "Producent/model:"
        : "Manufacturer and model:";

    public static string BiosSerialLabel => CurrentLanguage == AppLanguage.Polish
        ? "Numer seryjny:"
        : "BIOS serial number:";

    public static string MachineTypeLabel => CurrentLanguage == AppLanguage.Polish
        ? "Typ urządzenia:"
        : "Device type:";

    public static string UserDataSection => CurrentLanguage == AppLanguage.Polish ? "Twoje dane" : "Your data";
    public static string UserLoginLabel => CurrentLanguage == AppLanguage.Polish ? "Login użytkownika:" : "User login:";
    public static string UserDisplayNameLabel => CurrentLanguage == AppLanguage.Polish ? "Nazwa użytkownika:" : "Display name:";

    public static string TeamViewerSection => "TeamViewer";
    public static string TeamViewerStatusLabel => "Status:";

    public static string CopyButton => CurrentLanguage == AppLanguage.Polish
        ? "Kopiuj dane do schowka"
        : "Copy data to clipboard";

    public static string RefreshButton => CurrentLanguage == AppLanguage.Polish ? "Odśwież" : "Refresh";
    public static string ReportButton => CurrentLanguage == AppLanguage.Polish ? "Zgłoś problem" : "Report problem";
    public static string CloseButton => CurrentLanguage == AppLanguage.Polish ? "Zamknij" : "Close";

    public static string LanguagePolish => "Polski";
    public static string LanguageEnglish => "English";

    public static string TeamViewerInstalled => CurrentLanguage == AppLanguage.Polish ? "Zainstalowany" : "Installed";
    public static string TeamViewerNotInstalled => CurrentLanguage == AppLanguage.Polish ? "Nie zainstalowano" : "Not installed";

    public static string CopySuccessTitle => WindowTitle;

    public static string CopySuccessMessage => CurrentLanguage == AppLanguage.Polish
        ? "Dane zostały skopiowane do schowka."
        : "Data has been copied to the clipboard.";

    public static string CopyErrorMessage => CurrentLanguage == AppLanguage.Polish
        ? "Nie udało się skopiować danych do schowka."
        : "Could not copy data to the clipboard.";

    public static string MailErrorMessage => CurrentLanguage == AppLanguage.Polish
        ? "Nie udało się otworzyć klienta poczty"
        : "Unable to open mail client";

    public static string ReportSubject(string computerName)
    {
        var support = ConfigurationService.Current.Support;
        var prefix = CurrentLanguage == AppLanguage.Polish
            ? support.EmailSubjectPrefixPl
            : support.EmailSubjectPrefixEn;
        return $"{prefix} - {computerName}";
    }

    public static string GetTeamViewerStatus(bool installed) =>
        installed ? TeamViewerInstalled : TeamViewerNotInstalled;

    public static string LaunchTeamViewerButton => CurrentLanguage == AppLanguage.Polish
        ? "Uruchom TeamViewer"
        : "Launch TeamViewer";

    public static string LaunchTeamViewerError => CurrentLanguage == AppLanguage.Polish
        ? "Nie udało się uruchomić TeamViewer."
        : "Could not launch TeamViewer.";

    public static string UpdateFooterIndicator => "⬆️";

    public static string UpdateFooterTooltip(string version) =>
        CurrentLanguage == AppLanguage.Polish
            ? $"Dostępna nowa wersja: v{version}\nKliknij, aby otworzyć stronę pobierania."
            : $"New version available: v{version}\nClick to open download page.";

    public static string UpdateAboutAvailable(string version) =>
        CurrentLanguage == AppLanguage.Polish
            ? $"Dostępna nowa wersja: v{version}"
            : $"New version available: v{version}";

    public static string AboutLink => CurrentLanguage == AppLanguage.Polish ? "O aplikacji" : "About";

    public static string AboutDialogTitle => AboutLink;

    public static string AboutDescription => CurrentLanguage == AppLanguage.Polish
        ? "PC Inform umożliwia szybki podgląd informacji o komputerze oraz przygotowanie danych potrzebnych do zgłoszenia serwisowego."
        : "PC Inform provides quick access to computer information and support request details.";

    public static string AboutVersionLabel => CurrentLanguage == AppLanguage.Polish ? "Wersja:" : "Version:";

    public static string AboutAuthorLabel => CurrentLanguage == AppLanguage.Polish ? "Autor:" : "Author:";

    public static string AboutGitHubLabel =>
        CurrentLanguage == AppLanguage.Polish ? "Projekt:" : "Project:";

    public static string AboutLicenseLabel =>
        CurrentLanguage == AppLanguage.Polish ? "Licencja:" : "License:";

    public static string AboutLicenseName => "MIT License";

    public static string AboutLicenseNote => CurrentLanguage == AppLanguage.Polish
        ? "Bezpłatna do użytku prywatnego i komercyjnego."
        : "Free for personal and commercial use.";

    public static string AboutGitHubButton => "GitHub";

    public const string AboutAuthorName = "Michał Watkowski";

    public const string AboutGitHubUrl = "https://github.com/TimeWizard007/pcinform";

    public const string ConfigurationDocUrl = "https://github.com/TimeWizard007/pcinform#configuration";

    public static string ConfigurationLink =>
        CurrentLanguage == AppLanguage.Polish ? "Konfiguracja" : "Configuration";
}
