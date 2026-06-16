using System.Diagnostics;
using PCInform.Configuration;
using PCInform.Localization;
using PCInform.Models;
using PCInform.Services;
using PCInform.UI;

namespace PCInform;

internal sealed class MainForm : Form
{
    private sealed class BoundField
    {
        public required Label Caption { get; init; }
        public required TextBox Value { get; init; }
        public required Func<string> GetLabel { get; init; }
        public required Action<SystemInfoData> ApplyValue { get; init; }
    }

    private readonly Font _labelFont = new("Segoe UI", 9F, FontStyle.Regular);
    private readonly Font _valueFont = new("Segoe UI", 9F, FontStyle.Regular);
    private readonly Font _bannerFont = new("Segoe UI", 14F, FontStyle.Bold);
    private readonly Font _footerFont = new("Segoe UI", 8F, FontStyle.Regular);

    private SystemInfoData _data = new();
    private bool _isLoading;
    private CancellationTokenSource? _loadCts;

    private Panel _contentPanel = null!;
    private Label _loadingLabel = null!;
    private Label _bannerLabel = null!;

    private Label? _contactSectionLabel;
    private RoundedPanel? _contactPanel;
    private Label? _emailCaptionLabel;
    private Label? _hotlineCaptionLabel;
    private Label? _mobilePhoneCaptionLabel;
    private Label? _websiteCaptionLabel;
    private LinkLabel? _emailLink;
    private Label? _hotlineValueLabel;
    private Label? _mobilePhoneValueLabel;
    private LinkLabel? _websiteLink;

    private Label? _computerSectionLabel;
    private Label? _userSectionLabel;
    private Label? _teamViewerSectionLabel;
    private Label? _teamViewerStatusCaption;
    private TextBox? _teamViewerStatusValue;
    private Button? _launchTeamViewerButton;
    private RoundedPanel? _teamViewerPanel;

    private readonly List<BoundField> _computerFields = [];
    private readonly List<BoundField> _userFields = [];

    private LinkLabel? _polishLink;
    private LinkLabel? _englishLink;
    private Label? _languageSeparator;
    private Label _footerLabel = null!;
    private Label? _networkStatusLabel;
    private LinkLabel? _updateIndicatorLink;
    private FlowLayoutPanel? _footerLeftPanel;
    private ToolTip _footerToolTip = null!;
    private LinkLabel _configLink = null!;
    private LinkLabel _aboutLink = null!;

    private Button _copyButton = null!;
    private Button _refreshButton = null!;
    private Button _reportButton = null!;
    private Button _closeButton = null!;

    private const int TableRowHeight = 36;
    private const int TableRowVerticalPadding = 10;
    private const int TablePanelExtraHeight = 20;
    private const int ContactRowHeight = 32;
    private const int ContactPanelExtraHeight = 26;
    private const int ContactValueLeft = 124;

