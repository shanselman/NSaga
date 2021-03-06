public class BuildParameters
{
    public String Solution = "./src/NSaga.sln";
    public string Artefacts = "./artefacts/";
    public string ArtefactsBin = "./artefacts/bin/";

    public string NSagaBinDir { get; private set; }
    public string AutofacBinDir { get; private set; }
    public string SimpleInjectorBinDir { get; private set; }
    public string StructureMapBinDir { get; private set; }
    public string AzureTablesBinDir { get; private set; }


    public string Target { get; private set; }
    public string Configuration { get; private set; }
    public bool IsLocalBuild { get; private set; }
    public bool IsRunningOnAppVeyor { get; private set; }

    public ReleaseNotes ReleaseNotes { get; private set; }
    public bool IsMasterBranch { get; private set; }

    public string Version { get; private set; }
    public string SemVersion { get; private set; }


    public void Initialize(ICakeContext context)
    {
        context.Information("Executing GitVersion");
        var result = context.GitVersion(new GitVersionSettings{
            UpdateAssemblyInfoFilePath = "./SolutionInfo.cs",
            UpdateAssemblyInfo = true,
        });
        Version = result.MajorMinorPatch ?? "0.1.0";
        SemVersion = result.LegacySemVerPadded ?? "0.1.0";

		// print gitversion
        context.GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = false,
            OutputType = GitVersionOutput.BuildServer
        });
    }

    public static BuildParameters GetParameters(ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        var target = context.Argument("target", "Default");
        var configuration = context.Argument("configuration", "Release");
        var buildSystem = context.BuildSystem();
		var isMaster = StringComparer.OrdinalIgnoreCase.Equals("master", buildSystem.AppVeyor.Environment.Repository.Branch);

		context.Information("IsMasterBranch: {0}", isMaster);

        return new BuildParameters {
            Target = target,
            Configuration = configuration,
            IsLocalBuild = buildSystem.IsLocalBuild,
            IsRunningOnAppVeyor = buildSystem.AppVeyor.IsRunningOnAppVeyor,
            IsMasterBranch = isMaster,
            ReleaseNotes = context.ParseReleaseNotes("./ReleaseNotes.md"),

            NSagaBinDir = "./src/NSaga/bin/" + configuration + "/",
            AutofacBinDir = "./src/NSaga.Autofac/bin/" + configuration + "/",
            SimpleInjectorBinDir = "./src/NSaga.SimpleInjector/bin/" + configuration + "/",
            StructureMapBinDir = "./src/NSaga.StructureMap/bin/" + configuration + "/",
            AzureTablesBinDir = "./src/NSaga.AzureTables/bin/" + configuration + "/",
        };
    }
}

