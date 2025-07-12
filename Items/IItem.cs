using GodmistWPF.Enums.Items;

namespace GodmistWPF.Items;

/// <summary>
/// Interfejs bazowy dla wszystkich przedmiotów w grze.
/// Definiuje podstawowe właściwości i zachowania wspólne dla wszystkich przedmiotów.
/// </summary>
public interface IItem
{
    /// <summary>
    /// Pobiera nazwę przedmiotu.
    /// </summary>
    /// <value>Nazwa przedmiotu.</value>
    public string Name { get; }
    
    /// <summary>
    /// Pobiera lub ustawia alias (skróconą nazwę) przedmiotu.
    /// </summary>
    public string Alias { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia wagę przedmiotu.
    /// </summary>
    public int Weight { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia unikalny identyfikator przedmiotu.
    /// </summary>
    public int ID { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia wartość przedmiotu w walucie gry.
    /// </summary>
    public int Cost { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia rzadkość przedmiotu.
    /// </summary>
    public ItemRarity Rarity { get; set; }
    
    /// <summary>
    /// Określa, czy przedmioty tego samego typu mogą być składane w jednym slocie ekwipunku.
    /// </summary>
    public bool Stackable { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia opis przedmiotu wyświetlany przy inspekcji.
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia typ przedmiotu.
    /// </summary>
    public ItemType ItemType { get; set; }
    
    /// <summary>
    /// Wyświetla szczegółowe informacje o przedmiocie.
    /// </summary>
    /// <param name="amount">Ilość przedmiotów do wyświetlenia (dla przedmiotów składalnych).</param>
    public void Inspect(int amount = 1);
    
    /// <summary>
    /// Wyświetla nazwę przedmiotu z uwzględnieniem jego rzadkości.
    /// </summary>
    public void WriteName();
    
    /// <summary>
    /// Wyświetla typ przedmiotu.
    /// </summary>
    public void WriteItemType();
    
    /// <summary>
    /// Wyświetla rzadkość przedmiotu.
    /// </summary>
    public void WriteRarity();
    
    /// <summary>
    /// Zwraca nazwę rzadkości przedmiotu jako łańcuch znaków.
    /// </summary>
    /// <returns>Nazwa rzadkości przedmiotu.</returns>
    public string RarityName();
    
    /// <summary>
    /// Zwraca nazwę typu przedmiotu jako łańcuch znaków.
    /// </summary>
    /// <returns>Nazwa typu przedmiotu.</returns>
    public string ItemTypeName();
}