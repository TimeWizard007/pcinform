namespace PCInform.UI;

internal static class AppTheme
{
    public static Color BannerBlue { get; private set; } = Color.FromArgb(0, 82, 155);
    public static Color Accent { get; private set; } = Color.FromArgb(232, 119, 34);
    public static Color AccentHover { get; private set; } = Color.FromArgb(245, 140, 55);
    public static Color AccentLight { get; private set; } = Color.FromArgb(255, 236, 220);

    public static readonly Color Background = Color.FromArgb(245, 247, 250);
    public static readonly Color PanelBackground = Color.White;
    public static readonly Color LabelText = Color.FromArgb(80, 90, 100);
    public static readonly Color ValueText = Color.FromArgb(30, 35, 40);
    public static readonly Color ButtonPanelBackground = Color.FromArgb(235, 238, 242);
    public static readonly Color FooterBackground = Color.FromArgb(230, 233, 237);
    public static readonly Color FooterText = Color.FromArgb(90, 95, 100);
    public static readonly Color BorderColor = Color.FromArgb(218, 222, 228);
    public static readonly Color SecondaryButtonBorder = Color.FromArgb(180, 185, 190);

    public const int CornerRadius = 6;

    public static void Initialize(string accentColorHex)
    {
        if (!TryParseColor(accentColorHex, out var accent))
        {
            accent = Color.FromArgb(232, 119, 34);
        }

        Accent = accent;
        AccentHover = Lighten(accent, 0.12f);
        AccentLight = Color.FromArgb(255,
            Math.Min(255, accent.R + 20),
            Math.Min(255, accent.G + 40),
            Math.Min(255, accent.B + 60));
    }

    private static bool TryParseColor(string hex, out Color color)
    {
        color = Color.Empty;
        try
        {
            hex = hex.Trim();
            if (!hex.StartsWith('#'))
            {
                hex = "#" + hex;
            }

            color = ColorTranslator.FromHtml(hex);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static Color Lighten(Color color, float amount)
    {
        var r = (int)(color.R + (255 - color.R) * amount);
        var g = (int)(color.G + (255 - color.G) * amount);
        var b = (int)(color.B + (255 - color.B) * amount);
        return Color.FromArgb(color.A, Math.Clamp(r, 0, 255), Math.Clamp(g, 0, 255), Math.Clamp(b, 0, 255));
    }
}
