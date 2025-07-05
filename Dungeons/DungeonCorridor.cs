using DungeonFieldType = GodmistWPF.Enums.Dungeons.DungeonFieldType;

namespace GodmistWPF.Dungeons
{
    public class DungeonCorridor(DungeonFieldType fieldType) : DungeonRoom(fieldType);
}