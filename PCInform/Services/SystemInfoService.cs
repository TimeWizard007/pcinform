using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using PCInform.Localization;
using PCInform.Models;

namespace PCInform.Services;

internal static class SystemInfoService
{
    private static readonly string[] TeamViewerPaths =
    [
        @"C:\Program Files\TeamViewer\TeamViewer.exe",
        @"C:\Program Files (x86)\TeamViewer\TeamViewer.exe"
    ];

    private static readonly string[] AteraPaths =
    [
        @"C:\Program Files\ATERA Networks\AteraAgent\AteraAgent.exe",
        @"C:\Program Files (x86)\ATERA Networks\AteraAgent\AteraAgent.exe",
        @"C:\Program Files\ATERA Networks\Agent\AteraAgent.exe",
        @"C:\Program Files (x86)\ATERA Networks\Agent\AteraAgent.exe"
    ];

    private static readonly HashSet<int> LaptopChassisTypes =
    [
        8, 9, 10, 11, 12, 14, 18, 21, 30, 31, 32
    ];

    private static readonly string[] VirtualAdapterKeywords =
    [
        "docker", "vethernet", "virtualbox", "vmware", "hyper-v", "virtual",
        "tap-windows", "wintun", "npcap", "bluetooth", "loopback", "tunnel",
        "pseudo", "wan miniport", "miniport", "isatap", "teredo", "6to4"
    ];

    public static SystemInfoData Collect(AppLanguage language)
    {
        var noData = language == AppLanguage.Polish ? "brak danych" : "no data";
        var outsideAd = language == AppLanguage.Polish
            ? "poza Active Directory"
            : "outside Active Directory";

        var teamViewerPath = FindTeamViewerPath();

        return new SystemInfoData
        {
            ComputerName = SafeGet(() => Environment.MachineName, noData),
            Domain = SafeGet(() => GetDomain(outsideAd), noData),
            OperatingSystem = SafeGet(GetOperatingSystem, noData),
            IpAddress = SafeGet(GetActiveIpAddress, noData),
            DnsServers = SafeGet(GetDnsServers, noData),
            Uptime = SafeGet(() => GetUptime(language), noData),
            ManufacturerModel = SafeGet(GetManufacturerModel, noData),
            BiosSerial = SafeGet(GetBiosSerial, noData),
            MachineType = SafeGet(() => GetMachineType(language), noData),
            UserLogin = SafeGet(GetUserLogin, noData),
            UserDisplayName = SafeGet(GetUserDisplayName, noData),
            TeamViewerInstalled = teamViewerPath is not null,
            TeamViewerPath = teamViewerPath,
            AteraInstalled = IsAteraInstalled()
        };
    }

