namespace PCInform.Configuration;

public static class AppDiagnosticLog
{
    public static void Write(string message)
    {
        try
        {
            var logDirectory = Path.Combine(AppPaths.GlobalConfigDirectory, "Logs");
            Directory.CreateDirectory(logDirectory);
            var logFilePath = Path.Combine(logDirectory, "PCInform.log");
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
            File.AppendAllText(logFilePath, line);
        }
        catch
        {
            // Logging must never crash the application.
        }
    }
}
