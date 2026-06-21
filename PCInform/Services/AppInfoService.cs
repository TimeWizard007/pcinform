using System.Reflection;

namespace PCInform.Services;

internal static class AppInfoService
{
    public static string Version
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            var informational = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            if (!string.IsNullOrWhiteSpace(informational))
            {
                var plusIndex = informational.IndexOf('+', StringComparison.Ordinal);
                return plusIndex >= 0 ? informational[..plusIndex] : informational;
            }

            var version = assembly.GetName().Version;
            if (version is null)
            {
                return "1.2.1";
            }

            return version.Revision >= 0 && version.Build >= 0
                ? $"{version.Major}.{version.Minor}.{version.Build}"
                : $"{version.Major}.{version.Minor}";
        }
    }

    public static string FooterText => $"v{Version}";
}
