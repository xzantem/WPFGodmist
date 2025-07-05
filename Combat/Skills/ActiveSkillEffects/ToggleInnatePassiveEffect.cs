using Character = GodmistWPF.Characters.Character;
using InnatePassiveEffect = GodmistWPF.Combat.Modifiers.PassiveEffects.InnatePassiveEffect;
using SkillTarget = GodmistWPF.Enums.SkillTarget;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

public class ToggleInnatePassiveEffect : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public string PassiveEffect { get; set; }
    public dynamic[]? Effects { get; set; }

    public ToggleInnatePassiveEffect(SkillTarget target, string passiveEffect, dynamic[]? effects = null)
    {
        Target = target;
        PassiveEffect = passiveEffect;
        Effects = effects;
    }
    
    public ToggleInnatePassiveEffect() {} // JSON constructor

    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                if (caster.PassiveEffects.InnateEffects.Any(x => x.Type == PassiveEffect))
                    caster.PassiveEffects.InnateEffects.RemoveAll(x => x.Type == PassiveEffect);
                else
                    caster.PassiveEffects.Add(new InnatePassiveEffect(caster, source, PassiveEffect, Effects));
                break;
            case SkillTarget.Enemy:
                if (enemy.PassiveEffects.InnateEffects.Any(x => x.Type == PassiveEffect))
                    enemy.PassiveEffects.InnateEffects.RemoveAll(x => x.Type == PassiveEffect);
                else
                    enemy.PassiveEffects.Add(new InnatePassiveEffect(enemy, source, PassiveEffect, Effects));
                break;
        }
    }
}