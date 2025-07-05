using ConsoleGodmist;
using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Items;
using GodmistWPF.Items;
using GodmistWPF.Items.Potions;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;


namespace GodmistWPF.Towns.NPCs;

[JsonConverter(typeof(NPCConverter))]
public class Alchemist : NPC
{
    public Alchemist(string alias)
    {
        Alias = alias;
        Inventory = new NPCInventory([ItemType.Alchemy, ItemType.Potion]);
        CraftableItems = ItemManager.CraftableIngredients.Where(x => x.ItemType == ItemType.Alchemy)
            .Cast<ICraftable>().ToList();
        LoyaltyLevel = 1;
        GoldSpent = 0;
    }
    [JsonConstructor]
    public Alchemist()
    {
        Alias = "Alchemist";
    }
    

    public void Treat()
    {
        var player = PlayerHandler.player;
        var cost = (int)(ServiceCostMod * PlayerHandler.HonorDiscountModifier * 
            Math.Pow(4, (player.Level - 1) / 10.0) * 0.1 * player.MaximalHealth * 
            (player.MaximalHealth - player.CurrentHealth) / (2 * player.MaximalHealth - player.CurrentHealth));
        if (player.Gold < cost)
        {
            return;
        }
        if (!UtilityMethods.Confirmation(locale.WantTreatmentThird, true)) return;
        SpendGold(cost);
        player.Heal(player.MaximalHealth - player.CurrentHealth);
    }
    public void RefillPotion()
    {
        var player = PlayerHandler.player;
        var potions = player.Inventory.Items
            .Where(x => x.Key.ItemType == ItemType.Potion)
            .Select(x => x.Key).Cast<Potion>().ToList();
        if (potions.Count == 0 || potions.All(x => x.MaximalCharges == x.CurrentCharges))
        {
            return;
        }
        var cost = (int)(6 * PlayerHandler.HonorDiscountModifier * ServiceCostMod * Math.Pow(4, (player.Level - 1)/10.0));
        var potion = PotionManager.ChoosePotion(potions, true);
        if (potion == null) return;
        // For WPF compatibility, use a default amount
        var amount = potion.MaximalCharges - potion.CurrentCharges;
        if (player.Gold < amount * cost)
        {
            return;
        }
        if (player.Inventory.GetCount(potion.Components[0].Material) == 0 && player.Inventory.GetCount(potion.Components[1].Material) == 0 
            && player.Inventory.GetCount(potion.Components[2].Material) == 0 )
        {
            // For WPF compatibility, just return without console output
            return;
        }
        if (!UtilityMethods.Confirmation(locale.WantRefillThird, true)) return;
        SpendGold((amount * cost));
        player.Inventory.TryRemoveItem(ItemManager.GetItem(potion.Components[0].Material));
        player.Inventory.TryRemoveItem(ItemManager.GetItem(potion.Components[1].Material));
        player.Inventory.TryRemoveItem(ItemManager.GetItem(potion.Components[2].Material));
        potion.Refill(amount);
    }
}