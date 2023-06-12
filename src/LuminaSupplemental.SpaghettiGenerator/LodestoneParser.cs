using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Lumina.Data;
using Lumina.Excel.GeneratedSheets;
using HtmlAgilityPack;
using Newtonsoft.Json;

public class LodestoneParser {

    public static void ParsePages(string cacheDirectory)
    {
        var storeProductCacheDirectory = Path.Combine(cacheDirectory,"FFXIV Lodestone Cache");
        var itemIdMapFile = "./lodestone-item-id.txt";
        var itemIdData = File.ReadLines( itemIdMapFile ).ToList();
        var itemIdMap = new Dictionary< int, string >();
        for( var index = 0; index < itemIdData.Count; index++ )
        {
            var line = itemIdData[ index ];
            if( line != "" )
            {
                var cacheFile = Path.Combine(storeProductCacheDirectory, $"{index}.html");
                if( !File.Exists( cacheFile ) )
                {
                    itemIdMap.Add( index, line );
                }
            }
        }


        Directory.CreateDirectory(storeProductCacheDirectory);
        var client = Program.client;
        var random = new Random();
        var index2 = 0;
        foreach( var itemId in itemIdMap )
        {
            var cacheFile = Path.Combine(storeProductCacheDirectory, $"{itemId.Key}.html");
            if( !File.Exists( cacheFile ) )
            {
                try
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"https://na.finalfantasyxiv.com/lodestone/playguide/db/item/{itemId.Value}/?patch=")))
                    {
                        request.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,*/*;q=0.8");
                        request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                        request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
                        request.Headers.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");
                        request.Headers.TryAddWithoutValidation("Cookie", "ldst_touchstone=1; ldst_is_support_browser=1;");
                        request.Headers.TryAddWithoutValidation("Accept-Language", "en-GB,en;q=0.5");

                        using (var response = client.SendAsync(request).WaitAsync( TimeSpan.FromSeconds( 20 ) ).Result)
                        {
                            response.EnsureSuccessStatusCode();
                            using (var responseStream = response.Content.ReadAsStream())
                            using (var streamReader = new StreamReader(responseStream,Encoding.UTF8))
                            {
                                string responseBody = streamReader.ReadToEndAsync().Result;
                                
                                Console.WriteLine($"Cached Lodestone Info: {itemId.Value} - at " + index2 + "/" + itemIdMap.Count);
                                File.WriteAllText(cacheFile, responseBody);
                                Thread.Sleep( random.Next(200,500) );
                                ;
                            }
                        }
                    }

                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught while parsing " + itemId.Value);
                    Console.WriteLine("Message :{0} ", e.Message);
                }
                catch (TimeoutException e)
                {
                    Console.WriteLine("\nTimed out on " + itemId.Value); Console.WriteLine("Message :{0} ", e.Message);
                }
                catch (AggregateException e)
                {
                    foreach( var exception in e.InnerExceptions )
                    {
                        if(exception is HttpRequestException re)
                        {
                            Console.WriteLine("\nException Caught while parsing " + itemId.Value);
                            Console.WriteLine("Message :{0} ", re.Message);
                        }
                        else if(exception is TimeoutException te)
                        {
                            Console.WriteLine("\nTimed out on " + itemId.Value); Console.WriteLine("Message :{0} ", te.Message);
                        }
                        else
                        {
                            Console.WriteLine("\nUnknown exception while parsing " + itemId.Value); Console.WriteLine("Message :{0} ", exception.Message);
                        }
                    }
                }
            }
            index2++;
        }
    }

    public static List< MobDrop > ParseMobDrops()
    {
         var itemIdMapFile = "./lodestone-item-id.txt";
        var itemIdData = File.ReadLines( itemIdMapFile ).ToList();
        var itemIdMap = new Dictionary< int, string >();
        for( var index = 0; index < itemIdData.Count; index++ )
        {
            var line = itemIdData[ index ];
            if( line != "" )
            {
                itemIdMap.Add( index, line );
            }
        }
        var allMobs = Service.GameData.Excel.GetSheet<BNpcName>(Language.English);
        var parsedNames = allMobs.Where( c => c.Singular.ToString() != "" ).GroupBy( c => c.Singular.ToString().ToParseable() ).ToDictionary( c => c.Key, c => c );
        
        var drops = new List< MobDrop >();
        var storeProductCacheDirectory = "./FFXIV Lodestone Cache";
        Directory.CreateDirectory(storeProductCacheDirectory);
        //This is very shit code but parsing 30k HTML files is slow and I'll do everything I can to speed it up
        foreach(var itemId in  itemIdMap.AsParallel())
        {
            var actualItemId = itemId.Key;
            var cacheFile = Path.Combine(storeProductCacheDirectory, $"{actualItemId - 1}.html");
            if( File.Exists( cacheFile ) )
            {
                
                string html = File.ReadAllText( cacheFile );

                if( !html.Contains( "Dropped By" ) )
                {
                    Console.WriteLine("Skipped " + cacheFile);
                    continue;
                }

                string tablePattern = "<table class=\"db-table db-table__item_source\">[\\s\\S]*?Dropped By[\\s\\S]*?</table>";
                Regex regex = new Regex(tablePattern);
                Match match = regex.Match(html);
                Console.WriteLine("Checked " + cacheFile);
                if (match.Success)
                {
                    string tableHtml = match.Value;

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(tableHtml);

                    foreach(var table in doc.DocumentNode.SelectNodes("//table"))
                    {
                        if( !table.OuterHtml.Contains( "Dropped By" ) ) continue;
                        var tbody = table.SelectSingleNode( "tbody" );
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
                            if( parsedNames.ContainsKey( bnpcName ) )
                            {
                                var bNpcNameIds = parsedNames[ bnpcName ];
                                foreach( var bNpcNameId in bNpcNameIds )
                                {
                                    var mobDrop = new MobDrop();
                                    mobDrop.ItemId = (uint)actualItemId;
                                    mobDrop.BNpcNameId = bNpcNameId.RowId;
                                    drops.Add( mobDrop );
                                }
                            }
                        }
                    }
                }                
                
                // var doc = new HtmlDocument();
                // doc.Load( cacheFile );
                // Console.WriteLine("Processing " + cacheFile);
                // var sourceLinks = doc.DocumentNode.SelectNodes("//table[@class=\"db-table db-table__item_source\"]//tbody//tr//td//a[@class=\"db_popup db-table__txt--detail_link\"]");
                // if( sourceLinks != null )
                // {
                //     foreach( var sourceLink in sourceLinks )
                //     {
                //         var bnpcName = sourceLink.InnerText.ToParseable();
                //         if( parsedNames.ContainsKey( bnpcName ) )
                //         {
                //             var bNpcNameIds = parsedNames[ bnpcName ];
                //             foreach( var bNpcNameId in bNpcNameIds )
                //             {
                //                 var mobDrop = new MobDrop();
                //                 mobDrop.ItemId = (uint)itemId.Key;
                //                 mobDrop.BNpcNameId = bNpcNameId.RowId;
                //                 drops.Add( mobDrop );
                //             }
                //         }
                //
                //     }
                // }
                //Hack
            }
        } 

        return drops;
    }
}
