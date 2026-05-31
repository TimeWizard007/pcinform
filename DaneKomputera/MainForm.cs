using DaneKomputera.Localization;
using DaneKomputera.Models;
using DaneKomputera.Services;
using DaneKomputera.UI;

namespace DaneKomputera;

internal sealed class MainForm : Form
{
    private readonly Font _labelFont = new("Segoe UI", 9F, FontStyle.Regular);
    private readonly Font _valueFont = new("Segoe UI", 9F, FontStyle.Regular);
    private readonly Font _bannerFont = new("Segoe UI", 14F, FontStyle.Bold);
    private readonly Font _footerFont = new("Segoe UI", 8F, FontStyle.Regular);

    private SystemInfoData _data = new();
    private bool _isLoading;
    private CancellationTokenSource? _loadCts;

    private Panel _contentPanel = null!;
    private Label _loadingLabel = null!;

    private Label _contactSectionLabel = null!;
    private Label _emailCaptionLabel = null!;
    private Label _hotlineCaptionLabel = null!;
    private LinkLabel _emailLink = null!;
    private Label _hotlineValueLabel = null!;

    private Label _computerSectionLabel = null!;
    private Label _computerNameCaption = null!;
    private Label _domainCaption = null!;
    private Label _osCaption = null!;
    private Label _ipCaption = null!;
    private Label _dnsCaption = null!;
    private Label _uptimeCaption = null!;
    private Label _manufacturerCaption = null!;
    private Label _biosCaption = null!;
    private Label _machineTypeCaption = null!;

    private TextBox _computerNameValue = null!;
    private TextBox _domainValue = null!;
    private TextBox _osValue = null!;
    private TextBox _ipValue = null!;
    private TextBox _dnsValue = null!;
    private TextBox _uptimeValue = null!;
    private TextBox _manufacturerValue = null!;
    private TextBox _biosValue = null!;
    private TextBox _machineTypeValue = null!;

    private Label _userSectionLabel = null!;
    private Label _userLoginCaption = null!;
    private Label _userDisplayNameCaption = null!;
    private TextBox _userLoginValue = null!;
    private TextBox _userDisplayNameValue = null!;

    private Label _teamViewerSectionLabel = null!;
    private Label _teamViewerStatusCaption = null!;
    private TextBox _teamViewerStatusValue = null!;
    private Button _launchTeamViewerButton = null!;
    private RoundedPanel _teamViewerPanel = null!;

    private LinkLabel _polishLink = null!;
    private LinkLabel _englishLink = null!;
    private Label _languageSeparator = null!;
    private Label _footerLabel = null!;

    private Button _copyButton = null!;
    private Button _refreshButton = null!;
    private Button _reportButton = null!;
    private Button _closeButton = null!;

    private TextBox[] _dataValueBoxes = null!;

    public MainForm()
    {
        InitializeForm();
        BuildLayout();
        LocalizationManager.LanguageChanged += (_, _) =>
        {
            ApplyLanguageLabels();
            _ = RefreshDataAsync();
        };
        ApplyLanguageLabels();
        Shown += async (_, _) => await RefreshDataAsync();
    }

    private void InitializeForm()
    {
        Text = LocalizationManager.WindowTitle;
        ClientSize = new Size(640, 760);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = true;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = AppTheme.Background;
        Font = new Font("Segoe UI", 9F);
        Icon = LoadWindowIcon();
    }

    private static Icon LoadWindowIcon()
    {
        try
        {
            var iconPath = Path.Combine(AppContext.BaseDirectory, "icon.ico");
            if (File.Exists(iconPath))
            {
                return new Icon(iconPath);
            }

            var associated = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            if (associated is not null)
            {
                return associated;
            }
        }
        catch
        {
            // Fall through.
        }

        return SystemIcons.Information;
    }

    private void BuildLayout()
    {
        var bannerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 56,
            BackColor = AppTheme.BannerBlue
        };

