
using GodmistWPF.Utilities;
using Character = GodmistWPF.Characters.Character;
using DamageBase = GodmistWPF.Enums.DamageBase;
using DamageType = GodmistWPF.Enums.DamageType;
using EnemyCharacter = GodmistWPF.Characters.EnemyCharacter;
using EnemyType = GodmistWPF.Enums.EnemyType;
using SkillTarget = GodmistWPF.Enums.SkillTarget;
using StatModifier = GodmistWPF.Combat.Modifiers.StatModifier;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

/// <summary>
/// Efekt umiejętności zadający obrażenia celowi.
/// </summary>
/// <remarks>
/// Obsługuje różne typy obrażeń, krytyki, kradzież życia i penetrację pancerza.
/// Uwzględnia modyfikatory obrażeń w zależności od typu celu i innych czynników.
/// </remarks>
public class DealDamage : IActiveSkillEffect
{
    /// <summary>
    /// Pobiera lub ustawia cel efektu (domyślnie przeciwnik).
    /// </summary>
    public SkillTarget Target { get; set; }
    /// <summary>
    /// Pobiera lub ustawia typ zadawanych obrażeń (fizyczne, magiczne itp.).
    /// </summary>
    public DamageType DamageType { get; set; }
    /// <summary>
    /// Pobiera lub ustawia bazę obrażeń (minimalne, losowe, maksymalne).
    /// </summary>
    public DamageBase DamageBase { get; set; }
    /// <summary>
    /// Pobiera lub ustawia mnożnik obrażeń.
    /// </summary>
    public double DamageMultiplier { get; set; }
    /// <summary>
    /// Pobiera lub ustawia wartość określającą, czy umiejętność może krytykować.
    /// </summary>
    public bool CanCrit { get; set; }
    /// <summary>
    /// Pobiera lub ustawia wartość określającą, czy umiejętność zawsze krytykuje.
    /// </summary>
    public bool AlwaysCrits { get; set; }
    /// <summary>
    /// Pobiera lub ustawia współczynnik kradzieży życia (jako ułamek zadanych obrażeń).
    /// </summary>
    public double LifeSteal { get; set; }
    /// <summary>
    /// Pobiera lub ustawia wartość penetracji pancerza.
    /// </summary>
    public double ArmorPen { get; set; }
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DealDamage"/>.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizm deserializacji.
    /// </remarks>
    public DealDamage() {}

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DealDamage"/> z określonymi parametrami.
    /// </summary>
    /// <param name="damageType">Typ zadawanych obrażeń.</param>
    /// <param name="damageBase">Baza obrażeń (minimalne/losowe/maksymalne).</param>
    /// <param name="damageMultiplier">Mnożnik obrażeń.</param>
    /// <param name="canCrit">Czy umiejętność może krytykować.</param>
    /// <param name="alwaysCrits">Czy umiejętność zawsze krytykuje.</param>
    /// <param name="lifeSteal">Współczynnik kradzieży życia (jako ułamek).</param>
    /// <param name="armorPen">Wartość penetracji pancerza.</param>
    public DealDamage(DamageType damageType, DamageBase damageBase, double damageMultiplier, bool canCrit,
        bool alwaysCrits, double lifeSteal, double armorPen)
    {
        Target = SkillTarget.Enemy;
        DamageType = damageType;
        DamageBase = damageBase;
        DamageMultiplier = damageMultiplier;
        CanCrit = canCrit;
        AlwaysCrits = alwaysCrits;
        LifeSteal = lifeSteal;
        ArmorPen = armorPen;
    }

    /// <summary>
    /// Oblicza wartość obrażeń przed uwzględnieniem obrony celu.
    /// </summary>
    /// <param name="caster">Postać zadająca obrażenia.</param>
    /// <param name="target">Cel ataku.</param>
    /// <returns>Obliczona wartość obrażeń.</returns>
    private double CalculateDamage(Character caster, Character target)
    {
        var damage = DamageBase switch
        {
            DamageBase.Minimal => caster.MinimalAttack,
            DamageBase.Random => UtilityMethods.RandomDouble(caster.MinimalAttack, caster.MaximalAttack + 1),
            DamageBase.Maximal => caster.MaximalAttack
        };
        damage *= DamageMultiplier;
        damage = UtilityMethods.CalculateModValue(damage, GetDamageModifiers(caster, target));
        if ((!CanCrit || !(Random.Shared.NextDouble() < caster.CritChance)) && !AlwaysCrits) return damage;
        if (Random.Shared.NextDouble() <
            UtilityMethods.CalculateModValue(0, target.PassiveEffects.GetModifiers("CritSaveChance"))) return damage;
        damage *= caster.CritMod;

        return damage;
    }

    /// <summary>
    /// Wykonuje efekt zadawania obrażeń.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności).</param>
    /// <remarks>
    /// Oblicza i zadaje obrażenia celowi, uwzględniając wszystkie modyfikatory,
    /// szansę na trafienie krytyczne oraz kradzież życia.
    /// </remarks>
    public void Execute(Character caster, Character enemy, string source)
    {
        var damage = Target switch
        {
            SkillTarget.Self => CalculateDamage(caster, caster),
            SkillTarget.Enemy => CalculateDamage(caster, enemy),
            _ => 0.0
        };
        var mitigatedDamage = Target switch
        {
            SkillTarget.Self => caster.TakeDamage(DamageType, damage, caster),
            SkillTarget.Enemy => enemy.TakeDamage(DamageType, damage, caster),
            _ => damage
        };
        if (!(LifeSteal > 0) || !(damage > 0)) return;
        switch (Target)
        {
            case SkillTarget.Self:
                enemy.Heal(damage * LifeSteal);
                break;
            case SkillTarget.Enemy:
                caster.Heal(damage * LifeSteal);
                break;
        }
    }

    /// <summary>
    /// Pobiera listę modyfikatorów obrażeń dla atakującego i celu.
    /// </summary>
    /// <param name="caster">Postać zadająca obrażenia.</param>
    /// <param name="target">Cel ataku.</param>
    /// <returns>Lista modyfikatorów obrażeń.</returns>
    /// <remarks>
    /// Uwzględnia ogólne modyfikatory obrażeń, modyfikatory dla konkretnych typów obrażeń
    /// oraz modyfikatory przeciwko określonym typom przeciwników.
    /// </remarks>
    private List<StatModifier> GetDamageModifiers(Character caster, Character target)
    {

        var mods = caster.PassiveEffects.GetModifiers("DamageDealtMod");
        switch (DamageType)
        {
            case DamageType.Physical:
                mods.AddRange(caster.PassiveEffects.GetModifiers("PhysicalDamageDealtMod"));
                break;
            case DamageType.Magic:
                mods.AddRange(caster.PassiveEffects.GetModifiers("MagicDamageDealtMod"));
                break;
        }

        if (target is not EnemyCharacter character) return mods;
        foreach (var monsterType in character.EnemyType)
        {
            switch (monsterType)
            {
                case EnemyType.Undead:
                    mods.AddRange(caster.PassiveEffects.GetModifiers("UndeadDamageDealtMod"));
                    break;
                case EnemyType.Beast:
                    mods.AddRange(caster.PassiveEffects.GetModifiers("BeastDamageDealtMod"));
                    break;
                case EnemyType.Human:
                    mods.AddRange(caster.PassiveEffects.GetModifiers("HumanDamageDealtMod"));
                    break;
                case EnemyType.Demon:
                    mods.AddRange(caster.PassiveEffects.GetModifiers("DemonDamageDealtMod"));
                    break;
                default:
                    continue;
            }
            break;
        }
        return mods;
    }
}