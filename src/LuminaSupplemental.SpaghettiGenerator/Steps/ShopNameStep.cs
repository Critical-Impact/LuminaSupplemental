using System;
using System.Collections.Generic;
using System.Linq;

using Lumina;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class ShopNameStep : GeneratorStep
{
    private readonly ExcelSheet<Item> itemSheet;
    private readonly ExcelSheet<CustomTalk> customTalkSheet;
    private readonly Dictionary<string, Item> itemsByName;

    public override Type OutputType => typeof(ShopName);

    public override string FileName => "ShopName.csv";

    public override string Name => "Shop Names";


    public ShopNameStep(ExcelSheet<CustomTalk> customTalkSheet)
    {
        this.customTalkSheet = customTalkSheet;
    }


    public override List<ICsv> Run()
    {
        List<ShopName> items = new ();
        items.AddRange(this.ProcessShopNames());

        return [..items.Select(c => c).OrderBy(c => c.ShopId)];
    }

    public List<ShopName> ProcessShopNames()
    {
        var shopNames = new List<ShopName>();
        foreach( var customTalk in customTalkSheet )
        {
            var instructions = new List<(uint, string)>();
            foreach (var scriptStruct in customTalk.Script)
            {
                instructions.Add( (scriptStruct.ScriptArg, scriptStruct.ScriptInstruction.ExtractText()) );
            }

            foreach (var scriptStruct in customTalk.Script)
            {
                var scriptInstructions = scriptStruct.ScriptInstruction.ExtractText();
                if (scriptInstructions.Contains("SHOP") && !scriptInstructions.Contains("LOGMSG"))
                {
                    var argument = scriptStruct.ScriptArg;
                    var shopName = Utils.GetShopName(argument, scriptInstructions);
                    if (shopName != null)
                    {
                        shopNames.Add(
                            new ShopName()
                            {
                                ShopId = argument,
                                Name = shopName
                            });
                    }
                }
            }
        }

        var shops = new Dictionary< uint, string >()
        {
            {1769869, "Request to keep your aetherpool gear"},
            {1769743, "Exchange Wolf Marks (Melee)"},
            {1769744, "Exchange Wolf Marks (Ranged)"},
            {1769820, "Create or augment Eureka gear. (Paladin)"},
            {1769821, "Create or augment Eureka gear. (Warrior)"},
            {1769822,"Create or augment Eureka gear. (Dark Knight)"},
            {1769823,"Create or augment Eureka gear. (Dragoon)"},
            {1769824,"Create or augment Eureka gear. (Monk)"},
            {1769825,"Create or augment Eureka gear. (Ninja)"},
            {1769826,"Create or augment Eureka gear. (Samurai)"},
            {1769827,"Create or augment Eureka gear. (Bard)"},
            {1769828,"Create or augment Eureka gear. (Machinist)"},
            {1769829,"Create or augment Eureka gear. (Black Mage)"},
            {1769830,"Create or augment Eureka gear. (Summoner)"},
            {1769831,"Create or augment Eureka gear. (Red Mage)"},
            {1769832,"Create or augment Eureka gear. (White Mage)"},
            {1769833,"Create or augment Eureka gear. (Scholar)"},
            {1769834,"Create or augment Eureka gear. (Astrologian)"},
            {1769871,"Exchange artifacts"},
            {1769870,"Request to keep your empyrean aetherpool gear"},
            {1770282,"Exchange Faux Leaves"},
            {1770286,"Exchange Faire Voucher"},
            {1770087,"Exchange Bozjan clusters for items."},
            {1769957,"Gemstone Trader"},
            {1769958,"Gemstone Trader"},
            {1769959,"Gemstone Trader"},
            {1769960,"Gemstone Trader"},
            {1769961,"Gemstone Trader"},
            {1769962,"Gemstone Trader"},
            {1769963,"Gemstone Trader"},
            {1769964,"Gemstone Trader"},
            {262919,"Doman Junkmonger"},
        };
        foreach( var shopName in shops )
        {
            shopNames.Add( new ShopName()
            {
                ShopId = shopName.Key,
                Name = shopName.Value
            } );
        }

        return shopNames;
    }
}
