using Character = GodmistWPF.Characters.Character;
using SkillTarget = GodmistWPF.Enums.SkillTarget;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

public class ExtendDoT : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public string DoTType { get; set; }
    public int Duration { get; set; }

    public ExtendDoT(SkillTarget target, string doTType, int duration)
    {
        if (doTType != "Bleed" && doTType != "Poison" && doTType != "Burn")
        {
            throw new ArgumentException("Invalid DoT type. Must be Bleed, Poison, or Burn.");
        }
        Target = target;
        DoTType = doTType;
        Duration = duration;
    }
    
    public ExtendDoT() {} // JSON constructor

    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                foreach (var effect in caster.PassiveEffects.TimedEffects.Where(e => e.Type == DoTType))
                {
                    effect.Extend(Duration);
                    return;
                }
                break;
            case SkillTarget.Enemy:
                foreach (var effect in caster.PassiveEffects.TimedEffects.Where(e => e.Type == DoTType))
                {
                    effect.Extend(Duration);
                    return;
                }
                break;
        }
    }
}