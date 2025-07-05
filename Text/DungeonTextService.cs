using ConsoleGodmist;
using GodmistWPF.Dungeons;
using GodmistWPF.Enums.Dungeons;


namespace GodmistWPF.Text;

public static class DungeonTextService
{

    private static string LocationText(Dungeon dungeon)
    {
        var locationText = dungeon.DungeonType switch
        {
            DungeonType.Catacombs => locale.Catacombs,
            DungeonType.Forest => locale.Forest,
            DungeonType.ElvishRuins => locale.ElvishRuins,
            DungeonType.Cove => locale.Cove,
            DungeonType.Desert => locale.Desert,
            DungeonType.Temple => locale.Temple,
            DungeonType.Mountains => locale.Mountains,
            DungeonType.Swamp => locale.Swamp,
            _ => locale.Catacombs,
        };
        return locationText;
    }
}