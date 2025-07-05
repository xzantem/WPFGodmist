using ConsoleGodmist;
using GodmistWPF.Enums.Items;
using GodmistWPF.Utilities;


namespace GodmistWPF.Items;

public abstract class BaseItem : IItem
{
    public virtual string Name
    {
        get => NameAliasHelper.GetName(Alias);
        set => Alias = value;
    }

    public virtual string Alias { get; set; }
    public virtual int Weight { get; set; }
    public virtual int ID { get; set; }
    public virtual int Cost { get; set; }
    public virtual ItemRarity Rarity { get; set; }
    public virtual bool Stackable { get; set; }
    public virtual string Description { get; set; }
    public virtual ItemType ItemType { get; set; }
    public virtual void Inspect(int amount = 1){
        // WPF handles item inspection UI
    }
    
    public void WriteName()
    {
        // WPF handles item name display UI
    }

    public void WriteItemType()
    {
        // WPF handles item type display UI
    }

    public void WriteRarity()
    {
        // WPF handles item rarity display UI
    }

    public string RarityName()
    {
        return Rarity switch
        {
            ItemRarity.Common => locale.Common,
            ItemRarity.Uncommon => locale.Uncommon,
            ItemRarity.Rare => locale.Rare,
            ItemRarity.Ancient => locale.Ancient,
            ItemRarity.Legendary => locale.Legendary,
            ItemRarity.Mythical => locale.Mythical,
            ItemRarity.Godly => locale.Godly,
            ItemRarity.Destroyed => locale.Destroyed,
            ItemRarity.Damaged => locale.Damaged,
            ItemRarity.Junk => locale.Junk,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string ItemTypeName()
    {
        return ItemType switch
        {
            ItemType.Weapon => locale.Weapon,
            ItemType.Armor => locale.Armor,
            ItemType.Smithing => locale.Smithing,
            ItemType.Alchemy => locale.Alchemy,
            ItemType.Potion => locale.Potion,
            ItemType.Runeforging => locale.Runeforging,
            ItemType.WeaponGaldurite => locale.WeaponGaldurite,
            ItemType.ArmorGaldurite => locale.ArmorGaldurite,
            ItemType.LootBag => locale.LootBag,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}