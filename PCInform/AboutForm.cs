using System.Diagnostics;
using PCInform.Localization;
using PCInform.Services;
using PCInform.UI;

namespace PCInform;

internal sealed class AboutForm : Form
{
    private readonly Label _descriptionLabel;
    private readonly Label _versionLabel;
    private readonly LinkLabel _githubLink;

    public AboutForm(IWin32Window? owner)
    {
        Text = LocalizationManager.AboutDialogTitle;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(520, 300);
        BackColor = AppTheme.Background;
        Font = new Font("Segoe UI", 9F);
        ShowInTaskbar = false;

        if (owner is Form ownerForm)
        {
            Icon = ownerForm.Icon;
        }

        const int left = 24;
        const int contentWidth = 472;

        var titleLabel = new Label
        {
            Text = "PC Inform",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = AppTheme.BannerBlue,
            AutoSize = true,
            Location = new Point(left, 20)
        };

        _versionLabel = new Label
        {
            Text = $"{LocalizationManager.AboutVersionLabel} v{AppInfoService.Version}",
            AutoSize = true,
            Location = new Point(left, 52),
            ForeColor = AppTheme.ValueText
        };

        _descriptionLabel = new Label
        {
            Text = LocalizationManager.AboutDescription,
            Location = new Point(left, 84),
            Size = new Size(contentWidth, 56),
            ForeColor = AppTheme.LabelText
        };

        var authorCaption = new Label
        {
            Text = LocalizationManager.AboutAuthorLabel,
            AutoSize = true,
            Location = new Point(left, 152),
            ForeColor = AppTheme.LabelText
        };

        var authorValue = new Label
        {
            Text = LocalizationManager.AboutAuthorName,
            AutoSize = true,
            Location = new Point(left + 56, 152),
            ForeColor = AppTheme.ValueText
        };

        var githubCaption = new Label
        {
            Text = LocalizationManager.AboutGitHubLabel,
            AutoSize = true,
            Location = new Point(left, 180),
            ForeColor = AppTheme.LabelText
        };

        _githubLink = new LinkLabel
        {
            Text = LocalizationManager.AboutGitHubUrl,
            AutoSize = true,
            Location = new Point(left, 202),
            LinkColor = AppTheme.BannerBlue,
            ActiveLinkColor = AppTheme.Accent,
            VisitedLinkColor = AppTheme.BannerBlue,
            Cursor = Cursors.Hand
        };
        _githubLink.LinkClicked += (_, _) => OpenGitHub();

        var closeButton = new Button
        {
            Text = LocalizationManager.CloseButton,
            DialogResult = DialogResult.OK,
            Size = new Size(90, 30),
            Location = new Point(406, 252),
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
        Controls.Add(githubCaption);
        Controls.Add(_githubLink);
        Controls.Add(closeButton);
    }

    public void ApplyLanguage()
    {
        Text = LocalizationManager.AboutDialogTitle;
        _versionLabel.Text = $"{LocalizationManager.AboutVersionLabel} v{AppInfoService.Version}";
        _descriptionLabel.Text = LocalizationManager.AboutDescription;
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
