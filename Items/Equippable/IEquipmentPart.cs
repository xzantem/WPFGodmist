
using GodmistWPF.Enums;
using GodmistWPF.Utilities;

namespace GodmistWPF.Items.Equippable;

/// <summary>
/// Interfejs reprezentujący część ekwipunku (np. głowicę broni lub element zbroi).
/// Definiuje podstawowe właściwości wspólne dla wszystkich części ekwipunku.
/// </summary>
public interface IEquipmentPart
{
    /// <summary>
    /// Pobiera nazwę części ekwipunku na podstawie jej aliasu.
    /// </summary>
    public string Name => NameAliasHelper.GetName(Alias);
    
    /// <summary>
    /// Pobiera lub ustawia unikalny identyfikator części ekwipunku.
    /// </summary>
    public string Alias { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia klasę postaci, dla której przeznaczona jest ta część ekwipunku.
    /// </summary>
    public CharacterClass IntendedClass { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia poziom zaawansowania części ekwipunku.
    /// Wyższy poziom oznacza lepszą jakość i statystyki.
    /// </summary>
    public int Tier { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia materiał, z którego wykonana jest część ekwipunku.
    /// </summary>
    public string Material { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia koszt materiału potrzebnego do wytworzenia części ekwipunku.
    /// </summary>
    public int MaterialCost { get; set; }
}