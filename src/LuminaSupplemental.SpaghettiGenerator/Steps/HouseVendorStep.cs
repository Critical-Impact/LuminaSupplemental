using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class HouseVendorStep : GeneratorStep
{
    private readonly ExcelSheet<ENpcResident> eNpcResidentSheet;

    public override Type OutputType => typeof(HouseVendor);

    public override string FileName => "HouseVendor.csv";

    public override string Name => "Housing Vendors";


    public HouseVendorStep(ExcelSheet<ENpcResident> eNpcResidentSheet)
    {
        this.eNpcResidentSheet = eNpcResidentSheet;
    }


    public override List<ICsv> Run()
    {
        List<HouseVendor> items = new ();
        items.AddRange(this.Process());
        return [..items.Select(c => c).OrderBy(c => c.ENpcResidentId)];
    }

    private List<HouseVendor> Process()
    {
        List<HouseVendor> houseVendors = new();

        var reader = CSVFile.CSVReader.FromFile(Path.Combine( "ManualData","HouseVendors.csv"));

        Dictionary< string, List< uint > > groupedResidents = new();

        foreach( var line in reader.Lines() )
        {
            var residentId = uint.Parse(line[ 0 ]);
            var resident = eNpcResidentSheet.GetRowOrDefault( residentId );
            if( resident != null )
            {
                var residentName = resident.Value.Singular.ToString().ToParseable();
                groupedResidents.TryAdd( residentName, new List< uint >() );
                groupedResidents[residentName].Add( residentId );
            }
        }

        foreach( var item in groupedResidents )
        {
            var parentId = item.Value.First();
            var children = item.Value.Skip( 1 );
            var houseVendor = new HouseVendor( parentId, 0);
            houseVendors.Add( houseVendor );
            foreach( var child in children )
            {
                var childVendor = new HouseVendor(child, parentId);
                houseVendors.Add( childVendor );
            }
        }

        return houseVendors;
    }
}
