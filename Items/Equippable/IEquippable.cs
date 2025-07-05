
using GodmistWPF.Enums;
using GodmistWPF.Enums.Items;
using GodmistWPF.Items.Galdurites;

namespace GodmistWPF.Items.Equippable;

public interface IEquippable : IItem
{
    public int RequiredLevel { get; }
    public CharacterClass RequiredClass { get; }
    public Quality Quality { get; }
    public double UpgradeModifier { get; set; }
    public int BaseCost { get; }
    public List<Galdurite> Galdurites { get; set; }

    public double RarityModifier
    {
        get
        {
            return Rarity switch
            {
                ItemRarity.Destroyed => 0.7,
                ItemRarity.Damaged => 0.85,
                ItemRarity.Junk => 1.05,
                ItemRarity.Common => 1,
                ItemRarity.Uncommon => 1.05,
                ItemRarity.Rare => 1.1,
                ItemRarity.Ancient => 1.2,
                ItemRarity.Legendary => 1.3,
                ItemRarity.Mythical => 1.5,
                ItemRarity.Godly => 1.75,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    public int GalduriteSlots => (int)Math.Floor((UpgradeModifier - 1) / 0.2);

}