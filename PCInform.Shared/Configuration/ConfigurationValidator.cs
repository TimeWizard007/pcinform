using PCInform.Models;

namespace PCInform.Configuration;

public static class ConfigurationValidator
{
    public static ConfigurationValidationResult Validate(AppSettings settings)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        settings.Application ??= new ApplicationSettings();
        settings.Support ??= new SupportSettings();
        settings.Features ??= new FeatureSettings();
        settings.Update ??= new UpdateSettings();

        if (!settings.Application.EnablePolish && !settings.Application.EnableEnglish)
        {
            errors.Add("At least one UI language must be enabled (EnablePolish or EnableEnglish).");
        }

        if (settings.Update.Enabled && string.IsNullOrWhiteSpace(settings.Update.VersionUrl))
        {
            errors.Add("Update.VersionUrl is required when Update.Enabled is true.");
        }

        if (string.IsNullOrWhiteSpace(settings.Support.EmailTo))
        {
            warnings.Add("Support.EmailTo is empty. Report problem may be unavailable in PC Inform.");
        }

        if (settings.Features.CheckUpdates && !settings.Update.Enabled)
        {
            warnings.Add("Features.CheckUpdates is enabled but Update.Enabled is false. Update checks will not run.");
        }

        return new ConfigurationValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors,
            Warnings = warnings
        };
    }
}
