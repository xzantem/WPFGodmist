using GodmistWPF.Enums.Items;
using GodmistWPF.Items.Equippable;
using GodmistWPF.Utilities;

namespace GodmistWPF.Items.Galdurites;

public class Galdurite : BaseItem
{
    public new string Name => NameAliasHelper.GetName(Alias);
    public override int Weight => 1;
    public override int ID => 562;
    public override bool Stackable => false;
    public GalduriteComponent[] Components { get; set; }
    
    public override int Cost => (int)(BaseCost * EquippableItemService.RarityPriceModifier(Rarity) * (Revealed ? 
                                      Components.Aggregate(1.0, (x, y) => 
                                          x * ("DCBAS".IndexOf(y.EffectTier) + 18) / 20) : 1));
    
    public int BaseCost { get; set; }
    public int Tier { get; set; }
    public int RequiredLevel => Tier == 3 ? 41 : Tier * 10 + 1; // Tier 1 is Level 11, Tier 2 is Level 21, Tier 3 is Level 41
    public bool Revealed { get; set; }
    
    
    public void Reveal()
    {
        Revealed = true;
    }

    public Galdurite(bool equipmentType, int tier, int bias, string color = "Random")
    {
        ItemType = equipmentType ? ItemType.ArmorGaldurite : ItemType.WeaponGaldurite;
        Alias = (equipmentType, tier) switch
        {
            (false, 1) => "MeagreWeaponGaldurite",
            (false, 2) => "FairWeaponGaldurite",
            (false, 3) => "PotentWeaponGaldurite",
            (true, 1) => "MeagreArmorGaldurite",
            (true, 2) => "FairArmorGaldurite",
            (true, 3) => "PotentArmorGaldurite",
            _ => throw new ArgumentOutOfRangeException(nameof(equipmentType), equipmentType, null)
        };
        Revealed = false;
        Rarity = EquippableItemService.GetRandomGalduriteRarity(bias);
        Tier = tier;
        BaseCost = Tier switch
        {
            1 => 500,
            2 => 1000,
            3 => 12000,
            _ => throw new ArgumentOutOfRangeException(nameof(tier), tier, null)
        };
        var componentCount = Random.Shared.Next(1, 3) + Tier;
        Components = new GalduriteComponent[componentCount];
        var tiers = new List<string>();
        for (var i = 0; i < componentCount; i++)
        {
            tiers.Add(Rarity switch
            {
                ItemRarity.Common => UtilityMethods.RandomChoice(new Dictionary<string, double>
                    { { "D", 0.5 }, { "C", 0.5 } }),
                ItemRarity.Uncommon => UtilityMethods.RandomChoice(new Dictionary<string, double>
                    { { "D", 0.25 }, { "C", 0.5 }, { "B", 0.25 } }),
                ItemRarity.Rare => UtilityMethods.RandomChoice(new Dictionary<string, double>
                    { { "C", 0.5 }, { "B", 0.25 }, { "A", 0.25 } }),
                ItemRarity.Ancient => UtilityMethods.RandomChoice(new Dictionary<string, double>
                    { { "C", 0.25 }, { "B", 0.5 }, { "A", 0.25 } }),
                ItemRarity.Legendary => UtilityMethods.RandomChoice(new Dictionary<string, double>
                    { { "B", 0.5 }, { "A", 0.25 }, { "S", 0.25 } }),
                ItemRarity.Mythical => UtilityMethods.RandomChoice(new Dictionary<string, double>
                    { { "B", 0.25 }, { "A", 0.5 }, { "S", 0.25 } }),
                ItemRarity.Godly => UtilityMethods.RandomChoice(new Dictionary<string, double>
                    { { "A", 0.5 }, { "S", 0.5 } }),
            });
        }
        tiers.Sort((a, b) => "SABCD".IndexOf(a).CompareTo("SABCD".IndexOf(b)));
        var excludedColors = new HashSet<string>();
        for (var i = 0; i < componentCount; i++)
        {
            Components[i] = GalduriteManager.GetGalduriteComponent(tiers[i], i == 0 ? color : "Random", equipmentType, excludedColors);
            excludedColors.Add(Components[i].PoolColor);
        }
    }
    
    public Galdurite() {}

    public override void Inspect(int amount = 1)
    {
        base.Inspect(amount);
        ShowEffects();
    }

    public void ShowEffects()
    {
        // WPF handles galdurite effects display through UI
    }
}