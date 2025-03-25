using System;
using System.Collections.Generic;
using System.Linq;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

using Serilog;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class FateItemStep : GeneratorStep
{
    private readonly GubalApi gubalApi;
    private readonly ExcelSheet<Fate> fateSheet;
    private readonly ILogger logger;

    public override Type OutputType => typeof(FateItem);

    public override string FileName => "FateItem.csv";

    public override string Name => "Fate Items";


    public FateItemStep(GubalApi gubalApi, ExcelSheet<Fate> fateSheet, ILogger logger)
    {
        this.gubalApi = gubalApi;
        this.fateSheet = fateSheet;
        this.logger = logger;
    }


    public override List<ICsv> Run()
    {
        List<FateItem> items = new List<FateItem>();
        items.AddRange(this.ProcessGubalData());
        items = items.DistinctBy(c => (c.ItemId, c.FateId)).ToList();

        return [..items.Select(c => c).OrderBy(c => c.FateId).ThenBy(c => c.ItemId)];
    }

}
