using System.Net.NetworkInformation;
using PCInform.Configuration;

namespace PCInform.Services;

internal static class NetworkStatusService
{
    public static bool IsOnline { get; private set; }

    public static Task CheckAsync()
    {
        AppDiagnosticLog.Write("Network check started");

        try
        {
            IsOnline = NetworkInterface.GetIsNetworkAvailable();
            AppDiagnosticLog.Write($"Network result: {(IsOnline ? "online" : "offline")}");
        }
        catch (Exception ex)
        {
            IsOnline = false;
            AppDiagnosticLog.Write($"Network check failed: {ex.Message}");
        }

        return Task.CompletedTask;
    }
}
