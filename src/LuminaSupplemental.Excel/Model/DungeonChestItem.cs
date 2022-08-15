using System.Numerics;

namespace LuminaSupplemental.Excel.Model
{
    public struct DungeonChestItem
    {
        public uint ItemId;
        public uint ContentFinderConditionId;
        public Vector2 Coordinates;

        public DungeonChestItem( uint itemId, uint contentFinderConditionId, Vector2 coordinates)
        {
            ItemId = itemId;
            ContentFinderConditionId = contentFinderConditionId;
            Coordinates = coordinates;
        }
    }
}