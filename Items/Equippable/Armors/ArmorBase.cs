using GodmistWPF.Characters.Player;
using GodmistWPF.Enums;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;

namespace GodmistWPF.Items.Equippable.Armors;

/// <summary>
/// Reprezentuje bazową część zbroi, która jest jednym z głównych komponentów tworzących zbroję.
/// Zapewnia podstawowe bonusy do obrony i uników.
/// </summary>
[JsonConverter(typeof(EquipmentPartConverter))]
public class ArmorBase : IEquipmentPart
{
    /// <summary>
    /// Dodatkowy procentowy bonus do maksymalnego zdrowia.
    /// </summary>
    public double HealthBonus { get; set; }
    
    /// <summary>
    /// Wartość uników dodawana do podstawowej wartości postaci.
    /// </summary>
    public int Dodge { get; set; }
    
    /// <summary>
    /// Procentowy bonus do obrony fizycznej.
    /// </summary>
    public double PhysicalDefenseBonus { get; set; }
    
    /// <summary>
    /// Procentowy bonus do obrony magicznej.
    /// </summary>
    public double MagicDefenseBonus { get; set; }
    
    /// <summary>
    /// Nazwa części zbroi generowana na podstawie aliasu.
    /// </summary>
    public string Name => NameAliasHelper.GetName(Alias[..^4]);
    
    /// <summary>
    /// Unikalny identyfikator części zbroi.
    /// </summary>
    public string Alias { get; set; }
    
    /// <summary>
    /// Klasa postaci, dla której przeznaczona jest ta część zbroi.
    /// </summary>
    public CharacterClass IntendedClass { get; set; }
    
    /// <summary>
    /// Poziom części zbroi, określający jej moc i wymagany poziom postaci.
    /// </summary>
    public int Tier { get; set; }
    
    /// <summary>
    /// Materiał, z którego wykonana jest część zbroi.
    /// </summary>
    public string Material { get; set; }
    
    /// <summary>
    /// Koszt materiałów potrzebnych do wytworzenia części zbroi.
    /// </summary>
    public int MaterialCost { get; set; }

    /// <summary>
    /// Generuje opis tekstowy części zbroi z uwzględnieniem kosztu wytworzenia.
    /// </summary>
    /// <param name="costMultiplier">Mnożnik kosztu materiałów.</param>
    /// <returns>Sformatowany opis części zbroi z informacjami o parametrach.</returns>
    public string DescriptionText(double costMultiplier)
    {
        return $"{Name}, {locale.Level} {Tier * 10 - 5} " +
               $"({MaterialCost * costMultiplier}x {NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.
                   Items.FirstOrDefault(x => x.Key.Alias == Material).Value}))\n{locale.HealthC}: " +
               $"*{1+HealthBonus:P0} | {locale.Dodge}: {Dodge} | {locale.Defense}: " +
               $"*{1+PhysicalDefenseBonus:P0}:{1+MagicDefenseBonus:P0}\n";
    }
}