using GodmistWPF.Enums.Items;

namespace GodmistWPF.Items;

/// <summary>
/// Klasa bazowa reprezentująca podstawowy składnik używany w procesie tworzenia przedmiotów.
/// Rozszerza klasę BaseItem, dodając domyślne ustawienia dla składników.
/// </summary>
public class BaseIngredient : BaseItem
{

    /// <summary>
    /// Inicjalizuje nową instancję klasy BaseIngredient.
    /// Konstruktor używany do deserializacji JSON.
    /// </summary>
    public BaseIngredient() // For JSON deserialization
    {
        Stackable = true;
        Weight = 0;
    }  

    /// <summary>
    /// Inicjalizuje nową instancję klasy BaseIngredient z określonymi parametrami.
    /// </summary>
    /// <param name="alias">Alias (skrócona nazwa) składnika.</param>
    /// <param name="id">Unikalny identyfikator składnika.</param>
    /// <param name="cost">Koszt składnika.</param>
    /// <param name="rarity">Rzadkość składnika.</param>
    /// <param name="desc">Opis składnika.</param>
    /// <param name="itemType">Typ przedmiotu.</param>
    public BaseIngredient(string alias, int id, int cost, ItemRarity rarity, string desc, ItemType itemType)
    {
        Alias = alias;
        Weight = 0;
        ID = id;
        Cost = cost;
        Rarity = rarity;
        Stackable = true;
        Description = desc;
        ItemType = itemType;
    }
}