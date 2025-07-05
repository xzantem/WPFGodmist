using GodmistWPF.Utilities;
using Character = GodmistWPF.Characters.Character;
using ModifierType = GodmistWPF.Enums.Modifiers.ModifierType;
using SkillTarget = GodmistWPF.Enums.SkillTarget;
using StatModifier = GodmistWPF.Combat.Modifiers.StatModifier;
using StatusEffectType = GodmistWPF.Enums.Modifiers.StatusEffectType;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

public class DebuffResistance : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public StatusEffectType ResistanceToDebuff { get; set; }
    public ModifierType ModifierType { get; set; }
    public double DebuffStrength { get; set; }
    public double DebuffChance { get; set; }
    public int DebuffDuration { get; set; }

    public DebuffResistance(SkillTarget target, StatusEffectType resistanceToDebuff,
        ModifierType modifier, double debuffStrength, double debuffChance, int debuffDuration)
    {
        Target = target;
        ResistanceToDebuff = resistanceToDebuff;
        ModifierType = modifier;
        DebuffStrength = debuffStrength;
        DebuffChance = debuffChance;
        DebuffDuration = debuffDuration;
    }
    public DebuffResistance() {}

    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                caster.AddResistanceModifier(ResistanceToDebuff,
                    new StatModifier(ModifierType, -DebuffStrength, source, DebuffDuration));
                break;
            case SkillTarget.Enemy:
                if (Random.Shared.NextDouble() <
                    UtilityMethods.EffectChance(enemy.Resistances[StatusEffectType.Debuff].Value(enemy, "DebuffResistance"), DebuffChance))
                {
                    enemy.AddResistanceModifier(ResistanceToDebuff,
                        new StatModifier(ModifierType, -DebuffStrength, source, DebuffDuration));
                }
                break;
        }
    }
}