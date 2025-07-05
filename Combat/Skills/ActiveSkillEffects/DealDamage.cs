using ConsoleGodmist;
using GodmistWPF.Utilities;
using BattleManager = GodmistWPF.Combat.Battles.BattleManager;
using Character = GodmistWPF.Characters.Character;
using DamageBase = GodmistWPF.Enums.DamageBase;
using DamageType = GodmistWPF.Enums.DamageType;
using EnemyCharacter = GodmistWPF.Characters.EnemyCharacter;
using EnemyType = GodmistWPF.Enums.EnemyType;
using SkillTarget = GodmistWPF.Enums.SkillTarget;
using StatModifier = GodmistWPF.Combat.Modifiers.StatModifier;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

public class DealDamage : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public DamageType DamageType { get; set; }
    public DamageBase DamageBase { get; set; }
    public double DamageMultiplier { get; set; }
    public bool CanCrit { get; set; }
    public bool AlwaysCrits { get; set; }
    public double LifeSteal { get; set; }
    public double ArmorPen { get; set; }
    
    public DealDamage() {}

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
        
        BattleManager.CurrentBattle!.Interface.AddBattleLogLines($"{caster.Name} {locale.StrikesCritically}! ");
        return damage;
    }

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