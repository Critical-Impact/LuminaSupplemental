using System;
using System.Collections.Generic;

using CsvHelper.Configuration.Attributes;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace LuminaSupplemental.Excel.Model;

public class RelicWeapon : ICsv
{
    [Name("RowId")] public uint RowId { get; set; }
    [Name("ItemId")] public uint ItemId { get; set; }
    [Name("OffhandItemId")] public uint OffhandItemId { get; set; }
    [Name("Stage")] public uint Stage { get; set; }
    [Name("ClassJobId")] public uint ClassJobId { get; set; }
    [Name("Category")] [TypeConverter(typeof(EnumConverter))] public RelicWeaponCategory Category { get; set; }
    [Name("Type")] [TypeConverter(typeof(EnumConverter))] public RelicWeaponType Type { get; set; }
    [Name("QuestId")] public uint QuestId { get; set; }
    [Name("PreviousRelicWeaponId")] public uint PreviousRelicWeaponId { get; set; }

    public RowRef<Item> Item;
    public RowRef<Item> OffhandItem;
    public RowRef<ClassJob> ClassJob;
    public RowRef<Quest> Quest;

    public RelicWeapon(uint rowId, uint itemId, uint offhandItemId, uint stage, uint classJobId, RelicWeaponCategory category, RelicWeaponType type, uint questId, uint previousRelicWeaponId)
    {
        this.RowId = rowId;
        this.ItemId = itemId;
        this.OffhandItemId = offhandItemId;
        this.Stage = stage;
        this.ClassJobId = classJobId;
        this.Category = category;
        this.Type = type;
        this.QuestId = questId;
        this.PreviousRelicWeaponId = previousRelicWeaponId;
    }

    public RelicWeapon()
    {

    }

    public void FromCsv(string[] lineData)
    {
        this.RowId = uint.Parse(lineData[0]);
        this.ItemId = uint.Parse(lineData[1]);
        this.OffhandItemId = uint.Parse(lineData[2]);
        this.Stage = uint.Parse(lineData[3]);
        this.ClassJobId = uint.Parse(lineData[4]);
        this.Category = (RelicWeaponCategory)Enum.Parse(typeof(RelicWeaponCategory), lineData[5]);
        this.Type = (RelicWeaponType)Enum.Parse(typeof(RelicWeaponType), lineData[6]);
        this.QuestId = uint.Parse(lineData[7]);
        this.PreviousRelicWeaponId = uint.Parse(lineData[8]);
    }

    public string[] ToCsv()
    {
        List<string> data = new List<string>()
        {
            this.RowId.ToString(),
            this.ItemId.ToString(),
            this.OffhandItemId.ToString(),
            this.Stage.ToString(),
            this.ClassJobId.ToString(),
            ((int)this.Category).ToString(),
            ((int)this.Type).ToString(),
            this.QuestId.ToString(),
            this.PreviousRelicWeaponId.ToString(),
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
        this.OffhandItem = new RowRef<Item>(module, this.OffhandItemId);
        this.ClassJob = new RowRef<ClassJob>(module, this.ClassJobId);
        this.Quest = new RowRef<Quest>(module, this.QuestId);
    }
}
