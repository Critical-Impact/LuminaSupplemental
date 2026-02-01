using System;
using System.Globalization;
using System.Linq;
using System.Numerics;

using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.Excel.DataShare.Sheets;

public struct ENpcPlace : ISupplementalRow<Tuple<uint, uint, uint, uint, Vector2>>, ICsv
{
    private Tuple<uint, uint, uint, uint, Vector2> data;
    
    public uint ENpcResidentId => this.data.Item1;
    
    public uint TerritoryTypeId => this.data.Item2;
    
    public uint MapId => this.data.Item3;
    
    public uint PlaceNameId => this.data.Item4;
    
    public Vector2 Position => this.data.Item5;
    
    public int RowId { get; }
    
    public RowRef<ENpcResident> ENpcResident;
    
    public RowRef<TerritoryType> TerritoryType;
    
    public RowRef<Map> Map;
    
    public RowRef<PlaceName> PlaceName;
    
    public ENpcPlace(Tuple<uint, uint, uint, uint, Vector2> data, int rowId)
    {
        this.data = data;
        this.RowId = rowId;
    }
    
    public ENpcPlace()
    {
    }
    
    public void FromCsv(string[] lineData)
    {
        var positionData = lineData[4].Split(";").Select(c => float.Parse(c, CultureInfo.InvariantCulture)).ToList();
        this.data = new Tuple<uint, uint, uint, uint, Vector2>(
            uint.Parse(lineData[0]),
            uint.Parse(lineData[1]),
            uint.Parse(lineData[2]),
            uint.Parse(lineData[3]),
            new Vector2(positionData[0], positionData[1])
        );
    }
    
    public string[] ToCsv()
    {
        return Array.Empty<string>();
    }
    
    public bool IncludeInCsv()
    {
        return false;
    }
    
    public void PopulateData(ExcelModule module, Language language)
    {
        this.ENpcResident = new RowRef<ENpcResident>(module, this.ENpcResidentId);
        this.TerritoryType = new RowRef<TerritoryType>(module, this.TerritoryTypeId);
        this.Map = new RowRef<Map>(module, this.MapId);
        this.PlaceName = new RowRef<PlaceName>(module, this.PlaceNameId);
    }
    
    public bool EqualRounded(ENpcPlace other)
    {
        if (this.Map.RowId.Equals(other.Map.RowId) && this.PlaceName.RowId.Equals(other.PlaceName.RowId))
        {
            var x = (int)this.Position.X;
            var y = (int)this.Position.Y;
            var otherX = (int)other.Position.X;
            var otherY = (int)other.Position.Y;
            if (x == otherX && y == otherY) return true;
        }
        
        return false;
    }
}
