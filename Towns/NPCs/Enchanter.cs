using ConsoleGodmist;
using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Items;
using GodmistWPF.Items;
using GodmistWPF.Items.Galdurites;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;


namespace GodmistWPF.Towns.NPCs;

[JsonConverter(typeof(NPCConverter))]
public class Enchanter : NPC
{
    public Enchanter(string alias)
    {
        Alias = alias;
        Inventory = new NPCInventory([ItemType.Runeforging, ItemType.WeaponGaldurite, ItemType.ArmorGaldurite]);
        CraftableItems = ItemManager.CraftableIngredients.Where(x => x.ItemType == ItemType.Runeforging)
            .Cast<ICraftable>().ToList();
        LoyaltyLevel = 1;
        GoldSpent = 0;
    }
    [JsonConstructor]
    public Enchanter()
    {
        Alias = "Enchanter";
    }
    public void ExamineGaldurite()
    {
        var player = PlayerHandler.player;
        var galdurites = player.Inventory.Items
            .Where(x => x.Key.ItemType is ItemType.WeaponGaldurite or ItemType.ArmorGaldurite && 
                        !(x.Key as Galdurite).Revealed)
            .Select(x => x.Key).Cast<Galdurite>().ToList();
        if (galdurites.Count == 0)
        {
            return;
        }
        var galdurite = GalduriteManager.ChooseGaldurite(galdurites);
        if (galdurite == null) return;
        var cost = (int)(PlayerHandler.HonorDiscountModifier * ServiceCostMod * 0.2 * galdurite.Cost);
        if (player.Gold < cost)
        {
            return;
        }
        if (!UtilityMethods.Confirmation(locale.WantRevealThird, true)) return;
        SpendGold(cost);
        galdurite.Reveal();
        galdurite.Inspect();
    }
    public void InsertWeaponGaldurite()
    {
        var player = PlayerHandler.player;
        if (player.Weapon.Galdurites.Count < player.Weapon.GalduriteSlots)
        {
            var galdurites = player.Inventory.Items.Keys
            .Where(x => x.ItemType == ItemType.WeaponGaldurite)
            .Cast<Galdurite>()
            .Where(x => player.Weapon.Rarity >= x.Rarity && player.Weapon.RequiredLevel >= x.RequiredLevel)
            .ToList();
            if (galdurites.Count == 0)
            {
                return;
            }
            var galdurite = GalduriteManager.ChooseGaldurite(galdurites);
            if (galdurite == null) return;
            var cost = (int)(PlayerHandler.HonorDiscountModifier * ServiceCostMod * galdurite.Cost * 0.75);
            if (player.Gold < cost)
            {
                return;
            }
            galdurite.Inspect();
            if (!UtilityMethods.Confirmation(locale.WantApplyThird, true)) return;
            SpendGold(cost);
            player.Weapon.AddGaldurite(galdurite);
            player.Inventory.TryRemoveItem(galdurite);
        }
    }
    public void InsertArmorGaldurite()
    {
        var player = PlayerHandler.player;
        if (player.Armor.Galdurites.Count < player.Armor.GalduriteSlots)
        {
            var galdurites = player.Inventory.Items.Keys
                .Where(x => x.ItemType == ItemType.ArmorGaldurite)
                .Cast<Galdurite>()
                .Where(x => player.Armor.Rarity >= x.Rarity && player.Armor.RequiredLevel >= x.RequiredLevel)
                .ToList();
            if (galdurites.Count == 0)
            {
                return;
            }
            var galdurite = GalduriteManager.ChooseGaldurite(galdurites);
            if (galdurite == null) return;
            var cost = (int)(PlayerHandler.HonorDiscountModifier * ServiceCostMod * galdurite.Cost * 0.75);
            if (player.Gold < cost)
            {
                return;
            }
            galdurite.Inspect();
            if (!UtilityMethods.Confirmation(locale.WantApplyThird, true)) return;
            SpendGold(cost);
            player.Armor.AddGaldurite(galdurite);
            player.Inventory.TryRemoveItem(galdurite);
        }
        else
        {
        }
    }
    public void RemoveWeaponGaldurite()
    {
        var player = PlayerHandler.player;
        var galdurites = player.Weapon.Galdurites;
        if (galdurites.Count == 0)
        {
            return;
        }
        var galdurite = GalduriteManager.ChooseGaldurite(galdurites);
        if (galdurite == null) return;
        var cost = (int)(PlayerHandler.HonorDiscountModifier * ServiceCostMod * galdurite.Cost * 4);
        if (player.Gold < cost)
        {
            return;
        }
        galdurite.Inspect();
        if (!UtilityMethods.Confirmation(locale.WantRemoveThird, true)) return;
        SpendGold(cost);
        player.Weapon.RemoveGaldurite(galdurite);
    }
    public void RemoveArmorGaldurite()
    {
        var player = PlayerHandler.player;
        var galdurites = player.Armor.Galdurites;
        if (galdurites.Count == 0)
        {
            return;
        }
        var galdurite = GalduriteManager.ChooseGaldurite(galdurites);
        if (galdurite == null) return;
        var cost = (int)(PlayerHandler.HonorDiscountModifier * ServiceCostMod * galdurite.Cost * 4);
        if (player.Gold < cost)
        {
            return;
        }
        galdurite.Inspect();
        if (!UtilityMethods.Confirmation(locale.WantRemoveThird, true)) return;
        SpendGold(cost);
        player.Armor.RemoveGaldurite(galdurite);
    }
}