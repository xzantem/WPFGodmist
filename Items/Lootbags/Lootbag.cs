using GodmistWPF.Enums.Items;
using GodmistWPF.Items.Drops;
using GodmistWPF.Utilities;

namespace GodmistWPF.Items.Lootbags;

public class Lootbag : BaseItem, IUsable
{
    public new string Name => NameAliasHelper.GetName(Alias);
    public override int Weight => 0;
    public override bool Stackable => true;
    public override ItemType ItemType => ItemType.LootBag;
    
    public override int ID => Alias switch
    {
        "BonySupplyBag" => 563,
        "LeafySupplyBag" => 564,
        "DemonicSupplyBag" => 565,
        "PirateSupplyBag" => 566,
        "SandySupplyBag" => 567,
        "TempleSupplyBag" => 568,
        "MountainousSupplyBag" => 569,
        "WeaponBag" => 570,
        "ArmorBag" => 571,
        "GalduriteBag" => 572,
        "SkeletonExecutionerBag" => 573,
    };
    public DropTable DropTable { get; set; }
    public Lootbag(string alias, int level, DropTable dropTable)
    {
        Alias = alias;
        Level = level;
        DropTable = dropTable;
    }

    protected Lootbag()
    {
    }

    public override ItemRarity Rarity => Level switch
    {
        <=10 => ItemRarity.Common,
        >10 and <= 20 => ItemRarity.Uncommon,
        >20 and <= 30 => ItemRarity.Rare,
        >30 and <= 40 => ItemRarity.Ancient,
        _ => ItemRarity.Legendary
    };
    public int Level { get; set; }
    public bool Use()
    {
        // WPF handles lootbag opening UI
        return false;
    }
}