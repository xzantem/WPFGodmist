

using GodmistWPF.Enums.Items;

namespace GodmistWPF.Items;

public class CraftableIngredient : BaseItem, ICraftable
{
    public Dictionary<string, int> CraftingRecipe { get; set; }
    public int CraftedAmount { get; set; }
    
    public override int Cost => (int)Math.Floor((double)CraftingRecipe
        .Sum(x => x.Value * ItemManager.GetItem(x.Key).Cost) / CraftedAmount);

    public CraftableIngredient() // For JSON deserialization
    {
        Stackable = true;
        Weight = 0;
    }

    public CraftableIngredient(string alias, int id, int cost, ItemRarity rarity,
        string desc, ItemType itemType, Dictionary<string, int> craftingRecipe, int craftedAmount)
    {
        Alias = alias;
        ID = id;
        Cost = cost;
        Rarity = rarity;
        Description = desc;
        ItemType = itemType;
        CraftingRecipe = craftingRecipe;
        Stackable = true;
        Weight = 0;
        CraftedAmount = craftedAmount;
    }
}