    private TextBox[] _dataValueBoxes = [];

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
        Shown += OnFormShown;
    }

    private async void OnFormShown(object? sender, EventArgs e)
    {
        await RefreshDataAsync();
        _ = RefreshFooterIndicatorsAsync();
    }

    private async Task RefreshFooterIndicatorsAsync()
    {
        try
        {
            await NetworkStatusService.CheckAsync().ConfigureAwait(true);
            await UpdateService.CheckForUpdatesAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            AppDiagnosticLog.Write($"Footer indicator checks failed: {ex.Message}");
        }

        if (IsDisposed)
        {
            return;
        }

        if (InvokeRequired)
        {
            BeginInvoke(RefreshFooterIndicators);
            return;
        }

        RefreshFooterIndicators();
    }

    private void RefreshFooterIndicators()
    {
        UpdateNetworkStatusIndicator();
        UpdateFooterIndicator();
        _footerLeftPanel?.PerformLayout();
        _footerLeftPanel?.Invalidate();
        AppDiagnosticLog.Write($"Footer network indicator visible: {_networkStatusLabel?.Visible == true}, text: '{_networkStatusLabel?.Text}'");
        AppDiagnosticLog.Write($"Footer update indicator visible: {_updateIndicatorLink?.Visible == true}, text: '{_updateIndicatorLink?.Text}'");
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
        var support = ConfigurationService.Current.Support;
        var features = ConfigurationService.Current.Features;
        var showLanguageSwitch = LocalizationManager.IsLanguageSwitchVisible;
        var hasReportEmail = !string.IsNullOrWhiteSpace(support.EmailTo);

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
            BackColor = AppTheme.Accent
        };

        _bannerLabel = new Label
        {
            Text = LocalizationManager.BannerTitle,
            ForeColor = Color.White,
            Font = _bannerFont,
            AutoSize = true,
            Location = new Point(20, 16)
        };

        bannerPanel.Controls.Add(_bannerLabel);

        if (showLanguageSwitch)
        {
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
            bannerPanel.Controls.Add(_polishLink);
            bannerPanel.Controls.Add(_languageSeparator);
            bannerPanel.Controls.Add(_englishLink);
        }

        bannerPanel.Controls.Add(accentStrip);

        var showNetworkStatus = features.ShowNetworkStatus;
        var footerIndicatorFont = CreateFooterIndicatorFont();

        _footerLabel = new Label
        {
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.FooterBackground,
            ForeColor = AppTheme.FooterText,
            Font = _footerFont,
            Text = AppInfoService.FooterText,
            Margin = new Padding(0, 4, 6, 4)
        };

        _networkStatusLabel = new Label
        {
            AutoSize = false,
            Size = new Size(22, 18),
            Visible = showNetworkStatus,
            Font = footerIndicatorFont,
            ForeColor = AppTheme.FooterText,
            Margin = new Padding(4, 3, 4, 3),
            TextAlign = ContentAlignment.MiddleCenter,
            Text = LocalizationManager.NetworkStatusOnlineIndicator
        };

        _updateIndicatorLink = new LinkLabel
        {
            AutoSize = false,
            Size = new Size(22, 18),
            Text = LocalizationManager.UpdateFooterIndicator,
            Visible = false,
            LinkColor = AppTheme.Accent,
            ActiveLinkColor = AppTheme.BannerBlue,
            VisitedLinkColor = AppTheme.Accent,
            Font = footerIndicatorFont,
            Cursor = Cursors.Hand,
            LinkBehavior = LinkBehavior.HoverUnderline,
            Margin = new Padding(4, 3, 4, 3),
            TextAlign = ContentAlignment.MiddleCenter
        };
        _updateIndicatorLink.Click += (_, _) => UpdateService.OpenDownloadPage();

        _footerToolTip = new ToolTip();

        _footerLeftPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = false,
            BackColor = AppTheme.FooterBackground,
            Padding = new Padding(12, 0, 0, 0)
        };
        _footerLeftPanel.Controls.Add(_footerLabel);
        _footerLeftPanel.Controls.Add(_networkStatusLabel);
        _footerLeftPanel.Controls.Add(_updateIndicatorLink);

        AppDiagnosticLog.Write("Footer controls created");
        AppDiagnosticLog.Write($"Footer network control added, initial visible: {_networkStatusLabel.Visible}");
        AppDiagnosticLog.Write($"Footer update control added, initial visible: {_updateIndicatorLink.Visible}");

        _aboutLink = new LinkLabel
        {
            Text = LocalizationManager.AboutLink,
            AutoSize = true,
            LinkColor = AppTheme.BannerBlue,
            ActiveLinkColor = AppTheme.Accent,
            VisitedLinkColor = AppTheme.BannerBlue,
            Font = _footerFont,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 0, 0)
        };
        _aboutLink.Click += (_, _) => ShowAboutDialog();

        _configLink = new LinkLabel
        {
            Text = LocalizationManager.ConfigurationLink,
            AutoSize = true,
            LinkColor = AppTheme.BannerBlue,
            ActiveLinkColor = AppTheme.Accent,
            VisitedLinkColor = AppTheme.BannerBlue,
            Font = _footerFont,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 6, 0)
        };
        _configLink.LinkClicked += (_, _) => OpenConfigurationDoc();

        var footerSeparator = new Label
        {
            Text = "|",
            AutoSize = true,
            ForeColor = AppTheme.FooterText,
            Font = _footerFont,
            Margin = new Padding(0, 0, 6, 0)
        };

        var footerLinksPanel = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = AppTheme.FooterBackground,
            Margin = new Padding(0, 2, 12, 0)
        };
        footerLinksPanel.Controls.Add(_configLink);
        footerLinksPanel.Controls.Add(footerSeparator);
        footerLinksPanel.Controls.Add(_aboutLink);

        var footerPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 28,
            BackColor = AppTheme.FooterBackground,
            ColumnCount = 2,
            RowCount = 1
        };
        footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        footerPanel.Controls.Add(_footerLeftPanel, 0, 0);
        footerPanel.Controls.Add(footerLinksPanel, 1, 0);
        AppDiagnosticLog.Write("Footer panel assembled");

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
        _reportButton.Enabled = hasReportEmail;

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
            ForeColor = AppTheme.Accent,
            Font = new Font("Segoe UI", 9F, FontStyle.Italic),
            Location = new Point(20, 0),
            Visible = false
        };
        _contentPanel.Controls.Add(_loadingLabel);

        var y = 24;
        BuildContactSection(support, ref y);
        BuildComputerSection(features, ref y);
        BuildUserSection(features, ref y);
        BuildTeamViewerSection(features, ref y);

        var valueBoxes = new List<TextBox>();
        valueBoxes.AddRange(_computerFields.Select(f => f.Value));
        valueBoxes.AddRange(_userFields.Select(f => f.Value));
        if (_teamViewerStatusValue is not null)
        {
            valueBoxes.Add(_teamViewerStatusValue);
        }

        _dataValueBoxes = valueBoxes.ToArray();
        _contentPanel.AutoScrollMinSize = new Size(0, y);

        Controls.Add(_contentPanel);
        Controls.Add(buttonPanel);
        Controls.Add(footerPanel);
        Controls.Add(bannerPanel);
    }

    private void BuildContactSection(SupportSettings support, ref int y)
    {
        if (!VisibilityHelper.IsContactSectionVisible(support))
        {
            return;
        }

        var rowCount =
            (VisibilityHelper.IsEmailVisible(support) ? 1 : 0) +
            (VisibilityHelper.IsPhoneVisible(support) ? 1 : 0) +
            (VisibilityHelper.IsMobilePhoneVisible(support) ? 1 : 0) +
            (VisibilityHelper.IsWebsiteVisible(support) ? 1 : 0);

        _contactSectionLabel = AddSectionHeader(_contentPanel, LocalizationManager.ContactSection, ref y);
        _contactPanel = CreateSectionPanel(_contentPanel, y, rowCount * ContactRowHeight + ContactPanelExtraHeight);
        y += _contactPanel.Height + 12;

        var rowY = 14;
        if (VisibilityHelper.IsEmailVisible(support))
        {
            _emailCaptionLabel = CreateCaptionLabel(LocalizationManager.EmailLabel);
            _emailCaptionLabel.Location = new Point(12, rowY);
            _emailLink = new LinkLabel
            {
                Text = support.EmailTo,
                AutoSize = true,
                    Location = new Point(ContactValueLeft, rowY),
                Font = _valueFont,
                LinkColor = AppTheme.BannerBlue,
                ActiveLinkColor = AppTheme.Accent,
                VisitedLinkColor = AppTheme.BannerBlue,
                Cursor = Cursors.Hand
            };
            _emailLink.Click += (_, _) => TryOpenEmail(MailHelper.TryOpenHelpdeskEmail);
            _contactPanel.Controls.Add(_emailCaptionLabel);
            _contactPanel.Controls.Add(_emailLink);
            rowY += ContactRowHeight;
        }

        if (VisibilityHelper.IsPhoneVisible(support))
        {
            _hotlineCaptionLabel = CreateCaptionLabel(LocalizationManager.HotlineLabel);
            _hotlineCaptionLabel.Location = new Point(12, rowY);
            _hotlineValueLabel = new Label
            {
                Text = support.Phone,
                AutoSize = true,
                    Location = new Point(ContactValueLeft, rowY),
                Font = _valueFont,
                ForeColor = AppTheme.ValueText
            };
            _contactPanel.Controls.Add(_hotlineCaptionLabel);
            _contactPanel.Controls.Add(_hotlineValueLabel);
            rowY += ContactRowHeight;
        }

        if (VisibilityHelper.IsMobilePhoneVisible(support))
        {
            _mobilePhoneCaptionLabel = CreateCaptionLabel(LocalizationManager.MobilePhoneLabel);
            _mobilePhoneCaptionLabel.Location = new Point(12, rowY);
            _mobilePhoneValueLabel = new Label
            {
                Text = support.MobilePhone,
                AutoSize = true,
                    Location = new Point(ContactValueLeft, rowY),
                Font = _valueFont,
                ForeColor = AppTheme.ValueText
            };
            _contactPanel.Controls.Add(_mobilePhoneCaptionLabel);
            _contactPanel.Controls.Add(_mobilePhoneValueLabel);
            rowY += ContactRowHeight;
        }

        if (VisibilityHelper.IsWebsiteVisible(support))
        {
            _websiteCaptionLabel = CreateCaptionLabel(LocalizationManager.WebsiteLabel);
            _websiteCaptionLabel.Location = new Point(12, rowY);
            _websiteLink = new LinkLabel
            {
                Text = FormatWebsiteDisplay(support.WebsiteUrl),
                AutoSize = true,
                    Location = new Point(ContactValueLeft, rowY),
                Font = _valueFont,
                LinkColor = AppTheme.BannerBlue,
                ActiveLinkColor = AppTheme.Accent,
                VisitedLinkColor = AppTheme.BannerBlue,
                Cursor = Cursors.Hand
            };
            _websiteLink.LinkClicked += (_, _) => OpenWebsite(support.WebsiteUrl);
            _contactPanel.Controls.Add(_websiteCaptionLabel);
            _contactPanel.Controls.Add(_websiteLink);
        }
    }

    private void BuildComputerSection(FeatureSettings features, ref int y)
    {
        var definitions = new List<(bool Visible, Func<string> Label, Action<SystemInfoData, TextBox> Apply)>();
        if (features.ShowComputerName)
        {
            definitions.Add((true, () => LocalizationManager.ComputerNameLabel, (d, t) => t.Text = d.ComputerName));
        }

        if (features.ShowDomain)
        {
            definitions.Add((true, () => LocalizationManager.DomainLabel, (d, t) => t.Text = d.Domain));
        }

        if (features.ShowOperatingSystem)
        {
            definitions.Add((true, () => LocalizationManager.OperatingSystemLabel, (d, t) => t.Text = d.OperatingSystem));
        }

        if (features.ShowIpAddress)
        {
            definitions.Add((true, () => LocalizationManager.IpAddressLabel, (d, t) => t.Text = d.IpAddress));
        }

        if (features.ShowDnsServers)
        {
            definitions.Add((true, () => LocalizationManager.DnsLabel, (d, t) => t.Text = d.DnsServers));
        }

        if (features.ShowUptime)
        {
            definitions.Add((true, () => LocalizationManager.UptimeLabel, (d, t) => t.Text = d.Uptime));
        }

        if (features.ShowManufacturerModel)
        {
            definitions.Add((true, () => LocalizationManager.ManufacturerLabel, (d, t) => t.Text = d.ManufacturerModel));
        }

        if (features.ShowSerialNumber)
        {
            definitions.Add((true, () => LocalizationManager.BiosSerialLabel, (d, t) => t.Text = d.BiosSerial));
        }

        if (features.ShowDeviceType)
        {
            definitions.Add((true, () => LocalizationManager.MachineTypeLabel, (d, t) => t.Text = d.MachineType));
        }

        if (definitions.Count == 0)
        {
            return;
        }

        _computerSectionLabel = AddSectionHeader(_contentPanel, LocalizationManager.ComputerDataSection, ref y);
        var panel = CreateSectionPanel(_contentPanel, y, GetTablePanelHeight(definitions.Count));
        y += panel.Height + 12;

        var table = CreateFieldTable(panel, definitions.Count);
        for (var i = 0; i < definitions.Count; i++)
        {
            var definition = definitions[i];
            var valueBox = table.Values[i];
            _computerFields.Add(new BoundField
            {
                Caption = table.Captions[i],
                Value = valueBox,
                GetLabel = definition.Label,
                ApplyValue = data => definition.Apply(data, valueBox)
            });
        }
    }

    private void BuildUserSection(FeatureSettings features, ref int y)
    {
        var definitions = new List<(Func<string> Label, Action<SystemInfoData, TextBox> Apply)>();
        if (features.ShowUserLogin)
        {
            definitions.Add((() => LocalizationManager.UserLoginLabel, (d, t) => t.Text = d.UserLogin));
        }

        if (features.ShowDisplayName)
        {
            definitions.Add((() => LocalizationManager.UserDisplayNameLabel, (d, t) => t.Text = d.UserDisplayName));
        }

        if (definitions.Count == 0)
        {
            return;
        }

        _userSectionLabel = AddSectionHeader(_contentPanel, LocalizationManager.UserDataSection, ref y);
        var panel = CreateSectionPanel(_contentPanel, y, GetTablePanelHeight(definitions.Count));
        y += panel.Height + 12;

        var table = CreateFieldTable(panel, definitions.Count);
        for (var i = 0; i < definitions.Count; i++)
        {
            var definition = definitions[i];
            var valueBox = table.Values[i];
            _userFields.Add(new BoundField
            {
                Caption = table.Captions[i],
                Value = valueBox,
                GetLabel = definition.Label,
                ApplyValue = data => definition.Apply(data, valueBox)
            });
        }
    }

    private void BuildTeamViewerSection(FeatureSettings features, ref int y)
    {
        if (!features.ShowTeamViewerSection)
        {
            return;
        }

        _teamViewerSectionLabel = AddSectionHeader(_contentPanel, LocalizationManager.TeamViewerSection, ref y);
        var teamViewerPanelHeight = GetTablePanelHeight(1);
        if (features.AllowLaunchTeamViewer)
        {
            teamViewerPanelHeight += 36;
        }

        _teamViewerPanel = CreateSectionPanel(_contentPanel, y, teamViewerPanelHeight);
        y += _teamViewerPanel.Height + 12;

        var teamViewerTable = CreateFieldTable(_teamViewerPanel, 1);
        _teamViewerStatusCaption = teamViewerTable.Captions[0];
        _teamViewerStatusValue = teamViewerTable.Values[0];

        if (features.AllowLaunchTeamViewer)
        {
            _launchTeamViewerButton = CreateAccentButton(
                LocalizationManager.LaunchTeamViewerButton,
                new Point(12, TableRowHeight + 8),
                new Size(170, 28));
            _launchTeamViewerButton.Visible = false;
            _launchTeamViewerButton.Click += (_, _) => LaunchTeamViewer();
            _teamViewerPanel.Controls.Add(_launchTeamViewerButton);
        }
    }

    private void ShowAboutDialog()
    {
        using var about = new AboutForm(this);
        about.ApplyLanguage();
        about.ShowDialog(this);
    }

    private static int GetTablePanelHeight(int rowCount) =>
        rowCount * TableRowHeight + TablePanelExtraHeight;

    private static string FormatWebsiteDisplay(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        var trimmed = url.Trim();
        if (Uri.TryCreate(trimmed, UriKind.Absolute, out var uri))
        {
            var display = uri.Host;
            var path = uri.PathAndQuery;
            if (!string.IsNullOrEmpty(path) && path != "/")
            {
                display += path.TrimEnd('/');
            }

            return display;
        }

        if (trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed[8..].TrimEnd('/');
        }

        if (trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed[7..].TrimEnd('/');
        }

        return trimmed.TrimEnd('/');
    }

    private static void OpenWebsite(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch
        {
            // Ignore browser launch failures.
        }
    }

    private static void OpenConfigurationDoc() => OpenWebsite(LocalizationManager.ConfigurationDocUrl);

    private LinkLabel CreateLanguageLink(string text, int x) => new()
    {
        Text = text,
        AutoSize = true,
        Location = new Point(x, 20),
        Font = _labelFont,
        LinkColor = Color.White,
        ActiveLinkColor = AppTheme.AccentLight,
        VisitedLinkColor = Color.White,
        LinkBehavior = LinkBehavior.HoverUnderline,
        Cursor = Cursors.Hand
    };

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

    private Label CreateCaptionLabel(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Font = _labelFont,
        ForeColor = AppTheme.LabelText
    };

    private (Label[] Captions, TextBox[] Values) CreateFieldTable(Panel parent, int rowCount)
    {
        const float labelColumnWidth = 200F;

        var table = new TableLayoutPanel
        {
            Location = new Point(0, 0),
            Dock = DockStyle.Top,
            ColumnCount = 2,
            RowCount = rowCount,
            AutoSize = true,
            Width = parent.ClientSize.Width - parent.Padding.Horizontal
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, labelColumnWidth));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        var captions = new Label[rowCount];
        var values = new TextBox[rowCount];

        for (var i = 0; i < rowCount; i++)
        {
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, TableRowHeight));
            captions[i] = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(12, TableRowVerticalPadding, 8, TableRowVerticalPadding),
                Font = _labelFont,
                ForeColor = AppTheme.LabelText
            };
            values[i] = new TextBox
            {
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = AppTheme.PanelBackground,
                ForeColor = AppTheme.ValueText,
                Font = _valueFont,
                Margin = new Padding(0, TableRowVerticalPadding, 12, TableRowVerticalPadding),
                Dock = DockStyle.Fill,
                TabStop = false,
                Multiline = false
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
        WireButtonHover(button, AppTheme.BannerBlue, AppTheme.Accent);
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
            BackColor = AppTheme.Accent,
            ForeColor = Color.White,
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        WireButtonHover(button, AppTheme.Accent, AppTheme.AccentHover);
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
        WireButtonHover(button, Color.White, AppTheme.AccentLight, Color.FromArgb(60, 65, 70), AppTheme.Accent);
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
        foreach (var box in _dataValueBoxes)
        {
            if (loading)
            {
                box.Text = LocalizationManager.LoadingText;
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
        foreach (var box in _dataValueBoxes)
        {
            box.Text = LocalizationManager.NoData;
        }
    }

    private void SetActionButtonsEnabled(bool enabled)
    {
        _copyButton.Enabled = enabled;
        _refreshButton.Enabled = enabled;
        _reportButton.Enabled = enabled && !string.IsNullOrWhiteSpace(ConfigurationService.Current.Support.EmailTo);
        if (_launchTeamViewerButton is not null)
        {
            _launchTeamViewerButton.Enabled = enabled && _data.TeamViewerInstalled;
        }
    }

    private void UpdateValues()
    {
        foreach (var field in _computerFields)
        {
            field.ApplyValue(_data);
        }

        foreach (var field in _userFields)
        {
            field.ApplyValue(_data);
        }

        if (_teamViewerStatusValue is not null)
        {
            _teamViewerStatusValue.Text = LocalizationManager.GetTeamViewerStatus(_data.TeamViewerInstalled);
            if (_launchTeamViewerButton is not null && _teamViewerPanel is not null)
            {
                var showButton = ConfigurationService.Current.Features.AllowLaunchTeamViewer &&
                                 _data.TeamViewerInstalled &&
                                 !string.IsNullOrEmpty(_data.TeamViewerPath);
                _launchTeamViewerButton.Visible = showButton;
                _teamViewerPanel.Height = showButton
                    ? GetTablePanelHeight(1) + 36
                    : GetTablePanelHeight(1);
            }
        }
    }

    private void UpdateNetworkStatusIndicator()
    {
        if (_networkStatusLabel is null)
        {
            AppDiagnosticLog.Write("Footer network indicator skipped: control is null");
            return;
        }

        var showIndicator = ConfigurationService.Current.Features.ShowNetworkStatus;
        AppDiagnosticLog.Write($"Footer network indicator ShowNetworkStatus = {showIndicator}");

        _networkStatusLabel.Visible = showIndicator;
        if (!showIndicator)
        {
            return;
        }

        var isOnline = NetworkStatusService.IsOnline;
        _networkStatusLabel.Text = isOnline
            ? LocalizationManager.NetworkStatusOnlineIndicator
            : LocalizationManager.NetworkStatusOfflineIndicator;
        _networkStatusLabel.ForeColor = isOnline ? AppTheme.FooterText : AppTheme.Accent;
        _footerToolTip.SetToolTip(_networkStatusLabel, LocalizationManager.NetworkStatusTooltip(isOnline));
    }

    private void UpdateFooterIndicator()
    {
        if (_updateIndicatorLink is null)
        {
            AppDiagnosticLog.Write("Footer update indicator skipped: control is null");
            return;
        }

        var result = UpdateService.LastResult;
        var showFooterIndicator = ConfigurationService.Current.Update.ShowFooterIndicator;
        var showIndicator = result is not null && showFooterIndicator;
        AppDiagnosticLog.Write($"Footer update indicator ShowFooterIndicator = {showFooterIndicator}, update available = {result is not null}");

        _updateIndicatorLink.Visible = showIndicator;
        if (!showIndicator)
        {
            return;
        }

        _updateIndicatorLink.Text = LocalizationManager.UpdateFooterIndicator;
        _footerToolTip.SetToolTip(_updateIndicatorLink, LocalizationManager.UpdateFooterTooltip(result!.RemoteVersion));
    }

    private static Font CreateFooterIndicatorFont()
    {
        try
        {
            return new Font("Segoe UI Emoji", 9F);
        }
        catch
        {
            return new Font("Segoe UI", 9F, FontStyle.Bold);
        }
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
        _bannerLabel.Text = LocalizationManager.BannerTitle;
        _footerLabel.Text = AppInfoService.FooterText;
        RefreshFooterIndicators();
        _configLink.Text = LocalizationManager.ConfigurationLink;
        _aboutLink.Text = LocalizationManager.AboutLink;

        if (_contactSectionLabel is not null)
        {
            _contactSectionLabel.Text = LocalizationManager.ContactSection;
            if (_emailCaptionLabel is not null)
            {
                _emailCaptionLabel.Text = LocalizationManager.EmailLabel;
            }

            if (_hotlineCaptionLabel is not null)
            {
                _hotlineCaptionLabel.Text = LocalizationManager.HotlineLabel;
            }

            if (_mobilePhoneCaptionLabel is not null)
            {
                _mobilePhoneCaptionLabel.Text = LocalizationManager.MobilePhoneLabel;
            }

            if (_websiteCaptionLabel is not null)
            {
                _websiteCaptionLabel.Text = LocalizationManager.WebsiteLabel;
            }
        }

        if (_computerSectionLabel is not null)
        {
            _computerSectionLabel.Text = LocalizationManager.ComputerDataSection;
            foreach (var field in _computerFields)
            {
                field.Caption.Text = field.GetLabel();
            }
        }

        if (_userSectionLabel is not null)
        {
            _userSectionLabel.Text = LocalizationManager.UserDataSection;
            foreach (var field in _userFields)
            {
                field.Caption.Text = field.GetLabel();
            }
        }

        if (_teamViewerSectionLabel is not null)
        {
            _teamViewerSectionLabel.Text = LocalizationManager.TeamViewerSection;
            if (_teamViewerStatusCaption is not null)
            {
                _teamViewerStatusCaption.Text = LocalizationManager.TeamViewerStatusLabel;
            }

            if (_launchTeamViewerButton is not null)
            {
                _launchTeamViewerButton.Text = LocalizationManager.LaunchTeamViewerButton;
            }
        }

        _copyButton.Text = LocalizationManager.CopyButton;
        _refreshButton.Text = LocalizationManager.RefreshButton;
        _reportButton.Text = LocalizationManager.ReportButton;
        _closeButton.Text = LocalizationManager.CloseButton;
        if (_polishLink is not null)
        {
            _polishLink.Text = LocalizationManager.LanguagePolish;
        }

        if (_englishLink is not null)
        {
            _englishLink.Text = LocalizationManager.LanguageEnglish;
        }

        HighlightActiveLanguage();

        if (!_isLoading && _teamViewerStatusValue is not null)
        {
            _teamViewerStatusValue.Text = LocalizationManager.GetTeamViewerStatus(_data.TeamViewerInstalled);
        }
    }

    private void HighlightActiveLanguage()
    {
        if (_polishLink is null || _englishLink is null)
        {
            return;
        }

        var active = AppTheme.AccentLight;
        var inactive = Color.White;
        _polishLink.LinkColor = LocalizationManager.CurrentLanguage == AppLanguage.Polish ? active : inactive;
        _englishLink.LinkColor = LocalizationManager.CurrentLanguage == AppLanguage.English ? active : inactive;
    }

    private void CopyDataToClipboard()
    {
        if (_isLoading) return;

        try
        {
            Clipboard.SetText(ReportFormatter.FormatClipboard(_data, LocalizationManager.CurrentLanguage));
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
        if (_isLoading) return;

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
