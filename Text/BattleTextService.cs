using ConsoleGodmist;
using GodmistWPF.Characters;
using GodmistWPF.Characters.Player;
using GodmistWPF.Combat.Battles;
using GodmistWPF.Combat.Skills;
using GodmistWPF.Enums;
using GodmistWPF.Utilities;

namespace GodmistWPF.Text;

public static class BattleTextService
{
    public static string ResourceShortText(Character character)
    {
        return character.ResourceType switch
        {
            ResourceType.Fury => locale.FuryShort,
            ResourceType.Mana => locale.ManaShort,
            ResourceType.Momentum => locale.MomentumShort
        };
    }
    
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

    public static string SkillDescription(ActiveSkill skill)
    {
        // ActiveSkill doesn't have a Description property, return empty string for now
        return "";
    }
}