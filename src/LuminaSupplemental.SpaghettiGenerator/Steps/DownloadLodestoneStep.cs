using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public class DownloadLodestoneStep : DownloadStep
{
    private readonly LodestoneParser lodestoneParser;
    private readonly AppConfig appConfig;

    public override string Name => "Parse Lodestone";

    public DownloadLodestoneStep(LodestoneParser lodestoneParser, AppConfig appConfig)
    {
        this.lodestoneParser = lodestoneParser;
        this.appConfig = appConfig;
    }
    
    public override bool ShouldRun()
    {
        return this.appConfig.Parsing.ParseOnlineSources;
    }

    public override void Run()
    {
        this.lodestoneParser.ParsePages(this.appConfig.Parsing.OnlineCacheDirectory);
    }
}
