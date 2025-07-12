using GodmistWPF.Characters.Player;
using GodmistWPF.Enums;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;

namespace GodmistWPF.Items.Equippable.Weapons;

/// <summary>
/// Reprezentuje oprawę broni, która jest jednym z trzech głównych komponentów tworzących broń.
/// Wpływa na dodatkowe modyfikatory obrażeń i szansy na trafienie krytyczne.
/// </summary>
[JsonConverter(typeof(EquipmentPartConverter))]
public class WeaponBinder : IEquipmentPart
{
    /// <summary>
    /// Dodatkowy modyfikator obrażeń (wartość procentowa).
    /// </summary>
    public double AttackBonus { get; set; }
    
    /// <summary>
    /// Dodatkowa szansa na trafienie krytyczne (wartość procentowa).
    /// </summary>
    public double CritChance { get; set; }
    
    /// <summary>
    /// Dodatkowy modyfikator obrażeń krytycznych (wartość procentowa).
    /// </summary>
    public double CritModBonus { get; set; }
    
    /// <summary>
    /// Bonus do celności (wartość procentowa).
    /// </summary>
    public double AccuracyBonus { get; set; }
    
    /// <summary>
    /// Nazwa oprawy broni generowana na podstawie aliasu.
    /// </summary>
    public string Name => NameAliasHelper.GetName(Alias[..^6]);
    
    /// <summary>
    /// Unikalny identyfikator oprawy broni.
    /// </summary>
    public string Alias { get; set; }
    
    /// <summary>
    /// Klasa postaci, dla której przeznaczona jest ta oprawa.
    /// </summary>
    public CharacterClass IntendedClass { get; set; }
    
    /// <summary>
    /// Poziom oprawy, określający jej moc i wymagany poziom postaci.
    /// </summary>
    public int Tier { get; set; }
    
    /// <summary>
    /// Materiał, z którego wykonana jest oprawa.
    /// </summary>
    public string Material { get; set; }
    
    /// <summary>
    /// Koszt materiałów potrzebnych do wytworzenia oprawy.
    /// </summary>
    public int MaterialCost { get; set; }

    /// <summary>
    /// Generuje opis tekstowy oprawy z uwzględnieniem kosztu wytworzenia.
    /// </summary>
    /// <param name="costMultiplier">Mnożnik kosztu materiałów.</param>
    /// <returns>Sformatowany opis oprawy z informacjami o parametrach.</returns>
    /// <remarks>
    /// Dla klasy Sorcerer wyświetlane są inne etykiety (np. mana zamiast szansy na trafienie krytyczne).
    /// </remarks>
    public string DescriptionText(double costMultiplier)
    {
        return IntendedClass == CharacterClass.Sorcerer
            ? $"{Name}, {locale.Level} {Tier * 10 - 5} " +
              $"({MaterialCost * costMultiplier}x {NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.
                  Items.FirstOrDefault(x => x.Key.Alias == Material).Value}))\n{locale.Attack}: " +
              $"*{1 + AttackBonus:P0} | {locale.ManaShort}: *{1 + AccuracyBonus:P0} " +
              $"({CritChance:N0}/t) | {locale.Crit}: *{CritModBonus:P0}\n"
            : $"{Name}, {locale.Level} {Tier * 10 - 5} " +
              $"({MaterialCost * costMultiplier}x {NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.
                  Items.FirstOrDefault(x => x.Key.Alias == Material).Value}))\n{locale.Attack}: " +
              $"*{1 + AttackBonus:P0} | {locale.CritChance}: {CritChance:P0} | {locale.Crit}: " +
              $"*{1 + CritModBonus:P0} | {locale.Accuracy}: *{1 + AccuracyBonus:P0}\n";
    }
}