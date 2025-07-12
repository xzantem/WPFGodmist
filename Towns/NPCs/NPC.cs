using Newtonsoft.Json;
using CraftingManager = GodmistWPF.Items.CraftingManager;
using ICraftable = GodmistWPF.Items.ICraftable;
using IItem = GodmistWPF.Items.IItem;
using NameAliasHelper = GodmistWPF.Utilities.NameAliasHelper;
using NPCInventory = GodmistWPF.Items.NPCInventory;
using PlayerHandler = GodmistWPF.Characters.Player.PlayerHandler;

namespace GodmistWPF.Towns.NPCs;

/// <summary>
/// Klasa bazowa dla wszystkich NPC (niezależnych postaci) w grze.
/// </summary>
/// <remarks>
/// Klasa NPC reprezentuje niezależną postać w grze, która może mieć swój ekwipunek, poziom lojalności i możliwość tworzenia przedmiotów.
/// </remarks>
public abstract class NPC
{
    /// <summary>
    /// Pobiera lub ustawia unikalny identyfikator NPC.
    /// </summary>
    /// <value>Unikalny identyfikator NPC.</value>
    [JsonIgnore]
    public string Alias { get; set; }

    /// <summary>
    /// Pobiera przyjazną nazwę NPC na podstawie jego aliasu.
    /// </summary>
    /// <value>Przyjazna nazwa NPC.</value>
    public string Name => NameAliasHelper.GetName(Alias);
    
    /// <summary>
    /// Pobiera lub ustawia ekwipunek NPC.
    /// </summary>
    /// <value>Ekwipunek NPC.</value>
    public NPCInventory Inventory { get; set; }
    /// <summary>
    /// Pobiera lub ustawia listę przedmiotów, które można wytworzyć u danego NPC.
    /// </summary>
    /// <value>Lista przedmiotów, które można wytworzyć u danego NPC.</value>
    [JsonIgnore]
    public List<ICraftable?> CraftableItems { get; set; }
     
    /// <summary>
    /// Pobiera lub ustawia poziom lojalności gracza u danego NPC.
    /// </summary>
    /// <value>Poziom lojalności gracza u danego NPC.</value>
    public int LoyaltyLevel { get; set; }
    /// <summary>
    /// Pobiera lub ustawia ilość złota wydaną u danego NPC.
    /// </summary>
    public int GoldSpent { get; set; }
    /// <summary>
    /// Pobiera wymaganą ilość złota do wydania, aby osiągnąć następny poziom lojalności.
    /// </summary>
    public int RequiredGoldSpent => CalculateGoldRequired(LoyaltyLevel);

    /// <summary>
    /// Pobiera modyfikator kosztu usług w zależności od poziomu lojalności.
    /// </summary>
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
    
    /// <summary>
    /// Wydaje określoną ilość złota u danego NPC, zwiększając jego lojalność.
    /// </summary>
    /// <param name="gold">Ilość złota do wydania.</param>
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
    
    /// <summary>
    /// Aktualizuje asortyment towarów dostępnych u NPC.
    /// </summary>
    public void AddWares()
    {
        Inventory.UpdateWares(LoyaltyLevel);
    }
    
    /// <summary>
    /// Otwiera interfejs tworzenia przedmiotów u danego NPC.
    /// </summary>
    public void CraftItem()
    {
        CraftingManager.OpenCraftingMenu(CraftableItems);
    }
    
    /// <summary>
    /// Pobiera listę przedmiotów dostępnych w sklepie NPC.
    /// </summary>
    /// <returns>Lista informacji o przedmiotach w sklepie.</returns>
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

    /// <summary>
    /// Sprawdza, czy gracz może kupić określoną ilość przedmiotu.
    /// </summary>
    /// <param name="item">Przedmiot do kupienia.</param>
    /// <param name="quantity">Ilość przedmiotu (domyślnie 1).</param>
    /// <returns>True, jeśli gracz może kupić przedmiot; w przeciwnym razie false.</returns>
    public bool CanBuyItem(IItem item, int quantity = 1)
    {
        var player = PlayerHandler.player;
        if (player == null) return false;
        
        var totalCost = item.Cost * quantity;
        return player.Gold >= totalCost;
    }

    /// <summary>
    /// Kupuje określoną ilość przedmiotu od NPC.
    /// </summary>
    /// <param name="item">Przedmiot do kupienia.</param>
    /// <param name="quantity">Ilość przedmiotu (domyślnie 1).</param>
    /// <exception cref="InvalidOperationException">Wyrzucany, gdy gracz nie ma wystarczająco złota.</exception>
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
        Inventory.RemoveAt(Inventory.RotatingShop.Concat(Inventory.BoughtFromPlayer).ToList().FindIndex(i => i.Key == item), quantity);
    }

    /// <summary>
    /// Sprawdza, czy gracz może sprzedać określoną ilość przedmiotu.
    /// </summary>
    /// <param name="item">Przedmiot do sprzedania.</param>
    /// <param name="quantity">Ilość przedmiotu (domyślnie 1).</param>
    /// <returns>True, jeśli gracz może sprzedać przedmiot; w przeciwnym razie false.</returns>
    public bool CanSellItem(IItem item, int quantity = 1)
    {
        var player = PlayerHandler.player;
        if (player == null) return false;
        
        return player.Inventory.GetCount(item.Alias) >= quantity;
    }

    /// <summary>
    /// Sprzedaje określoną ilość przedmiotu NPC.
    /// </summary>
    /// <param name="item">Przedmiot do sprzedania.</param>
    /// <param name="quantity">Ilość przedmiotu (domyślnie 1).</param>
    /// <exception cref="InvalidOperationException">Wyrzucany, gdy gracz nie ma wystarczającej ilości przedmiotów.</exception>
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

/// <summary>
/// Klasa przechowująca informacje o przedmiocie w sklepie.
/// </summary>
public class ShopItemInfo
{
    /// <summary>
    /// Pobiera lub ustawia przedmiot.
    /// </summary>
    public IItem Item { get; set; }
    /// <summary>
    /// Pobiera lub ustawia ilość przedmiotu.
    /// </summary>
    public int Quantity { get; set; }
    /// <summary>
    /// Pobiera lub ustawia koszt pojedynczego przedmiotu.
    /// </summary>
    public int Cost { get; set; }
    /// <summary>
    /// Pobiera lub ustawia wartość wskazującą, czy przedmiot jest składany.
    /// </summary>
    public bool IsStackable { get; set; }
    
    /// <summary>
    /// Pobiera nazwę wyświetlaną przedmiotu (z ilością, jeśli przedmiot jest składany).
    /// </summary>
    public string DisplayName => IsStackable ? $"{Item.Name} (x{Quantity})" : Item.Name;
    /// <summary>
    /// Pobiera sformatowany koszt przedmiotu (z kosztem całkowitym, jeśli przedmiot jest składany).
    /// </summary>
    public string DisplayCost => IsStackable ? $"{Cost} Gold (Total: {Cost * Quantity})" : $"{Cost} Gold";
}