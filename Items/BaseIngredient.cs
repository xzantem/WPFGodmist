
using GodmistWPF.Enums.Items;

namespace GodmistWPF.Items;

public class BaseIngredient : BaseItem
{

    public BaseIngredient() // For JSON deserialization
    {
        Stackable = true;
        Weight = 0;
    }  

    public BaseIngredient(string alias, int id, int cost, ItemRarity rarity, string desc, ItemType itemType)
    {
        Alias = alias;
        Weight = 0;
        ID = id;
        Cost = cost;
        Rarity = rarity;
        Stackable = true;
        Description = desc;
        ItemType = itemType;
    }
}