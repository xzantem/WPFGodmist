using Character = GodmistWPF.Characters.Character;
using DamageBase = GodmistWPF.Enums.DamageBase;
using SkillTarget = GodmistWPF.Enums.SkillTarget;
using StatusEffectFactory = GodmistWPF.Combat.Modifiers.PassiveEffects.StatusEffectFactory;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

/// <summary>
/// Efekt umiejętności, który przyznaje tarczę ochronną celowi.
/// </summary>
/// <remarks>
/// Tarcza pochłania obrażenia, zanim zostaną one odjęte od punktów zdrowia.
/// Może być stosowana zarówno na sojuszników, jak i przeciwników.
/// </remarks>
public class GainShield : IActiveSkillEffect
{
    /// <summary>
    /// Pobiera lub ustawia cel efektu (własna postać lub przeciwnik).
    /// </summary>
    public SkillTarget Target { get; set; }
    /// <summary>
    /// Pobiera lub ustawia bazę obliczania wartości tarczy.
    /// </summary>
    /// <remarks>
    /// Określa, na podstawie jakiej wartości (np. ataku, zdrowia) ma być obliczana siła tarczy.
    /// </remarks>
    public DamageBase ShieldBase { get; set; }
    /// <summary>
    /// Pobiera lub ustawia mnożnik siły tarczy.
    /// </summary>
    /// <remarks>
    /// Mnożnik jest stosowany do wartości bazowej określonej przez <see cref="ShieldBase">ShieldBase</see>.
    /// </remarks>
    public double ShieldStrength { get; set; }
    /// <summary>
    /// Pobiera lub ustawia szansę na założenie tarczy (0.0-1.0).
    /// </summary>
    public double ShieldChance { get; set; }
    /// <summary>
    /// Pobiera lub ustawia czas trwania tarczy w turach.
    /// </summary>
    public int ShieldDuration { get; set; }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GainShield"/>.
    /// </summary>
    /// <param name="target">Cel efektu.</param>
    /// <param name="shieldBase">Baza obliczania wartości tarczy.</param>
    /// <param name="shieldStrength">Mnożnik siły tarczy.</param>
    /// <param name="shieldChance">Szansa na założenie tarczy (0.0-1.0).</param>
    /// <param name="shieldDuration">Czas trwania tarczy w turach.</param>
    public GainShield(SkillTarget target, DamageBase shieldBase,
        double shieldStrength, double shieldChance, int shieldDuration)
    {
        Target = target;
        ShieldBase = shieldBase;
        ShieldStrength = shieldStrength;
        ShieldChance = shieldChance;
        ShieldDuration = shieldDuration;
    }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GainShield"/>.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizm deserializacji.
    /// </remarks>
    public GainShield() {}

    /// <summary>
    /// Wykonuje efekt przyznania tarczy.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności).</param>
    /// <remarks>
    /// Oblicza wartość tarczy i próbuje ją założyć na cel, uwzględniając szansę powodzenia.
    /// </remarks>
    public void Execute(Character caster, Character enemy, string source)
    {
        var target = Target switch
        {
            SkillTarget.Self => caster,
            SkillTarget.Enemy => enemy
        };
        StatusEffectFactory.TryAddEffect(target, StatusEffectFactory
            .CreateShieldEffect(target, source, CalculateShield(caster, enemy), ShieldDuration, ShieldChance));
    }

    /// <summary>
    /// Oblicza wartość tarczy na podstawie wybranej bazy i mnożnika.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <returns>Obliczona wartość tarczy.</returns>
    /// <remarks>
    /// Wartość tarczy może być zależna od różnych parametrów, takich jak:
    /// - Wartość stała (Flat)
    /// - Atak postaci (Minimal, Random, Maximal)
    /// - Zdrowie postaci (Caster/Target Max/Current/Missing Health)
    /// </remarks>
    public double CalculateShield(Character caster, Character enemy)
    {
        return ShieldBase switch
        {
            DamageBase.Flat => ShieldStrength,
            DamageBase.Minimal => ShieldStrength * caster.MinimalAttack,
            DamageBase.Random => ShieldStrength * Random.Shared.Next((int)caster.MinimalAttack, (int)caster.MaximalAttack + 1),
            DamageBase.Maximal => ShieldStrength * caster.MaximalAttack, 
            DamageBase.CasterMaxHealth => ShieldStrength * caster.MaximalHealth, 
            DamageBase.TargetMaxHealth => ShieldStrength * enemy.MaximalHealth, 
            DamageBase.CasterCurrentHealth => ShieldStrength * caster.CurrentHealth, 
            DamageBase.TargetCurrentHealth => ShieldStrength * enemy.CurrentHealth, 
            DamageBase.CasterMissingHealth => ShieldStrength * (caster.MaximalAttack - caster.CurrentHealth), 
            DamageBase.TargetMissingHealth => ShieldStrength * (enemy.MaximalAttack - enemy.CurrentHealth) 
        };
    }
}