using GodmistWPF.Enums;

namespace GodmistWPF.Utilities;

/// <summary>
/// Klasa przechowująca globalne ustawienia gry, takie jak poziom trudności.
/// Zapewnia dostęp do wspólnych ustawień z różnych części aplikacji.
/// </summary>
public static class GameSettings
{
    /// <summary>
    /// Pobiera lub ustawia aktualny poziom trudności gry.
    /// Wpływa na parametry rozgrywki, takie jak statystyki przeciwników i dostępne przedmioty.
    /// </summary>
    public static Difficulty Difficulty { get; set; }
}