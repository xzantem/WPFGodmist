using System.IO;
using System.Text.Json;
using GodmistWPF.Enums.Dungeons;
using GodmistWPF.Items.Drops;

namespace GodmistWPF.Items.Lootbags;

public static class LootbagManager
{
    private static Dictionary<string, DropTable> DropTables { get; set; }

    public static void InitItems()
    {
        var path = "json/lootbag-drop-tables.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            DropTables = JsonSerializer.Deserialize<Dictionary<string, DropTable>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }
    
    public static Lootbag GetLootbag(string alias, int level)
    {
        return new Lootbag(alias, level, DropTables
            .FirstOrDefault(i => i.Key == alias).Value);
    }
    public static Lootbag GetSupplyBag(DungeonType dungeonType, int level)
    {
        var alias = dungeonType switch
        {
            DungeonType.Catacombs => "BonySupplyBag",
            DungeonType.Forest => "LeafySupplyBag",
            DungeonType.ElvishRuins => "DemonicSupplyBag",
            DungeonType.Cove => "PirateSupplyBag",
            DungeonType.Desert => "SandySupplyBag",
            DungeonType.Temple => "TempleSupplyBag",
            DungeonType.Mountains => "MountainousSupplyBag",
            DungeonType.Swamp => "MurkySupplyBag",
            _ => throw new ArgumentOutOfRangeException(nameof(dungeonType), dungeonType, "Wrong dungeon type specified")
        };
        return new Lootbag(alias, level, DropTables
            .FirstOrDefault(i => i.Key == alias).Value);
    }
}