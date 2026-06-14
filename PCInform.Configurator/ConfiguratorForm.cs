using PCInform.Configuration;
using PCInform.Models;

namespace PCInform.Configurator;

internal sealed class ConfiguratorForm : Form
{
    private string _currentPath = AppPaths.ConfigFilePath;
    private AppSettings _settings = ConfigurationService.CreateDefaultSettings();

    private TextBox _pathTextBox = null!;

    private TextBox _appName = null!;
    private TextBox _appWindowTitle = null!;
    private TextBox _appBannerText = null!;
    private TextBox _appDefaultLanguage = null!;
    private TextBox _appAccentColor = null!;
    private TextBox _appWebsiteUrl = null!;
    private CheckBox _appEnablePolish = null!;
    private CheckBox _appEnableEnglish = null!;

    private TextBox _supportCompanyName = null!;
    private TextBox _supportEmailTo = null!;
    private TextBox _supportEmailCc = null!;
    private TextBox _supportEmailBcc = null!;
    private TextBox _supportSubjectPl = null!;
    private TextBox _supportSubjectEn = null!;
    private TextBox _supportPhone = null!;
    private TextBox _supportMobilePhone = null!;
    private TextBox _supportWebsiteUrl = null!;
    private CheckBox _supportShowCompanyName = null!;
    private CheckBox _supportShowEmail = null!;
    private CheckBox _supportShowPhone = null!;
    private CheckBox _supportShowMobilePhone = null!;
    private CheckBox _supportShowWebsite = null!;

    private CheckBox _featShowComputerName = null!;
    private CheckBox _featShowDomain = null!;
    private CheckBox _featShowOperatingSystem = null!;
    private CheckBox _featShowIpAddress = null!;
    private CheckBox _featShowDnsServers = null!;
    private CheckBox _featShowUptime = null!;
    private CheckBox _featShowManufacturerModel = null!;
    private CheckBox _featShowSerialNumber = null!;
    private CheckBox _featShowDeviceType = null!;
    private CheckBox _featShowUserLogin = null!;
    private CheckBox _featShowDisplayName = null!;
    private CheckBox _featShowTeamViewerSection = null!;
    private CheckBox _featShowTeamViewer = null!;
    private CheckBox _featAllowLaunchTeamViewer = null!;
    private CheckBox _featDetectAtera = null!;
    private CheckBox _featShowAteraInGui = null!;
    private CheckBox _featIncludeAteraInReports = null!;
    private CheckBox _featCheckUpdates = null!;

    private CheckBox _updateEnabled = null!;
    private TextBox _updateVersionUrl = null!;

    public ConfiguratorForm()
    {
        Text = "PC Inform Configurator";
        ClientSize = new Size(920, 720);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9F);

