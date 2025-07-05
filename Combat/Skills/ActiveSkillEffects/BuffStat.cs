using ConsoleGodmist;
using BattleManager = GodmistWPF.Combat.Battles.BattleManager;
using Character = GodmistWPF.Characters.Character;
using ModifierType = GodmistWPF.Enums.Modifiers.ModifierType;
using SkillTarget = GodmistWPF.Enums.SkillTarget;
using StatModifier = GodmistWPF.Combat.Modifiers.StatModifier;
using StatType = GodmistWPF.Enums.Modifiers.StatType;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

public class BuffStat : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public StatType StatToBuff { get; set; }
    public ModifierType ModifierType { get; set; }
    public double BuffStrength { get; set; }
    public double BuffChance { get; set; }
    public int BuffDuration { get; set; }

    public BuffStat(SkillTarget target, StatType statToBuff, ModifierType modifierType, double buffStrength,
        double buffChance, int buffDuration)
    {
        Target = target;
        StatToBuff = statToBuff;
        ModifierType = modifierType;
        BuffStrength = buffStrength;
        BuffChance = buffChance;
        BuffDuration = buffDuration;
    }
    public BuffStat() {}

    public void Execute(Character caster, Character enemy, string source)
    {
        var target = Target switch
        {
            SkillTarget.Self => caster,
            SkillTarget.Enemy => enemy
        };
        if (!(Random.Shared.NextDouble() < BuffChance)) return;
        target.AddModifier(StatToBuff,
            new StatModifier(ModifierType, BuffStrength, source, BuffDuration));
        var txt1 = StatToBuff switch
        {
            StatType.MaximalHealth => $"{locale.HealthC}",
            StatType.MinimalAttack => $"{locale.MinimalAttack}",
            StatType.MaximalAttack => $"{locale.MaximalAttack}",
            StatType.Dodge => $"{locale.Dodge}",
            StatType.PhysicalDefense => $"{locale.PhysicalDefense}",
            StatType.MagicDefense => $"{locale.MagicDefense}",
            StatType.CritChance => $"{locale.CritChance}",
            StatType.Speed => $"{locale.Speed}",
            StatType.Accuracy => $"{locale.Accuracy}",
            StatType.MaximalResource => $"{locale.MaximalResource}",
        };
        var txt2 = ModifierType switch
        {
            ModifierType.Relative or ModifierType.Multiplicative => $"{BuffStrength:P}",
            ModifierType.Absolute or ModifierType.Additive => $"{BuffStrength:#}",
        };
        BattleManager.CurrentBattle?.Interface.AddBattleLogLines(
            $"{target.Name}: {txt1} {locale.IsBuffed} {txt2} {locale.ForTheNext} {BuffDuration} {locale.Turns}");
    }
}