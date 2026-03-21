using System;
using System.Collections.Generic;

using CsvHelper.Configuration.Attributes;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace LuminaSupplemental.Excel.Model;

public class BGMOrchestrion : ICsv
{
    [Name("BGMId")] public uint BGMId { get; set; }
    [Name("OrchestrionId")] public uint OrchestrionId { get; set; }
    [Name("Name")] public string Name { get; set; }

    public RowRef< BGM > BGM;
    public RowRef< Orchestrion > Orchestrion;

    public BGMOrchestrion(uint bgmId, uint orchestrionId, string name)
    {
        this.BGMId = bgmId;
        this.OrchestrionId = orchestrionId;
        this.Name = name;
    }

    public BGMOrchestrion()
    {

    }

    public void FromCsv(string[] lineData)
    {
        BGMId = uint.Parse( lineData[ 0 ] );
        OrchestrionId = uint.Parse( lineData[ 1 ] );
        Name = lineData[ 2 ];
    }

    public string[] ToCsv()
    {
        List<String> data = new List<string>()
        {
            BGMId.ToString(),
            OrchestrionId.ToString(),
            Name
        };
        return data.ToArray();
    }

    public bool IncludeInCsv()
    {
        return true;
    }

    public void PopulateData(ExcelModule module, Language language)
    {
        this.BGM = new RowRef<BGM>( module, this.BGMId);
        this.Orchestrion = new RowRef<Orchestrion>( module, this.OrchestrionId);
    }
}
