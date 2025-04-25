namespace Power.Revit.Build.Tasks;

public class GenerateCompatibleDefineConstants : Task
{
    [Required]
    public string Configuration { get; set; }

    [Required]
    public string[] Configurations { get; set; }

    public string RevitVersion { get; set; }

    [Output]
    public string[] DefineConstants { get; private set; }

    public override bool Execute()
    {
        try
        {
            var currentVersion = 0d;

            if (string.IsNullOrEmpty(RevitVersion))
            {
                if (!TryGetRevitVersion(Configuration, out currentVersion))
                    return true;
            }

            else
            {
                if (!double.TryParse(RevitVersion, out currentVersion))
                    return true;
            }

            var constants = new List<string>();

            AddVersionConstants(constants, currentVersion);
            AddGreaterConstants(constants, currentVersion);

            DefineConstants = [.. constants.Distinct().OrderBy(constant => constant)];

            return true;
        }
        catch (Exception exception)
        {
            Log.LogErrorFromException(exception, false);

            return false;
        }
    }

    private static void AddVersionConstants(List<string> constants, double currentVersion)
    {
        constants.Add($"{Constant_Revit}{FormatVersion(currentVersion)}");
    }

    private void AddGreaterConstants(List<string> constants, double currentVersion)
    {
        foreach (var configuration in Configurations)
        {
            if (!TryGetRevitVersion(configuration, out var version))
                continue;

            if (version > currentVersion)
                continue;

            constants.Add($"{Constant_Revit}{FormatVersion(version)}{Constant_OrGreater}");
        }
    }

    private static string FormatVersion(double version)
    {
        return version.ToString().Replace('.', '_');
    }

    private static bool TryGetRevitVersion(string configuration, out double version)
    {
        version = 0d;

        if (configuration.Length >= 6)
        {
            if (double.TryParse(configuration.Substring(configuration.Length - 6), out version))
            {
                return true;
            }
        }

        if (configuration.Length >= 4)
        {
            if (double.TryParse(configuration.Substring(configuration.Length - 2), out version))
            {
                version += 2000;

                return true;
            }
        }

        return false;
    }
}