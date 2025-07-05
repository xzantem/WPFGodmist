

using ConsoleGodmist;
using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Items;
using GodmistWPF.Utilities;

namespace GodmistWPF.Items.Potions;

public class PotionCatalyst(PotionCatalystEffect effect, int tier)
{
    public PotionCatalystEffect Effect { get; private set; } = effect;
    public int Tier { get; private set; } = tier;
    public double Strength { get; private set; } = PotionManager.GetCatalystStrength(effect, tier);
    public string Material { get; private set; } = PotionManager.GetCatalystMaterial(effect, tier);

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