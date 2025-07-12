using GodmistWPF.Enums.Dungeons;

namespace GodmistWPF.Utilities;

/// <summary>
/// Klasa pomocnicza do pobierania zlokalizowanych nazw i aliasów.
/// Zapewnia tłumaczenia dla typów lokacji i innych elementów interfejsu użytkownika.
/// </summary>
public static class NameAliasHelper
{
    /// <summary>
    /// Pobiera zlokalizowaną nazwę na podstawie aliasu.
    /// </summary>
    /// <param name="alias">Alias elementu, dla którego ma zostać pobrana nazwa.</param>
    /// <returns>Zlokalizowana nazwa lub oryginalny alias, jeśli tłumaczenie nie zostało znalezione.</returns>
    public static string GetName(string alias)
    {
        return (locale.ResourceManager.GetString(alias) == null ? alias : locale.ResourceManager.GetString(alias))!;
    }

    /// <summary>
    /// Pobiera zlokalizowaną nazwę typu lochu w określonym przypadku gramatycznym.
    /// </summary>
    /// <param name="type">Typ lochu.</param>
    /// <param name="grammarCase">Przypadek gramatyczny (np. "Nominative" - mianownik, "Locative" - miejscownik).</param>
    /// <returns>Zlokalizowana nazwa typu lochu w żądanym przypadku gramatycznym.</returns>
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