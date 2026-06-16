namespace PCInform.Models;

internal enum NetworkCheckState
{
    Ok,
    Error,
    NoConnectivity,
    NotTested
}

internal sealed class NetworkStatusResult
{
    public NetworkCheckState Internet { get; init; } = NetworkCheckState.NotTested;
    public NetworkCheckState Dns { get; init; } = NetworkCheckState.NotTested;

    public bool IsInternetAvailable => Internet == NetworkCheckState.Ok;
}

internal sealed class RemoteVersionInfo
{
    public string Version { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public string Sha256 { get; set; } = string.Empty;
    public bool Mandatory { get; set; }
    public string ReleaseNotesPl { get; set; } = string.Empty;
    public string ReleaseNotesEn { get; set; } = string.Empty;
}

internal sealed class SystemInfoData
{
    public string ComputerName { get; init; } = string.Empty;
    public string Domain { get; init; } = string.Empty;
    public string OperatingSystem { get; init; } = string.Empty;
    public string IpAddress { get; init; } = string.Empty;
    public string DnsServers { get; init; } = string.Empty;
    public string Uptime { get; init; } = string.Empty;
    public string ManufacturerModel { get; init; } = string.Empty;
    public string BiosSerial { get; init; } = string.Empty;
    public string MachineType { get; init; } = string.Empty;
    public string UserLogin { get; init; } = string.Empty;
    public string UserDisplayName { get; init; } = string.Empty;
    public bool TeamViewerInstalled { get; init; }
    public string? TeamViewerPath { get; init; }
    public bool AteraInstalled { get; init; }
}
