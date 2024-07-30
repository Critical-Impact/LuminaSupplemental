using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Lumina.Data;
using Lumina.Excel.GeneratedSheets;
using HtmlAgilityPack;

public class LodestoneParser {
    private readonly HttpClient client;

    public LodestoneParser(HttpClient client)
    {
        this.client = client;
    }
    public void ParsePages(string cacheDirectory)
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
                            if (response.Content.Headers.ContentEncoding.Contains("gzip"))
                            {
                                using (var responseStream = response.Content.ReadAsStream())
                                using (var decompressedStream = new System.IO.Compression.GZipStream(responseStream, System.IO.Compression.CompressionMode.Decompress))
                                using (var streamReader = new StreamReader(decompressedStream))
                                {
                                    string responseBody = streamReader.ReadToEndAsync().Result;
                                    Console.WriteLine($"Cached Lodestone Info: {itemId.Value} - at " + index2 + "/" + itemIdMap.Count);
                                    File.WriteAllText(cacheFile, responseBody);
                                    Thread.Sleep( random.Next(200,800) );
                                }
                            }
                            else
                            {
                                using (var responseStream = response.Content.ReadAsStream())
                                using (var streamReader = new StreamReader(responseStream,Encoding.UTF8))
                                {
                                    string responseBody = streamReader.ReadToEndAsync().Result;
                                
                                    Console.WriteLine($"Cached Lodestone Info: {itemId.Value} - at " + index2 + "/" + itemIdMap.Count);
                                    File.WriteAllText(cacheFile, responseBody);
                                    Thread.Sleep( random.Next(200,800) );
                                    ;
                                }
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
}
