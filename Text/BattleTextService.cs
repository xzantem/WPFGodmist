using GodmistWPF.Characters;
using GodmistWPF.Characters.Player;
using GodmistWPF.Combat.Battles;
using GodmistWPF.Combat.Skills;
using GodmistWPF.Enums;
using GodmistWPF.Utilities;

namespace GodmistWPF.Text;

/// <summary>
/// Serwis odpowiedzialny za generowanie tekstów używanych w interfejsie walki.
/// </summary>
public static class BattleTextService
{
    /// <summary>
    /// Zwraca skróconą nazwę zasobu postaci.
    /// </summary>
    /// <param name="character">Postać, dla której ma zostać zwrócona nazwa zasobu.</param>
    /// <returns>Skrócona nazwa zasobu (np. "FUR", "MANA", "MOM").</returns>
    public static string ResourceShortText(Character character)
    {
        return character.ResourceType switch
        {
            ResourceType.Fury => locale.FuryShort,
            ResourceType.Mana => locale.ManaShort,
            ResourceType.Momentum => locale.MomentumShort
        };
    }
    
    /// <summary>
    /// Generuje sformatowany tekst umiejętności, która nie może być wybrana.
    /// </summary>
    /// <param name="skill">Umiejętność, dla której generowany jest tekst.</param>
    /// <param name="player">Gracz, którego zasoby są sprawdzane.</param>
    /// <returns>Sformatowany tekst z informacją o kosztach umiejętności.</returns>
    /// <remarks>
    /// Format: "- Nazwa umiejętności (koszt_zasobów, koszt_punktów_akcji)"
    /// </remarks>
    public static string UnselectableSkillMarkup(ActiveSkill skill, BattleUser player)
    {
        var resourceCostInfo = (skill.ResourceCost <= player.User.CurrentResource ||
                                Math.Abs(player.User.MaximalResource - player.User.CurrentResource) < 0.01)
            ? $"{(int)UtilityMethods.CalculateModValue(skill.ResourceCost, player.User.PassiveEffects.GetModifiers("ResourceCost"))} " +
              $"{ResourceShortText(player.User as PlayerCharacter)}"
            : $"{(int)UtilityMethods.CalculateModValue(skill.ResourceCost, player.User.PassiveEffects.GetModifiers("ResourceCost"))} " +
              $"{ResourceShortText(player.User as PlayerCharacter)}";
        var actionCostInfo = (skill.ActionCost * player.MaxActionPoints.Value(player.User, "MaxActionPoints")
                              <= player.CurrentActionPoints)
            ? $"{(int)(skill.ActionCost * player.MaxActionPoints.BaseValue)} {locale.ActionPointsShort}"
            : $"{(int)(skill.ActionCost * player.MaxActionPoints.BaseValue)} {locale.ActionPointsShort}";
        return "\n- " + skill.Name + $" ({resourceCostInfo}, {actionCostInfo})";
    }

    /// <summary>
    /// Pobiera opis umiejętności.
    /// </summary>
    /// <param name="skill">Umiejętność, dla której ma zostać zwrócony opis.</param>
    /// <returns>Opis umiejętności lub pusty ciąg, jeśli brak opisu.</returns>
    /// <remarks>
    /// Obecnie zwraca pusty ciąg, ponieważ klasa ActiveSkill nie posiada właściwości Description.
    /// </remarks>
    public static string SkillDescription(ActiveSkill skill)
    {
        // ActiveSkill doesn't have a Description property, return empty string for now
        return "";
    }
}