    public static void LaunchTeamViewer(string path)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        });
    }

    private static string SafeGet(Func<string> getter, string fallback)
    {
        try
        {
            var value = getter();
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }
        catch
        {
            return fallback;
        }
    }

    private static string? FindTeamViewerPath()
    {
        foreach (var path in TeamViewerPaths)
        {
            try
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }
            catch
            {
                // Ignore path access errors.
            }
        }

        return null;
    }

    private static bool IsAteraInstalled()
    {
        foreach (var path in AteraPaths)
        {
            try
            {
                if (File.Exists(path))
                {
                    return true;
                }
            }
            catch
            {
                // Ignore path access errors.
            }
        }

        try
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT Name FROM Win32_Service WHERE Name='AteraAgent' OR Name='AteraAgentService'");
            using var results = searcher.Get();
            foreach (ManagementObject _ in results)
            {
                return true;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    private static string GetDomain(string outsideAd)
    {
        using var searcher = new ManagementObjectSearcher("SELECT PartOfDomain, Domain FROM Win32_ComputerSystem");
        foreach (ManagementObject item in searcher.Get())
        {
            if (item["PartOfDomain"] is true && item["Domain"] is string domain && !string.IsNullOrWhiteSpace(domain))
            {
                return domain.Trim();
            }

            return outsideAd;
        }

        return outsideAd;
    }

    private static string GetOperatingSystem()
    {
        using var searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
        foreach (ManagementObject item in searcher.Get())
        {
            if (item["Caption"] is string caption)
            {
                return caption.Trim();
            }
        }

        return Environment.OSVersion.VersionString;
    }

    private static IReadOnlyList<NetworkInterface> GetPreferredNetworkInterfaces() =>
        NetworkInterface.GetAllNetworkInterfaces()
            .Where(IsUsableNetworkInterface)
            .OrderBy(GetAdapterPriority)
            .ThenByDescending(ni => ni.Speed)
            .ToList();

    private static bool IsUsableNetworkInterface(NetworkInterface ni)
    {
        if (ni.OperationalStatus != OperationalStatus.Up ||
            ni.NetworkInterfaceType is NetworkInterfaceType.Loopback or NetworkInterfaceType.Tunnel)
        {
            return false;
        }

        var description = ni.Description.ToLowerInvariant();
        var name = ni.Name.ToLowerInvariant();
        if (VirtualAdapterKeywords.Any(keyword =>
                description.Contains(keyword, StringComparison.Ordinal) ||
                name.Contains(keyword, StringComparison.Ordinal)))
        {
            return false;
        }

        return ni.GetIPProperties().UnicastAddresses
            .Any(a => IsIpv4Address(a.Address) &&
                      !IsApipa(a.Address.ToString()));
    }

    private static int GetAdapterPriority(NetworkInterface ni) => ni.NetworkInterfaceType switch
    {
        NetworkInterfaceType.Ethernet or NetworkInterfaceType.GigabitEthernet => 0,
        NetworkInterfaceType.Wireless80211 => 1,
        NetworkInterfaceType.FastEthernetT or NetworkInterfaceType.FastEthernetFx => 2,
        _ => 3
    };

    private static string GetActiveIpAddress()
    {
        foreach (var ni in GetPreferredNetworkInterfaces())
        {
            var address = ni.GetIPProperties().UnicastAddresses
                .Where(a => IsIpv4Address(a.Address))
                .Select(a => a.Address.ToString())
                .FirstOrDefault(ip => !IsApipa(ip));

            if (!string.IsNullOrEmpty(address))
            {
                return address;
            }
        }

        return string.Empty;
    }

    private static string GetDnsServers()
    {
        foreach (var ni in GetPreferredNetworkInterfaces())
        {
            var servers = ni.GetIPProperties().DnsAddresses
                .Where(IsIpv4Address)
                .Select(a => a.ToString())
                .Distinct()
                .ToList();

            if (servers.Count > 0)
            {
                return string.Join(", ", servers);
            }
        }

        return string.Empty;
    }

    private static bool IsApipa(string ip) => ip.StartsWith("169.254.", StringComparison.Ordinal);

    private static bool IsIpv4Address(IPAddress address) =>
        address.AddressFamily == AddressFamily.InterNetwork;

    private static string GetUptime(AppLanguage language)
    {
        using var searcher = new ManagementObjectSearcher("SELECT LastBootUpTime FROM Win32_OperatingSystem");
        foreach (ManagementObject item in searcher.Get())
        {
            if (item["LastBootUpTime"] is not null)
            {
                var bootTime = ManagementDateTimeConverter.ToDateTime(item["LastBootUpTime"].ToString()!);
                return FormatUptime(DateTime.Now - bootTime, language);
            }
        }

        return FormatUptime(TimeSpan.FromMilliseconds(Environment.TickCount64), language);
    }

    private static string FormatUptime(TimeSpan uptime, AppLanguage language)
    {
        var days = (int)uptime.TotalDays;
        var hours = uptime.Hours;
        var minutes = uptime.Minutes;

        if (language == AppLanguage.Polish)
        {
            if (days > 0) return $"{days} dni {hours} godzin";
            if (hours > 0) return $"{hours} godz. {minutes} min.";
            return $"{minutes} min.";
        }

        if (days > 0) return $"{days} days {hours} hours";
        if (hours > 0) return $"{hours} hours {minutes} minutes";
        return $"{minutes} minutes";
    }

    private static string GetManufacturerModel()
    {
        using var searcher = new ManagementObjectSearcher("SELECT Manufacturer, Model FROM Win32_ComputerSystem");
        foreach (ManagementObject item in searcher.Get())
        {
            var manufacturer = item["Manufacturer"]?.ToString()?.Trim() ?? string.Empty;
            var model = item["Model"]?.ToString()?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(manufacturer) && !string.IsNullOrEmpty(model))
            {
                return $"{manufacturer} {model}";
            }

            return !string.IsNullOrEmpty(model) ? model : manufacturer;
        }

        return string.Empty;
    }

    private static string GetBiosSerial()
    {
        using var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS");
        foreach (ManagementObject item in searcher.Get())
        {
            return item["SerialNumber"]?.ToString()?.Trim() ?? string.Empty;
        }

        return string.Empty;
    }

    private static string GetUserLogin()
    {
        var domain = Environment.UserDomainName;
        var user = Environment.UserName;
        return string.IsNullOrWhiteSpace(domain) || string.IsNullOrWhiteSpace(user)
            ? string.Empty
            : $@"{domain}\{user}";
    }

    private static string GetUserDisplayName()
    {
        var username = Environment.UserName?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(username))
        {
            return string.Empty;
        }

        var machineName = Environment.MachineName;
        var userDomain = Environment.UserDomainName;
        var isLocalSession = string.IsNullOrWhiteSpace(userDomain) ||
                             userDomain.Equals(machineName, StringComparison.OrdinalIgnoreCase);

        var localFullName = GetWin32UserAccountFullName(username, machineName, localOnly: true);
        if (IsReliableDisplayName(localFullName, username))
        {
            return localFullName;
        }

        if (!isLocalSession)
        {
            var domainFullName = GetWin32UserAccountFullName(username, userDomain, localOnly: false);
            if (IsReliableDisplayName(domainFullName, username))
            {
                return domainFullName;
            }

            var profileDisplayName = GetUserNameExDisplayName();
            if (IsReliableDisplayName(profileDisplayName, username))
            {
                return profileDisplayName;
            }
        }
        else
        {
            var profileDisplayName = GetUserNameExDisplayName();
            if (IsReliableDisplayName(profileDisplayName, username))
            {
                return profileDisplayName;
            }

            var localAccountFullName = GetWin32UserAccountFullName(username, machineName, localOnly: false);
            if (IsReliableDisplayName(localAccountFullName, username))
            {
                return localAccountFullName;
            }
        }

        return username;
    }

    private static bool IsReliableDisplayName(string? value, string username)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var trimmed = value.Trim();
        if (trimmed.Contains('\\', StringComparison.Ordinal))
        {
            return false;
        }

        if (trimmed.Equals(username, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (trimmed.Equals(Environment.UserDomainName, StringComparison.OrdinalIgnoreCase) ||
            trimmed.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (trimmed.Length < 2)
        {
            return false;
        }

        return trimmed.Any(char.IsLetter);
    }

    private static string GetWin32UserAccountFullName(string username, string domain, bool localOnly)
    {
        var escapedUser = username.Replace("'", "''");
        var escapedDomain = domain.Replace("'", "''");
        var query = localOnly
            ? $"SELECT FullName FROM Win32_UserAccount WHERE Name='{escapedUser}' AND (LocalAccount=True OR Domain='{escapedDomain}')"
            : $"SELECT FullName FROM Win32_UserAccount WHERE Name='{escapedUser}' AND Domain='{escapedDomain}'";

        try
        {
            using var searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject item in searcher.Get())
            {
                var fullName = item["FullName"]?.ToString()?.Trim();
                if (!string.IsNullOrWhiteSpace(fullName))
                {
                    return fullName;
                }
            }
        }
        catch
        {
            // Ignore WMI lookup failures.
        }

        return string.Empty;
    }

    private const int NameDisplay = 3;

    [DllImport("secur32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool GetUserNameEx(int nameFormat, StringBuilder userName, ref int userNameSize);

    private static string GetUserNameExDisplayName()
    {
        if (!OperatingSystem.IsWindows())
        {
            return string.Empty;
        }

        try
        {
            var size = 256;
            var builder = new StringBuilder(size);
            if (GetUserNameEx(NameDisplay, builder, ref size))
            {
                return builder.ToString().Trim();
            }

            builder = new StringBuilder(size);
            if (GetUserNameEx(NameDisplay, builder, ref size))
            {
                return builder.ToString().Trim();
            }
        }
        catch
        {
            // Ignore profile display name lookup failures.
        }

        return string.Empty;
    }

    private static bool IsVirtualMachine()
    {
        var combined = string.Empty;
        var hypervisorPresent = false;

        try
        {
            using var systemSearcher = new ManagementObjectSearcher(
                "SELECT Manufacturer, Model, HypervisorPresent FROM Win32_ComputerSystem");
            foreach (ManagementObject item in systemSearcher.Get())
            {
                combined += $"{item["Manufacturer"]} {item["Model"]} ";
                hypervisorPresent = item["HypervisorPresent"] is true;
            }
        }
        catch
        {
            // Ignore WMI failures.
        }

        try
        {
            using var biosSearcher = new ManagementObjectSearcher("SELECT Manufacturer, SMBIOSBIOSVersion FROM Win32_BIOS");
            foreach (ManagementObject item in biosSearcher.Get())
            {
                combined += $"{item["Manufacturer"]} {item["SMBIOSBIOSVersion"]} ";
            }
        }
        catch
        {
            // Ignore WMI failures.
        }

        combined = combined.ToLowerInvariant();
        if (hypervisorPresent)
        {
            return true;
        }

        string[] vmKeywords =
        [
            "vmware", "virtualbox", "vbox", "innotek", "virtual machine",
            "qemu", "kvm", "proxmox", "xen", "parallels", "bochs", "hyper-v"
        ];

        return vmKeywords.Any(keyword => combined.Contains(keyword, StringComparison.Ordinal));
    }

    private static string GetMachineType(AppLanguage language)
    {
        if (IsVirtualMachine())
        {
            return language == AppLanguage.Polish ? "Komputer wirtualny" : "Virtual Machine";
        }

        try
        {
            using var enclosureSearcher = new ManagementObjectSearcher("SELECT ChassisTypes FROM Win32_SystemEnclosure");
            foreach (ManagementObject item in enclosureSearcher.Get())
            {
                if (item["ChassisTypes"] is ushort[] chassisTypes &&
                    chassisTypes.Any(t => LaptopChassisTypes.Contains(t)))
                {
                    return "Laptop";
                }
            }
        }
        catch
        {
            // Ignore WMI failures.
        }

        try
        {
            using var systemSearcher = new ManagementObjectSearcher("SELECT PCSystemType FROM Win32_ComputerSystem");
            foreach (ManagementObject item in systemSearcher.Get())
            {
                if (item["PCSystemType"] is ushort systemType)
                {
                    if (systemType == 1) return "Laptop";
                    if (systemType == 2) return language == AppLanguage.Polish ? "Komputer stacjonarny" : "Desktop";
                }
            }
        }
        catch
        {
            // Ignore WMI failures.
        }

        return language == AppLanguage.Polish ? "Komputer stacjonarny" : "Desktop";
    }
}
