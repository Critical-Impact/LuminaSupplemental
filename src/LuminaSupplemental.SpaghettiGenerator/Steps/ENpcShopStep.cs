using System;
using System.Collections.Generic;
using System.Linq;

using Lumina;
using Lumina.Excel;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class ENpcShopStep : GeneratorStep
{
    public override Type OutputType => typeof(ENpcShop);

    public override string FileName => "ENpcShop.csv";

    public override string Name => "Shop Names";


    public ENpcShopStep()
    {
    }


    public override List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData)
    {
        List<ENpcShop> items = new ();
        items.AddRange(this.ProcessEventShops());

        return [..items.Select(c => c).OrderBy(c => c.ENpcResidentId).ThenBy(c => c.ShopId)];
    }

    private List<ENpcShop> ProcessEventShops()
    {
        List<ENpcShop> eNpcShops = new();

        var shops = new Dictionary< uint, uint >()
        {
            {1769635, 1016289},
            {1769675, 1017338},
            {1769869, 1017338},
            {1769743, 1018655},
            {1769744, 1018655},

            {1769820, 1025047},
            {1769821, 1025047},
            {1769822, 1025047},
            {1769823, 1025047},
            {1769824, 1025047},
            {1769825, 1025047},
            {1769826, 1025047},
            {1769827, 1025047},
            {1769828, 1025047},
            {1769829, 1025047},
            {1769830, 1025047},
            {1769831, 1025047},
            {1769832, 1025047},
            {1769833, 1025047},
            {1769834, 1025047},

            {1769871, 1025848},
            {1769870, 1025848},

            {262919, 1025763},

            {1769957, 1027998},
            {1769958, 1027538},
            {1769959, 1027385},
            {1769960, 1027497},
            {1769961, 1027892},
            {1769962, 1027665},
            {1769963, 1027709},
            {1769964, 1027766},

            {1770282, 1033921},

            {1770087, 1034007},
            //Support this later{1770087, 1036895},
        };

        foreach( var shopName in shops )
        {
            eNpcShops.Add( new ENpcShop()
            {
                ShopId = shopName.Key,
                ENpcResidentId = shopName.Value
            } );
        }

        return eNpcShops;
    }
}
