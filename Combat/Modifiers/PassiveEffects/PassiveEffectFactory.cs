using GodmistWPF.Combat.Skills.ActiveSkillEffects;
using GodmistWPF.Characters.Player;

namespace GodmistWPF.Combat.Modifiers.PassiveEffects;

public static class PassiveEffectFactory
{
    public static ListenerPassiveEffect StunOnHit(int duration, double chance, string source)
    {
        return new ListenerPassiveEffect(data => data.EventType == "OnHit",
            data => new InflictGenericStatusEffect("Stun", duration, chance,
                source).Execute(PlayerHandler.player, data.Target?.User, source), 
            PlayerHandler.player, source);
    }
}