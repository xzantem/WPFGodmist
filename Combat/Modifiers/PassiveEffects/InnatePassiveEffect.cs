using GodmistWPF.Characters;

namespace GodmistWPF.Combat.Modifiers.PassiveEffects;

/// <summary>
/// Abstrakcyjna klasa bazowa dla wrodzonych efektów pasywnych w grze.
/// </summary>
/// <remarks>
/// Efekty wrodzone to stałe właściwości, które są przypisane do postaci lub przedmiotów,
/// takie jak modyfikatory statystyk czy specjalne zdolności.
/// </remarks>
/// <param name="owner">Właściciel efektu (postać)</param>
/// <param name="source">Źródło pochodzenia efektu</param>
/// <param name="type">Typ efektu (np. "Buff", "Debuff", "Aura")</param>
/// <param name="effects">Tablica efektów do zastosowania</param>
public class InnatePassiveEffect(Character owner, string source, string type, dynamic[] effects) : 
    PassiveEffect(owner, source)
{
    /// <summary>
    /// Pobiera typ efektu.
    /// </summary>
    /// <value>
    /// Typ efektu, który określa jego kategorię (np. "Buff", "Debuff", "Aura").
    /// </value>
    public string Type { get; } = type;
    /// <summary>
    /// Pobiera tablicę efektów przypisanych do tego obiektu.
    /// </summary>
    /// <value>
    /// Tablica efektów, które są aktywne dla tego efektu pasywnego.
    /// Może zawierać różne typy efektów w zależności od implementacji.
    /// </value>
    public dynamic[] Effects { get; } = effects;
}