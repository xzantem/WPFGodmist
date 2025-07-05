using Newtonsoft.Json;
using CraftingManager = GodmistWPF.Items.CraftingManager;
using ICraftable = GodmistWPF.Items.ICraftable;
using IItem = GodmistWPF.Items.IItem;
using NameAliasHelper = GodmistWPF.Utilities.NameAliasHelper;
using NPCInventory = GodmistWPF.Items.NPCInventory;
using PlayerHandler = GodmistWPF.Characters.Player.PlayerHandler;

namespace GodmistWPF.Towns.NPCs;

public abstract class NPC
{
    [JsonIgnore]
    public string Alias { get; set; }

    public string Name => NameAliasHelper.GetName(Alias);
    
    public NPCInventory Inventory { get; set; }
    [JsonIgnore]
    public List<ICraftable?> CraftableItems { get; set; }
     
    public int LoyaltyLevel { get; set; }
    public int GoldSpent { get; set; }
    public int RequiredGoldSpent => CalculateGoldRequired(LoyaltyLevel);

    public double ServiceCostMod => LoyaltyLevel switch
    {
        < 2 => 1.0,
        < 4 => 0.99,
        < 7 => 0.96,
        < 9 => 0.91,
        < 12 => 0.84,
        >= 12 => 0.75
    };

    private static int CalculateGoldRequired(int level)
    {
        var value = 0;
        for (var i = 1; i <= Math.Min(level, 14); i++)
        {
            value += (int)Math.Pow(4, i/3.0 + 4);
        }
        return value;
    }
    
    public void SpendGold(int gold)
    {
        GoldSpent += gold;
        while (GoldSpent >= RequiredGoldSpent) {
            if (LoyaltyLevel < 15)
                LoyaltyLevel++;
            else
            {
                GoldSpent = RequiredGoldSpent;
                return;
            }
        }
        PlayerHandler.player.LoseGold(gold);
    }

    public void DisplayShop()
    {
        // This method is now handled by WPF dialogs
        // For console compatibility, do nothing
    }
    public void AddWares()
    {
        Inventory.UpdateWares(LoyaltyLevel);
    }

    public void InspectItem(int index)
    {
        // This method is now handled by WPF dialogs
        // For console compatibility, do nothing
    }
    public void BuyItem(int index)
    {
        // This method is now handled by WPF dialogs
        // For console compatibility, do nothing
    }
    public void SellItem(int index)
    {
        // This method is now handled by WPF dialogs
        // For console compatibility, do nothing
    }
    public void CraftItem()
    {
        CraftingManager.OpenCraftingMenu(CraftableItems);
    }

    // WPF-compatible shop methods that handle business logic
    public List<ShopItemInfo> GetShopItems()
    {
        var items = new List<ShopItemInfo>();
        var allItems = Inventory.RotatingShop.Concat(Inventory.BoughtFromPlayer).ToList();
        
        foreach (var item in allItems)
        {
            items.Add(new ShopItemInfo
            {
                Item = item.Key,
                Quantity = item.Value,
                Cost = item.Key.Cost,
                IsStackable = item.Key.Stackable
            });
        }
        
        return items;
    }

    public bool CanBuyItem(IItem item, int quantity = 1)
    {
        var player = PlayerHandler.player;
        if (player == null) return false;
        
        var totalCost = item.Cost * quantity;
        return player.Gold >= totalCost;
    }

    public void BuyItem(IItem item, int quantity = 1)
    {
        if (!CanBuyItem(item, quantity))
        {
            throw new InvalidOperationException("Not enough gold to buy this item.");
        }

        var player = PlayerHandler.player;
        var totalCost = item.Cost * quantity;
        
        // Remove gold from player
        player.LoseGold(totalCost);
        
        // Add item to player inventory
        player.Inventory.AddItem(item, quantity);
        
        // Remove item from shop inventory
        Inventory.RemoveAt(Inventory.RotatingShop.Concat(Inventory.BoughtFromPlayer).ToList().IndexOf(new KeyValuePair<IItem, int>(item, quantity)), quantity);
    }

    public bool CanSellItem(IItem item, int quantity = 1)
    {
        var player = PlayerHandler.player;
        if (player == null) return false;
        
        return player.Inventory.GetCount(item.Alias) >= quantity;
    }

    public void SellItem(IItem item, int quantity = 1)
    {
        if (!CanSellItem(item, quantity))
        {
            throw new InvalidOperationException("Not enough items to sell.");
        }

        var player = PlayerHandler.player;
        var sellPrice = (int)(item.Cost * 0.5); // 50% of original cost
        
        // Remove item from player inventory
        player.Inventory.TryRemoveItem(item, quantity);
        
        // Add gold to player
        player.GainGold(sellPrice * quantity);
        
        // Add item to shop inventory
        Inventory.AddItem(item, quantity);
    }
}

public class ShopItemInfo
{
    public IItem Item { get; set; }
    public int Quantity { get; set; }
    public int Cost { get; set; }
    public bool IsStackable { get; set; }
    
    public string DisplayName => IsStackable ? $"{Item.Name} (x{Quantity})" : Item.Name;
    public string DisplayCost => IsStackable ? $"{Cost} Gold (Total: {Cost * Quantity})" : $"{Cost} Gold";
}