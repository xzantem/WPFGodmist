using ConsoleGodmist;
using GodmistWPF.Utilities;
using BattleManager = GodmistWPF.Combat.Battles.BattleManager;
using Character = GodmistWPF.Characters.Character;
using SkillTarget = GodmistWPF.Enums.SkillTarget;
using StatusEffectFactory = GodmistWPF.Combat.Modifiers.PassiveEffects.StatusEffectFactory;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

public class InflictDoTStatusEffect : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public int Duration { get; set; }
    public double Strength { get; set; }
    public string DoTType { get; set; }
    public double Chance { get; set; }
    
    public InflictDoTStatusEffect() {} // For JSON serialization

    public InflictDoTStatusEffect(SkillTarget target, int duration, double strength, 
        string doTType, double chance)
    {
        if (doTType != "Bleed" && doTType != "Poison" && doTType != "Burn")
        {
            throw new ArgumentException("Invalid DoT type. Must be Bleed, Poison, or Burn.");
        }
        Target = target;
        Duration = duration;
        Strength = strength;
        DoTType = doTType;
        Chance = chance;
    }


    public void Execute(Character caster, Character enemy, string source)
    {
        var strength = Strength * Random.Shared.Next((int)caster.MinimalAttack, (int)caster.MinimalAttack + 1);
        var chance =
            UtilityMethods.CalculateModValue(Chance, caster.PassiveEffects.GetModifiers("DoTChanceMod"));
        var target = Target switch
        {
            SkillTarget.Self => caster,
            SkillTarget.Enemy => enemy
        };
        if (!StatusEffectFactory.TryAddEffect(target,
                StatusEffectFactory.CreateDoTEffect(target, source, DoTType, strength, Duration, chance))) return;
        var text = DoTType switch
        {
            "Bleed" => $"{target.Name} {locale.Bleeds}, {locale.Taking} {(int)strength} {locale.DamageGenitive}" +
                       $" {locale.ForTheNext} {Duration} {locale.Turns}",
            "Poison" => $"{target.Name} {locale.IsPoisoned}, {locale.Taking} {(int)strength} {locale.DamageGenitive}" +
                        $" {locale.ForTheNext} {Duration} {locale.Turns}",
            "Burn" => $"{target.Name} {locale.Burns}, {locale.Taking} {(int)strength} {locale.DamageGenitive}" +
                  $" {locale.ForTheNext} {Duration} {locale.Turns}"
        };
        BattleManager.CurrentBattle?.Interface.AddBattleLogLines(text);
    }
}