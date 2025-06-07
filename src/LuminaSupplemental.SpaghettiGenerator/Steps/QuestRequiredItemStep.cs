using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public class QuestRequiredItemStep : GeneratorStep
{
    public override Type OutputType { get; } = typeof(QuestRequiredItem);

    public override string FileName { get; } = "QuestRequiredItem.csv";

    public override string Name { get; } = "Required Quest Items";

    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        var reader = CSVFile.CSVReader.FromFile(Path.Combine("ManualData", "QuestRequiredItems.csv"));
        var items = new List<QuestRequiredItem>();
        foreach (var line in reader.Lines())
        {
            var questId = uint.Parse(line[0], CultureInfo.InvariantCulture);
            var itemId = uint.Parse(line[1], CultureInfo.InvariantCulture);
            var quantity = uint.Parse(line[2], CultureInfo.InvariantCulture);
            var isHq = bool.Parse(line[3]);
            items.Add(new QuestRequiredItem()
            {
                QuestId = questId,
                ItemId = itemId,
                Quantity = quantity,
                IsHq = isHq
            });
        }

        return [..items.Select(c => c).OrderBy(c => c.QuestId).ThenBy(c => c.ItemId)];
    }
}
