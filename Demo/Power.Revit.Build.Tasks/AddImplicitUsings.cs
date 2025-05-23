namespace Power.Revit.Build.Tasks;

public class AddImplicitUsings : Task
{
    [Required]
    public ITaskItem[] AdditionalUsings { get; set; }

    [Required]
    public ITaskItem[] References { get; set; }

    [Output]
    public string[] Usings { get; private set; }

    public override bool Execute()
    {
        try
        {
            var usings = new List<string>();

            foreach (var additionalUsing in AdditionalUsings)
            {
                var requiredPackage = additionalUsing.GetMetadata(Metadata_RequiredReference);

                if (string.IsNullOrEmpty(requiredPackage))
                {
                    usings.Add(additionalUsing.ItemSpec);
                }

                else
                {
                    var existedPackage = References.FirstOrDefault(item => item.ItemSpec.EndsWith(requiredPackage));

                    if (existedPackage is not null)
                        usings.Add(additionalUsing.ItemSpec);
                }
            }

            Usings = [.. usings];

            return true;
        }
        catch (Exception exception)
        {
            Log.LogErrorFromException(exception, false);

            return false;
        }
    }
}