using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Items;
using GodmistWPF.Items.Equippable;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;


namespace GodmistWPF.Items;

public class Inventory
{
    [JsonConverter(typeof(ItemConverter))]
    public Dictionary<IItem, int> Items { get; set; } = new();
    public int PackWeight => Items.Sum(item => item.Key.Weight * item.Value);
    public int MaxPackWeight { get; set; } = 60;

    public void AddItem(IItem item, int quantity = 1)
    {
        if (item.Weight * quantity + PackWeight > MaxPackWeight)
        {
            // For WPF compatibility, just return without console output
            return;
        }
        if (item.Stackable && Items.Keys.Any(x => x.Alias == item.Alias))
        {
            Items[Items.FirstOrDefault(x => x.Key.Alias == item.Alias).Key] += quantity;
            // Item added to stack - handled by WPF dialogs
        }
        else
        {
            Items.Add(item, quantity);
            // New item added - handled by WPF dialogs
        }
    }
    public bool TryRemoveItem(IItem item, int amount = 1)
    {
        if (Items.ContainsKey(item))
        {
            if (Items[item] >= amount)
            {
                Items[item] -= amount;
                if (Items[item] == 0)
                {
                    Items.Remove(item);
                }
                return true;
            }
            // Not enough items - handled by WPF dialogs
            return false;
        }
        // Item not found - handled by WPF dialogs
        return false;
    }
    public void RemoveJunk()
    {
        foreach (var item in Items)
        {
            if (item.Key.ItemType is not (ItemType.Weapon or ItemType.Armor)) continue;
            if ((item.Key as IEquippable).RequiredClass != PlayerHandler.player.CharacterClass)
                TryRemoveItem(item.Key, item.Value);
        }
    }
    public void SortInventory(SortType sortType)
    {
        Items = sortType switch
        {
            SortType.ItemType => Items.OrderBy(item => item.Key.ItemType)
                .ToDictionary(pair => pair.Key, pair => pair.Value),
            SortType.Rarity => Items.OrderByDescending(item => item.Key.Rarity)
                .ToDictionary(pair => pair.Key, pair => pair.Value),
            SortType.Cost => Items.OrderByDescending(item => item.Key.Cost)
                .ToDictionary(pair => pair.Key, pair => pair.Value),
            SortType.Name => Items.OrderBy(item => item.Key.Name).ToDictionary(pair => pair.Key, pair => pair.Value),
            _ => Items
        };
    }

    public void UseItem(IItem item)
    {
        if (Items.ContainsKey(item))
        {
            if (item is not IUsable usable) return;
            if (usable.Use())
            {
                TryRemoveItem(item);
            }
        }
        // Item not found - handled by WPF dialogs
    }

    public int GetCount(string itemAlias)
    {
        return Items.Any(x => x.Key.Alias == itemAlias) ? 
            Items.FirstOrDefault(x => x.Key.Alias == itemAlias).Value : 0;
    }
}