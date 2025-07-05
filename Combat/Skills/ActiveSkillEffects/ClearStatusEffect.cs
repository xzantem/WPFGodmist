using Character = GodmistWPF.Characters.Character;
using SkillTarget = GodmistWPF.Enums.SkillTarget;
using StatusEffectType = GodmistWPF.Enums.Modifiers.StatusEffectType;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

public class ClearStatusEffect : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public StatusEffectType StatusEffectType { get; set; }
    
    public ClearStatusEffect(SkillTarget target, StatusEffectType statusEffectType)
    {
        Target = target;
        StatusEffectType = statusEffectType;
    }
    public ClearStatusEffect() {}
    public void Execute(Character caster, Character enemy, string source)
    {
        //TODO
    }
}