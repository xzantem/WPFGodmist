using GodmistWPF.Enums.Items;
using GodmistWPF.Items.Equippable;
using GodmistWPF.Items.Galdurites;
using GodmistWPF.Items.Potions;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;

namespace GodmistWPF.Items;

/// <summary>
/// Klasa reprezentująca ekwipunek NPC, w tym sklep i przedmioty kupione od gracza.
/// Zarządza asortymentem sklepowym, który zmienia się w zależności od poziomu lojalności.
/// </summary>
public class NPCInventory
{
    /// <summary>
    /// Słownik przedmiotów dostępnych w sklepie i ich ilości.
    /// Kluczem jest przedmiot, a wartością ilość sztuk w sklepie.
    /// </summary>
    [JsonConverter(typeof(ItemConverter))]
    public Dictionary<IItem, int> RotatingShop { get; set; }
    
    /// <summary>
    /// Słownik przedmiotów kupionych od gracza i ich ilości.
    /// Kluczem jest przedmiot, a wartością ilość sztuk.
    /// </summary>
    [JsonConverter(typeof(ItemConverter))]
    public Dictionary<IItem, int> BoughtFromPlayer { get; set; }
    
    /// <summary>
    /// Lista typów przedmiotów, które mogą pojawić się w sklepie.
    /// </summary>
    public List<ItemType> PossibleWares { get; set; }

    /// <summary>
    /// Inicjalizuje nową instancję klasy NPCInventory z określonymi typami przedmiotów.
    /// </summary>
    /// <param name="itemTypesInShop">Lista typów przedmiotów, które mogą pojawić się w sklepie.</param>
    public NPCInventory(List<ItemType> itemTypesInShop)
    {
        RotatingShop = new Dictionary<IItem, int>();
        BoughtFromPlayer = new Dictionary<IItem, int>();
        PossibleWares = itemTypesInShop;
        UpdateWares(1);
    }
    /// <summary>
    /// Inicjalizuje nową instancję klasy NPCInventory.
    /// Konstruktor używany do deserializacji JSON.
    /// </summary>
    public NPCInventory() {}

    /// <summary>
    /// Aktualizuje asortyment sklepu na podstawie poziomu lojalności gracza.
    /// Wyższy poziom lojalności odblokowuje lepsze przedmioty.
    /// </summary>
    /// <param name="loyaltyLevel">Poziom lojalności gracza z danym NPC.</param>
    public void UpdateWares(int loyaltyLevel)
    {
        RotatingShop.Clear();
        var tier = loyaltyLevel switch
        {
            <= 1 => 1,
            > 1 and <= 5 => 2,
            > 5 and <= 8 => 3,
            > 8 and <= 10 => 4,
            > 10 and <= 13 => 5
        };
        foreach (var type in PossibleWares)
        {
            var initial = RotatingShop.Count;
            switch (type)
            {
                case ItemType.Weapon:
                    while (RotatingShop.Count - initial < 5)
                        RotatingShop.TryAdd(EquippableItemService.GetRandomWeapon(tier), 1);
                    break;
                case ItemType.Armor:
                    while (RotatingShop.Count - initial < 5)
                        RotatingShop.TryAdd(EquippableItemService.GetRandomArmor(tier), 1);
                    break;
                case ItemType.Smithing:
                case ItemType.Alchemy:
                case ItemType.Runeforging:
                    var items = ItemManager.Items
                        .Where(x => x.ItemType == type &&
                                    (int)x.Rarity <= tier + 2 && !RotatingShop.ContainsKey(x)).ToList(); 
                    while (RotatingShop.Count - initial < 15 && RotatingShop.Count - initial < items.Count)
                        RotatingShop.TryAdd(UtilityMethods.RandomChoice(items), Random.Shared.Next(10, 21));
                    break;
                case ItemType.Potion:
                    while (RotatingShop.Count - initial < 5)
                        RotatingShop.TryAdd(PotionManager.GetRandomPotion(tier), 1);
                    break;
                case ItemType.WeaponGaldurite:
                    while (RotatingShop.Count - initial < 5)
                        RotatingShop.TryAdd(new Galdurite(false, tier switch
                        {
                            1 or 2 => 1,
                            3 or 4 => 2,
                            _ => 3
                        }, 0), 1);
                    break;
                case ItemType.ArmorGaldurite:
                    while (RotatingShop.Count - initial < 5)
                        RotatingShop.TryAdd(new Galdurite(true, tier switch
                        {
                            1 or 2 => 1,
                            3 or 4 => 2,
                            _ => 3
                        }, 0), 1);
                    break;
                case ItemType.LootBag:
                    throw new ArgumentOutOfRangeException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    /// <summary>
    /// Dodaje przedmiot do listy przedmiotów kupionych od gracza.
    /// Jeśli przedmiot jest składalny, zwiększa jego ilość.
    /// </summary>
    /// <param name="item">Przedmiot do dodania.</param>
    /// <param name="quantity">Ilość przedmiotów do dodania (domyślnie 1).</param>
    public void AddItem(IItem item, int quantity = 1)
    {
        if (item.Stackable && BoughtFromPlayer.ContainsKey(item))
        {
            BoughtFromPlayer[item] += quantity;
        }
        else
        {
            BoughtFromPlayer.Add(item, quantity);
        }
    }
    /// <summary>
    /// Usuwa określoną ilość przedmiotów z ekwipunku NPC na podanym indeksie.
    /// Jeśli ilość spadnie do zera, przedmiot jest całkowicie usuwany.
    /// </summary>
    /// <param name="index">Indeks przedmiotu do usunięcia.</param>
    /// <param name="amount">Ilość do usunięcia (domyślnie 1).</param>
    public void RemoveAt(int index, int amount = 1)
    {
        var items = RotatingShop.Concat(BoughtFromPlayer).ToList();
        if (index <= RotatingShop.Count - 1)
        {
            RotatingShop[RotatingShop.ElementAt(index).Key] -= amount;
            if (RotatingShop[RotatingShop.ElementAt(index).Key] <= 0)
                RotatingShop.Remove(RotatingShop.ElementAt(index).Key);
        }
        else
        {
            BoughtFromPlayer[BoughtFromPlayer.ElementAt(index - RotatingShop.Count + 1).Key] -= amount;
            if (BoughtFromPlayer[BoughtFromPlayer.ElementAt(index - RotatingShop.Count + 1).Key] <= 0)
                BoughtFromPlayer.Remove(BoughtFromPlayer.ElementAt(index - RotatingShop.Count + 1).Key);
        }
    }

    /// <summary>
    /// Zwraca przedmiot i jego ilość na podanym indeksie.
    /// Indeksacja obejmuje zarówno przedmioty w sklepie, jak i kupione od gracza.
    /// </summary>
    /// <param name="index">Indeks przedmiotu.</param>
    /// <returns>Para klucz-wartość zawierająca przedmiot i jego ilość.</returns>
    public KeyValuePair<IItem, int> ElementAt(int index)
    {
        var items = RotatingShop.Concat(BoughtFromPlayer).ToList();
        return index <= RotatingShop.Count + 1 ? RotatingShop.ElementAt(index) : 
            BoughtFromPlayer.ElementAt(index - RotatingShop.Count);
    }
}