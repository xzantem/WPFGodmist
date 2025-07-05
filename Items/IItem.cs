
using GodmistWPF.Enums.Items;

namespace GodmistWPF.Items;

public interface IItem
{
    public string Name { get; }
    public string Alias { get; set; }
    public int Weight { get; set; }
    public int ID { get; set; }
    public int Cost { get; set; }
    public ItemRarity Rarity { get; set; }
    public bool Stackable { get; set; }
    public string Description { get; set; }
    public ItemType ItemType { get; set; }
    public void Inspect(int amount = 1);
    public void WriteName();
    public void WriteItemType();
    public void WriteRarity();
    public string RarityName();
    public string ItemTypeName();
}