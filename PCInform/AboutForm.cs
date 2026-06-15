using System.Diagnostics;
using PCInform.Localization;
using PCInform.Services;
using PCInform.UI;

namespace PCInform;

internal sealed class AboutForm : Form
{
    private readonly Label _versionLabel;
    private readonly Label _updateAvailableLabel;
    private readonly Label _descriptionLabel;
    private readonly Label _authorCaptionLabel;
    private readonly Label _projectCaptionLabel;
    private readonly Label _projectValueLabel;
    private readonly Label _licenseCaptionLabel;
    private readonly Label _licenseValueLabel;
    private readonly Button _githubButton;
    private readonly Button _closeButton;

    public AboutForm(IWin32Window? owner)
    {
        Text = LocalizationManager.AboutDialogTitle;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(520, 332);
        BackColor = AppTheme.Background;
        Font = new Font("Segoe UI", 9F);
        ShowInTaskbar = false;

        if (owner is Form ownerForm)
        {
            Icon = ownerForm.Icon;
        }

        const int left = 24;
        const int contentWidth = 472;
        const int labelColumnWidth = 76;
        var valueLeft = left + labelColumnWidth;
        var valueWidth = contentWidth - labelColumnWidth;

        var titleLabel = new Label
        {
            Text = "PC Inform",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = AppTheme.BannerBlue,
            AutoSize = false,
            Location = new Point(left, 20),
            Size = new Size(contentWidth, 28),
            TextAlign = ContentAlignment.MiddleLeft
        };

        _versionLabel = new Label
        {
            AutoSize = false,
            Location = new Point(left, 54),
            Size = new Size(contentWidth, 20),
            ForeColor = AppTheme.ValueText,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _updateAvailableLabel = new Label
        {
            AutoSize = false,
            Location = new Point(left, 76),
            Size = new Size(contentWidth, 18),
            ForeColor = AppTheme.Accent,
            TextAlign = ContentAlignment.MiddleLeft,
            Visible = false
        };

        _descriptionLabel = new Label
        {
            Location = new Point(left, 82),
            Size = new Size(contentWidth, 54),
            ForeColor = AppTheme.LabelText,
            TextAlign = ContentAlignment.TopLeft
        };

        _authorCaptionLabel = new Label
        {
            Text = LocalizationManager.AboutAuthorLabel,
            AutoSize = false,
            Location = new Point(left, 148),
            Size = new Size(labelColumnWidth, 22),
            ForeColor = AppTheme.LabelText,
            TextAlign = ContentAlignment.MiddleLeft
        };

        var authorValueLabel = new Label
        {
            Text = LocalizationManager.AboutAuthorName,
            AutoSize = false,
            Location = new Point(valueLeft, 148),
            Size = new Size(valueWidth, 22),
            ForeColor = AppTheme.ValueText,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _projectCaptionLabel = new Label
        {
            AutoSize = false,
            Location = new Point(left, 176),
            Size = new Size(labelColumnWidth, 22),
            ForeColor = AppTheme.LabelText,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _projectValueLabel = new Label
        {
            AutoSize = false,
            Location = new Point(valueLeft, 176),
            Size = new Size(valueWidth, 22),
            ForeColor = AppTheme.ValueText,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _licenseCaptionLabel = new Label
        {
            AutoSize = false,
            Location = new Point(left, 204),
            Size = new Size(labelColumnWidth, 22),
            ForeColor = AppTheme.LabelText,
            TextAlign = ContentAlignment.TopLeft
        };

        _licenseValueLabel = new Label
        {
            AutoSize = false,
            Location = new Point(valueLeft, 204),
            Size = new Size(valueWidth, 40),
            ForeColor = AppTheme.ValueText,
            TextAlign = ContentAlignment.TopLeft
        };

        const int buttonWidth = 96;
        const int buttonHeight = 32;
        const int buttonY = 284;

        _closeButton = new Button
        {
            DialogResult = DialogResult.OK,
            Size = new Size(buttonWidth, buttonHeight),
            Location = new Point(left + contentWidth - buttonWidth, buttonY),
            FlatStyle = FlatStyle.Flat,
            BackColor = AppTheme.BannerBlue,
            ForeColor = Color.White
        };
        _closeButton.FlatAppearance.BorderSize = 0;

        _githubButton = new Button
        {
            Size = new Size(buttonWidth, buttonHeight),
            Location = new Point(_closeButton.Left - 12 - buttonWidth, buttonY),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = AppTheme.BannerBlue
        };
        _githubButton.FlatAppearance.BorderColor = AppTheme.BannerBlue;
        _githubButton.FlatAppearance.BorderSize = 1;
        _githubButton.Click += (_, _) => OpenGitHub();

        AcceptButton = _closeButton;
        CancelButton = _closeButton;

        Controls.Add(titleLabel);
        Controls.Add(_versionLabel);
        Controls.Add(_updateAvailableLabel);
        Controls.Add(_descriptionLabel);
        Controls.Add(_authorCaptionLabel);
        Controls.Add(authorValueLabel);
        Controls.Add(_projectCaptionLabel);
        Controls.Add(_projectValueLabel);
        Controls.Add(_licenseCaptionLabel);
        Controls.Add(_licenseValueLabel);
        Controls.Add(_githubButton);
        Controls.Add(_closeButton);

        ApplyLanguage();
    }

    public void ApplyLanguage()
    {
        Text = LocalizationManager.AboutDialogTitle;
        _versionLabel.Text = $"{LocalizationManager.AboutVersionLabel} v{AppInfoService.Version}";
        ApplyUpdateAvailability();
        _descriptionLabel.Text = LocalizationManager.AboutDescription;
        _authorCaptionLabel.Text = LocalizationManager.AboutAuthorLabel;
        _projectCaptionLabel.Text = LocalizationManager.AboutGitHubLabel;
        _projectValueLabel.Text = LocalizationManager.AboutGitHubUrl;
        _licenseCaptionLabel.Text = LocalizationManager.AboutLicenseLabel;
        _licenseValueLabel.Text = $"{LocalizationManager.AboutLicenseName}{Environment.NewLine}{LocalizationManager.AboutLicenseNote}";
        _githubButton.Text = LocalizationManager.AboutGitHubButton;
        _closeButton.Text = LocalizationManager.CloseButton;
    }

    private void ApplyUpdateAvailability()
    {
        const int descriptionTopDefault = 82;
        const int descriptionTopWithUpdate = 98;

        if (UpdateService.IsUpdateAvailable)
        {
            _updateAvailableLabel.Text = LocalizationManager.UpdateAboutAvailable(UpdateService.LastResult!.RemoteVersion);
            _updateAvailableLabel.Visible = true;
            _descriptionLabel.Location = new Point(_descriptionLabel.Location.X, descriptionTopWithUpdate);
            return;
        }

        _updateAvailableLabel.Visible = false;
        _descriptionLabel.Location = new Point(_descriptionLabel.Location.X, descriptionTopDefault);
    }

    private static void OpenGitHub()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = LocalizationManager.AboutGitHubUrl,
                UseShellExecute = true
            });
        }
        catch
        {
            // Ignore browser launch failures.
        }
    }
}
