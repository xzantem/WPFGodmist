using GodmistWPF.Characters.Player;
using GodmistWPF.Enums;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;

namespace GodmistWPF.Items.Equippable.Armors;

/// <summary>
/// Reprezentuje oprawę zbroi, która jest jednym z głównych komponentów tworzących zbroję.
/// Wpływa na zdrowie, uniki i obronę.
/// </summary>
[JsonConverter(typeof(EquipmentPartConverter))]
public class ArmorBinder : IEquipmentPart
{
    /// <summary>
    /// Dodatkowa wartość punktów zdrowia.
    /// </summary>
    public int Health { get; set; }
    
    /// <summary>
    /// Procentowy bonus do uników.
    /// </summary>
    public double DodgeBonus { get; set; }
    
    /// <summary>
    /// Procentowy bonus do obrony fizycznej.
    /// </summary>
    public double PhysicalDefenseBonus { get; set; }
    
    /// <summary>
    /// Procentowy bonus do obrony magicznej.
    /// </summary>
    public double MagicDefenseBonus { get; set; }
    
    /// <summary>
    /// Nazwa oprawy zbroi generowana na podstawie aliasu.
    /// </summary>
    public string Name => NameAliasHelper.GetName(Alias[..^6]);
    
    /// <summary>
    /// Unikalny identyfikator oprawy zbroi.
    /// </summary>
    public string Alias { get; set; }
    
    /// <summary>
    /// Klasa postaci, dla której przeznaczona jest ta oprawa zbroi.
    /// </summary>
    public CharacterClass IntendedClass { get; set; }
    
    /// <summary>
    /// Poziom oprawy zbroi, określający jej moc i wymagany poziom postaci.
    /// </summary>
    public int Tier { get; set; }
    
    /// <summary>
    /// Materiał, z którego wykonana jest oprawa zbroi.
    /// </summary>
    public string Material { get; set; }
    
    /// <summary>
    /// Koszt materiałów potrzebnych do wytworzenia oprawy zbroi.
    /// </summary>
    public int MaterialCost { get; set; }

    /// <summary>
    /// Generuje opis tekstowy oprawy zbroi z uwzględnieniem kosztu wytworzenia.
    /// </summary>
    /// <param name="costMultiplier">Mnożnik kosztu materiałów.</param>
    /// <returns>Sformatowany opis oprawy zbroi z informacjami o parametrach.</returns>
    public string DescriptionText(double costMultiplier)
    {
        return $"{Name}, {locale.Level} {Tier * 10 - 5} " +
               $"({MaterialCost * costMultiplier}x {NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.
                   Items.FirstOrDefault(x => x.Key.Alias == Material).Value}))\n{locale.HealthC}: " +
               $"{Health} | {locale.Dodge}: *{1 + DodgeBonus:P0} | {locale.Defense}: " +
               $"*{1+PhysicalDefenseBonus:P0}:{1+MagicDefenseBonus:P0}\n";
    }
}