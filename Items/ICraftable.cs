namespace GodmistWPF.Items;

/// <summary>
/// Interfejs określający zachowanie przedmiotów, które mogą być wytwarzane przez gracza.
/// Rozszerza interfejs IItem o właściwości związane z rzemiosłem.
/// </summary>
public interface ICraftable : IItem
{
    /// <summary>
    /// Pobiera lub ustawia przepis potrzebny do wytworzenia przedmiotu.
    /// Kluczem jest nazwa składnika, a wartością wymagana ilość.
    /// </summary>
    public Dictionary<string, int> CraftingRecipe { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia ilość przedmiotów wytwarzanych w jednej partii.
    /// </summary>
    public int CraftedAmount { get; set; }
}