using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CSVFile;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Extensions;
using LuminaSupplemental.SpaghettiGenerator.Generator;

using Serilog;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public class BGMOrchestrionStep : GeneratorStep
{
    private readonly ExcelSheet<Orchestrion> orchestrionSheet;
    private readonly ILogger logger;

    public override Type OutputType => typeof(BGMOrchestrion);

    public override string FileName => "BGMOrchestrion.csv";

    public override string Name => "BGM Orchestrion";

    public BGMOrchestrionStep(ExcelSheet<Orchestrion> orchestrionSheet, ILogger logger)
    {
        this.orchestrionSheet = orchestrionSheet;
        this.logger = logger;
    }

    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        var orchestrionDict = orchestrionSheet.Where(c => !c.Name.IsEmpty).ToDictionary(c => c.Name.ToImGuiString().ToParseable(), c => c.RowId);
        var reader = CSVFile.CSVReader.FromFile("xiv_bgm_en.csv", CSVSettings.CSV);
        var bgmOrchestrions = new List<BGMOrchestrion>();
        foreach (var item in reader)
        {
            var bgmId = uint.Parse(item[0]);
            var bgmTitle = item[1]!;
            if (bgmTitle == "None" || bgmTitle == "")
            {
                continue;
            }
            var bgmParseable = bgmTitle.ToParseable();
            if (orchestrionDict.TryGetValue(bgmParseable, out var orchestrion))
            {

            }
            else
            {
                logger.Information("Could not find orchestrion with name " + bgmTitle);
                orchestrion = 0;
            }

            bgmOrchestrions.Add(new BGMOrchestrion(bgmId, orchestrion, orchestrion == 0 ? bgmTitle : ""));

        }

        return bgmOrchestrions.Select(ICsv (c) => c).ToList();
    }
}
