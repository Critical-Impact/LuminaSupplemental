using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public class DownloadSQStoreStep : DownloadStep
{
    private readonly StoreParser storeParser;
    private readonly AppConfig appConfig;

    public override string Name => "Download from SQ Store";

    public DownloadSQStoreStep(StoreParser storeParser, AppConfig appConfig)
    {
        this.storeParser = storeParser;
        this.appConfig = appConfig;
    }

    public override bool ShouldRun()
    {
        return this.appConfig.Parsing.ParseOnlineSources;
    }

    public override void Run()
    {
        this.storeParser.UpdateItems();
    }
}
