using GodmistWPF.Items.Equippable;
using GodmistWPF.Items.Galdurites;
using GodmistWPF.Utilities;


namespace GodmistWPF.Items.Drops;

public class DropPool
{
    public Dictionary<string, ItemDrop> Pool { get; set; }
    public double[] Chances { get; set; }

    public DropPool() { } // For JSON deserialization

    public DropPool(DropPool other)
    {
        Pool = other.Pool.ToDictionary(x => x.Key, x => 
            new ItemDrop(x.Value.MinLevel, x.Value.MaxLevel, x.Value.Weight, x.Value.MinAmount, x.Value.MaxAmount));
        Chances = (double[])other.Chances.Clone();
    }

    public KeyValuePair<IItem, ItemDrop> Choice(int level)
    {
        var choice = UtilityMethods.RandomChoice(Pool
            .Where(item => item.Value.MinLevel <= level && item.Value.MaxLevel >= level)
            .ToDictionary(item => item.Key, item => item.Value)
            .ToDictionary(x => x, x => x.Value.Weight));
        var item = new KeyValuePair<IItem, ItemDrop>();
        var choiceSplit = choice.Key.Split('_');
        item = choiceSplit[0] switch
        {
            "WeaponDrop" => new KeyValuePair<IItem, ItemDrop>(EquippableItemService.GetRandomWeapon(level / 10 + 1),
                choice.Value),
            "ArmorDrop" => new KeyValuePair<IItem, ItemDrop>(EquippableItemService.GetRandomArmor(level / 10 + 1),
                choice.Value),
            "BossEquipmentDrop" => new KeyValuePair<IItem, ItemDrop>(
                EquippableItemService.GetBossDrop(level / 10 + 1, choiceSplit[1]), choice.Value),
            "GalduriteDrop" => new KeyValuePair<IItem, ItemDrop>(
                new Galdurite(Random.Shared.Next(0, 2) == 1, level < 21 ? 1 : level < 41 ? 2 : 3,
                    Convert.ToInt32(choiceSplit[1])), choice.Value),
            _ => new KeyValuePair<IItem, ItemDrop>(ItemManager.GetItem(choice.Key), choice.Value)
        };
        return item;
    }

    public KeyValuePair<IItem, int> GetDrop(int level)
    {
        var drop = Choice(level);
        return new KeyValuePair<IItem, int>(drop.Key, 
            Random.Shared.Next(drop.Value.MinAmount, drop.Value.MaxAmount + 1));
    }
}