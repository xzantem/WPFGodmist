using Character = GodmistWPF.Characters.Character;
using DamageBase = GodmistWPF.Enums.DamageBase;
using SkillTarget = GodmistWPF.Enums.SkillTarget;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

public class HealTarget : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public double HealAmount { get; set; }
    public DamageBase HealBase { get; set; }
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

    public HealTarget(SkillTarget target, double healAmount, DamageBase healBase)
    {
        Target = target;
        HealAmount = healAmount;
        HealBase = healBase;
    }
    public HealTarget() {}

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