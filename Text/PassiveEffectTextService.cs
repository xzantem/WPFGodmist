
using GodmistWPF.Combat.Modifiers;
using GodmistWPF.Combat.Modifiers.PassiveEffects;
using GodmistWPF.Enums.Modifiers;
using GodmistWPF.Utilities;

namespace GodmistWPF.Text;

public static class PassiveEffectTextService
{
    public static string PassiveEffectShortDescription(PassiveEffect effect)
    {
        // PassiveEffect doesn't have a Name property, return Source instead
        return effect.Source ?? "";
    }
    
    public static string PassiveEffectDescription(PassiveEffect effect)
    {
        // PassiveEffect doesn't have a Description property, return empty string for now
        return "";
    }
    
    public static string ModifierText(StatModifier modifier, StatType statType)
    {
        return modifier.Type switch
        {
            ModifierType.Absolute or ModifierType.Additive => 
                $"{NameAliasHelper.GetName(statType.ToString())}: " +
                $"{modifier.Mod:+#;-#;0} [{modifier.Duration}]",
            ModifierType.Relative or ModifierType.Multiplicative =>
                $"{NameAliasHelper.GetName(statType.ToString())}: " +
                $"*{1+modifier.Mod:P} [{modifier.Duration}]",
            _ => ""
        };
    }
}

