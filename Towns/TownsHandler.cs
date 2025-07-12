using GodmistWPF.Towns.NPCs;

namespace GodmistWPF.Towns;

/// <summary>
/// Klasa zarządzająca miastami w grze.
/// </summary>
public static class TownsHandler
{
    /// <summary>
    /// Pobiera lub ustawia główne miasto w grze - Arungard.
    /// </summary>
    public static Town Arungard { get; set; }

    /// <summary>
    /// Znajduje NPC o podanym aliasie w mieście Arungard.
    /// </summary>
    /// <param name="alias">Alias NPC do znalezienia.</param>
    /// <returns>Znaleziony NPC lub null, jeśli nie znaleziono.</returns>
    public static NPC FindNPC(string alias)
    {
        return Arungard.NPCs.FirstOrDefault(x => x.Alias == alias);
    }
}