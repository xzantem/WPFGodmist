namespace GodmistWPF.Items.Drops;

public class ItemDrop
{
    public int MinAmount { get; set; }
    public int MaxAmount { get; set; }
    public int MinLevel { get; set; }
    public int MaxLevel { get; set; }
    
    public int Weight { get; set; }
    public ItemDrop() { } // For JSON deserialization
    public ItemDrop(int minLevel, int maxLevel, int weight, int minAmount = 1, int maxAmount = 1)
    {
        MinLevel = minLevel;
        MaxLevel = maxLevel;
        MinAmount = minAmount;
        MaxAmount = maxAmount;
        Weight = weight;
    }
}