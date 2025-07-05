using GodmistWPF.Characters;

namespace GodmistWPF.Combat.Modifiers.PassiveEffects;

public class InnatePassiveEffect(Character owner, string source, string type, dynamic[] effects) : 
    PassiveEffect(owner, source)
{
    public string Type { get; } = type;
    public dynamic[] Effects { get; } = effects;
}