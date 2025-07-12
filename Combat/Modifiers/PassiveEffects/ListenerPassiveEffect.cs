using GodmistWPF.Characters;

namespace GodmistWPF.Combat.Modifiers.PassiveEffects;

/// <summary>
/// Klasa reprezentująca efekt pasywny, który reaguje na określone zdarzenia w walce.
/// </summary>
/// <remarks>
/// Ten typ efektu aktywuje się, gdy zostanie spełniony warunek wyzwalacza.
/// Umożliwia tworzenie dynamicznych efektów, które reagują na akcje w walce.
/// </remarks>
/// <param name="triggerCondition">Warunek, który musi zostać spełniony, aby efekt się aktywował</param>
/// <param name="onTrigger">Akcja wykonywana, gdy efekt zostanie aktywowany</param>
/// <param name="owner">Właściciel efektu (postać)</param>
/// <param name="source">Źródło pochodzenia efektu</param>
public class ListenerPassiveEffect(
    Func<BattleEventData, bool> triggerCondition,
    Action<BattleEventData> onTrigger,
    Character owner,
    string source)
    : PassiveEffect(owner, source)
{
    /// <summary>
    /// Wywołuje efekt, jeśli warunek wyzwalacza zostanie spełniony.
    /// </summary>
    /// <param name="eventData">Dane zdarzenia, które może wywołać efekt</param>
    /// <remarks>
    /// Sprawdza warunek wyzwalacza i jeśli jest spełniony, wywołuje przypisaną akcję.
    /// </remarks>
    public void OnTrigger(BattleEventData eventData)
    {
        if (triggerCondition(eventData))
        {
            onTrigger(eventData);
        }
    }
}