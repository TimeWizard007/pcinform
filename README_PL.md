[🇬🇧 English](README.md) | 🇵🇱 Polski

# PC Inform

**PC Inform** to lekka aplikacja okienkowa dla Windows, przeznaczona dla help desków i zespołów IT. Zbiera podstawowe informacje o komputerze i użytkowniku oraz ułatwia kopiowanie danych lub przygotowanie wiadomości e-mail do wsparcia — bez wymagania uprawnień administratora do uruchomienia aplikacji.

Obsługiwany jest interfejs po polsku i angielsku. Branding, dane kontaktowe, widoczne pola i opcjonalne zachowania są kontrolowane przez jeden, wspólny dla całej maszyny plik konfiguracyjny.

## Funkcje

- **Informacje o komputerze:** nazwa, domena AD, system operacyjny, IPv4, DNS, czas pracy, producent/model, numer seryjny BIOS, typ urządzenia (każde pole można pokazać lub ukryć)
- **Informacje o użytkowniku:** login i nazwa wyświetlana (opcjonalnie)
- **Sekcja kontaktowa:** konfigurowalny e-mail, telefon, telefon komórkowy i strona WWW
- **Obsługa zgłoszeń:** kopiowanie sformatowanego raportu do schowka, otwieranie wstępnie wypełnionego szkicu e-mail **Zgłoś problem**
- **Integracja z pocztą:** szkic w Outlook Classic, gdy istnieje profil MAPI, z rezerwowym `mailto:`; CC/BCC używane po skonfigurowaniu
- **Lokalizacja:** polski i angielski; przełącznik języka można ograniczyć do jednego języka w konfiguracji
- **Okno O programie:** wersja, opis, autor i link do projektu
- **Opcjonalne powiadomienie o aktualizacji:** administratorzy mogą włączyć informacyjne sprawdzanie wersji zdalnej (domyślnie wyłączone); gdy dostępna jest nowsza wersja, dyskretny wskaźnik **⬆️** pojawia się w stopce — bez wyskakującego okna i bez automatycznej instalacji
- **Status sieci:** opcjonalny wskaźnik w stopce (🌐 / ⚠️) z podpowiedziami Internet i DNS, gdy włączone jest `features.showNetworkStatus`

## Wymagania

- Windows 10 lub 11 (64-bit)
- Standardowe konto użytkownika do **uruchamiania** PC Inform (administrator nie jest wymagany w czasie działania)
- Uprawnienia administratora do **instalacji** lub aktualizacji przez pakiet instalacyjny
- Klient poczty dla **Zgłoś problem** (na przykład Outlook lub dowolna aplikacja zarejestrowana dla `mailto:`)

## Instalacja

### Instalator (zalecane)

