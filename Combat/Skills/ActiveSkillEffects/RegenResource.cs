using Character = GodmistWPF.Characters.Character;
using DamageBase = GodmistWPF.Enums.DamageBase;
using SkillTarget = GodmistWPF.Enums.SkillTarget;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

/// <summary>
/// Efekt umiejętności, który przywraca punkty zasobów (np. many) celowi.
/// </summary>
/// <remarks>
/// Może być używany do regeneracji zasobów zarówno sojusznikom, jak i przeciwnikom.
/// Wartość regeneracji może być obliczana na podstawie różnych parametrów.
/// </remarks>
public class RegenResource : IActiveSkillEffect
{
    /// <summary>
    /// Pobiera lub ustawia cel efektu (własna postać lub przeciwnik).
    /// </summary>
    public SkillTarget Target { get; set; }
    /// <summary>
    /// Pobiera lub ustawia podstawową wartość regeneracji.
    /// </summary>
    /// <remarks>
    /// Mnożnik stosowany do wartości bazowej określonej przez <see cref="RegenBase">RegenBase</see>.
    /// </remarks>
    public double RegenAmount { get; set; }
    /// <summary>
    /// Pobiera lub ustawia bazę obliczania wartości regeneracji.
    /// </summary>
    /// <remarks>
    /// Określa, na podstawie jakiej wartości (np. ataku, zasobów) ma być obliczana wartość regeneracji.
    /// </remarks>
    public DamageBase RegenBase { get; set; }
    /// <summary>
    /// Wykonuje efekt regeneracji zasobów na wybranym celu.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności).</param>
    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                caster.RegenResource((int)CalculateRegen(caster, enemy));
                break;
            case SkillTarget.Enemy:
                enemy.RegenResource((int)CalculateRegen(caster, enemy));
                break;
        }
    }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="RegenResource"/>.
    /// </summary>
    /// <param name="target">Cel efektu.</param>
    /// <param name="regenAmount">Podstawowa wartość regeneracji (mnożnik).</param>
    /// <param name="regenBase">Baza obliczania wartości regeneracji.</param>
    public RegenResource(SkillTarget target, double regenAmount, DamageBase regenBase)
    {
        Target = target;
        RegenAmount = regenAmount;
        RegenBase = regenBase;
    }
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="RegenResource"/>.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizm deserializacji.
    /// </remarks>
    public RegenResource() { }

    /// <summary>
    /// Oblicza wartość regeneracji na podstawie wybranej bazy i mnożnika.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <returns>Obliczona wartość regeneracji.</returns>
    /// <remarks>
    /// Wartość regeneracji może być zależna od różnych parametrów, takich jak:
    /// - Wartość stała (Flat)
    /// - Atak postaci (Minimal, Random, Maximal)
    /// - Zasoby postaci (Caster/Target Max/Current/Missing Resource)
    /// </remarks>
    public double CalculateRegen(Character caster, Character enemy)
    {
        return RegenBase switch
        {
            DamageBase.Flat => RegenAmount,
            DamageBase.Minimal => RegenAmount * caster.MinimalAttack,
            DamageBase.Random => RegenAmount * Random.Shared.Next((int)caster.MinimalAttack, (int)caster.MaximalAttack + 1),
            DamageBase.Maximal => RegenAmount * caster.MinimalAttack,
            DamageBase.CasterMaxHealth => RegenAmount * caster.MaximalResource,
            DamageBase.TargetMaxHealth => RegenAmount * enemy.MaximalResource,
            DamageBase.CasterCurrentHealth => RegenAmount * caster.CurrentResource,
            DamageBase.TargetCurrentHealth => RegenAmount * enemy.CurrentResource,
            DamageBase.CasterMissingHealth => RegenAmount * (caster.MaximalResource - caster.CurrentResource),
            DamageBase.TargetMissingHealth => RegenAmount * (enemy.MaximalResource - enemy.CurrentResource)
        };
    }
}