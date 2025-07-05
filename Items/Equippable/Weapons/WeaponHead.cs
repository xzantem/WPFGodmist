using ConsoleGodmist;
using GodmistWPF.Characters.Player;
using GodmistWPF.Enums;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;


namespace GodmistWPF.Items.Equippable.Weapons;

[JsonConverter(typeof(EquipmentPartConverter))]
public class WeaponHead : IEquipmentPart
{
    public int MinimalAttack { get; set; }
    public int MaximalAttack { get; set; }
    public double CritMod { get; set; }
    public double CritChanceBonus { get; set; }
    public double AccuracyBonus { get; set; }
    public string Name => NameAliasHelper.GetName(Alias[..^4]);
    public string Alias { get; set; }
    public CharacterClass IntendedClass { get; set; }
    public string Adjective { get; set; }
    public int Tier { get; set; }
    public string Material { get; set; }
    public int MaterialCost { get; set; }


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