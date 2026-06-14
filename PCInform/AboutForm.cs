using System.Diagnostics;
using PCInform.Localization;
using PCInform.Services;
using PCInform.UI;

namespace PCInform;

internal sealed class AboutForm : Form
{
    private readonly Label _descriptionLabel;
    private readonly Label _versionLabel;
    private readonly Label _projectCaptionLabel;
    private readonly LinkLabel _githubLink;

    public AboutForm(IWin32Window? owner)
    {
        Text = LocalizationManager.AboutDialogTitle;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(600, 380);
        BackColor = AppTheme.Background;
        Font = new Font("Segoe UI", 9F);
        ShowInTaskbar = false;

        if (owner is Form ownerForm)
        {
            Icon = ownerForm.Icon;
        }

        const int left = 32;
        const int contentWidth = 536;
        const int labelColumnWidth = 80;
        var valueLeft = left + labelColumnWidth;
        var valueWidth = contentWidth - labelColumnWidth;

        var titleLabel = new Label
        {
            Text = "PC Inform",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = AppTheme.BannerBlue,
            AutoSize = false,
            Location = new Point(left, 24),
            Size = new Size(contentWidth, 28),
            TextAlign = ContentAlignment.MiddleLeft
        };

        _versionLabel = new Label
        {
            Text = $"{LocalizationManager.AboutVersionLabel} v{AppInfoService.Version}",
            AutoSize = false,
            Location = new Point(left, 58),
            Size = new Size(contentWidth, 22),
            ForeColor = AppTheme.ValueText,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _descriptionLabel = new Label
        {
            Text = LocalizationManager.AboutDescription,
            Location = new Point(left, 88),
            Size = new Size(contentWidth, 72),
            ForeColor = AppTheme.LabelText,
            TextAlign = ContentAlignment.TopLeft
        };

        var authorCaption = new Label
        {
            Text = LocalizationManager.AboutAuthorLabel,
            AutoSize = false,
            Location = new Point(left, 172),
            Size = new Size(labelColumnWidth, 22),
            ForeColor = AppTheme.LabelText,
            TextAlign = ContentAlignment.MiddleLeft
        };

        var authorValue = new Label
        {
            Text = LocalizationManager.AboutAuthorName,
            AutoSize = false,
            Location = new Point(valueLeft, 172),
            Size = new Size(valueWidth, 22),
            ForeColor = AppTheme.ValueText,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _projectCaptionLabel = new Label
        {
            Text = LocalizationManager.AboutGitHubLabel,
            AutoSize = false,
            Location = new Point(left, 204),
            Size = new Size(labelColumnWidth, 22),
            ForeColor = AppTheme.LabelText,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _githubLink = new LinkLabel
        {
            Text = LocalizationManager.AboutGitHubUrl,
            AutoSize = false,
            Location = new Point(valueLeft, 204),
            Size = new Size(valueWidth, 22),
            LinkColor = AppTheme.BannerBlue,
            ActiveLinkColor = AppTheme.Accent,
            VisitedLinkColor = AppTheme.BannerBlue,
            Cursor = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleLeft
        };
        _githubLink.LinkClicked += (_, _) => OpenGitHub();

        var closeButton = new Button
        {
            Text = LocalizationManager.CloseButton,
            DialogResult = DialogResult.OK,
            Size = new Size(90, 32),
            Location = new Point(478, 324),
            FlatStyle = FlatStyle.Flat,
            BackColor = AppTheme.BannerBlue,
            ForeColor = Color.White
        };
        closeButton.FlatAppearance.BorderSize = 0;
        AcceptButton = closeButton;
        CancelButton = closeButton;

        Controls.Add(titleLabel);
        Controls.Add(_versionLabel);
        Controls.Add(_descriptionLabel);
        Controls.Add(authorCaption);
        Controls.Add(authorValue);
        Controls.Add(_projectCaptionLabel);
        Controls.Add(_githubLink);
        Controls.Add(closeButton);
    }

    public void ApplyLanguage()
    {
        Text = LocalizationManager.AboutDialogTitle;
        _versionLabel.Text = $"{LocalizationManager.AboutVersionLabel} v{AppInfoService.Version}";
        _descriptionLabel.Text = LocalizationManager.AboutDescription;
        _projectCaptionLabel.Text = LocalizationManager.AboutGitHubLabel;
        _githubLink.Text = LocalizationManager.AboutGitHubUrl;
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
