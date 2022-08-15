namespace LuminaSupplemental.Excel.Model
{
    public struct DungeonBossChest
    {
        public uint BNpcNameId;
        public ChestEnum ChestType;
        public uint ItemId;
        public uint ContentFinderConditionId;
        public uint Quantity;

        public DungeonBossChest( uint bNpcNameId, ChestEnum chestType, uint itemId, uint contentFinderConditionId, uint quantity )
        {
            BNpcNameId = bNpcNameId;
            ChestType = chestType;
            ItemId = itemId;
            ContentFinderConditionId = contentFinderConditionId;
            Quantity = quantity;
        }
    }
}