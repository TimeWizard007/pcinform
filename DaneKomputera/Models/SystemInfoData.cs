namespace DaneKomputera.Models;

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
