using DungeonFieldType = GodmistWPF.Enums.Dungeons.DungeonFieldType;

namespace GodmistWPF.Dungeons
{
    /// <summary>
    /// Klasa reprezentująca korytarz w lochu, będący podstawowym typem pola, po którym może poruszać się gracz.
    /// </summary>
    public class DungeonCorridor(DungeonFieldType fieldType) : DungeonRoom(fieldType);
}