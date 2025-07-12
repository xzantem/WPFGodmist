using System.IO;
using System.Text.Json;
using GodmistWPF.Enums.Dungeons;
using GodmistWPF.Items.Drops;

namespace GodmistWPF.Items.Lootbags;

/// <summary>
/// Statyczna klasa zarządzająca tworzeniem i zarządzaniem workami z łupami w grze.
/// Odpowiada za wczytywanie tabel przedmiotów i tworzenie odpowiednich worków.
/// </summary>
public static class LootbagManager
{
    /// <summary>
    /// Słownik przechowujący tabele przedmiotów dla różnych typów worków.
    /// Kluczem jest alias worka, a wartością odpowiednia tabela przedmiotów.
    /// </summary>
    private static Dictionary<string, DropTable> DropTables { get; set; }

    /// <summary>
    /// Inicjalizuje menedżer worków, wczytując dane z pliku JSON.
    /// </summary>
    /// <exception cref="FileNotFoundException">Wyrzucany, gdy nie znaleziono pliku konfiguracyjnego.</exception>
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
    
    /// <summary>
    /// Tworzy nowy worek z łupami o określonym aliasie i poziomie.
    /// </summary>
    /// <param name="alias">Alias worka (np. "WeaponBag", "ArmorBag").</param>
    /// <param name="level">Poziom worka, który wpływa na jakość przedmiotów.</param>
    /// <returns>Nowa instancja worka z odpowiednią tabelą przedmiotów.</returns>
    public static Lootbag GetLootbag(string alias, int level)
    {
        return new Lootbag(alias, level, DropTables
            .FirstOrDefault(i => i.Key == alias).Value);
    }
    /// <summary>
    /// Tworzy nowy worek z zaopatrzeniem odpowiedni dla danego typu lochu.
    /// </summary>
    /// <param name="dungeonType">Typ lochu, dla którego ma zostać utworzony worek.</param>
    /// <param name="level">Poziom worka, który wpływa na jakość przedmiotów.</param>
    /// <returns>Nowa instancja worka z odpowiednią tabelą przedmiotów.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Wyrzucany, gdy podano nieznany typ lochu.</exception>
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