namespace GodmistWPF.Enums.Items;

/// <summary>
/// Określa sposób sortowania przedmiotów w ekwipunku lub sklepie.
/// </summary>
public enum SortType
{
    /// <summary>
    /// Sortowanie według typu przedmiotu.
    /// </summary>
    ItemType,
    
    /// <summary>
    /// Sortowanie według rzadkości przedmiotu.
    /// </summary>
    Rarity,
    
    /// <summary>
    /// Sortowanie według wartości przedmiotu.
    /// </summary>
    Cost,
    
    /// <summary>
    /// Sortowanie alfabetyczne według nazwy przedmiotu.
    /// </summary>
    Name
}