        BuildLayout();
        LoadFromPath(_currentPath, showMessages: false);
    }

    private void BuildLayout()
    {
        var pathPanel = new Panel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(12, 8, 12, 0) };
        pathPanel.Controls.Add(new Label
        {
            Text = "Configuration file:",
            AutoSize = true,
            Location = new Point(0, 6)
        });
        _pathTextBox = new TextBox
        {
            Location = new Point(120, 3),
            Width = 760,
            ReadOnly = true,
            BackColor = SystemColors.Window
        };
        pathPanel.Controls.Add(_pathTextBox);

        var tabControl = new TabControl { Dock = DockStyle.Fill };
        tabControl.TabPages.Add(CreateApplicationTab());
        tabControl.TabPages.Add(CreateSupportTab());
        tabControl.TabPages.Add(CreateFeaturesTab());
        tabControl.TabPages.Add(CreateUpdateTab());

        var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 48, Padding = new Padding(12, 8, 12, 8) };
        var loadButton = CreateButton("Load", new Point(0, 4), LoadConfiguration);
        var saveButton = CreateButton("Save", new Point(90, 4), () => SaveConfiguration(saveAs: false));
        var saveAsButton = CreateButton("Save As", new Point(180, 4), () => SaveConfiguration(saveAs: true));
        var resetButton = CreateButton("Reset to defaults", new Point(280, 4), ResetToDefaults);
        var closeButton = CreateButton("Close", new Point(810, 4), () => Close());
        buttonPanel.Controls.Add(loadButton);
        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(saveAsButton);
        buttonPanel.Controls.Add(resetButton);
        buttonPanel.Controls.Add(closeButton);

        Controls.Add(tabControl);
        Controls.Add(buttonPanel);
        Controls.Add(pathPanel);
    }

    private TabPage CreateApplicationTab()
    {
        var page = new TabPage("Application") { Padding = new Padding(12) };
        var panel = CreateScrollPanel(page);
        var table = CreateFieldTable(panel, 8);
        _appName = AddTextRow(table, 0, "Name");
        _appWindowTitle = AddTextRow(table, 1, "WindowTitle");
        _appBannerText = AddTextRow(table, 2, "BannerText");
        _appDefaultLanguage = AddTextRow(table, 3, "DefaultLanguage");
        _appAccentColor = AddTextRow(table, 4, "AccentColor");
        _appWebsiteUrl = AddTextRow(table, 5, "WebsiteUrl");
        _appEnablePolish = AddCheckRow(table, 6, "EnablePolish");
        _appEnableEnglish = AddCheckRow(table, 7, "EnableEnglish");
        return page;
    }

    private TabPage CreateSupportTab()
    {
        var page = new TabPage("Support") { Padding = new Padding(12) };
        var panel = CreateScrollPanel(page);
        var table = CreateFieldTable(panel, 14);
        _supportCompanyName = AddTextRow(table, 0, "CompanyName");
        _supportEmailTo = AddTextRow(table, 1, "EmailTo");
        _supportEmailCc = AddTextRow(table, 2, "EmailCc");
        _supportEmailBcc = AddTextRow(table, 3, "EmailBcc");
        _supportSubjectPl = AddTextRow(table, 4, "EmailSubjectPrefixPl");
        _supportSubjectEn = AddTextRow(table, 5, "EmailSubjectPrefixEn");
        _supportPhone = AddTextRow(table, 6, "Phone");
        _supportMobilePhone = AddTextRow(table, 7, "MobilePhone");
        _supportWebsiteUrl = AddTextRow(table, 8, "WebsiteUrl");
        _supportShowCompanyName = AddCheckRow(table, 9, "ShowCompanyName");
        _supportShowEmail = AddCheckRow(table, 10, "ShowEmail");
        _supportShowPhone = AddCheckRow(table, 11, "ShowPhone");
        _supportShowMobilePhone = AddCheckRow(table, 12, "ShowMobilePhone");
        _supportShowWebsite = AddCheckRow(table, 13, "ShowWebsite");
        return page;
    }

    private TabPage CreateFeaturesTab()
    {
        var page = new TabPage("Features") { Padding = new Padding(12) };
        var panel = CreateScrollPanel(page);
        var table = CreateFieldTable(panel, 18);
        _featShowComputerName = AddCheckRow(table, 0, "ShowComputerName");
        _featShowDomain = AddCheckRow(table, 1, "ShowDomain");
        _featShowOperatingSystem = AddCheckRow(table, 2, "ShowOperatingSystem");
        _featShowIpAddress = AddCheckRow(table, 3, "ShowIpAddress");
        _featShowDnsServers = AddCheckRow(table, 4, "ShowDnsServers");
        _featShowUptime = AddCheckRow(table, 5, "ShowUptime");
        _featShowManufacturerModel = AddCheckRow(table, 6, "ShowManufacturerModel");
        _featShowSerialNumber = AddCheckRow(table, 7, "ShowSerialNumber");
        _featShowDeviceType = AddCheckRow(table, 8, "ShowDeviceType");
        _featShowUserLogin = AddCheckRow(table, 9, "ShowUserLogin");
        _featShowDisplayName = AddCheckRow(table, 10, "ShowDisplayName");
        _featShowTeamViewerSection = AddCheckRow(table, 11, "ShowTeamViewerSection");
        _featShowTeamViewer = AddCheckRow(table, 12, "ShowTeamViewer");
        _featAllowLaunchTeamViewer = AddCheckRow(table, 13, "AllowLaunchTeamViewer");
        _featDetectAtera = AddCheckRow(table, 14, "DetectAtera");
        _featShowAteraInGui = AddCheckRow(table, 15, "ShowAteraInGui");
        _featIncludeAteraInReports = AddCheckRow(table, 16, "IncludeAteraInReports");
        _featCheckUpdates = AddCheckRow(table, 17, "CheckUpdates");
        return page;
    }

    private TabPage CreateUpdateTab()
    {
        var page = new TabPage("Update") { Padding = new Padding(12) };
        var panel = CreateScrollPanel(page);
        var table = CreateFieldTable(panel, 2);
        _updateEnabled = AddCheckRow(table, 0, "Enabled");
        _updateVersionUrl = AddTextRow(table, 1, "VersionUrl");
        return page;
    }

    private static Panel CreateScrollPanel(TabPage page)
    {
        var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        page.Controls.Add(panel);
        return panel;
    }

    private static TableLayoutPanel CreateFieldTable(Panel parent, int rows)
    {
        var table = new TableLayoutPanel
        {
            ColumnCount = 2,
            RowCount = rows,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Top,
            Width = parent.ClientSize.Width - 24,
            Padding = new Padding(0, 0, 0, 8)
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        for (var i = 0; i < rows; i++)
        {
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        parent.Controls.Add(table);
        return table;
    }

    private static TextBox AddTextRow(TableLayoutPanel table, int row, string label)
    {
        table.Controls.Add(new Label
        {
            Text = label,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(0, 8, 8, 8)
        }, 0, row);

        var textBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 6, 0, 6)
        };
        table.Controls.Add(textBox, 1, row);
        return textBox;
    }

    private static CheckBox AddCheckRow(TableLayoutPanel table, int row, string label)
    {
        table.Controls.Add(new Label
        {
            Text = label,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(0, 8, 8, 8)
        }, 0, row);

        var checkBox = new CheckBox
        {
            AutoSize = true,
            Margin = new Padding(0, 8, 0, 8)
        };
        table.Controls.Add(checkBox, 1, row);
        return checkBox;
    }

    private static Button CreateButton(string text, Point location, Action onClick)
    {
        var button = new Button
        {
            Text = text,
            Location = location,
            Size = new Size(text.Length > 12 ? 130 : 80, 30)
        };
        button.Click += (_, _) => onClick();
        return button;
    }

    private void LoadConfiguration()
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            FileName = "appsettings.json",
            InitialDirectory = AppPaths.GlobalConfigDirectory
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            LoadFromPath(dialog.FileName, showMessages: true);
        }
    }

    private void LoadFromPath(string path, bool showMessages)
    {
        _currentPath = path;
        _pathTextBox.Text = path;
        _settings = File.Exists(path)
            ? ConfigurationService.LoadFromFile(path)
            : ConfigurationService.CreateDefaultSettings();
        BindToUi();
        if (showMessages)
        {
            MessageBox.Show(this, File.Exists(path) ? "Configuration loaded." : "File not found. Safe defaults loaded.",
                Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void SaveConfiguration(bool saveAs)
    {
        ReadFromUi();
        var validation = ConfigurationValidator.Validate(_settings);
        if (!validation.IsValid)
        {
            MessageBox.Show(this, string.Join(Environment.NewLine, validation.Errors),
                "Validation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (validation.Warnings.Count > 0)
        {
            var warningText = string.Join(Environment.NewLine, validation.Warnings);
            var result = MessageBox.Show(this, warningText + Environment.NewLine + Environment.NewLine + "Save anyway?",
                "Validation warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes)
            {
                return;
            }
        }

        var targetPath = _currentPath;
        if (saveAs)
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                FileName = "appsettings.json",
                InitialDirectory = AppPaths.GlobalConfigDirectory
            };
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            targetPath = dialog.FileName;
        }

        try
        {
            ConfigurationService.SaveToFile(targetPath, _settings);
            _currentPath = targetPath;
            _pathTextBox.Text = targetPath;
            MessageBox.Show(this, "Configuration saved.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Could not save configuration:\n" + ex.Message,
                Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ResetToDefaults()
    {
        var result = MessageBox.Show(this, "Reset all fields to safe defaults?",
            Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes)
        {
            return;
        }

        _settings = ConfigurationService.CreateDefaultSettings();
        BindToUi();
    }

    private void BindToUi()
    {
        var app = _settings.Application;
        _appName.Text = app.Name;
        _appWindowTitle.Text = app.WindowTitle;
        _appBannerText.Text = app.BannerText;
        _appDefaultLanguage.Text = app.DefaultLanguage;
        _appAccentColor.Text = app.AccentColor;
        _appWebsiteUrl.Text = app.WebsiteUrl;
        _appEnablePolish.Checked = app.EnablePolish;
        _appEnableEnglish.Checked = app.EnableEnglish;

        var support = _settings.Support;
        _supportCompanyName.Text = support.CompanyName;
        _supportEmailTo.Text = support.EmailTo;
        _supportEmailCc.Text = support.EmailCc;
        _supportEmailBcc.Text = support.EmailBcc;
        _supportSubjectPl.Text = support.EmailSubjectPrefixPl;
        _supportSubjectEn.Text = support.EmailSubjectPrefixEn;
        _supportPhone.Text = support.Phone;
        _supportMobilePhone.Text = support.MobilePhone;
        _supportWebsiteUrl.Text = support.WebsiteUrl;
        _supportShowCompanyName.Checked = support.ShowCompanyName;
        _supportShowEmail.Checked = support.ShowEmail;
        _supportShowPhone.Checked = support.ShowPhone;
        _supportShowMobilePhone.Checked = support.ShowMobilePhone;
        _supportShowWebsite.Checked = support.ShowWebsite;

        var features = _settings.Features;
        _featShowComputerName.Checked = features.ShowComputerName;
        _featShowDomain.Checked = features.ShowDomain;
        _featShowOperatingSystem.Checked = features.ShowOperatingSystem;
        _featShowIpAddress.Checked = features.ShowIpAddress;
        _featShowDnsServers.Checked = features.ShowDnsServers;
        _featShowUptime.Checked = features.ShowUptime;
        _featShowManufacturerModel.Checked = features.ShowManufacturerModel;
        _featShowSerialNumber.Checked = features.ShowSerialNumber;
        _featShowDeviceType.Checked = features.ShowDeviceType;
        _featShowUserLogin.Checked = features.ShowUserLogin;
        _featShowDisplayName.Checked = features.ShowDisplayName;
        _featShowTeamViewerSection.Checked = features.ShowTeamViewerSection;
        _featShowTeamViewer.Checked = features.ShowTeamViewer;
        _featAllowLaunchTeamViewer.Checked = features.AllowLaunchTeamViewer;
        _featDetectAtera.Checked = features.DetectAtera;
        _featShowAteraInGui.Checked = features.ShowAteraInGui;
        _featIncludeAteraInReports.Checked = features.IncludeAteraInReports;
        _featCheckUpdates.Checked = features.CheckUpdates;

        _updateEnabled.Checked = _settings.Update.Enabled;
        _updateVersionUrl.Text = _settings.Update.VersionUrl;
    }

    private void ReadFromUi()
    {
        _settings.Application.Name = _appName.Text.Trim();
        _settings.Application.WindowTitle = _appWindowTitle.Text.Trim();
        _settings.Application.BannerText = _appBannerText.Text.Trim();
        _settings.Application.DefaultLanguage = _appDefaultLanguage.Text.Trim();
        _settings.Application.AccentColor = _appAccentColor.Text.Trim();
        _settings.Application.WebsiteUrl = _appWebsiteUrl.Text.Trim();
        _settings.Application.EnablePolish = _appEnablePolish.Checked;
        _settings.Application.EnableEnglish = _appEnableEnglish.Checked;

        _settings.Support.CompanyName = _supportCompanyName.Text.Trim();
        _settings.Support.EmailTo = _supportEmailTo.Text.Trim();
        _settings.Support.EmailCc = _supportEmailCc.Text.Trim();
        _settings.Support.EmailBcc = _supportEmailBcc.Text.Trim();
        _settings.Support.EmailSubjectPrefixPl = _supportSubjectPl.Text.Trim();
        _settings.Support.EmailSubjectPrefixEn = _supportSubjectEn.Text.Trim();
        _settings.Support.Phone = _supportPhone.Text.Trim();
        _settings.Support.MobilePhone = _supportMobilePhone.Text.Trim();
        _settings.Support.WebsiteUrl = _supportWebsiteUrl.Text.Trim();
        _settings.Support.ShowCompanyName = _supportShowCompanyName.Checked;
        _settings.Support.ShowEmail = _supportShowEmail.Checked;
        _settings.Support.ShowPhone = _supportShowPhone.Checked;
        _settings.Support.ShowMobilePhone = _supportShowMobilePhone.Checked;
        _settings.Support.ShowWebsite = _supportShowWebsite.Checked;

        _settings.Features.ShowComputerName = _featShowComputerName.Checked;
        _settings.Features.ShowDomain = _featShowDomain.Checked;
        _settings.Features.ShowOperatingSystem = _featShowOperatingSystem.Checked;
        _settings.Features.ShowIpAddress = _featShowIpAddress.Checked;
        _settings.Features.ShowDnsServers = _featShowDnsServers.Checked;
        _settings.Features.ShowUptime = _featShowUptime.Checked;
        _settings.Features.ShowManufacturerModel = _featShowManufacturerModel.Checked;
        _settings.Features.ShowSerialNumber = _featShowSerialNumber.Checked;
        _settings.Features.ShowDeviceType = _featShowDeviceType.Checked;
        _settings.Features.ShowUserLogin = _featShowUserLogin.Checked;
        _settings.Features.ShowDisplayName = _featShowDisplayName.Checked;
        _settings.Features.ShowTeamViewerSection = _featShowTeamViewerSection.Checked;
        _settings.Features.ShowTeamViewer = _featShowTeamViewer.Checked;
        _settings.Features.AllowLaunchTeamViewer = _featAllowLaunchTeamViewer.Checked;
        _settings.Features.DetectAtera = _featDetectAtera.Checked;
        _settings.Features.ShowAteraInGui = _featShowAteraInGui.Checked;
        _settings.Features.IncludeAteraInReports = _featIncludeAteraInReports.Checked;
        _settings.Features.CheckUpdates = _featCheckUpdates.Checked;

        _settings.Update.Enabled = _updateEnabled.Checked;
        _settings.Update.VersionUrl = _updateVersionUrl.Text.Trim();
    }
}
