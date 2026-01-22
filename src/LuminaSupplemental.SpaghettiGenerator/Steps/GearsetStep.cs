using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

using HtmlAgilityPack;

using Lumina;
using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Extensions;
using LuminaSupplemental.SpaghettiGenerator.Generator;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

using ILogger = Serilog.ILogger;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public class GearsetStep : GeneratorStep
{
    private readonly DataCacher dataCacher;
    private readonly ILogger logger;
    private readonly ExcelSheet<Item> itemSheet;
    private readonly ExcelSheet<ClassJob> classJobSheet;
    private readonly GubalApi gubalApi;
    private readonly GameData gameData;
    private readonly AppConfig appConfig;
    private readonly Dictionary<string, uint> itemsByName;
    private Dictionary<string, uint> lodestoneToItemId;
    private Dictionary<string, uint> gameItems;

    public override Type OutputType => typeof(Gearset);

    public override string FileName => "Gearset.csv";

    public override string Name => "Gearsets";

    public HashSet<uint> bannedItems { get; set; }

    public HashSet<uint> MatchedItems { get; set; }

    public GearsetStep(DataCacher dataCacher, ILogger logger, ExcelSheet<Item> itemSheet, ExcelSheet<ClassJob> classJobSheet, GubalApi gubalApi, GameData gameData, AppConfig appConfig)
    {
        this.dataCacher = dataCacher;
        this.logger = logger;
        this.itemSheet = itemSheet;
        this.classJobSheet = classJobSheet;
        this.gubalApi = gubalApi;
        this.gameData = gameData;
        this.appConfig = appConfig;
        MatchedItems = new();
        bannedItems = new HashSet< uint >()
        {
            0,
            24225
        };
        this.itemsByName = this.dataCacher.ByName<Item>(item => item.Name.ToString().ToParseable(), item => !bannedItems.Contains(item.RowId));
    }


    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        List<Gearset> items = new List<Gearset>();

        items.AddRange(this.Parse());
        items = items.Where(c =>c.ItemId1 != 0).DistinctBy(c => (c.ItemId1, c.ItemId2, c.ItemId3, c.ItemId4, c.ItemId5, c.ItemId6, c.ItemId7, c.ItemId8, c.ItemId9, c.ItemId10,
                                          c.ItemId11, c.ItemId12, c.ItemId13, c.ItemId14))
                     .OrderBy(c => c.ItemId1)
                     .ThenBy(c => c.ItemId2)
                     .ThenBy(c => c.ItemId3)
                     .ThenBy(c => c.ItemId4)
                     .ThenBy(c => c.ItemId5)
                     .ThenBy(c => c.ItemId6)
                     .ThenBy(c => c.ItemId7)
                     .ThenBy(c => c.ItemId8)
                     .ThenBy(c => c.ItemId9)
                     .ThenBy(c => c.ItemId10)
                     .ThenBy(c => c.ItemId11)
                     .ThenBy(c => c.ItemId12)
                     .ThenBy(c => c.ItemId13)
                     .ThenBy(c => c.ItemId14)
                     .ToList();

        return [..items.Select(c => c)];
    }

    public List<Gearset> Parse()
    {
        var gearsets = new List<Gearset>();

        var lodestoneItemIds = File.ReadAllLines("./lodestone-item-id.txt");
        lodestoneToItemId = new Dictionary<string, uint>();
        for (var index = 0u; index < lodestoneItemIds.Length; index++)
        {
            var lodestoneItemId = lodestoneItemIds[index];
            if (lodestoneItemId != string.Empty)
            {
                lodestoneToItemId[lodestoneItemId] = index + 1;
            }
        }

        //Extra mapping
        this.lodestoneToItemId["08e54d3b37c"] = 40452;
        this.lodestoneToItemId["88fab817ca0"] = 13328;
        this.lodestoneToItemId["1a2a4866dba"] = 17470;
        this.lodestoneToItemId["fbdb66b15f6"] = 17470;

        var itemSheet = gameData.GetExcelSheet<Item>();

        gameItems = itemSheet.Select(c => (c.Name.ToImGuiString().ToParseable(), c.RowId)).DistinctBy(c => c.Item1).ToDictionary(c => c.Item1, c => c.RowId);


        var eorzeaCacheDir = Path.Combine(this.appConfig.Parsing.OnlineCacheDirectory,"Eorzea Cache");;
        foreach(var path in new DirectoryInfo(eorzeaCacheDir).GetFiles())
        {
            var result = ParsePage(path.FullName);
            foreach (var set in result)
            {
                var gearset = new Gearset();
                gearset.Key = path.Name.Replace(".html", "");
                gearset.Name = set.Item1;
                var setItems = set.Item2.ToList();

                for (int i = 0; i < 14; i++)
                {
                    if (i >= 0 && i < setItems.Count)
                    {
                        SetItemByIndex(gearset, (uint)i, setItems[i]);
                    }
                }
                gearsets.Add(gearset);
            }
        }

        return gearsets;
    }

    public void SetItemByIndex(Gearset gearset, uint index, uint? itemId)
    {
        if (index == 0)
        {
            gearset.ItemId1 = itemId!.Value;
        }
        else if (index == 1)
        {
            gearset.ItemId2 = itemId ?? 0;
        }
        else if (index == 2)
        {
            gearset.ItemId3 = itemId ?? 0;
        }
        else if (index == 3)
        {
            gearset.ItemId4 = itemId ?? 0;
        }
        else if (index == 4)
        {
            gearset.ItemId5 = itemId ?? 0;
        }
        else if (index == 5)
        {
            gearset.ItemId6 = itemId ?? 0;
        }
        else if (index == 6)
        {
            gearset.ItemId7 = itemId ?? 0;
        }
        else if (index == 7)
        {
            gearset.ItemId8 = itemId ?? 0;
        }
        else if (index == 8)
        {
            gearset.ItemId9 = itemId ?? 0;
        }
        else if (index == 9)
        {
            gearset.ItemId10 = itemId ?? 0;
        }
        else if (index == 10)
        {
            gearset.ItemId11 = itemId ?? 0;
        }
        else if (index == 11)
        {
            gearset.ItemId12 = itemId ?? 0;
        }
        else if (index == 12)
        {
            gearset.ItemId13 = itemId ?? 0;
        }
        else if (index == 13)
        {
            gearset.ItemId14 = itemId ?? 0;
        }
    }

    /// <summary>
    /// This code is terrible, please forgive me
    /// </summary>
    /// <param name="pagePath"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private List<(string, HashSet<uint>)> ParsePage(string pagePath)
    {
        List<(string, HashSet<uint>)> sets = new List<(string, HashSet<uint>)>();

        var doc = new HtmlDocument();
        doc.Load(pagePath);

        var titleNode = doc.DocumentNode.SelectSingleNode("//head/title");

        string titleText = titleNode != null
            ? HtmlEntity.DeEntitize(titleNode.InnerText).Trim()
            : string.Empty;

        titleText = titleText.Replace(" | Eorzea Collection", "").Trim();
        var secondaryTitleText = titleText;

        var links = doc.DocumentNode.SelectNodes("//div[@v-if='showGearNormal']//a[contains(@href,'/lodestone/playguide/db/item/')]");
        var secondaryLinks = doc.DocumentNode.SelectNodes("//div[@v-else]//a[contains(@href,'/lodestone/playguide/db/item/')]");

        if (links == null || links.Count == 0)
        {
            links = doc.DocumentNode.SelectNodes(
                "//*[contains(concat(' ', normalize-space(@class), ' '), ' b-info-box ')]//a[contains(@href,'/lodestone/playguide/db/item/')]");
        }

        if (links == null || links.Count == 0)
        {
            links = doc.DocumentNode.SelectNodes(
                "//*[" +
                "contains(concat(' ', normalize-space(@class), ' '), ' list ') and " +
                "contains(concat(' ', normalize-space(@class), ' '), ' box ')" +
                "]//a[contains(@href,'/lodestone/playguide/db/item/')]"
            );
        }

        if (links == null || links.Count == 0)
        {
            links = doc.DocumentNode.SelectNodes("//*[contains(concat(' ', normalize-space(@class), ' '), ' b-info-box ')]//a[contains(@href,'/lodestone/playguide/db/item/')]");
        }

        var itemIds = new HashSet<uint>();
        var secondaryItemIds = new HashSet<uint>();

        var missingLodestoneEntries = false;

        if (links != null)
        {
            foreach (var link in links)
            {
                var href = link.GetAttributeValue("href", "");
                if (!string.IsNullOrEmpty(href))
                {
                    var parts = href.Split('/');
                    var lodestoneId = parts.Last();
                    if (lodestoneToItemId.TryGetValue(lodestoneId, out var itemId))
                    {
                        itemIds.Add(itemId);
                    }
                    else
                    {
                        missingLodestoneEntries = true;
                        itemIds = [];
                        logger.Verbose($"Could not find lodestone item id: {lodestoneId}");
                        break;
                    }
                }
            }
        }

        if (secondaryLinks != null)
        {
            foreach (var link in secondaryLinks)
            {
                var href = link.GetAttributeValue("href", "");
                if (!string.IsNullOrEmpty(href))
                {
                    var parts = href.Split('/');
                    var lodestoneId = parts.Last();
                    if (lodestoneToItemId.TryGetValue(lodestoneId, out var itemId))
                    {
                        secondaryItemIds.Add(itemId);
                    }
                    else
                    {
                        logger.Verbose($"Could not find lodestone item id: {lodestoneId}");
                        break;
                    }
                }
            }
        }

        //handle online shit where there's no entry for whatever reason
        if (links == null || links.Count == 0 || missingLodestoneEntries)
        {
            links = doc.DocumentNode.SelectNodes(
                "//*[" +
                "contains(concat(' ', normalize-space(@class), ' '), ' list ') and " +
                "contains(concat(' ', normalize-space(@class), ' '), ' box ')" +
                "]//div[contains(concat(' ', normalize-space(@class), ' '), ' list-item-title ')]//span"
            );
            if (links != null)
            {
                foreach (var link in links)
                {
                    var itemName = HttpUtility.HtmlDecode(link.InnerText.Trim());
                    var parsedItemName = itemName.ToParseable();
                    if (gameItems.TryGetValue(parsedItemName, out var itemId))
                    {
                        itemIds.Add(itemId);
                    }
                    else
                    {
                        logger.Verbose("Could not find item with name: " + itemName);
                    }
                    var a = "";
                }
            }
        }

        if (itemIds.Count == 0 && !missingLodestoneEntries)
        {
            this.logger.Error($"Could not find any lodestone item ids: {pagePath}");
        }

        sets.Add(new ValueTuple<string, HashSet<uint>>(titleText, itemIds));

        if (secondaryItemIds.Count != 0)
        {
            var liNode = doc.DocumentNode
                .SelectNodes("//li") // select all li
                ?.FirstOrDefault(li => li.Attributes.Contains("@click") &&
                                       li.Attributes["@click"].Value == "setGearAlternative");

            if (liNode != null)
            {
                string text = HtmlEntity.DeEntitize(liNode.InnerText).Trim();
                secondaryTitleText += " - " + text;
            }
            else
            {
                logger.Verbose("Alternative gear tab not found.");
            }

            sets.Add(new ValueTuple<string, HashSet<uint>>(secondaryTitleText, secondaryItemIds));
        }

        return sets;
    }

}
