using GodmistWPF.Combat.Battles;

namespace GodmistWPF.Combat.Modifiers.PassiveEffects;

/// <summary>
/// Klasa przechowująca informacje o zdarzeniu, które wystąpiło podczas walki.
/// </summary>
/// <remarks>
/// Używana do przekazywania danych między obiektami w systemie efektów pasywnych.
/// Pozwala na reagowanie na różne typy zdarzeń w walce.
/// </remarks>
public class BattleEventData(string eventType, BattleUser source, BattleUser? target = null, dynamic[]? value = null)
{
    /// <summary>
    /// Pobiera typ zdarzenia.
    /// </summary>
    /// <value>
    /// Identyfikator typu zdarzenia (np. "DamageDealt", "HealReceived", itp.).
    /// </value>
    public string EventType { get; } = eventType;
    /// <summary>
    /// Pobiera źródło zdarzenia (postać, która wywołała zdarzenie).
    /// </summary>
    public BattleUser Source { get; } = source;
    /// <summary>
    /// Pobiera cel zdarzenia (opcjonalny).
    /// </summary>
    /// <value>
    /// Postać, na którą wpłynęło zdarzenie, lub null jeśli nie dotyczy.
    /// </value>
    public BattleUser? Target { get; } = target;
    /// <summary>
    /// Pobiera dodatkowe wartości związane ze zdarzeniem (opcjonalne).
    /// </summary>
    /// <value>
    /// Tablica wartości liczbowych, których znaczenie zależy od typu zdarzenia.
    /// Może być używana do przekazywania np. obrażeń, wartości leczenia itp.
    /// </value>
    public dynamic[]? Value { get; } = value; // Optional, used for damage, healing, etc.
}