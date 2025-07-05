using GodmistWPF.Combat.Battles;

namespace GodmistWPF.Combat.Modifiers.PassiveEffects;

public class BattleEventData(string eventType, BattleUser source, BattleUser? target = null, dynamic[]? value = null)
{
    public string EventType { get; } = eventType;
    public BattleUser Source { get; } = source;
    public BattleUser? Target { get; } = target;
    public dynamic[]? Value { get; } = value; // Optional, used for damage, healing, etc.
}