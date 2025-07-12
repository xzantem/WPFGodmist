using GodmistWPF.Characters;

namespace GodmistWPF.Combat.Modifiers.PassiveEffects;

/// <summary>
/// Abstrakcyjna klasa bazowa dla wszystkich efektów pasywnych w grze.
/// </summary>
/// <remarks>
/// Efekty pasywne to specjalne właściwości, które wpływają na postać w sposób ciągły,
/// takie jak modyfikatory statystyk, odporności czy specjalne zdolności.
/// </remarks>
/// <param name="owner">Postać, do której należy efekt</param>
/// <param name="source">Źródło efektu (np. nazwa umiejętności lub przedmiotu)</param>
public abstract class PassiveEffect(Character owner, string source)
{
    /// <summary>
    /// Pobiera postać, do której należy ten efekt.
    /// </summary>
    public Character Owner { get; } = owner;
    /// <summary>
    /// Pobiera źródło pochodzenia efektu.
    /// </summary>
    /// <value>
    /// Może to być np. nazwa przedmiotu ("Miecz Płomienia"), 
    /// umiejętności ("Drzewo Umiejętności: Wściekłość Berserka")
    /// lub innego źródła efektu.
    /// </value>
    public string Source { get; } = source;
}