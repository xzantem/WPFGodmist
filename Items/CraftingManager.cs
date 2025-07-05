using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Items;
using GodmistWPF.Items.Equippable.Armors;
using GodmistWPF.Items.Equippable.Weapons;
using GodmistWPF.Items.Galdurites;
using GodmistWPF.Items.Potions;
namespace GodmistWPF.Items;

public static class CraftingManager
{
    private static readonly Inventory Inventory = PlayerHandler.player.Inventory;
    private static readonly List<ItemRarity> RarityFilters = [ItemRarity.Common, 
        ItemRarity.Uncommon, ItemRarity.Rare, ItemRarity.Ancient, ItemRarity.Legendary, ItemRarity.Mythical, ItemRarity.Godly];
    private static bool _showOnlyCraftable;

    public static void OpenCraftingMenu(List<ICraftable> possibleItems)
    {
        // WPF handles crafting menu UI
    }

    public static ICraftable? ChooseItem(List<ICraftable> possibleItems)
    {
        // WPF handles item selection UI
        return null;
    }

    public static void CraftWeapon()
    {
        // WPF handles weapon crafting UI
    }
    
    public static void CraftArmor()
    {
        // WPF handles armor crafting UI
    }
    
    public static void CraftPotion()
    {
        // WPF handles potion crafting UI
    }
    
    public static void CraftGaldurite()
    {
        // WPF handles galdurite crafting UI
    }

    public static bool CanCraftWeapon(WeaponHead head, WeaponBinder binder, WeaponHandle handle, Quality quality)
    {
        var player = PlayerHandler.player;
        var costMultiplier = quality switch
        {
            Quality.Weak => 0.5,
            Quality.Normal => 1,
            Quality.Excellent => 2,
            Quality.Masterpiece => 2,
            _ => throw new ArgumentOutOfRangeException()
        };
        return player.Inventory.GetCount(head.Material) >= head.MaterialCost * costMultiplier &&
               player.Inventory.GetCount(binder.Material) >= binder.MaterialCost * costMultiplier &&
               player.Inventory.GetCount(handle.Material) >= handle.MaterialCost * costMultiplier;
    }

    public static Weapon CraftWeapon(WeaponHead head, WeaponBinder binder, WeaponHandle handle, Quality quality, string name)
    {
        var player = PlayerHandler.player;
        var costMultiplier = quality switch
        {
            Quality.Weak => 0.5,
            Quality.Normal => 1,
            Quality.Excellent => 2,
            Quality.Masterpiece => 2,
            _ => throw new ArgumentOutOfRangeException()
        };
        player.Inventory.TryRemoveItem(ItemManager.GetItem(head.Material),
            (int)(head.MaterialCost * costMultiplier));
        player.Inventory.TryRemoveItem(ItemManager.GetItem(binder.Material),
            (int)(binder.MaterialCost * costMultiplier));
        player.Inventory.TryRemoveItem(ItemManager.GetItem(handle.Material),
            (int)(handle.MaterialCost * costMultiplier));
        var weapon = new Weapon(head, binder, handle, player.CharacterClass, quality, name);
        player.Inventory.AddItem(weapon);
        return weapon;
    }

    public static bool CanCraftArmor(ArmorPlate plate, ArmorBinder binder, ArmorBase armorBase, Quality quality)
    {
        var player = PlayerHandler.player;
        var costMultiplier = quality switch
        {
            Quality.Weak => 0.5,
            Quality.Normal => 1,
            Quality.Excellent => 2,
            Quality.Masterpiece => 2,
            _ => throw new ArgumentOutOfRangeException()
        };
        return player.Inventory.GetCount(plate.Material) >= plate.MaterialCost * costMultiplier &&
               player.Inventory.GetCount(binder.Material) >= binder.MaterialCost * costMultiplier &&
               player.Inventory.GetCount(armorBase.Material) >= armorBase.MaterialCost * costMultiplier;
    }

    public static Armor CraftArmor(ArmorPlate plate, ArmorBinder binder, ArmorBase armorBase, Quality quality, string name)
    {
        var player = PlayerHandler.player;
        var costMultiplier = quality switch
        {
            Quality.Weak => 0.5,
            Quality.Normal => 1,
            Quality.Excellent => 2,
            Quality.Masterpiece => 2,
            _ => throw new ArgumentOutOfRangeException()
        };
        player.Inventory.TryRemoveItem(ItemManager.GetItem(plate.Material),
            (int)(plate.MaterialCost * costMultiplier));
        player.Inventory.TryRemoveItem(ItemManager.GetItem(binder.Material),
            (int)(binder.MaterialCost * costMultiplier));
        player.Inventory.TryRemoveItem(ItemManager.GetItem(armorBase.Material),
            (int)(armorBase.MaterialCost * costMultiplier));
        var armor = new Armor(plate, binder, armorBase, player.CharacterClass, quality, name);
        player.Inventory.AddItem(armor);
        return armor;
    }

    public static bool CanCraftPotion(List<PotionComponent> components, PotionCatalyst? catalyst)
    {
        var player = PlayerHandler.player;
        foreach (var component in components)
        {
            if (player.Inventory.GetCount(component.Material) < 1)
                return false;
        }
        if (catalyst != null && player.Inventory.GetCount(catalyst.Material) < 1)
            return false;
        return true;
    }

    public static Potion CraftPotion(List<PotionComponent> components, PotionCatalyst? catalyst, string name)
    {
        var player = PlayerHandler.player;
        foreach (var component in components)
        {
            player.Inventory.TryRemoveItem(ItemManager.GetItem(component.Material));
        }
        if (catalyst != null)
            player.Inventory.TryRemoveItem(ItemManager.GetItem(catalyst.Material));
        var potion = new Potion(name, components, catalyst);
        player.Inventory.AddItem(potion);
        return potion;
    }

    public static bool CanCraftGaldurite(int tier, string color, bool isArmorGaldurite)
    {
        var player = PlayerHandler.player;
        var powder = GalduriteManager.GetPowder(color);
        return player.Inventory.GetCount(powder) >= tier;
    }

    public static Galdurite CraftGaldurite(int tier, string color, bool isArmorGaldurite, string name)
    {
        var player = PlayerHandler.player;
        var powder = GalduriteManager.GetPowder(color);
        player.Inventory.TryRemoveItem(ItemManager.GetItem(powder), tier);
        var galdurite = new Galdurite(isArmorGaldurite, tier, tier, name);
        player.Inventory.AddItem(galdurite);
        return galdurite;
    }

    public static bool CanCraftItem(ICraftable item)
    {
        return item switch
        {
            Weapon weapon => CanCraftWeapon(weapon.Head, weapon.Binder, weapon.Handle, weapon.Quality),
            Armor armor => CanCraftArmor(armor.Plate, armor.Binder, armor.Base, armor.Quality),
            Potion potion => CanCraftPotion(potion.Components, potion.Catalyst),
            Galdurite galdurite => CanCraftGaldurite(galdurite.Tier, "Random", galdurite.ItemType == ItemType.ArmorGaldurite),
            _ => false
        };
    }

    public static void CraftItem(ICraftable item)
    {
        // WPF handles item crafting UI
    }
}