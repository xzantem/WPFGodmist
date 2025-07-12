using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Items;
using GodmistWPF.Items.Equippable;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;


namespace GodmistWPF.Items;

/// <summary>
/// Klasa reprezentująca ekwipunek gracza.
/// Zawiera metody do zarządzania przedmiotami, takie jak dodawanie, usuwanie i sortowanie.
/// </summary>
public class Inventory
{
    /// <summary>
    /// Słownik przechowujący przedmioty w ekwipunku i ich ilości.
    /// Kluczem jest przedmiot, a wartością ilość sztuk.
    /// </summary>
    [JsonConverter(typeof(ItemConverter))]
    public Dictionary<IItem, int> Items { get; set; } = new();
    
    /// <summary>
    /// Oblicza całkowitą wagę przedmiotów w ekwipunku.
    /// </summary>
    public int PackWeight => Items.Sum(item => item.Key.Weight * item.Value);
    
    /// <summary>
    /// Pobiera lub ustawia maksymalną wagę, jaką może unieść postać.
    /// Domyślna wartość to 60 jednostek.
    /// </summary>
    public int MaxPackWeight { get; set; } = 60;

    
    /// <summary>
    /// Dodaje określoną ilość przedmiotu do ekwipunku.
    /// Jeśli przedmiot jest składalny, zwiększa jego ilość.
    /// </summary>
    /// <param name="item">Przedmiot do dodania.</param>
    /// <param name="quantity">Ilość przedmiotów do dodania (domyślnie 1).</param>
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
    
    /// <summary>
    /// Próbuje usunąć określoną ilość przedmiotu z ekwipunku.
    /// </summary>
    /// <param name="item">Przedmiot do usunięcia.</param>
    /// <param name="amount">Ilość przedmiotów do usunięcia (domyślnie 1).</param>
    /// <returns>True, jeśli udało się usunąć przedmiot, w przeciwnym razie false.</returns>
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
    
    /// <summary>
    /// Usuwa z ekwipunku przedmioty, które są niepotrzebne dla klasy postaci gracza.
    /// Usuwa broń i zbroje, które nie są przeznaczone dla aktualnej klasy postaci.
    /// </summary>
    public void RemoveJunk()
    {
        foreach (var item in Items)
        {
            if (item.Key.ItemType is not (ItemType.Weapon or ItemType.Armor)) continue;
            if ((item.Key as IEquippable).RequiredClass != PlayerHandler.player.CharacterClass)
                TryRemoveItem(item.Key, item.Value);
        }
    }
    
    /// <summary>
    /// Sortuje przedmioty w ekwipunku według określonego kryterium.
    /// </summary>
    /// <param name="sortType">Kryterium sortowania.</param>
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

    
    /// <summary>
    /// Próbuje użyć przedmiotu z ekwipunku.
    /// Jeśli przedmiot implementuje interfejs IUsable, wywołuje jego metodę Use().
    /// W przypadku powodzenia usuwa jedną sztukę przedmiotu.
    /// </summary>
    /// <param name="item">Przedmiot do użycia.</param>
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

    
    /// <summary>
    /// Zwraca ilość przedmiotów o podanym aliasie w ekwipunku.
    /// </summary>
    /// <param name="itemAlias">Alias szukanego przedmiotu.</param>
    /// <returns>Ilość przedmiotów o podanym aliasie lub 0, jeśli przedmiot nie występuje w ekwipunku.</returns>
    public int GetCount(string itemAlias)
    {
        return Items.Any(x => x.Key.Alias == itemAlias) ? 
            Items.FirstOrDefault(x => x.Key.Alias == itemAlias).Value : 0;
    }
}