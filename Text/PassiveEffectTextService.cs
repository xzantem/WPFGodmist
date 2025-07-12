
using GodmistWPF.Combat.Modifiers;
using GodmistWPF.Combat.Modifiers.PassiveEffects;
using GodmistWPF.Enums.Modifiers;
using GodmistWPF.Utilities;

namespace GodmistWPF.Text;

/// <summary>
/// Serwis odpowiedzialny za generowanie tekstów związanych z efektami pasywnymi.
/// </summary>
public static class PassiveEffectTextService
{
    /// <summary>
    /// Zwraca krótki opis efektu pasywnego.
    /// </summary>
    /// <param name="effect">Efekt pasywny, dla którego ma zostać wygenerowany opis.</param>
    /// <returns>Krótki opis efektu lub pusty ciąg, jeśli brak źródła.</returns>
    /// <remarks>
    /// Obecnie zwraca wartość właściwości Source, ponieważ klasa PassiveEffect nie posiada dedykowanej właściwości Name.
    /// </remarks>
    public static string PassiveEffectShortDescription(PassiveEffect effect)
    {
        // PassiveEffect doesn't have a Name property, return Source instead
        return effect.Source ?? "";
    }
    
    /// <summary>
    /// Zwraca pełny opis efektu pasywnego.
    /// </summary>
    /// <param name="effect">Efekt pasywny, dla którego ma zostać wygenerowany opis.</param>
    /// <returns>Pełny opis efektu lub pusty ciąg, jeśli brak opisu.</returns>
    /// <remarks>
    /// Obecnie zwraca pusty ciąg, ponieważ klasa PassiveEffect nie posiada właściwości Description.
    /// </remarks>
    public static string PassiveEffectDescription(PassiveEffect effect)
    {
        // PassiveEffect doesn't have a Description property, return empty string for now
        return "";
    }
    
    /// <summary>
    /// Generuje sformatowany tekst modyfikatora statystyk.
    /// </summary>
    /// <param name="modifier">Modyfikator, dla którego generowany jest tekst.</param>
    /// <param name="statType">Typ statystyki, której dotyczy modyfikator.</param>
    /// <returns>Sformatowany tekst reprezentujący modyfikator.</returns>
    /// <remarks>
    /// Format zależy od typu modyfikatora:
    /// - Dla typów Absolute i Additive: "[Nazwa statystyki]: [+/-wartość] [czas trwania]"
    /// - Dla typów Relative i Multiplicative: "[Nazwa statystyki]: *wartość% [czas trwania]"
    /// </remarks>
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

