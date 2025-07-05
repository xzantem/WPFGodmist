using GodmistWPF.Characters;
using GodmistWPF.Combat.Skills.ActiveSkillEffects;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Modifiers;
using GodmistWPF.Utilities;

namespace GodmistWPF.Combat.Modifiers.PassiveEffects;

public static class StatusEffectFactory
{
    public static TimedPassiveEffect CreateDoTEffect(Character target, string source,
        string effectType, double strength, int duration, double chance)
    {
        return new TimedPassiveEffect(target, source,
            effectType, duration, [chance, strength], () =>
            {
                target.TakeDamage(
                    effectType switch
                    {
                        "Bleed" => DamageType.Bleed,
                        "Poison" => DamageType.Poison,
                        "Burn" => DamageType.Burn
                    }, strength, effectType);
            });
    }

    public static TimedPassiveEffect CreateStunEffect(Character target, string source, int duration, double chance)
    {
        return new TimedPassiveEffect(target, source, "Stun", duration, [chance]);
    }

    public static TimedPassiveEffect CreateFreezeEffect(Character target, string source, int duration, double chance)
    {
        var t = new TimedPassiveEffect(target, source,
            "Freeze", duration, [chance]);
        t.UpdateOnTick(() =>
        {
            if (t.Duration == 1)
                new DebuffStat(SkillTarget.Self, StatType.Speed, ModifierType.Additive, 
                        duration * 2, 1, duration).Execute(target, target, source);
        });
        return t;
    }

    public static TimedPassiveEffect CreateShieldEffect(Character target, string source, double strength,
        int duration, double chance)
    {
        return new TimedPassiveEffect(target, source, "Shield", duration, [chance, strength]);
    }

    public static bool TryAddEffect(Character target, TimedPassiveEffect effect, bool guaranteed = false)
    {
        if (!guaranteed && target.Resistances.ContainsKey(Enum.GetValues<StatusEffectType>()
                            .FirstOrDefault(x => x.ToString() == effect.Type))
                        && Random.Shared.NextDouble() >= UtilityMethods.EffectChance(target.Resistances
                                [Enum.GetValues<StatusEffectType>().FirstOrDefault(x => x.ToString() == effect.Type)]
                            .Value(target, effect.Type + "Resistance"), effect.Effects[0])) return false;
        target.PassiveEffects.Add(effect);
        return true;

    }
}