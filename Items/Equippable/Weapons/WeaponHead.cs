using GodmistWPF.Characters.Player;
using GodmistWPF.Enums;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;

namespace GodmistWPF.Items.Equippable.Weapons;

/// <summary>
/// Reprezentuje głowicę broni, która jest jednym z trzech głównych komponentów tworzących broń.
/// Określa podstawowe parametry obrażeń i szansy na trafienie krytyczne.
/// </summary>
[JsonConverter(typeof(EquipmentPartConverter))]
public class WeaponHead : IEquipmentPart
{
    /// <summary>
    /// Minimalna wartość obrażeń zadawanych przez broń.
    /// </summary>
    public int MinimalAttack { get; set; }
    
    /// <summary>
    /// Maksymalna wartość obrażeń zadawanych przez broń.
    /// </summary>
    public int MaximalAttack { get; set; }
    
    /// <summary>
    /// Modyfikator obrażeń krytycznych (mnożnik).
    /// </summary>
    public double CritMod { get; set; }
    
    /// <summary>
    /// Dodatkowa szansa na trafienie krytyczne (wartość procentowa).
    /// </summary>
    public double CritChanceBonus { get; set; }
    
    /// <summary>
    /// Bonus do celności (wartość procentowa).
    /// </summary>
    public double AccuracyBonus { get; set; }
    
    /// <summary>
    /// Nazwa głowicy broni generowana na podstawie aliasu.
    /// </summary>
    public string Name => NameAliasHelper.GetName(Alias[..^4]);
    
    /// <summary>
    /// Unikalny identyfikator głowicy broni.
    /// </summary>
    public string Alias { get; set; }
    
    /// <summary>
    /// Klasa postaci, dla której przeznaczona jest ta głowica.
    /// </summary>
    public CharacterClass IntendedClass { get; set; }
    
    /// <summary>
    /// Przymiotnik opisujący głowicę (np. "ostra", "ciężka").
    /// </summary>
    public string Adjective { get; set; }
    
    /// <summary>
    /// Poziom głowicy, określający jej moc i wymagany poziom postaci.
    /// </summary>
    public int Tier { get; set; }
    
    /// <summary>
    /// Materiał, z którego wykonana jest głowica.
    /// </summary>
    public string Material { get; set; }
    
    /// <summary>
    /// Koszt materiałów potrzebnych do wytworzenia głowicy.
    /// </summary>
    public int MaterialCost { get; set; }


    /// <summary>
    /// Generuje opis tekstowy głowicy z uwzględnieniem kosztu wytworzenia.
    /// </summary>
    /// <param name="costMultiplier">Mnożnik kosztu materiałów.</param>
    /// <returns>Sformatowany opis głowicy z informacjami o parametrach.</returns>
    /// <remarks>
    /// Dla klasy Sorcerer wyświetlane są inne etykiety (np. mana zamiast szansy na trafienie krytyczne).
    /// </remarks>
    public string DescriptionText(double costMultiplier)
    {
        return IntendedClass == CharacterClass.Sorcerer
            ? $"{Name}, {locale.Level} {Tier * 10 - 5} " +
              $"({MaterialCost * costMultiplier}x {NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.
                  Items.FirstOrDefault(x => x.Key.Alias == Material).Value}))\n{locale.Attack}: " +
              $"{MinimalAttack}-{MaximalAttack} | {locale.ManaShort}: *{1 + AccuracyBonus:P0} " +
              $"(*{1 + CritChanceBonus:P0}/t) | {locale.Crit}: {CritMod:F2}\n"
            : $"{Name}, {locale.Level} {Tier * 10 - 5} " +
              $"({MaterialCost * costMultiplier}x {NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.
                  Items.FirstOrDefault(x => x.Key.Alias == Material).Value}))\n{locale.Attack}: " +
              $"{MinimalAttack}-{MaximalAttack} | " +
              $"{locale.CritChance}: *{1 + CritChanceBonus:P0} | " +
              $"{locale.Crit}: {CritMod}x | {locale.Accuracy}: *{1 + AccuracyBonus:P0}\n";
    }
}