using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CSVFile;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public class FestivalNameStep : GeneratorStep
{
    public override Type OutputType { get; } = typeof(FestivalName);

    public override string FileName => "FestivalName.csv";

    public override string Name => "Festival Names";

    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        //Kind of redundant but should reduce the change of manual transcription errors from slipping in
        List<FestivalName> festivalNames = new();
        var reader = CSVFile.CSVReader.FromFile(Path.Combine("ManualData", "FestivalName.csv"), CSVSettings.CSV);

        foreach (var line in reader.Lines())
        {
            var festivalId = uint.Parse(line[0]);
            var festivalName = line[1];
            festivalNames.Add(new FestivalName(festivalId, festivalName));
        }
        return [..festivalNames.Select(c => c)];
    }
}
