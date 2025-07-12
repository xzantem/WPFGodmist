using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Items;
using GodmistWPF.Utilities;

namespace GodmistWPF.Items.Potions;

/// <summary>
/// Klasa reprezentująca katalizator mikstury, który modyfikuje jej właściwości.
/// Katalizator może wpływać na pojemność, siłę efektów, czas trwania lub kondensację mikstury.
/// </summary>
public class PotionCatalyst(PotionCatalystEffect effect, int tier)
{
    /// <summary>
    /// Pobiera typ efektu, który jest aplikowany przez ten katalizator.
    /// </summary>
    public PotionCatalystEffect Effect { get; private set; } = effect;
    
    /// <summary>
    /// Pobiera poziom katalizatora, który wpływa na jego siłę.
    /// </summary>
    public int Tier { get; private set; } = tier;
    
    /// <summary>
    /// Pobiera siłę efektu katalizatora, obliczaną na podstawie typu i poziomu.
    /// </summary>
    public double Strength { get; private set; } = PotionManager.GetCatalystStrength(effect, tier);
    
    /// <summary>
    /// Pobiera identyfikator materiału użytego jako ten katalizator.
    /// </summary>
    public string Material { get; private set; } = PotionManager.GetCatalystMaterial(effect, tier);

    /// <summary>
    /// Generuje opis katalizatora, uwzględniający jego efekt i dostępność w ekwipunku.
    /// </summary>
    /// <returns>Sformatowany opis katalizatora z informacją o ilości w ekwipunku.</returns>
    public string DescriptionText()
    {
        return effect switch
        {
            PotionCatalystEffect.Capacity => $"- {locale.PotionCapacity}: +{Strength}x ({NameAliasHelper.GetName(Material)} " +
            $"({PlayerHandler.player.Inventory.GetCount(Material)}))\n",
            PotionCatalystEffect.Strength => $"- {locale.EffectStrength}: +{Strength:P0} ({NameAliasHelper.GetName(Material)} " +
            $"({PlayerHandler.player.Inventory.GetCount(Material)}))\n",
            PotionCatalystEffect.Duration => $"- {locale.Duration}: +{Strength}t ({NameAliasHelper.GetName(Material)} " +
            $"({PlayerHandler.player.Inventory.GetCount(Material)}))\n",
            PotionCatalystEffect.Condensation => $"- {locale.Condensation}: -{Strength}t ({NameAliasHelper.GetName(Material)} " +
            $"({PlayerHandler.player.Inventory.GetCount(Material)}))\n",
            _ => throw new ArgumentOutOfRangeException(nameof(effect), effect, null)
        };
    }
}