1. Pobierz **PCInform-Setup.exe** z [GitHub Releases](https://github.com/TimeWizard007/pcinform/releases).
2. Uruchom instalator **jako administrator**.
3. PC Inform jest instalowany dla wszystkich użytkowników w `C:\Program Files\PCInform\`.
4. Przy pierwszej instalacji instalator tworzy `C:\ProgramData\PCInform\appsettings.json`, jeśli plik jeszcze nie istnieje (z `appsettings.example.json` lub z `appsettings.json` umieszczonego obok instalatora).
5. Istniejąca konfiguracja globalna **nigdy nie jest nadpisywana** podczas aktualizacji.
6. Opcjonalnie utwórz skrót na pulpicie dla wszystkich użytkowników podczas instalacji (skróty w menu Start są zawsze tworzone).

### Użycie przenośne

Możesz uruchomić **PCInform.exe** bezpośrednio. Konfiguracja nadal pochodzi z globalnego pliku opisanego poniżej (utwórz go ręcznie lub pozwól PC Inform utworzyć bezpieczne domyślne ustawienia przy pierwszym uruchomieniu).

## Konfiguracja

PC Inform jest konfigurowany przez **jeden, wspólny dla całej maszyny** plik `appsettings.json`, używany przez wszystkich użytkowników komputera.

### Gdzie przechowywana jest konfiguracja

```
C:\ProgramData\PCInform\appsettings.json
```

Forma ze zmienną środowiskową: `%PROGRAMDATA%\PCInform\appsettings.json`

PC Inform **nie** używa `%LOCALAPPDATA%\PCInform\appsettings.json` ani pliku obok pliku wykonywalnego.

Przy pierwszym uruchomieniu, jeśli brakuje pliku globalnego, PC Inform tworzy folder i domyślny plik z bezpiecznymi ustawieniami publicznymi. Istniejący plik nigdy nie jest modyfikowany automatycznie.

**Preferencja języka** użytkownika jest przechowywana wyłącznie w `%APPDATA%\PCInform\settings.json`.

Pełny przykład: [appsettings.example.json](appsettings.example.json).

### PC Inform Configurator (opcjonalne narzędzie administratora)

**PCInform.Configurator.exe** to oddzielne narzędzie administratora do tworzenia i edycji `C:\ProgramData\PCInform\appsettings.json`. **Nie** jest dołączony do standardowego instalatora dla użytkowników końcowych i **nie** jest częścią aplikacji dla użytkownika końcowego.

Pobierz go osobno z [GitHub Releases](https://github.com/TimeWizard007/pcinform/releases), gdy potrzebujesz graficznego edytora ustawień Application, Support, Features, Report i Update. Konfigurator weryfikuje ustawienia przed zapisem (na przykład co najmniej jeden włączony język oraz `versionUrl`, gdy aktualizacje są włączone).

Użytkownicy końcowi zwykle nie potrzebują konfiguratora — administratorzy IT przygotowują konfigurację wspólną dla maszyny przed wdrożeniem lub po nim, albo edytują `appsettings.json` bezpośrednio.

### Przegląd sekcji JSON

| Sekcja | Cel |
|---------|---------|
| **Application** | Nazwa aplikacji, tytuł okna, tekst banera, domyślny język, kolor akcentu, włączone języki |
| **Support** | Nazwa firmy, e-mail/telefon/telefon komórkowy/strona WWW wsparcia, CC/BCC, prefiksy tematu, flagi widoczności kontaktu |
| **Features** | Widoczność w głównym oknie (`features.show*`), opcjonalna integracja TeamViewer/Atera, wskaźnik statusu sieci w stopce |
| **Report** | Zawartość schowka i e-maila **Zgłoś problem** (`report.include*`) |
| **Update** | Informacyjne sprawdzanie aktualizacji przez zdalny `version.json` i wskaźnik w stopce (domyślnie wyłączone) |

**Wyświetlanie a raport:** flagi `features.show*` kontrolują **wyłącznie widoczność w głównym oknie**. Oddzielne flagi `report.include*` kontrolują **raporty w schowku** i zawartość e-maila **Zgłoś problem**. Możesz pokazać minimalny interfejs, a jednocześnie dołączyć pełną diagnostykę w raportach, lub odwrotnie. Flagi te **nie** zatrzymują wewnętrznego zbierania informacji o systemie przez aplikację.

### Application (`application`)

| Ustawienie | Cel |
|---------|---------|
| `name`, `windowTitle`, `bannerText` | Nazwa aplikacji i tekst banera |
| `defaultLanguage` | Domyślny język interfejsu (`pl` lub `en`), gdy użytkownik nie ma zapisanego wyboru |
| `accentColor` | Kolor akcentu (hex, np. `#E87722`) |
| `enablePolish`, `enableEnglish` | Dostępne języki; jeśli włączony jest tylko jeden, przełącznik języka jest ukryty |

### Support (`support`)

| Ustawienie | Cel |
|---------|---------|
| `companyName` | Nazwa organizacji (używana w tytule sekcji kontaktowej, gdy włączona) |
| `emailTo` | Odbiorca wsparcia dla **Zgłoś problem** i wyświetlany e-mail kontaktowy |
| `emailCc`, `emailBcc` | Opcjonalne CC/BCC w szkicach e-mail wsparcia |
| `emailSubjectPrefixPl`, `emailSubjectPrefixEn` | Prefiks tematu według języka |
| `phone` | Telefon stacjonarny / główny (np. `+48 22 123 45 67`) |
| `mobilePhone` | Telefon komórkowy (np. `+48 500 600 700`) |
| `showCompanyName`, `showEmail`, `showPhone`, `showMobilePhone`, `showWebsite` | Widoczność każdego elementu kontaktu |

**Zachowanie e-maila wsparcia:**

- **Zgłoś problem** tworzy szkic wyłącznie w kliencie poczty użytkownika (bez wysyłki SMTP).
- Adres **From** to zawsze konto pocztowe użytkownika — nie jest konfigurowany w PC Inform.
- Wyłączone pola kontaktu są ukryte w interfejsie, raporcie schowka i treści e-maila.

Etykiety telefonów w interfejsie: po polsku *Infolinia* / *Telefon komórkowy*; po angielsku *Phone* / *Mobile phone*.

### Konfiguracja strony WWW

Adres strony WWW ustawia się w `support.websiteUrl`. Wyświetlanie kontroluje `support.showWebsite`.

- Gdy włączone i ustawiony jest adres URL, sekcja kontaktowa pokazuje klikalny link (po polsku: **Strona WWW**, po angielsku: **Website**).
- Gdy adres URL jest pusty lub `showWebsite` ma wartość `false`, wiersz ze stroną WWW jest ukryty.

`application.websiteUrl` jest dostępny do przyszłego lub niestandardowego użycia; sekcja kontaktowa używa `support.websiteUrl`.

### Features (`features`)

Organizacje mogą pokazać **minimalny** interfejs (baner + kontakt + Zgłoś problem) lub **pełny widok diagnostyczny**, edytując `appsettings.json` — bez ponownej kompilacji.

**Widoczność pól w interfejsie** (domyślnie **true**, o ile nie zaznaczono inaczej):

- Komputer: `showComputerName`, `showDomain`, `showOperatingSystem`, `showIpAddress`, `showDnsServers`, `showUptime`, `showManufacturerModel`, `showSerialNumber`, `showDeviceType`
- Użytkownik: `showUserLogin`, `showDisplayName`
- Stopka: `showNetworkStatus` (wskaźnik statusu sieci; domyślnie **true**)
- Sekcja TeamViewer: `showTeamViewerSection` (domyślnie **false**)

Wyłączone pola `show*` są pomijane **wyłącznie w głównym oknie**.

### Report (`report`)

Kontroluje, które pola pojawiają się w **kopiowanych raportach schowka** i szkicach e-mail **Zgłoś problem** (domyślnie **true**, o ile nie zaznaczono inaczej):

- Komputer/użytkownik: `includeComputerName`, `includeDomain`, `includeOperatingSystem`, `includeIpAddress`, `includeDnsServers`, `includeUptime`, `includeManufacturerModel`, `includeSerialNumber`, `includeDeviceType`, `includeUserLogin`, `includeDisplayName`, `includeNetworkStatus`
- Agenci: `includeTeamViewer`, `includeAtera` (domyślnie **false**)

Podczas aktualizacji ze starszych konfiguracji bez sekcji `report`, PC Inform wyprowadza początkowe flagi raportu z poprzednich ustawień widoczności `features`, aby istniejące wdrożenia zachowały podobną zawartość raportów.

**Opcjonalne integracje** (domyślnie **false** w konfiguracji publicznej):

| Flaga | Cel |
|------|---------|
| `showTeamViewer` | Wykrywanie TeamViewer dla raportów |
| `allowLaunchTeamViewer` | Przycisk uruchomienia, gdy TeamViewer jest zainstalowany |
| `detectAtera` | Wykrywanie agenta Atera |
| `showAteraInGui` | Pokazywanie Atera w interfejsie |
| `includeAteraInReports` | Starsza flaga raportu (zastąpiona przez `report.includeAtera`; zachowana dla kompatybilności) |
| `checkUpdates` | Starsza flaga (zachowana dla kompatybilności; sprawdzanie aktualizacji kontroluje `update.enabled`) |

### Update (`update`)

Sprawdzanie aktualizacji jest **domyślnie wyłączone** i ma **wyłącznie charakter informacyjny**. PC Inform nie pobiera ani nie instaluje aktualizacji automatycznie i nie pokazuje wyskakującego okna przy starcie.

Po włączeniu PC Inform sprawdza `versionUrl` cicho w tle. Jeśli dostępna jest nowsza wersja i `showFooterIndicator` ma wartość **true** (domyślnie), dyskretny wskaźnik **⬆️** pojawia się w stopce. Kliknięcie otwiera adres pobierania w domyślnej przeglądarce (lub stronę projektu na GitHub, jeśli nie ustawiono adresu URL). Okno O programie również pokazuje krótką informację, gdy dostępna jest aktualizacja.

| Ustawienie | Cel |
|---------|---------|
| `enabled` | Główny przełącznik sprawdzania aktualizacji w tle (domyślnie `false`) |
| `versionUrl` | Adres URL zdalnego `version.json` (domyślnie pusty) |
| `showFooterIndicator` | Pokazuj **⬆️** w stopce, gdy dostępna jest nowsza wersja (domyślnie `true`) |

Przykładowy format zdalnych metadanych: [docs/version.example.json](docs/version.example.json).

Administratorzy, którzy włączają aktualizacje, muszą hostować `version.json` i wskazać go w `versionUrl`. Użytkownicy końcowi zwykle nie powinni zmieniać ustawień aktualizacji.

Logi diagnostyczne (gdy zapisywalne): `C:\ProgramData\PCInform\Logs\PCInform.log`

## GitHub Releases

Oficjalne buildy są publikowane pod adresem:

**https://github.com/TimeWizard007/pcinform/releases**

Typowe zasoby:

- **PCInform-Setup.exe** — zalecany instalator dla użytkownika końcowego (nie zawiera konfiguratora)
- **PCInform.Configurator.exe** — opcjonalny edytor konfiguracji administratora (pobierz osobno, gdy potrzebny)
- **PCInform.exe** — opcjonalny plik binarny przenośny (gdy udostępniony)
- **version.json** — opcjonalne metadane dla wdrożeń z włączonym sprawdzaniem aktualizacji

Pobierz instalator z najnowszego wydania, o ile zespół IT nie dostarcza wewnętrznego pakietu.

## Uwagi dotyczące bezpieczeństwa

- W aplikacji nie są osadzone żadne sekrety ani prywatne adresy URL.
- Sprawdzanie aktualizacji ma wyłącznie charakter informacyjny: wskaźnik w stopce i informacja w oknie O programie — bez automatycznej instalacji i bez wyskakującego okna przy starcie
- Opcjonalny wskaźnik statusu sieci w stopce (podpowiedzi Internet/DNS)
- Konfiguracja firmy, wsparcia i funkcji jest wspólna dla maszyny; tylko preferencja języka interfejsu jest per użytkownik.

## Licencja

Ten projekt jest licencjonowany na warunkach [MIT License](LICENSE).

## Dla deweloperów

Instrukcje budowania, układ projektu i proces wydawania są opisane w [docs/DEVELOPER.md](docs/DEVELOPER.md) i [docs/RELEASE_PROCESS.md](docs/RELEASE_PROCESS.md).
