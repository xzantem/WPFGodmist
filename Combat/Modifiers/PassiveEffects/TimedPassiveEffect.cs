using GodmistWPF.Characters;

namespace GodmistWPF.Combat.Modifiers.PassiveEffects;

/// <summary>
/// Klasa bazowa dla efektów pasywnych o ograniczonym czasie trwania.
/// </summary>
/// <remarks>
/// Efekty dziedziczące po tej klasie wygasają po określonej liczbie tur.
/// Można je wykorzystać do efektów tymczasowych, takich jak wzmocnienia czy osłabienia.
/// </remarks>
/// <param name="owner">Właściciel efektu (postać)</param>
/// <param name="source">Źródło pochodzenia efektu</param>
/// <param name="type">Typ efektu</param>
/// <param name="duration">Czas trwania efektu w turach</param>
public class TimedPassiveEffect(Character owner, string source, string type, int duration, 
    dynamic[] effects, Action? onTick = null) : 
    InnatePassiveEffect(owner, source, type, effects)
{
    /// <summary>
    /// Pobiera pozostały czas trwania efektu w turach.
    /// </summary>
    /// <value>
    /// Liczba tur pozostałych do wygaśnięcia efektu.
    /// Wartość 0 lub ujemna oznacza, że efekt wygasł.
    /// </value>
    public int Duration { get; private set; } = duration;

    /// <summary>
    /// Aktualizuje czas trwania efektu, zmniejszając go o jedną turę.
    /// </summary>
    /// <remarks>
    /// Metoda jest wywoływana na początku każdej tury.
    /// Może zostać nadpisana w klasach pochodnych, aby dodać dodatkową logikę.
    /// </remarks>
    public void Tick()
    {
        onTick?.Invoke();
        Duration--;
        if (Duration <= 0) Owner.PassiveEffects.Remove(this);
    }

    /// <summary>
    /// Wydłuża czas trwania efektu o określoną liczbę tur.
    /// </summary>
    /// <param name="turns">Liczba tur, o którą zostanie wydłużony czas trwania efektu.</param>
    public void Extend(int turns)
    {
        Duration += turns;
    }
    
    /// <summary>
    /// Aktualizuje akcję wywoływaną przy każdym tikcie efektu.
    /// </summary>
    /// <param name="newOnTick">Nowa akcja, która zostanie wywołana przy każdym tikcie efektu.</param>
    public void UpdateOnTick(Action newOnTick) {
        onTick = newOnTick;
    }
}