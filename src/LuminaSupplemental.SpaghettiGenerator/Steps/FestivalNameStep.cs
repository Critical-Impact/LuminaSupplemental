using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CSVFile;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public class FestivalNameStep : IGeneratorStep
{
    public Type OutputType { get; set; } = typeof(FestivalName);

    public string FileName { get; set; } = "FestivalName.csv";

    public string Name { get; set; } = "Festival Names";

    public bool ShouldRun()
    {
        return true;
    }

    public List<ICsv> Run()
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
