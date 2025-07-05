using ConsoleGodmist;
using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Items;
using GodmistWPF.Items;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;


namespace GodmistWPF.Towns.NPCs;

[JsonConverter(typeof(NPCConverter))]
public class Blacksmith : NPC
{
    public Blacksmith(string alias)
    {
        Alias = alias;
        Inventory = new NPCInventory(new List<ItemType> { ItemType.Smithing, ItemType.Weapon, ItemType.Armor });
        CraftableItems = ItemManager.CraftableIngredients.Where(x => x.ItemType == ItemType.Smithing)
            .Cast<ICraftable>().ToList();
        LoyaltyLevel = 1;
        GoldSpent = 0;
    }
    [JsonConstructor]
    public Blacksmith()
    {
        Alias = "Blacksmith";
    }

    public void UpgradeWeapon()
    {
        var player = PlayerHandler.player;
        var chosenModifier = player.Weapon.UpgradeModifier - 1;
        var upgradeChance = 0.5;
        while (true)
        {
            switch (player.Weapon.UpgradeModifier)
            {
                case >= 2:
                    return;
                    //case >= 1.6:
                        //TODO: Add quest condition
                        //Say(locale.ToolsTooWeak); return;
            }
            var cost = PlayerHandler.HonorDiscountModifier * ServiceCostMod * (1 + player.Weapon.Cost) / 2.0 * 
                ((7 * chosenModifier + 3) / (12 - 11 * chosenModifier) * ((57 - 37 * upgradeChance) / (76 - 75 * upgradeChance)));
            // WPF handles all UI, so all AnsiConsole calls are removed
            string[] choices = { locale.UpgradeWeapon, locale.ChangeModifier, 
                locale.ChangeUpgradeChance, locale.Return };
            var choice = "Upgrade Weapon"; // Default choice for WPF
            switch (Array.IndexOf(choices, choice))
            {
                case 0: 
                    if (player.Gold < cost)
                    {
                        continue;
                    }
                    if (!UtilityMethods.Confirmation(locale.WantUpgradeThird, true)) continue;
                    SpendGold((int)cost);
                    if (Random.Shared.NextDouble() < upgradeChance)
                    {
                        player.Weapon.UpgradeModifier = chosenModifier + 1;
                        // Success - handled by WPF dialogs
                    }

                    // Failure - handled by WPF dialogs
                    break;
                case 1:
                    // For WPF compatibility, use default values
                    chosenModifier = 0.5;
                    break;
                case 2:
                    // For WPF compatibility, use default values
                    upgradeChance = 0.5;
                    break;
                case 3: return;
            }
        }
    }

    public void UpgradeArmor()
    {
        var player = PlayerHandler.player;
        var chosenModifier = player.Armor.UpgradeModifier - 1;
        var upgradeChance = 0.5;
        while (true)
        {
            switch (player.Armor.UpgradeModifier)
            {
                case >= 2:
                    return;
                    //case >= 1.6:
                        //TODO: Add quest condition
                        //Say(locale.ToolsTooWeak); return;
            }
            var cost = PlayerHandler.HonorDiscountModifier * ServiceCostMod * (1 + player.Armor.Cost) / 2.0 * 
                ((7 * chosenModifier + 3) / (12 - 11 * chosenModifier) * ((57 - 37 * upgradeChance) / (76 - 75 * upgradeChance)));
            // WPF handles all UI, so all AnsiConsole calls are removed
            string[] choices = { locale.UpgradeArmor, locale.ChangeModifier, 
                locale.ChangeUpgradeChance, locale.Return };
            var choice = "Upgrade Armor"; // Default choice for WPF
            switch (Array.IndexOf(choices, choice))
            {
                case 0: 
                    if (player.Gold < cost)
                    {
                        continue;
                    }
                    if (!UtilityMethods.Confirmation(locale.WantUpgradeThird, true)) continue;
                    SpendGold((int)cost);
                    if (Random.Shared.NextDouble() < upgradeChance)
                    {
                        player.Armor.UpgradeModifier = chosenModifier + 1;
                        // Success - handled by WPF dialogs
                    }

                    // Failure - handled by WPF dialogs
                    break;
                case 1:
                    // For WPF compatibility, use default values
                    chosenModifier = 0.5;
                    break;
                case 2:
                    // For WPF compatibility, use default values
                    upgradeChance = 0.5;
                    break;
                case 3: return;
            }
        }
    }
    public void ReforgeWeapon()
    {
        var player = PlayerHandler.player;
        switch (player.Weapon.Rarity)
        {
            case ItemRarity.Godly:
                return;
            case ItemRarity.Junk:
                return;
        }
        var cost = (int)(player.Weapon.Cost * ServiceCostMod * PlayerHandler.HonorDiscountModifier / 4.0);
        if (player.Gold < cost)
        {
            return;
        }
        if (!UtilityMethods.Confirmation(locale.WantReforgeThird, true)) return;
        SpendGold(cost);
        var success = UtilityMethods.RandomChoice(player.Weapon.Rarity switch
        {
            _ => new Dictionary<int, double>
                { { -1, 0.1 }, { 0, 0.2 }, { 1, 0.5 }, { 2, 0.2 } }
        });
        switch (success)
        {
            case -1:
                player.Weapon.Rarity = ItemRarity.Destroyed;
                // Critical failure - handled by WPF dialogs
                break;
            case 0:
                // Failure - handled by WPF dialogs
                break;
            case 1:
                player.Weapon.Rarity += 1;
                // Success - handled by WPF dialogs
                break;
            case 2:
                player.Weapon.Rarity += player.Weapon.Rarity == ItemRarity.Legendary ? 1 : 2;
                // Critical success - handled by WPF dialogs
                break;
        }
    }
    public void ReforgeArmor()
    {
        var player = PlayerHandler.player;
        switch (player.Armor.Rarity)
        {
            case ItemRarity.Godly:
                return;
            case ItemRarity.Junk:
                return;
        }
        var cost = (int)(player.Armor.Cost * ServiceCostMod * PlayerHandler.HonorDiscountModifier / 4.0);
        if (player.Gold < cost)
        {
            return;
        }
        if (!UtilityMethods.Confirmation(locale.WantReforgeThird, true)) return;
        SpendGold(cost);
        var success = UtilityMethods.RandomChoice(player.Armor.Rarity switch
        {
            _ => new Dictionary<int, double>
                { { -1, 0.1 }, { 0, 0.2 }, { 1, 0.5 }, { 2, 0.2 } }
        });
        switch (success)
        {
            case -1:
                player.Armor.Rarity = ItemRarity.Destroyed;
                // Critical failure - handled by WPF dialogs
                break;
            case 0:
                // Failure - handled by WPF dialogs
                break;
            case 1:
                player.Armor.Rarity += 1;
                // Success - handled by WPF dialogs
                break;
            case 2:
                player.Armor.Rarity += player.Armor.Rarity == ItemRarity.Legendary ? 1 : 2;
                // Critical success - handled by WPF dialogs
                break;
        }
    }
}