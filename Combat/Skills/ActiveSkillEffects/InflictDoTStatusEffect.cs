
using GodmistWPF.Utilities;
using Character = GodmistWPF.Characters.Character;
using SkillTarget = GodmistWPF.Enums.SkillTarget;
using StatusEffectFactory = GodmistWPF.Combat.Modifiers.PassiveEffects.StatusEffectFactory;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

/// <summary>
/// Efekt umiejętności, który nakłada na cel efekt obrażeń odnoszonych z upływem czasu (DoT).
/// </summary>
/// <remarks>
/// Może powodować różne typy obrażeń, takie jak krwawienie, zatrucie czy oparzenia.
/// Efekt działa przez określoną liczbę tur i zadaje obrażenia na początku każdej tury.
/// </remarks>
public class InflictDoTStatusEffect : IActiveSkillEffect
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
    /// Pobiera lub ustawia siłę efektu DoT.
    /// </summary>
    /// <remarks>
    /// Mnożnik stosowany do wartości ataku postaci rzucającej umiejętność.
    /// </remarks>
    public double Strength { get; set; }
    /// <summary>
    /// Pobiera lub ustawia typ efektu DoT.
    /// </summary>
    /// <value>Dopuszczalne wartości: "Bleed", "Poison", "Burn" (wielkość liter ma znaczenie).</value>
    public string DoTType { get; set; }
    /// <summary>
    /// Pobiera lub ustawia szansę na nałożenie efektu (0.0-1.0).
    /// </summary>
    /// <remarks>
    /// W przypadku używania na przeciwniku, szansa jest modyfikowana przez jego odporności.
    /// </remarks>
    public double Chance { get; set; }
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="InflictDoTStatusEffect"/>.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizm deserializacji.
    /// </remarks>
    public InflictDoTStatusEffect() {}

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="InflictDoTStatusEffect"/>.
    /// </summary>
    /// <param name="target">Cel efektu.</param>
    /// <param name="duration">Czas trwania efektu w turach.</param>
    /// <param name="strength">Siła efektu DoT (mnożnik ataku).</param>
    /// <param name="doTType">Typ efektu DoT (Bleed, Poison lub Burn).</param>
    /// <param name="chance">Szansa na nałożenie efektu (0.0-1.0).</param>
    /// <exception cref="ArgumentException">Wyrzucany, gdy podano nieprawidłowy typ efektu DoT.</exception>
    public InflictDoTStatusEffect(SkillTarget target, int duration, double strength, 
        string doTType, double chance)
    {
        if (doTType != "Bleed" && doTType != "Poison" && doTType != "Burn")
        {
            throw new ArgumentException("Invalid DoT type. Must be Bleed, Poison, or Burn.");
        }
        Target = target;
        Duration = duration;
        Strength = strength;
        DoTType = doTType;
        Chance = chance;
    }


    /// <summary>
    /// Wykonuje efekt nałożenia obrażeń odnoszonych z upływem czasu.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności).</param>
    /// <remarks>
    /// Oblicza siłę efektu na podstawie ataku postaci i nakłada efekt na cel,
    /// o ile ten nie jest na niego odporny.
    /// </remarks>
    public void Execute(Character caster, Character enemy, string source)
    {
        var strength = Strength * Random.Shared.Next((int)caster.MinimalAttack, (int)caster.MinimalAttack + 1);
        var chance =
            UtilityMethods.CalculateModValue(Chance, caster.PassiveEffects.GetModifiers("DoTChanceMod"));
        var target = Target switch
        {
            SkillTarget.Self => caster,
            SkillTarget.Enemy => enemy
        };
        if (!StatusEffectFactory.TryAddEffect(target,
                StatusEffectFactory.CreateDoTEffect(target, source, DoTType, strength, Duration, chance))) return;
        var text = DoTType switch
        {
            "Bleed" => $"{target.Name} {locale.Bleeds}, {locale.Taking} {(int)strength} {locale.DamageGenitive}" +
                       $" {locale.ForTheNext} {Duration} {locale.Turns}",
            "Poison" => $"{target.Name} {locale.IsPoisoned}, {locale.Taking} {(int)strength} {locale.DamageGenitive}" +
                        $" {locale.ForTheNext} {Duration} {locale.Turns}",
            "Burn" => $"{target.Name} {locale.Burns}, {locale.Taking} {(int)strength} {locale.DamageGenitive}" +
                  $" {locale.ForTheNext} {Duration} {locale.Turns}"
        };
    }
}