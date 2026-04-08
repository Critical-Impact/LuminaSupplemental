using System;
using System.Collections.Generic;

using CsvHelper.Configuration.Attributes;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace LuminaSupplemental.Excel.Model;

public class RelicTool : ICsv
{
    [Name("RowId")] public uint RowId { get; set; }
    [Name("ItemId")] public uint ItemId { get; set; }
    [Name("Stage")] public uint Stage { get; set; }
    [Name("ClassJobId")] public uint ClassJobId { get; set; }
    [Name("Category")] [TypeConverter(typeof(EnumConverter))] public RelicToolCategory Category { get; set; }
    [Name("Type")] [TypeConverter(typeof(EnumConverter))] public RelicToolType Type { get; set; }
    [Name("QuestId")] public uint QuestId { get; set; }
    [Name("PreviousRelicToolId")] public uint PreviousRelicToolId { get; set; }

    public RowRef<Item> Item;
    public RowRef<ClassJob> ClassJob;
    public RowRef<Quest> Quest;

    public RelicTool(uint rowId, uint itemId, uint stage, uint classJobId, RelicToolCategory category, RelicToolType type, uint questId, uint previousRelicToolId)
    {
        this.RowId = rowId;
        this.ItemId = itemId;
        this.Stage = stage;
        this.ClassJobId = classJobId;
        this.Category = category;
        this.Type = type;
        this.QuestId = questId;
        this.PreviousRelicToolId = previousRelicToolId;
    }

    public RelicTool()
    {

    }

    public void FromCsv(string[] lineData)
    {
        this.RowId = uint.Parse(lineData[0]);
        this.ItemId = uint.Parse(lineData[1]);
        this.Stage = uint.Parse(lineData[2]);
        this.ClassJobId = uint.Parse(lineData[3]);
        this.Category = (RelicToolCategory)Enum.Parse(typeof(RelicToolCategory), lineData[4]);
        this.Type = (RelicToolType)Enum.Parse(typeof(RelicToolType), lineData[5]);
        this.QuestId = uint.Parse(lineData[6]);
        this.PreviousRelicToolId = uint.Parse(lineData[7]);
    }

    public string[] ToCsv()
    {
        List<string> data = new List<string>()
        {
            this.RowId.ToString(),
            this.ItemId.ToString(),
            this.Stage.ToString(),
            this.ClassJobId.ToString(),
            ((int)this.Category).ToString(),
            ((int)this.Type).ToString(),
            this.QuestId.ToString(),
            this.PreviousRelicToolId.ToString(),
        };

        return data.ToArray();
    }

    public bool IncludeInCsv()
    {
        return false;
    }

    public void PopulateData(ExcelModule module, Language language)
    {
        this.Item = new RowRef<Item>(module, this.ItemId);
        this.ClassJob = new RowRef<ClassJob>(module, this.ClassJobId);
        this.Quest = new RowRef<Quest>(module, this.QuestId);
    }
}
