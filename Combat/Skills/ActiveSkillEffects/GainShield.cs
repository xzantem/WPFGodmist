using Character = GodmistWPF.Characters.Character;
using DamageBase = GodmistWPF.Enums.DamageBase;
using SkillTarget = GodmistWPF.Enums.SkillTarget;
using StatusEffectFactory = GodmistWPF.Combat.Modifiers.PassiveEffects.StatusEffectFactory;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

public class GainShield : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public DamageBase ShieldBase { get; set; }
    public double ShieldStrength { get; set; }
    public double ShieldChance { get; set; }
    public int ShieldDuration { get; set; }

    public GainShield(SkillTarget target, DamageBase shieldBase,
        double shieldStrength, double shieldChance, int shieldDuration)
    {
        Target = target;
        ShieldBase = shieldBase;
        ShieldStrength = shieldStrength;
        ShieldChance = shieldChance;
        ShieldDuration = shieldDuration;
    }

    public GainShield() {}

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