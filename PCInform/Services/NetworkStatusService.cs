using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using PCInform.Configuration;
using PCInform.Models;

namespace PCInform.Services;

internal static class NetworkStatusService
{
    private const string DnsProbeHost = "dns.google";
    private const string InternetProbeHost = "1.1.1.1";
    private const int ProbeTimeoutMs = 3000;

    public static NetworkStatusResult LastResult { get; private set; } = new();

    public static bool IsOnline => LastResult.IsInternetAvailable;

    public static async Task CheckAsync()
    {
        AppDiagnosticLog.Write("Network check started");

        var internet = NetworkCheckState.NotTested;
        var dns = NetworkCheckState.NotTested;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                internet = NetworkCheckState.NoConnectivity;
                dns = NetworkCheckState.NotTested;
            }
            else
            {
                internet = await TestInternetAsync().ConfigureAwait(false)
                    ? NetworkCheckState.Ok
                    : NetworkCheckState.NoConnectivity;

                dns = internet == NetworkCheckState.Ok
                    ? (TestDns() ? NetworkCheckState.Ok : NetworkCheckState.Error)
                    : NetworkCheckState.NotTested;
            }

            LastResult = new NetworkStatusResult
            {
                Internet = internet,
                Dns = dns
            };

            AppDiagnosticLog.Write($"Network result: internet={internet}, dns={dns}");
        }
        catch (Exception ex)
        {
            LastResult = new NetworkStatusResult
            {
                Internet = NetworkCheckState.NoConnectivity,
                Dns = NetworkCheckState.NotTested
            };
            AppDiagnosticLog.Write($"Network check failed: {ex.Message}");
        }
    }

    private static async Task<bool> TestInternetAsync()
    {
        using var ping = new Ping();
        try
        {
            var reply = await ping.SendPingAsync(InternetProbeHost, ProbeTimeoutMs).ConfigureAwait(false);
            return reply.Status == IPStatus.Success;
        }
        catch
        {
            return false;
        }
    }

    private static bool TestDns()
    {
        try
        {
            var addresses = Dns.GetHostAddresses(DnsProbeHost);
            return addresses.Any(address => address.AddressFamily is AddressFamily.InterNetwork
                or AddressFamily.InterNetworkV6);
        }
        catch
        {
            return false;
        }
    }
}
