using GodmistWPF.Enums.Items;

namespace GodmistWPF.Items;

/// <summary>
/// Reprezentuje składnik, który może być używany w procesie tworzenia przedmiotów.
/// Implementuje interfejs ICraftable, co oznacza, że sam może być wytwarzany.
/// </summary>
public class CraftableIngredient : BaseItem, ICraftable
{
    /// <summary>
    /// Pobiera lub ustawia przepis potrzebny do wytworzenia tego składnika.
    /// Kluczem jest nazwa składnika, a wartością wymagana ilość.
    /// </summary>
    public Dictionary<string, int> CraftingRecipe { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia ilość wytwarzanych składników w jednej partii.
    /// </summary>
    public int CraftedAmount { get; set; }
    
    /// <summary>
    /// Pobiera koszt wytworzenia pojedynczego składnika.
    /// Obliczany na podstawie kosztów składników potrzebnych do wytworzenia.
    /// </summary>
    public override int Cost => (int)Math.Floor((double)CraftingRecipe
        .Sum(x => x.Value * ItemManager.GetItem(x.Key).Cost) / CraftedAmount);

    /// <summary>
    /// Inicjalizuje nową instancję klasy CraftableIngredient.
    /// Konstruktor używany do deserializacji JSON.
    /// </summary>
    public CraftableIngredient()
    {
        Stackable = true;
        Weight = 0;
    }

    /// <summary>
    /// Inicjalizuje nową instancję klasy CraftableIngredient z określonymi parametrami.
    /// </summary>
    /// <param name="alias">Alias (skrócona nazwa) składnika.</param>
    /// <param name="id">Unikalny identyfikator składnika.</param>
    /// <param name="cost">Podstawowy koszt składnika (może zostać nadpisany przez właściwość Cost).</param>
    /// <param name="rarity">Rzadkość składnika.</param>
    /// <param name="desc">Opis składnika.</param>
    /// <param name="itemType">Typ przedmiotu.</param>
    /// <param name="craftingRecipe">Przepis potrzebny do wytworzenia składnika.</param>
    /// <param name="craftedAmount">Ilość wytwarzanych składników w jednej partii.</param>
    public CraftableIngredient(string alias, int id, int cost, ItemRarity rarity,
        string desc, ItemType itemType, Dictionary<string, int> craftingRecipe, int craftedAmount)
    {
        Alias = alias;
        ID = id;
        Cost = cost;
        Rarity = rarity;
        Description = desc;
        ItemType = itemType;
        CraftingRecipe = craftingRecipe;
        Stackable = true;
        Weight = 0;
        CraftedAmount = craftedAmount;
    }
}