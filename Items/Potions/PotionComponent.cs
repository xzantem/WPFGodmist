using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Items;
using GodmistWPF.Text;
using GodmistWPF.Utilities;

namespace GodmistWPF.Items.Potions;

/// <summary>
/// Klasa reprezentująca pojedynczy komponent mikstury, który definiuje jej efekt i właściwości.
/// Każdy komponent odpowiada za konkretny efekt, który jest aktywowany po użyciu mikstury.
/// </summary>
public class PotionComponent
{
    /// <summary>
    /// Pobiera lub ustawia typ efektu, który jest aplikowany przez ten komponent.
    /// </summary>
    public PotionEffect Effect { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia poziom mocy efektu, który wpływa na jego siłę.
    /// </summary>
    public int StrengthTier { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia siłę efektu, która jest skalowana przez katalizator i czas trwania.
    /// </summary>
    public double EffectStrength { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia identyfikator materiału użytego do stworzenia tego komponentu.
    /// </summary>
    public string Material { get; set; }

    /// <summary>
    /// Generuje opis efektu komponentu, uwzględniając modyfikacje z katalizatora.
    /// </summary>
    /// <param name="catalyst">Katalizator mikstury, który może modyfikować efekt (opcjonalny).</param>
    /// <param name="showMaterial">Czy wyświetlać informacje o materiale i jego ilości w ekwipunku.</param>
    /// <param name="amount">Wymagana ilość materiału do stworzenia (domyślnie 1).</param>
    /// <returns>Sformatowany opis efektu komponentu.</returns>
    public string EffectDescription(PotionCatalyst? catalyst, bool showMaterial, int amount = 1)
    {
        var duration = 10 + (catalyst == null ? 0 : (int)(catalyst.Effect == PotionCatalystEffect.Duration ? catalyst.Strength : 0));
        var strength = EffectStrength * duration * (catalyst == null ? 1 : catalyst.Effect == PotionCatalystEffect.Strength ? 1 + catalyst.Strength : 1) / 10.0;
        var condensedDuration = duration - (int)(catalyst == null ? 0 : catalyst.Effect == PotionCatalystEffect.Condensation ? catalyst.Strength : 0);
        return Effect switch
        {
            PotionEffect.HealthRegain => showMaterial 
                ? $"- {strength:P0} {locale.HealthC} instantly " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- {strength:P0} {locale.HealthC} instantly\n",
            PotionEffect.HealthRegen => showMaterial 
                ? $"- {strength:P0} {locale.HealthC} over {condensedDuration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- {strength:P0} {locale.HealthC} over {condensedDuration} turns\n",
            PotionEffect.ResourceRegain => showMaterial 
                ? $"- {strength:P0} " +
                    $"{BattleTextService.ResourceShortText(PlayerHandler.player)} instantly " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- {strength:P0} " +
                    $"{BattleTextService.ResourceShortText(PlayerHandler.player)} instantly\n",
            PotionEffect.ResourceRegen => showMaterial 
                ? $"- {strength:P0} " +
                    $"{BattleTextService.ResourceShortText(PlayerHandler.player)} over {condensedDuration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- {strength:P0} " +
                    $"{BattleTextService.ResourceShortText(PlayerHandler.player)} over {condensedDuration} turns\n",
            PotionEffect.MaxResourceIncrease => showMaterial 
                ? $"- +{strength:P0} " +
                    $"{BattleTextService.ResourceShortText(PlayerHandler.player)} for {duration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- +{strength:P0} " +
                    $"{BattleTextService.ResourceShortText(PlayerHandler.player)} for {duration} turns\n",
            PotionEffect.DamageDealtIncrease => showMaterial 
                ? $"- +{strength:P0} {locale.DamageGenitive} for {duration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- +{strength:P0} {locale.DamageGenitive} for {duration} turns\n",
            PotionEffect.DamageTakenDecrease => showMaterial 
                ? $"- -{strength:P0} {locale.DamageGenitive} for {duration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- -{strength:P0} {locale.DamageGenitive} for {duration} turns\n",
            PotionEffect.ResistanceIncrease => showMaterial 
                ? $"- +{strength:P0} {locale.Resistances} for {duration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- +{strength:P0} {locale.Resistances} for {duration} turns\n",
            PotionEffect.SpeedIncrease => showMaterial 
                ? $"- +{strength:P0} {locale.Speed} for {duration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- +{strength:P0} {locale.Speed} for {duration} turns\n",
            PotionEffect.CritChanceIncrese => showMaterial 
                ? $"- +{strength:P0} {locale.CritChance} for {duration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- +{strength:P0} {locale.CritChance} for {duration} turns\n",
            PotionEffect.DodgeIncrease => showMaterial 
                ? $"- +{strength:F0} {locale.Dodge} for {duration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- +{strength:F0} {locale.Dodge} for {duration} turns\n",
            PotionEffect.AccuracyIncrease => showMaterial 
                ? $"- +{strength:P0} {locale.Accuracy} for {duration} turns " + 
                    $"({NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.GetCount(Material)}/{amount}))\n" 
                : $"- +{strength:P0} {locale.Accuracy} for {duration} turns\n",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}