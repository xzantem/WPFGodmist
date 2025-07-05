using ConsoleGodmist;
using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Items;
using GodmistWPF.Text;
using GodmistWPF.Utilities;

namespace GodmistWPF.Items.Potions;

public class PotionComponent
{
    public PotionEffect Effect { get; set; }
    public int StrengthTier { get; set; }
    public double EffectStrength { get; set; }
    public string Material { get; set; }

    public string EffectDescription(PotionCatalyst? catalyst, bool showMaterial, int amount = 1)
    {
        var duration = 10 + (catalyst == null ? 0 : (int)(catalyst.Effect == PotionCatalystEffect.Duration ? catalyst.Strength : 0));
        var strength = EffectStrength * duration * (catalyst == null ? 1 : catalyst.Effect == PotionCatalystEffect.Strength ? 1 + catalyst.Strength : 1) / 10.0;
        var condensedDuration = duration - (int)(catalyst == null ? 0 : catalyst.Effect == PotionCatalystEffect.Condensation ? catalyst.Strength : 0);
        return Effect switch
        {
            PotionEffect.HealthRegain => showMaterial 
                ? $"- {locale.HealthC} Regain: {strength:P0} {locale.HealthC} instantly " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- {locale.HealthC} Regain: {strength:P0} {locale.HealthC} instantly\n",
            PotionEffect.HealthRegen => showMaterial 
                ? $"- {locale.HealthC} Regen: {strength:P0} {locale.HealthC} over {condensedDuration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- {locale.HealthC} Regen: {strength:P0} {locale.HealthC} over {condensedDuration} turns\n",
            PotionEffect.ResourceRegain => showMaterial 
                ? $"- {BattleTextService.ResourceShortText(PlayerHandler.player)} Regain: {strength:P0} " +
                    $"{BattleTextService.ResourceShortText(PlayerHandler.player)} instantly " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- {BattleTextService.ResourceShortText(PlayerHandler.player)} Regain: {strength:P0} " +
                    $"{BattleTextService.ResourceShortText(PlayerHandler.player)} instantly\n",
            PotionEffect.ResourceRegen => showMaterial 
                ? $"- {BattleTextService.ResourceShortText(PlayerHandler.player)} Regen: {strength:P0} " +
                    $"{BattleTextService.ResourceShortText(PlayerHandler.player)} over {condensedDuration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- {BattleTextService.ResourceShortText(PlayerHandler.player)} Regen: {strength:P0} " +
                    $"{BattleTextService.ResourceShortText(PlayerHandler.player)} over {condensedDuration} turns\n",
            PotionEffect.MaxResourceIncrease => showMaterial 
                ? $"- Max {BattleTextService.ResourceShortText(PlayerHandler.player)} Increase: +{strength:P0} " +
                    $"{BattleTextService.ResourceShortText(PlayerHandler.player)} for {duration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- Max {BattleTextService.ResourceShortText(PlayerHandler.player)} Increase: +{strength:P0} " +
                    $"{BattleTextService.ResourceShortText(PlayerHandler.player)} for {duration} turns\n",
            PotionEffect.DamageDealtIncrease => showMaterial 
                ? $"- {locale.DamageDealt}: +{strength:P0} {locale.DamageGenitive} for {duration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- {locale.DamageDealt}: +{strength:P0} {locale.DamageGenitive} for {duration} turns\n",
            PotionEffect.DamageTakenDecrease => showMaterial 
                ? $"- {locale.DamageTaken}: -{strength:P0} {locale.DamageGenitive} for {duration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- {locale.DamageTaken}: -{strength:P0} {locale.DamageGenitive} for {duration} turns\n",
            PotionEffect.ResistanceIncrease => showMaterial 
                ? $"- {locale.Resistances}: +{strength:P0} {locale.Resistances} for {duration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- {locale.Resistances}: +{strength:P0} {locale.Resistances} for {duration} turns\n",
            PotionEffect.SpeedIncrease => showMaterial 
                ? $"- {locale.Speed}: +{strength:P0} {locale.Speed} for {duration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- {locale.Speed}: +{strength:P0} {locale.Speed} for {duration} turns\n",
            PotionEffect.CritChanceIncrese => showMaterial 
                ? $"- {locale.CritChance}: +{strength:P0} {locale.CritChance} for {duration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- {locale.CritChance}: +{strength:P0} {locale.CritChance} for {duration} turns\n",
            PotionEffect.DodgeIncrease => showMaterial 
                ? $"- {locale.Dodge}: +{strength:F0} {locale.Dodge} for {duration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- {locale.Dodge}: +{strength:F0} {locale.Dodge} for {duration} turns\n",
            PotionEffect.AccuracyIncrease => showMaterial 
                ? $"- {locale.Accuracy}: +{strength:P0} {locale.Accuracy} for {duration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- {locale.Accuracy}: +{strength:P0} {locale.Accuracy} for {duration} turns\n",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}