        var accentStrip = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 3,
            BackColor = AppTheme.AccentOrange
        };

        var bannerLabel = new Label
        {
            Text = LocalizationManager.BannerTitle,
            ForeColor = Color.White,
            Font = _bannerFont,
            AutoSize = true,
            Location = new Point(20, 16)
        };

        _polishLink = CreateLanguageLink(LocalizationManager.LanguagePolish, 456);
        _languageSeparator = new Label
        {
            Text = "|",
            ForeColor = Color.White,
            AutoSize = true,
            Font = _labelFont,
            Location = new Point(508, 20)
        };
        _englishLink = CreateLanguageLink(LocalizationManager.LanguageEnglish, 522);

        _polishLink.Click += (_, _) => LocalizationManager.SetLanguage(AppLanguage.Polish);
        _englishLink.Click += (_, _) => LocalizationManager.SetLanguage(AppLanguage.English);

        bannerPanel.Controls.Add(bannerLabel);
        bannerPanel.Controls.Add(_polishLink);
        bannerPanel.Controls.Add(_languageSeparator);
        bannerPanel.Controls.Add(_englishLink);
        bannerPanel.Controls.Add(accentStrip);

        _footerLabel = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 22,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = AppTheme.FooterBackground,
            ForeColor = AppTheme.FooterText,
            Font = _footerFont,
            Text = AppInfoService.FooterText
        };

        var buttonPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 56,
            BackColor = AppTheme.ButtonPanelBackground
        };

        _copyButton = CreatePrimaryButton(LocalizationManager.CopyButton, new Point(16, 12), new Size(175, 32));
        _copyButton.Click += (_, _) => CopyDataToClipboard();

        _refreshButton = CreateSecondaryButton(LocalizationManager.RefreshButton, new Point(200, 12), new Size(90, 32));
        _refreshButton.Click += async (_, _) => await RefreshDataAsync();

        _reportButton = CreateSecondaryButton(LocalizationManager.ReportButton, new Point(298, 12), new Size(120, 32));
        _reportButton.Click += (_, _) => ReportProblem();

        _closeButton = CreateSecondaryButton(LocalizationManager.CloseButton, new Point(524, 12), new Size(100, 32));
        _closeButton.Click += (_, _) => Close();

        buttonPanel.Controls.Add(_copyButton);
        buttonPanel.Controls.Add(_refreshButton);
        buttonPanel.Controls.Add(_reportButton);
        buttonPanel.Controls.Add(_closeButton);

        _contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = AppTheme.Background,
            Padding = new Padding(0, 8, 0, 8)
        };

        _loadingLabel = new Label
        {
            AutoSize = true,
            ForeColor = AppTheme.AccentOrange,
            Font = new Font("Segoe UI", 9F, FontStyle.Italic),
            Location = new Point(20, 0),
            Visible = false
        };
        _contentPanel.Controls.Add(_loadingLabel);

        var y = 24;

        _contactSectionLabel = AddSectionHeader(_contentPanel, LocalizationManager.ContactSection, ref y);
        var contactPanel = CreateSectionPanel(_contentPanel, y, 72);
        y += 84;

        _emailCaptionLabel = CreateCaptionLabel(LocalizationManager.EmailLabel);
        _emailCaptionLabel.Location = new Point(12, 14);
        _emailLink = new LinkLabel
        {
            Text = "helpdesk@itsolution.pl",
            AutoSize = true,
            Location = new Point(88, 14),
            Font = _valueFont,
            LinkColor = AppTheme.BannerBlue,
            ActiveLinkColor = AppTheme.AccentOrange,
            VisitedLinkColor = AppTheme.BannerBlue,
            Cursor = Cursors.Hand
        };
        _emailLink.Click += (_, _) => TryOpenEmail(MailHelper.TryOpenHelpdeskEmail);

        _hotlineCaptionLabel = CreateCaptionLabel(LocalizationManager.HotlineLabel);
        _hotlineCaptionLabel.Location = new Point(12, 42);
        _hotlineValueLabel = new Label
        {
            Text = "+48 22 612 63 60",
            AutoSize = true,
            Location = new Point(88, 42),
            Font = _valueFont,
            ForeColor = AppTheme.ValueText
        };

        contactPanel.Controls.Add(_emailCaptionLabel);
        contactPanel.Controls.Add(_emailLink);
        contactPanel.Controls.Add(_hotlineCaptionLabel);
        contactPanel.Controls.Add(_hotlineValueLabel);

        _computerSectionLabel = AddSectionHeader(_contentPanel, LocalizationManager.ComputerDataSection, ref y);
        var computerPanel = CreateSectionPanel(_contentPanel, y, 272);
        y += 284;

        var computerTable = CreateFieldTable(computerPanel, 9);
        _computerNameCaption = computerTable.Captions[0];
        _domainCaption = computerTable.Captions[1];
        _osCaption = computerTable.Captions[2];
        _ipCaption = computerTable.Captions[3];
        _dnsCaption = computerTable.Captions[4];
        _uptimeCaption = computerTable.Captions[5];
        _manufacturerCaption = computerTable.Captions[6];
        _biosCaption = computerTable.Captions[7];
        _machineTypeCaption = computerTable.Captions[8];

        _computerNameValue = computerTable.Values[0];
        _domainValue = computerTable.Values[1];
        _osValue = computerTable.Values[2];
        _ipValue = computerTable.Values[3];
        _dnsValue = computerTable.Values[4];
        _uptimeValue = computerTable.Values[5];
        _manufacturerValue = computerTable.Values[6];
        _biosValue = computerTable.Values[7];
        _machineTypeValue = computerTable.Values[8];

        _userSectionLabel = AddSectionHeader(_contentPanel, LocalizationManager.UserDataSection, ref y);
        var userPanel = CreateSectionPanel(_contentPanel, y, 78);
        y += 90;

        var userTable = CreateFieldTable(userPanel, 2);
        _userLoginCaption = userTable.Captions[0];
        _userDisplayNameCaption = userTable.Captions[1];
        _userLoginValue = userTable.Values[0];
        _userDisplayNameValue = userTable.Values[1];

        _teamViewerSectionLabel = AddSectionHeader(_contentPanel, LocalizationManager.TeamViewerSection, ref y);
        _teamViewerPanel = CreateSectionPanel(_contentPanel, y, 52);
        y += 64;

        var teamViewerTable = CreateFieldTable(_teamViewerPanel, 1);
        _teamViewerStatusCaption = teamViewerTable.Captions[0];
        _teamViewerStatusValue = teamViewerTable.Values[0];

        _launchTeamViewerButton = CreateAccentButton(LocalizationManager.LaunchTeamViewerButton, new Point(12, 34), new Size(170, 28));
        _launchTeamViewerButton.Visible = false;
        _launchTeamViewerButton.Click += (_, _) => LaunchTeamViewer();
        _teamViewerPanel.Controls.Add(_launchTeamViewerButton);

        _dataValueBoxes =
        [
            _computerNameValue, _domainValue, _osValue, _ipValue, _dnsValue, _uptimeValue,
            _manufacturerValue, _biosValue, _machineTypeValue,
            _userLoginValue, _userDisplayNameValue, _teamViewerStatusValue
        ];

        _contentPanel.AutoScrollMinSize = new Size(0, y);

        Controls.Add(_contentPanel);
        Controls.Add(buttonPanel);
        Controls.Add(_footerLabel);
        Controls.Add(bannerPanel);
    }

    private LinkLabel CreateLanguageLink(string text, int x)
    {
        return new LinkLabel
        {
            Text = text,
            AutoSize = true,
            Location = new Point(x, 20),
            Font = _labelFont,
            LinkColor = Color.White,
            ActiveLinkColor = AppTheme.AccentOrangeLight,
            VisitedLinkColor = Color.White,
            LinkBehavior = LinkBehavior.HoverUnderline,
            Cursor = Cursors.Hand
        };
    }

    private static Label AddSectionHeader(Panel parent, string text, ref int y)
    {
        var header = new Label
        {
            Text = text,
            AutoSize = true,
            Location = new Point(20, y),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = AppTheme.BannerBlue
        };
        parent.Controls.Add(header);
        y += 28;
        return header;
    }

    private static RoundedPanel CreateSectionPanel(Panel parent, int y, int height)
    {
        var panel = new RoundedPanel
        {
            Location = new Point(16, y),
            Size = new Size(592, height)
        };
        parent.Controls.Add(panel);
        return panel;
    }

    private Label CreateCaptionLabel(string text)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            Font = _labelFont,
            ForeColor = AppTheme.LabelText
        };
    }

    private (Label[] Captions, TextBox[] Values) CreateFieldTable(Panel parent, int rowCount)
    {
        var table = new TableLayoutPanel
        {
            Location = new Point(0, 0),
            Size = new Size(568, rowCount * 26),
            ColumnCount = 2,
            RowCount = rowCount,
            AutoSize = true
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        var captions = new Label[rowCount];
        var values = new TextBox[rowCount];

        for (var i = 0; i < rowCount; i++)
        {
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            captions[i] = new Label
            {
                Text = string.Empty,
                AutoSize = true,
                Margin = new Padding(0, 4, 8, 4),
                Font = _labelFont,
                ForeColor = AppTheme.LabelText,
                Anchor = AnchorStyles.Left
            };

            values[i] = new TextBox
            {
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = AppTheme.PanelBackground,
                ForeColor = AppTheme.ValueText,
                Font = _valueFont,
                Margin = new Padding(0, 4, 0, 4),
                Dock = DockStyle.Fill,
                TabStop = false
            };

            table.Controls.Add(captions[i], 0, i);
            table.Controls.Add(values[i], 1, i);
        }

        parent.Controls.Add(table);
        return (captions, values);
    }

    private Button CreatePrimaryButton(string text, Point location, Size size)
    {
        var button = new Button
        {
            Text = text,
            Location = location,
            Size = size,
            FlatStyle = FlatStyle.Flat,
            BackColor = AppTheme.BannerBlue,
            ForeColor = Color.White,
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        WireButtonHover(button, AppTheme.BannerBlue, AppTheme.AccentOrange);
        return button;
    }

    private Button CreateAccentButton(string text, Point location, Size size)
    {
        var button = new Button
        {
            Text = text,
            Location = location,
            Size = size,
            FlatStyle = FlatStyle.Flat,
            BackColor = AppTheme.AccentOrange,
            ForeColor = Color.White,
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        WireButtonHover(button, AppTheme.AccentOrange, AppTheme.AccentOrangeHover);
        return button;
    }

    private static Button CreateSecondaryButton(string text, Point location, Size size)
    {
        var button = new Button
        {
            Text = text,
            Location = location,
            Size = size,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(60, 65, 70),
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderColor = AppTheme.SecondaryButtonBorder;
        WireButtonHover(button, Color.White, AppTheme.AccentOrangeLight, Color.FromArgb(60, 65, 70), AppTheme.AccentOrange);
        return button;
    }

    private static void WireButtonHover(
        Button button,
        Color normalBack,
        Color hoverBack,
        Color? normalFore = null,
        Color? hoverFore = null)
    {
        var defaultFore = normalFore ?? button.ForeColor;
        var hoverForeground = hoverFore ?? button.ForeColor;

        button.MouseEnter += (_, _) =>
        {
            button.BackColor = hoverBack;
            button.ForeColor = hoverForeground;
        };
        button.MouseLeave += (_, _) =>
        {
            button.BackColor = normalBack;
            button.ForeColor = defaultFore;
        };
    }

    private async Task RefreshDataAsync()
    {
        if (_isLoading)
        {
            _loadCts?.Cancel();
        }

        _loadCts = new CancellationTokenSource();
        var token = _loadCts.Token;
        _isLoading = true;

        SetLoadingState(true);
        SetActionButtonsEnabled(false);

        try
        {
            var language = LocalizationManager.CurrentLanguage;
            var data = await Task.Run(() => SystemInfoService.Collect(language), token);

            if (token.IsCancellationRequested || IsDisposed)
            {
                return;
            }

            _data = data;
            UpdateValues();
        }
        catch (OperationCanceledException)
        {
            // Ignore cancelled refresh.
        }
        catch
        {
            if (!IsDisposed)
            {
                ShowLoadingError();
            }
        }
        finally
        {
            if (!IsDisposed)
            {
                SetLoadingState(false);
                SetActionButtonsEnabled(true);
                _isLoading = false;
            }
        }
    }

    private void SetLoadingState(bool loading)
    {
        _loadingLabel.Text = LocalizationManager.LoadingText;
        _loadingLabel.Visible = loading;

        var text = loading ? LocalizationManager.LoadingText : string.Empty;
        foreach (var box in _dataValueBoxes)
        {
            if (loading)
            {
                box.Text = text;
                box.ForeColor = Color.FromArgb(120, 125, 130);
            }
            else
            {
                box.ForeColor = AppTheme.ValueText;
            }
        }
    }

    private void ShowLoadingError()
    {
        var noData = LocalizationManager.NoData;
        foreach (var box in _dataValueBoxes)
        {
            box.Text = noData;
        }
    }

    private void SetActionButtonsEnabled(bool enabled)
    {
        _copyButton.Enabled = enabled;
        _refreshButton.Enabled = enabled;
        _reportButton.Enabled = enabled;
        _launchTeamViewerButton.Enabled = enabled && _data.TeamViewerInstalled;
    }

    private void UpdateValues()
    {
        _computerNameValue.Text = _data.ComputerName;
        _domainValue.Text = _data.Domain;
        _osValue.Text = _data.OperatingSystem;
        _ipValue.Text = _data.IpAddress;
        _dnsValue.Text = _data.DnsServers;
        _uptimeValue.Text = _data.Uptime;
        _manufacturerValue.Text = _data.ManufacturerModel;
        _biosValue.Text = _data.BiosSerial;
        _machineTypeValue.Text = _data.MachineType;
        _userLoginValue.Text = _data.UserLogin;
        _userDisplayNameValue.Text = _data.UserDisplayName;
        _teamViewerStatusValue.Text = LocalizationManager.GetTeamViewerStatus(_data.TeamViewerInstalled);

        var showTeamViewerButton = _data.TeamViewerInstalled && !string.IsNullOrEmpty(_data.TeamViewerPath);
        _launchTeamViewerButton.Visible = showTeamViewerButton;
        _teamViewerPanel.Height = showTeamViewerButton ? 88 : 52;
    }

    private void LaunchTeamViewer()
    {
        if (string.IsNullOrEmpty(_data.TeamViewerPath))
        {
            return;
        }

        try
        {
            SystemInfoService.LaunchTeamViewer(_data.TeamViewerPath);
        }
        catch
        {
            MessageBox.Show(
                LocalizationManager.LaunchTeamViewerError,
                LocalizationManager.WindowTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void ApplyLanguageLabels()
    {
        Text = LocalizationManager.WindowTitle;
        _footerLabel.Text = AppInfoService.FooterText;

        _contactSectionLabel.Text = LocalizationManager.ContactSection;
        _emailCaptionLabel.Text = LocalizationManager.EmailLabel;
        _hotlineCaptionLabel.Text = LocalizationManager.HotlineLabel;

        _computerSectionLabel.Text = LocalizationManager.ComputerDataSection;
        _computerNameCaption.Text = LocalizationManager.ComputerNameLabel;
        _domainCaption.Text = LocalizationManager.DomainLabel;
        _osCaption.Text = LocalizationManager.OperatingSystemLabel;
        _ipCaption.Text = LocalizationManager.IpAddressLabel;
        _dnsCaption.Text = LocalizationManager.DnsLabel;
        _uptimeCaption.Text = LocalizationManager.UptimeLabel;
        _manufacturerCaption.Text = LocalizationManager.ManufacturerLabel;
        _biosCaption.Text = LocalizationManager.BiosSerialLabel;
        _machineTypeCaption.Text = LocalizationManager.MachineTypeLabel;

        _userSectionLabel.Text = LocalizationManager.UserDataSection;
        _userLoginCaption.Text = LocalizationManager.UserLoginLabel;
        _userDisplayNameCaption.Text = LocalizationManager.UserDisplayNameLabel;

        _teamViewerSectionLabel.Text = LocalizationManager.TeamViewerSection;
        _teamViewerStatusCaption.Text = LocalizationManager.TeamViewerStatusLabel;
        _launchTeamViewerButton.Text = LocalizationManager.LaunchTeamViewerButton;

        _copyButton.Text = LocalizationManager.CopyButton;
        _refreshButton.Text = LocalizationManager.RefreshButton;
        _reportButton.Text = LocalizationManager.ReportButton;
        _closeButton.Text = LocalizationManager.CloseButton;

        _polishLink.Text = LocalizationManager.LanguagePolish;
        _englishLink.Text = LocalizationManager.LanguageEnglish;

        HighlightActiveLanguage();

        if (!_isLoading)
        {
            _teamViewerStatusValue.Text = LocalizationManager.GetTeamViewerStatus(_data.TeamViewerInstalled);
        }
    }

    private void HighlightActiveLanguage()
    {
        var active = AppTheme.AccentOrangeLight;
        var inactive = Color.White;

        _polishLink.LinkColor = LocalizationManager.CurrentLanguage == AppLanguage.Polish ? active : inactive;
        _englishLink.LinkColor = LocalizationManager.CurrentLanguage == AppLanguage.English ? active : inactive;
    }

    private void CopyDataToClipboard()
    {
        if (_isLoading)
        {
            return;
        }

        try
        {
            var text = ReportFormatter.FormatClipboard(_data, LocalizationManager.CurrentLanguage);
            Clipboard.SetText(text);
            MessageBox.Show(
                LocalizationManager.CopySuccessMessage,
                LocalizationManager.CopySuccessTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch
        {
            MessageBox.Show(
                LocalizationManager.CopyErrorMessage,
                LocalizationManager.CopySuccessTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void ReportProblem()
    {
        if (_isLoading)
        {
            return;
        }

        var subject = LocalizationManager.ReportSubject(_data.ComputerName);
        var body = ReportFormatter.FormatReportEmail(_data, LocalizationManager.CurrentLanguage);
        TryOpenEmail(() => MailHelper.TryOpenReport(subject, body));
    }

    private void TryOpenEmail(Func<bool> action)
    {
        if (!action())
        {
            MessageBox.Show(
                LocalizationManager.MailErrorMessage,
                LocalizationManager.WindowTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        base.OnFormClosed(e);
    }
}
