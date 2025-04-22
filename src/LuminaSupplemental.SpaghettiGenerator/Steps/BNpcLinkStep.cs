using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class BNpcLinkStep : GeneratorStep
{
    public override Type OutputType => typeof(BNpcLink);

    public override string FileName => "BNpcLink.csv";

    public override string Name => "BNPC Links";

    public override List<ICsv> Run()
    {
        List<BNpcLink> items = new ();
        items.AddRange(this.Process());

        return [..items.Select(c => c)];
    }

    private List<BNpcLink> Process()
    {
        List<MobSpawnPosition> positions = new();
        var importFiles = Directory.GetFiles( Path.Join(  AppDomain.CurrentDomain.BaseDirectory, "ManualData", "MobImports"), "*.csv" );
        foreach( var importFile in importFiles )
        {
            var reader = CSVFile.CSVReader.FromFile( importFile );
            var lines = reader.Lines();
            foreach( var line in lines )
            {
                MobSpawnPosition mobSpawnPosition = new MobSpawnPosition();
                mobSpawnPosition.FromCsv(line);
                positions.Add(mobSpawnPosition);
            }


        }

        return positions.Select(e => (e.BNpcBaseId, e.BNpcNameId)).Distinct().Where(c => c.BNpcBaseId != 0 && c.BNpcNameId != 0).Select(c => new BNpcLink(c.BNpcNameId, c.BNpcBaseId)).OrderBy(c => c.BNpcNameId).ThenBy(c => c.BNpcBaseId).ToList();
    }
}
