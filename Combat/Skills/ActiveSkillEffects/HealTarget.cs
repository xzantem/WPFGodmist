using Character = GodmistWPF.Characters.Character;
using DamageBase = GodmistWPF.Enums.DamageBase;
using SkillTarget = GodmistWPF.Enums.SkillTarget;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

/// <summary>
/// Efekt umiejętności, który przywraca punkty zdrowia celowi.
/// </summary>
/// <remarks>
/// Może być używany do leczenia zarówno sojuszników, jak i przeciwników.
/// Wartość lecznia może być obliczana na podstawie różnych parametrów.
/// </remarks>
public class HealTarget : IActiveSkillEffect
{
    /// <summary>
    /// Pobiera lub ustawia cel efektu (własna postać lub przeciwnik).
    /// </summary>
    public SkillTarget Target { get; set; }
    /// <summary>
    /// Pobiera lub ustawia podstawową wartość lecznia.
    /// </summary>
    /// <remarks>
    /// Mnożnik stosowany do wartości bazowej określonej przez <see cref="HealBase">HealBase</see>.
    /// </remarks>
    public double HealAmount { get; set; }
    /// <summary>
    /// Pobiera lub ustawia bazę obliczania wartości lecznia.
    /// </summary>
    /// <remarks>
    /// Określa, na podstawie jakiej wartości (np. ataku, zdrowia) ma być obliczana wartość lecznia.
    /// </remarks>
    public DamageBase HealBase { get; set; }
    /// <summary>
    /// Wykonuje efekt leczenia na wybranym celu.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności).</param>
    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                caster.Heal(CalculateHeal(caster, enemy));
                break;
            case SkillTarget.Enemy:
                enemy.Heal(CalculateHeal(caster, enemy));
                break;
        }
    }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="HealTarget"/>.
    /// </summary>
    /// <param name="target">Cel efektu.</param>
    /// <param name="healAmount">Podstawowa wartość leczenia (mnożnik).</param>
    /// <param name="healBase">Baza obliczania wartości lecznia.</param>
    public HealTarget(SkillTarget target, double healAmount, DamageBase healBase)
    {
        Target = target;
        HealAmount = healAmount;
        HealBase = healBase;
    }
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="HealTarget"/>.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizm deserializacji.
    /// </remarks>
    public HealTarget() {}

    /// <summary>
    /// Oblicza wartość lecznia na podstawie wybranej bazy i mnożnika.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <returns>Obliczona wartość lecznia.</returns>
    /// <remarks>
    /// Wartość lecznia może być zależna od różnych parametrów, takich jak:
    /// - Wartość stała (Flat)
    /// - Atak postaci (Minimal, Random, Maximal)
    /// - Zdrowie postaci (Caster/Target Max/Current/Missing Health)
    /// </remarks>
    public double CalculateHeal(Character caster, Character enemy)
    {
        return HealBase switch
        {
            DamageBase.Flat => HealAmount,
            DamageBase.Minimal => HealAmount * caster.MinimalAttack,
            DamageBase.Random => HealAmount * Random.Shared.Next((int)caster.MinimalAttack, (int)caster.MaximalAttack + 1),
            DamageBase.Maximal => HealAmount * caster.MaximalHealth,
            DamageBase.CasterMaxHealth => HealAmount * caster.MaximalHealth,
            DamageBase.TargetMaxHealth => HealAmount * enemy.MaximalHealth,
            DamageBase.CasterCurrentHealth => HealAmount * caster.CurrentHealth,
            DamageBase.TargetCurrentHealth => HealAmount * enemy.CurrentHealth,
            DamageBase.CasterMissingHealth => HealAmount * (caster.MaximalHealth - caster.CurrentHealth),
            DamageBase.TargetMissingHealth => HealAmount * (enemy.MaximalHealth - enemy.CurrentHealth)
        };
    }
}