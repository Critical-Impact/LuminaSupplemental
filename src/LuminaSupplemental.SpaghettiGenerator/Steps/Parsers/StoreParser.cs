using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

using Lumina.Data;
using Lumina.Excel.Sheets;

using Newtonsoft.Json;

namespace LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

public class StoreParser {
    private readonly AppConfig appConfig;

    public static Dictionary<uint, Dictionary<uint, Product>> StoreItems = new();
    public static Dictionary<uint, Product> StoreProducts = new();

    public static string UpdateStatus = string.Empty;

    public class ProductItem {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("number")]
        public int Number;
    }

    public class Product {
        [JsonProperty("id")]
        public uint ID;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("priceText")]
        public string PriceText;

        [JsonProperty("items")]
        public List<ProductItem> Items;
    }

    public class ProductListing {
        [JsonProperty("status")]
        public int Status;

        [JsonProperty("product")]
        public Product Product;
    }

    public class ProductList {
        [JsonProperty("status")]
        public int Status;

        [JsonProperty("products")]
        public List<Product> Products;
    }

    public StoreParser(AppConfig appConfig)
    {
        this.appConfig = appConfig;
    }

    public static Dictionary<string, uint> idReplacements = new()
    {
        {"Ballroom Etiquette - Improper Praise #1",22378},
        {"Ballroom Etiquette - Improper Praise #2",22379},
        {"Ballroom Etiquette - Improper Praise #3",22380},
        {"Ballroom Etiquette - Improper Praise #4",40359},
        {"Ballroom Etiquette - Improper Praise #5",40360},
        {"Ballroom Etiquette - Improper Praise #6",40361},
        {"Authentic Valentione's Day Advertisement",21872},
        {"Authentic Lovely Little Ladies' Day Advertisement",41153},
        {"Authentic Little Ladies' Day Advertisement",21872},
        {"Authentic Hippity-hoppity Hatching-tide Advertisement",41154},
        {"Authentic Hatching-tide Advertisement", 32969},
        {"Authentic Moonfire Faire Advertisement", 30429},
        {"Second Edition Moonfire Faire", 23339},
        {"Authentic All Saints' Wake Advertisement Faire", 21311},
        {"Authentic Rising Advertisement", 44332},
        {"Authentic Fantastical All Saints' Wake Advertisement", 31713},
        {"Authentic Choir Concert Advertisement", 32961},
    };

    //Apparently SQ haven't updated their own naming
    public static Dictionary<string, string> replacements = new()
    {
        {"Far Eastern Performer's Dogi","Far Eastern Performer's Halfrobe"},
        {"Far Eastern Performer's Kyakui","Far Eastern Performer's Trousers"},
        {"Far Eastern Performer's Zori","Far Eastern Performer's Shoes"},
        {"Eastern Lord Errant's Hat","Far Eastern Lord Errant's Hat"},
        {"Eastern Lord Errant's Jacket","Far Eastern Lord Errant's Jacket"},
        {"Eastern Lord Errant's Wristbands","Far Eastern Lord Errant's Wristbands"},
        {"Eastern Lord Errant's Trousers","Far Eastern Lord Errant's Trousers"},
        {"Eastern Lord Errant's Shoes","Far Eastern Lord Errant's Shoes"},
        {"Eastern Lady Errant's Hat","Far Eastern Lady Errant's Hat"},
        {"Eastern Lady Errant's Jacket","Far Eastern Lady Errant's Jacket"},
        {"Eastern Lady Errant's Wristbands","Far Eastern Lady Errant's Wristbands"},
        {"Eastern Lady Errant's Trousers","Far Eastern Lady Errant's Trousers"},
        {"Eastern Lady Errant's Shoes","Far Eastern Lady Errant's Shoes"},
        {"Shadow Wolf Whistle","Shadow Wolf Horn"},
        {"Eastern Journey Attire Coffer","Far Eastern Journey Attire Coffer"},
        {"Mun'gaek Hat","Far Eastern Enforcer's Hat"},
        {"Mun'gaek Uibok","Far Eastern Enforcer's Robe"},
        {"Mun'gaek Cuffs","Far Eastern Enforcer's Cuffs"},
        {"Mun'gaek Trousers","Far Eastern Enforcer's Trousers"},
        {"Mun'gaek Boots","Far Eastern Enforcer's Boots"},

        {"Eastern Socialite's Hat","Far Eastern Socialite's Hat"},
        {"Eastern Socialite's Cheongsam","Far Eastern Socialite's Dress"},
        {"Eastern Socialite's Gloves","Far Eastern Socialite's Gloves"},
        {"Eastern Socialite's Skirt","Far Eastern Socialite's Skirt"},
        {"Eastern Socialite's Boots","Far Eastern Socialite's Boots"},

        {"Eastern Lady Errant's Coat","Far Eastern Lady Errant's Coat"},
        {"Eastern Lady Errant's Gloves","Far Eastern Lady Errant's Gloves"},
        {"Eastern Lady Errant's Skirt","Far Eastern Lady Errant's Skirt"},
        {"Eastern Lady Errant's Boots","Far Eastern Lady Errant's Boots"},

        {"Nezha Lord's Togi","Nezha Lord's Jacket"},
        {"Nezha Lady's Togi","Nezha Lord's Jacket"},

        {"Far Eastern Gentleman's Haidate","Far Eastern Gentleman's Trousers"},
        {"Far Eastern Beauty's Koshita","Far Eastern Beauty's Skirt"},
        {"Eastern Lord's Togi","Far Eastern Lord's Robe"},
        {"Eastern Lord's Trousers","Far Eastern Lord's Trousers"},
        {"Eastern Lord's Crakows","Far Eastern Lord's Crakows"},
        {"Eastern Lady's Togi","Far Eastern Lady's Dress"},
        {"Eastern Lady's Loincloth","Far Eastern Lady's Loincloth"},
        {"Eastern Lady's Crakows","Far Eastern Lady's Crakows"},
    };


    public void UpdateItems() {
        UpdateStatus = "Fetching Product List";
        using var wc = new WebClient();
        var json = wc.DownloadString("https://api.store.finalfantasyxiv.com/ffxivcatalog/api/products/?lang=en-us&currency=USD&limit=10000");
        var productList = JsonConvert.DeserializeObject<ProductList>(json);
        if (productList == null) {
            UpdateStatus = "[Error] " + UpdateStatus;;
            return;
        }

        StoreItems.Clear();
        var storeProductCacheDirectory = Path.Combine(this.appConfig.Parsing.OnlineCacheDirectory,"FFXIV Store Cache");;
        Directory.CreateDirectory(storeProductCacheDirectory);

        var allItems = Service.GameData.Excel.GetSheet<Item>(Language.English);
        if (allItems == null) {
            UpdateStatus = "[Error] " + UpdateStatus;;
            return;
        }
        StoreProducts.Clear();
        for (var i = 0; i < productList.Products.Count; i++) {
            var p = productList.Products[i];
            try {
                string fullProductJson = null;

                var cacheFile = Path.Combine(storeProductCacheDirectory, $"{p.ID}.json");
                var usingCache = false;
                if (File.Exists(cacheFile)) {
                    UpdateStatus = $"Fetching Store Items: {i}/{productList.Products.Count} [{p.ID}, Cached]";

                    usingCache = true;
                    fullProductJson = File.ReadAllText(cacheFile);
                } else {
                    UpdateStatus = $"Fetching Store Items: {i}/{productList.Products.Count} [{p.ID}, from Store]";
                    fullProductJson = wc.DownloadString($"https://api.store.finalfantasyxiv.com/ffxivcatalog/api/products/{p.ID}?lang=en-us&currency=USD");
                }

                var productListing = JsonConvert.DeserializeObject<ProductListing>(fullProductJson);
                if (productListing?.Product == null) continue;
                if (productListing.Product.Items == null) {
                    Console.WriteLine($"{p.Name} has no Items?");
                } else {
                    StoreProducts.Add(p.ID, productListing.Product);

                    foreach (var item in productListing.Product.Items) {
                        var originalName = item.Name;
                        var name = item.Name;
                        if (replacements.TryGetValue(originalName, out var replacement))
                        {
                            name = replacement;
                        }

                        if (idReplacements.TryGetValue(originalName, out var idReplacement))
                        {
                            if (!StoreItems.ContainsKey(idReplacement)) {
                                StoreItems.Add(idReplacement, new Dictionary<uint, Product>());
                            }

                            if (!StoreItems[idReplacement].ContainsKey(p.ID)) {
                                StoreItems[idReplacement][p.ID] = p;
                            }

                            continue;
                        }
                        var matchingItems = allItems.Where(i => i.Name.ExtractText() == name).ToList();
                        if (matchingItems.Count == 0) {
                            Console.WriteLine($"Failed to find matching item for {originalName}.");
                            continue;
                        }

                        if (matchingItems.Count > 1) {
                            Console.WriteLine($"Found multiple matching items for {originalName}.");
                        }

                        foreach (var matchedItem in matchingItems) {
                            if (!StoreItems.ContainsKey(matchedItem.RowId)) {
                                StoreItems.Add(matchedItem.RowId, new Dictionary<uint, Product>());
                            }

                            if (!StoreItems[matchedItem.RowId].ContainsKey(p.ID)) {
                                StoreItems[matchedItem.RowId][p.ID] = p;
                            }
                        }
                    }

                    if (!usingCache) {
                        Console.WriteLine($"Cached Product Info: {p.ID}");
                        File.WriteAllText(cacheFile, fullProductJson);
                        Thread.Sleep( 500 );
                    }
                }
            } catch (Exception ex) {
                UpdateStatus = "[Error] " + UpdateStatus;
                Console.WriteLine(ex.Message + "Error in Update Task");
                return;
            }

            UpdateStatus = string.Empty;
        }
    }
}
