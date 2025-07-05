namespace GodmistWPF.Items;

public interface ICraftable : IItem
{
    public Dictionary<string, int> CraftingRecipe { get; set; }
    public int CraftedAmount { get; set; }
}