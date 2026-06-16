namespace PCInform.Services;

internal readonly struct SemanticVersion : IComparable<SemanticVersion>
{
    public int Major { get; init; }
    public int Minor { get; init; }
    public int Patch { get; init; }
    public string? PreRelease { get; init; }

    public int CompareTo(SemanticVersion other)
    {
        if (Major != other.Major)
        {
            return Major.CompareTo(other.Major);
        }

        if (Minor != other.Minor)
        {
            return Minor.CompareTo(other.Minor);
        }

        if (Patch != other.Patch)
        {
            return Patch.CompareTo(other.Patch);
        }

        return ComparePreRelease(PreRelease, other.PreRelease);
    }

    public override string ToString()
    {
        var core = $"{Major}.{Minor}.{Patch}";
        return string.IsNullOrEmpty(PreRelease) ? core : $"{core}-{PreRelease}";
    }

    private static int ComparePreRelease(string? left, string? right)
    {
        var leftIsRelease = string.IsNullOrEmpty(left);
        var rightIsRelease = string.IsNullOrEmpty(right);

        if (leftIsRelease && rightIsRelease)
        {
            return 0;
        }

        if (leftIsRelease)
        {
            return 1;
        }

        if (rightIsRelease)
        {
            return -1;
        }

        var leftParts = left!.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var rightParts = right!.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var count = Math.Max(leftParts.Length, rightParts.Length);

        for (var i = 0; i < count; i++)
        {
            if (i >= leftParts.Length)
            {
                return -1;
            }

            if (i >= rightParts.Length)
            {
                return 1;
            }

            var comparison = CompareIdentifier(leftParts[i], rightParts[i]);
            if (comparison != 0)
            {
                return comparison;
            }
        }

        return 0;
    }

    private static int CompareIdentifier(string left, string right)
    {
        var leftIsNumeric = int.TryParse(left, out var leftNumber);
        var rightIsNumeric = int.TryParse(right, out var rightNumber);

        if (leftIsNumeric && rightIsNumeric)
        {
            return leftNumber.CompareTo(rightNumber);
        }

        if (leftIsNumeric)
        {
            return -1;
        }

        if (rightIsNumeric)
        {
            return 1;
        }

        return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
    }
}

internal static class VersionHelper
{
    public static bool TryParseLoose(string? value, out SemanticVersion version)
    {
        version = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim();
        if (normalized.StartsWith('v') || normalized.StartsWith('V'))
        {
            normalized = normalized[1..].TrimStart();
        }

        var plusIndex = normalized.IndexOf('+');
        if (plusIndex >= 0)
        {
            normalized = normalized[..plusIndex];
        }

        string? preRelease = null;
        var dashIndex = normalized.IndexOf('-');
        if (dashIndex >= 0)
        {
            preRelease = normalized[(dashIndex + 1)..].Trim();
            if (string.IsNullOrEmpty(preRelease))
            {
                preRelease = null;
            }

            normalized = normalized[..dashIndex];
        }

        var parts = normalized.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0 || !int.TryParse(parts[0], out var major))
        {
            return false;
        }

        var minor = 0;
        if (parts.Length > 1 && !int.TryParse(parts[1], out minor))
        {
            return false;
        }

        var patch = 0;
        if (parts.Length > 2 && !int.TryParse(parts[2], out patch))
        {
            return false;
        }

        if (parts.Length > 3)
        {
            return false;
        }

        version = new SemanticVersion
        {
            Major = major,
            Minor = minor,
            Patch = patch,
            PreRelease = preRelease
        };
        return true;
    }

    public static int CompareLoose(string? left, string? right)
    {
        if (!TryParseLoose(left, out var leftVersion))
        {
            return 0;
        }

        if (!TryParseLoose(right, out var rightVersion))
        {
            return 0;
        }

        return rightVersion.CompareTo(leftVersion);
    }
}
