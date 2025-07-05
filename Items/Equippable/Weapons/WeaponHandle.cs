using ConsoleGodmist;
using GodmistWPF.Characters.Player;
using GodmistWPF.Enums;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;


namespace GodmistWPF.Items.Equippable.Weapons;

[JsonConverter(typeof(EquipmentPartConverter))]
public class WeaponHandle : IEquipmentPart
{
    public double AttackBonus { get; set; }
    public double CritChanceBonus { get; set; }
    public double CritModBonus { get; set; }
    public int Accuracy { get; set; }
    public string Name => NameAliasHelper.GetName(Alias[..^6]);
    public string Alias { get; set; }
    public CharacterClass IntendedClass { get; set; }
    public int Tier { get; set; }
    public string Material { get; set; }
    public int MaterialCost { get; set; }

    public string DescriptionText(double costMultiplier)
    {
        return IntendedClass == CharacterClass.Sorcerer
            ? $"{Name}, {locale.Level} {Tier * 10 - 5} " +
              $"({MaterialCost * costMultiplier}x {NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.
                  Items.FirstOrDefault(x => x.Key.Alias == Material).Value}))\n{locale.Attack}: " +
              $"*{1+AttackBonus:P0} | {locale.ManaShort}: {Accuracy} (*{1+CritChanceBonus:P0}/t) | {locale.Crit}: *{1+CritModBonus:P0}\n"
            : $"{Name}, {locale.Level} {Tier * 10 - 5} " +
              $"({MaterialCost * costMultiplier}x {NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.
                  Items.FirstOrDefault(x => x.Key.Alias == Material).Value}))\n{locale.Attack}: " +
              $"*{1+AttackBonus:P0} | {locale.CritChance}: *{1+CritChanceBonus:P0} | " +
              $"{locale.Crit}: *{1+CritModBonus:P0} | {locale.Accuracy}: {Accuracy}\n";
    }
}