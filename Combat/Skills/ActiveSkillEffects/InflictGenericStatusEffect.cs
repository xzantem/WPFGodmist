
using GodmistWPF.Utilities;
using Character = GodmistWPF.Characters.Character;
using SkillTarget = GodmistWPF.Enums.SkillTarget;
using StatusEffectFactory = GodmistWPF.Combat.Modifiers.PassiveEffects.StatusEffectFactory;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

/// <summary>
/// Efekt umiejętności, który nakłada na cel ogólny efekt statusowy, taki jak ogłuszenie czy zamrożenie.
/// </summary>
/// <remarks>
/// Umożliwia nałożenie różnych efektów, które mogą wpływać na możliwości akcji celu.
/// Efekty te zazwyczaj blokują określone akcje na określony czas.
/// </remarks>
public class InflictGenericStatusEffect : IActiveSkillEffect
{
    /// <summary>
    /// Pobiera lub ustawia cel efektu (własna postać lub przeciwnik).
    /// </summary>
    public SkillTarget Target { get; set; }
    /// <summary>
    /// Pobiera lub ustawia czas trwania efektu w turach.
    /// </summary>
    public int Duration { get; set; }
    /// <summary>
    /// Pobiera lub ustawia dodatkowe parametry efektu.
    /// </summary>
    /// <remarks>
    /// Używane do przekazania dodatkowych informacji specyficznych dla danego typu efektu.
    /// </remarks>
    public string Effect { get; set; }
    /// <summary>
    /// Pobiera lub ustawia typ efektu do nałożenia.
    /// </summary>
    /// <value>Dopuszczalne wartości: "Stun", "Freeze" (wielkość liter ma znaczenie).</value>
    public string EffectType { get; set; }
    /// <summary>
    /// Pobiera lub ustawia szansę na nałożenie efektu (0.0-1.0).
    /// </summary>
    /// <remarks>
    /// W przypadku używania na przeciwniku, szansa jest modyfikowana przez jego odporności.
    /// </remarks>
    public double Chance { get; set; }
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="InflictGenericStatusEffect"/>.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizm deserializacji.
    /// </remarks>
    public InflictGenericStatusEffect() {}

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="InflictGenericStatusEffect"/>.
    /// </summary>
    /// <param name="effectType">Typ efektu do nałożenia (np. "Stun", "Freeze").</param>
    /// <param name="duration">Czas trwania efektu w turach.</param>
    /// <param name="chance">Szansa na nałożenie efektu (0.0-1.0).</param>
    /// <param name="effect">Dodatkowe parametry efektu.</param>
    /// <param name="target">Cel efektu (domyślnie przeciwnik).</param>
    public InflictGenericStatusEffect(string effectType, int duration, double chance, string effect,
        SkillTarget target = SkillTarget.Enemy)
    {
        Target = target;
        EffectType = effectType;
        Duration = duration;
        Chance = chance;
        Effect = effect;
    }
    

    /// <summary>
    /// Wykonuje efekt nałożenia ogólnego efektu statusowego.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności).</param>
    /// <remarks>
    /// Tworzy odpowiedni efekt w zależności od typu i próbuje go nałożyć na cel.
    /// Wyświetla odpowiedni komunikat w przypadku powodzenia.
    /// </remarks>
    public void Execute(Character caster, Character enemy, string source)
    {
        var chance =
            UtilityMethods.CalculateModValue(Chance, caster.PassiveEffects.GetModifiers("SuppressionChanceMod"));
        var target = Target switch
        {
            SkillTarget.Self => caster,
            SkillTarget.Enemy => enemy
        };
        var effect = EffectType switch
        {
            "Stun" => StatusEffectFactory.CreateStunEffect(target, source, Duration, chance),
            "Freeze" => StatusEffectFactory.CreateFreezeEffect(target, source, Duration, chance)
        };
        if (!StatusEffectFactory.TryAddEffect(target, effect)) return;
        var text = EffectType switch
        {
            "Stun" => $"{target.Name} {locale.IsStunned} {locale.And1} {locale.CannotMove}" +
                      $" {locale.ForTheNext} {Duration} {locale.Turns}",
            "Freeze" => $"{target.Name} {locale.IsFrozen} {locale.And1} {locale.CannotMove}" +
                        $" {locale.ForTheNext} {Duration} {locale.Turns}"
        };
    }
}