using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using Lumina.Data;
using Lumina.Excel.GeneratedSheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class MobDropStep
{
    public List<MobDrop> ProcessLodestoneDrops()
    {
        var itemIdMapFile = "./lodestone-item-id.txt";
        var itemIdData = File.ReadLines(itemIdMapFile).ToList();
        var itemIdMap = new Dictionary<int, string>();
        for (var index = 0; index < itemIdData.Count; index++)
        {
            var line = itemIdData[index];
            if (line != "")
            {
                itemIdMap.Add(index, line);
            }
        }

        var allMobs = gameData.Excel.GetSheet<BNpcName>(Language.English);
        var parsedNames = allMobs.Where(c => c.Singular.ToString() != "").GroupBy(c => c.Singular.ToString().ToParseable()).ToDictionary(c => c.Key, c => c);

        var drops = new List<MobDrop>();
        var storeProductCacheDirectory = Path.Combine(this.appConfig.Parsing.OnlineCacheDirectory,"FFXIV Lodestone Cache");
        Directory.CreateDirectory(storeProductCacheDirectory);
        //This is very shit code but parsing 30k HTML files is slow and I'll do everything I can to speed it up
        foreach (var itemId in itemIdMap.AsParallel())
        {
            var actualItemId = itemId.Key;
            var cacheFile = Path.Combine(storeProductCacheDirectory, $"{actualItemId - 1}.html");
            if (File.Exists(cacheFile))
            {
                string html = File.ReadAllText(cacheFile);

                if (!html.Contains("Dropped By"))
                {
                    //Console.WriteLine("Skipped " + cacheFile);
                    continue;
                }

                string tablePattern = "<table class=\"db-table db-table__item_source\">[\\s\\S]*?Dropped By[\\s\\S]*?</table>";
                Regex regex = new Regex(tablePattern);
                Match match = regex.Match(html);
                //Console.WriteLine("Checked " + cacheFile);
                if (match.Success)
                {
                    string tableHtml = match.Value;

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(tableHtml);

                    foreach (var table in doc.DocumentNode.SelectNodes("//table"))
                    {
                        if (!table.OuterHtml.Contains("Dropped By")) continue;
                        var tbody = table.SelectSingleNode("tbody");
                        List<string[]> tableData = new List<string[]>();

                        foreach (HtmlNode row in tbody.SelectNodes("tr"))
                        {
                            string[] rowData = new string[2];
                            HtmlNodeCollection cells = row.SelectNodes("td");

                            if (cells != null && cells.Count >= 2)
                            {
                                rowData[0] = cells[0].InnerText.Trim(); // First column (Mob Name)
                                rowData[1] = cells[1].InnerText.Trim(); // Second column (Location)
                                tableData.Add(rowData);
                            }
                        }

                        // Print the extracted data
                        foreach (string[] rowData in tableData)
                        {
                            string mobName = rowData[0];
                            string location = rowData[1];

                            var bnpcName = mobName.ToParseable();
                            if (parsedNames.ContainsKey(bnpcName))
                            {
                                var bNpcNameIds = parsedNames[bnpcName];
                                foreach (var bNpcNameId in bNpcNameIds)
                                {
                                    var mobDrop = new MobDrop();
                                    mobDrop.ItemId = (uint)actualItemId;
                                    mobDrop.BNpcNameId = bNpcNameId.RowId;
                                    drops.Add(mobDrop);
                                }
                            }
                        }
                    }
                }
            }
        }

        return drops;
    }
}
