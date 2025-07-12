using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Items;
using GodmistWPF.Items.Equippable.Armors;
using GodmistWPF.Items.Equippable.Weapons;
using GodmistWPF.Items.Galdurites;
using GodmistWPF.Items.Potions;
namespace GodmistWPF.Items;

/// <summary>
/// Klasa zarządzająca procesem tworzenia przedmiotów w grze.
/// Zawiera metody sprawdzające możliwość wytworzenia przedmiotów oraz realizujące ich tworzenie.
/// </summary>
public static class CraftingManager
{
    /// <summary>
    /// Referencja do ekwipunku gracza.
    /// </summary>
    private static readonly Inventory Inventory = PlayerHandler.player.Inventory;
    
    /// <summary>
    /// Lista dostępnych rzadkości przedmiotów używana do filtrowania.
    /// </summary>
    private static readonly List<ItemRarity> RarityFilters = [ItemRarity.Common, 
        ItemRarity.Uncommon, ItemRarity.Rare, ItemRarity.Ancient, ItemRarity.Legendary, ItemRarity.Mythical, ItemRarity.Godly];
        
    /// <summary>
    /// Flaga określająca, czy wyświetlać tylko przedmioty, które można wykonać z dostępnych materiałów.
    /// </summary>
    private static bool _showOnlyCraftable;

    /// <summary>
    /// Otwiera menu tworzenia przedmiotów.
    /// </summary>
    /// <param name="possibleItems">Lista przedmiotów, które można wykonać.</param>
    public static void OpenCraftingMenu(List<ICraftable> possibleItems)
    {
        // WPF handles crafting menu UI
    }

    /// <summary>
    /// Sprawdza, czy możliwe jest wykonanie broni z podanych komponentów.
    /// </summary>
    /// <param name="head">Głowica broni.</param>
    /// <param name="binder">Wiązadło broni.</param>
    /// <param name="handle">Rękojeść broni.</param>
    /// <param name="quality">Jakość tworzonej broni.</param>
    /// <returns>True, jeśli możliwe jest wykonanie broni, w przeciwnym razie false.</returns>
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

    /// <summary>
    /// Wykonuje broń z podanych komponentów.
    /// </summary>
    /// <param name="head">Głowica broni.</param>
    /// <param name="binder">Wiązadło broni.</param>
    /// <param name="handle">Rękojeść broni.</param>
    /// <param name="quality">Jakość tworzonej broni.</param>
    /// <param name="name">Nazwa tworzonej broni.</param>
    /// <returns>Nowo utworzona broń.</returns>
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

    /// <summary>
    /// Sprawdza, czy możliwe jest wykonanie zbroi z podanych komponentów.
    /// </summary>
    /// <param name="plate">Płyta pancerza.</param>
    /// <param name="binder">Wiązadło pancerza.</param>
    /// <param name="armorBase">Podstawa pancerza.</param>
    /// <param name="quality">Jakość tworzonej zbroi.</param>
    /// <returns>True, jeśli możliwe jest wykonanie zbroi, w przeciwnym razie false.</returns>
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

    /// <summary>
    /// Wykonuje zbroję z podanych komponentów.
    /// </summary>
    /// <param name="plate">Płyta pancerza.</param>
    /// <param name="binder">Wiązadło pancerza.</param>
    /// <param name="armorBase">Podstawa pancerza.</param>
    /// <param name="quality">Jakość tworzonej zbroi.</param>
    /// <param name="name">Nazwa tworzonej zbroi.</param>
    /// <returns>Nowo utworzona zbroja.</returns>
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

    /// <summary>
    /// Sprawdza, czy możliwe jest wykonanie mikstury z podanych składników.
    /// </summary>
    /// <param name="components">Lista składników mikstury.</param>
    /// <param name="catalyst">Katalizator mikstury (opcjonalny).</param>
    /// <returns>True, jeśli możliwe jest wykonanie mikstury, w przeciwnym razie false.</returns>
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

    /// <summary>
    /// Wykonuje miksturę z podanych składników.
    /// </summary>
    /// <param name="components">Lista składników mikstury.</param>
    /// <param name="catalyst">Katalizator mikstury (opcjonalny).</param>
    /// <param name="name">Nazwa tworzonej mikstury.</param>
    /// <returns>Nowo utworzona mikstura.</returns>
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

    /// <summary>
    /// Sprawdza, czy możliwe jest wykonanie galduritu o podanych parametrach.
    /// </summary>
    /// <param name="tier">Poziom galduritu.</param>
    /// <param name="color">Kolor galduritu.</param>
    /// <param name="isArmorGaldurite">Określa, czy to galdurit do zbroi (true) czy do broni (false).</param>
    /// <returns>True, jeśli możliwe jest wykonanie galduritu, w przeciwnym razie false.</returns>
    public static bool CanCraftGaldurite(int tier, string color, bool isArmorGaldurite)
    {
        var player = PlayerHandler.player;
        var powder = GalduriteManager.GetPowder(color);
        return player.Inventory.GetCount(powder) >= tier;
    }

    /// <summary>
    /// Wykonuje galdurit o podanych parametrach.
    /// </summary>
    /// <param name="tier">Poziom galduritu.</param>
    /// <param name="color">Kolor galduritu.</param>
    /// <param name="isArmorGaldurite">Określa, czy to galdurit do zbroi (true) czy do broni (false).</param>
    /// <param name="name">Nazwa tworzonego galduritu.</param>
    /// <returns>Nowo utworzony galdurit.</returns>
    public static Galdurite CraftGaldurite(int tier, string color, bool isArmorGaldurite, string name)
    {
        var player = PlayerHandler.player;
        var powder = GalduriteManager.GetPowder(color);
        player.Inventory.TryRemoveItem(ItemManager.GetItem(powder), tier);
        var galdurite = new Galdurite(isArmorGaldurite, tier, tier, name);
        player.Inventory.AddItem(galdurite);
        return galdurite;
    }

    /// <summary>
    /// Sprawdza, czy możliwe jest wykonanie danego przedmiotu.
    /// </summary>
    /// <param name="item">Przedmiot do sprawdzenia.</param>
    /// <returns>True, jeśli możliwe jest wykonanie przedmiotu, w przeciwnym razie false.</returns>
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

    /// <summary>
    /// Wykonuje dany przedmiot.
    /// </summary>
    /// <param name="item">Przedmiot do wykonania.</param>
    public static void CraftItem(ICraftable item)
    {
        // WPF handles item crafting UI
    }
}