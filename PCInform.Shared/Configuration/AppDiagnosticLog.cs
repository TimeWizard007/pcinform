using System.Text;

namespace PCInform.Configuration;

public static class AppDiagnosticLog
{
    private static readonly object SyncRoot = new();
    private static bool _initialized;

    public static void Initialize()
    {
        lock (SyncRoot)
        {
            if (_initialized)
            {
                return;
            }

            try
            {
                Directory.CreateDirectory(AppPaths.LogDirectory);
                if (!File.Exists(AppPaths.LogFilePath))
                {
                    File.WriteAllText(AppPaths.LogFilePath, string.Empty, Encoding.UTF8);
                }

                _initialized = true;
            }
            catch
            {
                // Logging must never crash the application.
            }
        }
    }

    public static void Write(string message)
    {
        try
        {
            Initialize();

            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
            lock (SyncRoot)
            {
                File.AppendAllText(AppPaths.LogFilePath, line, Encoding.UTF8);
            }
        }
        catch
        {
            // Logging must never crash the application.
        }
    }
}
