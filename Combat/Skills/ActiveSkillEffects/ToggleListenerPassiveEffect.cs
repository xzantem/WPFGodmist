using Character = GodmistWPF.Characters.Character;
using ListenerPassiveEffect = GodmistWPF.Combat.Modifiers.PassiveEffects.ListenerPassiveEffect;
using SkillTarget = GodmistWPF.Enums.SkillTarget;
using StatType = GodmistWPF.Enums.Modifiers.StatType;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

public class ToggleListenerPassiveEffect : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public string PassiveEffect { get; set; }
    public dynamic[]? Effects { get; set; }

    public ToggleListenerPassiveEffect(SkillTarget target, string passiveEffect, dynamic[]? effects = null)
    {
        Target = target;
        PassiveEffect = passiveEffect;
        Effects = effects;
    }
    
    public ToggleListenerPassiveEffect() {} // JSON constructor

    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                if (caster.PassiveEffects.ListenerEffects.Any(x => x.Source == source))
                    caster.PassiveEffects.ListenerEffects.RemoveAll(x => x.Source == source);
                else
                    caster.PassiveEffects.Add(GetPassiveEffect(caster, enemy, source));
                break;
            case SkillTarget.Enemy:
                if (enemy.PassiveEffects.ListenerEffects.Any(x => x.Source == source))
                    enemy.PassiveEffects.ListenerEffects.RemoveAll(x => x.Source == source);
                else
                    enemy.PassiveEffects.Add(GetPassiveEffect(caster, enemy, source));
                break;
        }
    }

    private ListenerPassiveEffect GetPassiveEffect(Character caster, Character enemy, string source)
    {
        return PassiveEffect switch
        {
            "SlowOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                data => new DebuffStat(SkillTarget.Enemy, StatType.Speed, 
                    Effects[0], Effects[1], Effects[2], Effects[3])
                    .Execute(caster, enemy, source), caster, source)
        };
    }
}