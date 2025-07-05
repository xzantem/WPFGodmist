using ConsoleGodmist;
using GodmistWPF.Enums.Dungeons;

namespace GodmistWPF.Utilities;

public static class NameAliasHelper
{
    public static string GetName(string alias)
    {
        return (locale.ResourceManager.GetString(alias) == null ? alias : locale.ResourceManager.GetString(alias))!;
    }

    public static string GetDungeonType(DungeonType type, string grammarCase)
    {
        return (type, grammarCase) switch
        {
            (DungeonType.Catacombs, "Nominative") => locale.Catacombs,
            (DungeonType.Forest, "Nominative") => locale.Forest,
            (DungeonType.ElvishRuins, "Nominative") => locale.ElvishRuins,
            (DungeonType.Cove, "Nominative") => locale.Cove,
            (DungeonType.Desert, "Nominative") => locale.Desert,
            (DungeonType.Temple, "Nominative") => locale.Temple,
            (DungeonType.Mountains, "Nominative") => locale.Mountains,
            (DungeonType.Swamp, "Nominative") => locale.Swamp,
            
            (DungeonType.Catacombs, "Locative") => locale.CatacombsLocative,
            (DungeonType.Forest, "Locative") => locale.ForestLocative,
            (DungeonType.ElvishRuins, "Locative") => locale.ElvishRuinsLocative,
            (DungeonType.Cove, "Locative") => locale.CoveLocative,
            (DungeonType.Desert, "Locative") => locale.DesertLocative,
            (DungeonType.Temple, "Locative") => locale.TempleLocative,
            (DungeonType.Mountains, "Locative") => locale.MountainsLocative,
            (DungeonType.Swamp, "Locative") => locale.SwampLocative
        };
    }
}