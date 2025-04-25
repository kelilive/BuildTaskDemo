namespace Power.Revit.Build.Tasks;

public class PatchManifest : Task
{
    [Required] 
    public ITaskItem[] Manifests { get; set; }

    public string RevitVersion { get; set; }

    public override bool Execute()
    {
        try
        {
            if (!int.TryParse(RevitVersion, out var targetVersion))
                return true;

            foreach (var manifest in Manifests)
                PatchManifestSettings(manifest.GetMetadata(Metadata_FullPath), targetVersion);

            return true;
        }
        catch (Exception exception)
        {
            Log.LogErrorFromException(exception, false);

            return false;
        }
    }

    private void PatchManifestSettings(string manifestPath, int targetVersion)
    {
        if (targetVersion >= 2026)
            return;

        var xmlDocument = XDocument.Load(manifestPath);
        var manifestSettings = xmlDocument.Root?.Elements(XName_ManifestSettings).FirstOrDefault();

        if (manifestSettings is null) 
            return;

        Log.LogMessage(MessageImportance.High, string.Format(Msg_RemovingManifestSettings, manifestPath));
        manifestSettings.Remove();
        xmlDocument.Save(manifestPath);
    }
}