using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CSVFile;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public sealed class FauxHollowsPatternStep : GeneratorStep
{
    public override Type OutputType => typeof(FauxHollowsPattern);

    public override string FileName => "FauxHollowsPattern.csv";

    public override string Name => "Faux Hollows Patterns";

    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        var reader = CSVReader.FromFile(Path.Combine("ManualData", "FauxHollowsPattern.csv"), CSVSettings.CSV);
        return reader.Lines()
            .Select(line =>
            {
                var pattern = new FauxHollowsPattern();
                pattern.FromCsv(line);
                return (ICsv)pattern;
            })
            .ToList();
    }
}
