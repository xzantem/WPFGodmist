using GodmistWPF.Characters.Player;
using GodmistWPF.Enums;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;

namespace GodmistWPF.Items.Equippable.Armors;

/// <summary>
/// Reprezentuje płytę zbroi, która jest jednym z głównych komponentów tworzących zbroję.
/// Zapewnia podstawowe wartości obrony, zdrowia i uników.
/// </summary>
[JsonConverter(typeof(EquipmentPartConverter))]
public class ArmorPlate : IEquipmentPart
{
    /// <summary>
    /// Procentowy bonus do maksymalnego zdrowia.
    /// </summary>
    public double HealthBonus { get; set; }
    
    /// <summary>
    /// Procentowy bonus do uników.
    /// </summary>
    public double DodgeBonus { get; set; }
    
    /// <summary>
    /// Bazowa wartość obrony fizycznej.
    /// </summary>
    public int PhysicalDefense { get; set; }
    
    /// <summary>
    /// Bazowa wartość obrony magicznej.
    /// </summary>
    public int MagicDefense { get; set; }
    
    /// <summary>
    /// Nazwa płyty zbroi generowana na podstawie aliasu.
    /// </summary>
    public string Name => NameAliasHelper.GetName(Alias[..^5]);
    
    /// <summary>
    /// Unikalny identyfikator płyty zbroi.
    /// </summary>
    public string Alias { get; set; }
    
    /// <summary>
    /// Klasa postaci, dla której przeznaczona jest ta płyta zbroi.
    /// </summary>
    public CharacterClass IntendedClass { get; set; }
    
    /// <summary>
    /// Przymiotnik opisujący płytę zbroi, używany przy generowaniu nazw.
    /// </summary>
    public string Adjective { get; set; }
    
    /// <summary>
    /// Poziom płyty zbroi, określający jej moc i wymagany poziom postaci.
    /// </summary>
    public int Tier { get; set; }
    
    /// <summary>
    /// Materiał, z którego wykonana jest płyta zbroi.
    /// </summary>
    public string Material { get; set; }
    
    /// <summary>
    /// Koszt materiałów potrzebnych do wytworzenia płyty zbroi.
    /// </summary>
    public int MaterialCost { get; set; }

    /// <summary>
    /// Generuje opis tekstowy płyty zbroi z uwzględnieniem kosztu wytworzenia.
    /// </summary>
    /// <param name="costMultiplier">Mnożnik kosztu materiałów.</param>
    /// <returns>Sformatowany opis płyty zbroi z informacjami o parametrach.</returns>
    public string DescriptionText(double costMultiplier)
    {
        return $"{Name}, {locale.Level} {Tier * 10 - 5} " +
               $"({MaterialCost * costMultiplier}x {NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.
                   Items.FirstOrDefault(x => x.Key.Alias == Material).Value}))\n{locale.HealthC}: " +
               $"*{1+HealthBonus:P0} | {locale.Dodge}: *{1 + DodgeBonus:P0} | {locale.Defense}: {PhysicalDefense}:{MagicDefense}\n";
    }
}