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
        ClientSize = new Size(448, 276);
        BackColor = AppTheme.Background;
        Font = new Font("Segoe UI", 9F);
        ShowInTaskbar = false;

        if (owner is Form ownerForm)
        {
            Icon = ownerForm.Icon;
        }

        const int contentWidth = 408;
        const int leftMargin = 20;
        const int valueColumn = 72;

        var titleLabel = new Label
        {
            Text = "PC Inform",
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = AppTheme.BannerBlue,
            AutoSize = true,
            Location = new Point(leftMargin, 16)
        };

        _versionLabel = new Label
        {
            Text = $"{LocalizationManager.AboutVersionLabel} v{AppInfoService.Version}",
            AutoSize = true,
            Location = new Point(leftMargin, 44),
            ForeColor = AppTheme.ValueText
        };

        _descriptionLabel = new Label
        {
            Text = LocalizationManager.AboutDescription,
            AutoSize = true,
            MaximumSize = new Size(contentWidth, 0),
            Location = new Point(leftMargin, 68),
            ForeColor = AppTheme.LabelText
        };

        var authorCaption = new Label
        {
            Text = LocalizationManager.AboutAuthorLabel,
            AutoSize = true,
            Location = new Point(leftMargin, 140),
            ForeColor = AppTheme.LabelText
        };

        var authorValue = new Label
        {
            Text = LocalizationManager.AboutAuthorName,
            AutoSize = true,
            Location = new Point(valueColumn, 140),
            ForeColor = AppTheme.ValueText
        };

        var githubCaption = new Label
        {
            Text = LocalizationManager.AboutGitHubLabel,
            AutoSize = true,
            Location = new Point(leftMargin, 164),
            ForeColor = AppTheme.LabelText
        };

        _githubLink = new LinkLabel
        {
            Text = LocalizationManager.AboutGitHubUrl,
            AutoSize = true,
            MaximumSize = new Size(contentWidth - valueColumn + leftMargin, 0),
            Location = new Point(valueColumn, 164),
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
            Size = new Size(90, 28),
            Location = new Point(338, 232),
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
