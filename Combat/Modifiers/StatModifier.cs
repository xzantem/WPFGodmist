using GodmistWPF.Enums.Modifiers;

namespace GodmistWPF.Combat.Modifiers;

/// <summary>
/// Klasa reprezentująca pojedynczy modyfikator statystyki.
/// </summary>
/// <remarks>
/// Modyfikator może zmieniać wartość statystyki w różny sposób (dodatkowo, mnożnikowo itp.)
/// i posiada określony czas trwania. Wartość -1 czasu trwania oznacza modyfikator nieskończony.
/// </remarks>
/// <param name="modifierType">Typ modyfikatora (dodawanie, mnożenie itp.)</param>
/// <param name="value">Wartość modyfikatora</param>
/// <param name="source">Źródło modyfikatora (np. nazwa umiejętności lub efektu)</param>
/// <param name="duration">Czas trwania modyfikatora w turach (-1 = nieskończony)</param>
public class StatModifier(ModifierType modifierType, double value, string source, int duration = -1)
    // optional parameter, duration -1 represents infinite modifier
{
    /// <summary>
    /// Pobiera typ modyfikatora (dodawanie, mnożenie itp.)
    /// </summary>
    public ModifierType Type { get; private set; } = modifierType;
    /// <summary>
    /// Pobiera wartość modyfikatora.
    /// </summary>
    public double Mod { get; private set; } = value;
    /// <summary>
    /// Pobiera pozostały czas trwania modyfikatora w turach.
    /// </summary>
    /// <value>
    /// Liczba tur pozostałych do wygaśnięcia modyfikatora.
    /// Wartość -1 oznacza modyfikator nieskończony.
    /// </value>
    public int Duration { get; private set; } = duration;
    /// <summary>
    /// Pobiera źródło modyfikatora (np. nazwę umiejętności lub efektu).
    /// </summary>
    public string Source { get; private set; } = source;

    /// <summary>
    /// Zmniejsza czas trwania modyfikatora o jedną turę.
    /// </summary>
    /// <remarks>
    /// Jeśli czas trwania wynosi 0, modyfikator powinien zostać usunięty.
    /// Modyfikatory z czasem trwania -1 (nieskończone) nie są modyfikowane.
    /// </remarks>
    public void Tick()
    {
        Duration--;
    }
}