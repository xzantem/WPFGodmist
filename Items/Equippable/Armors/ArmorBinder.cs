using ConsoleGodmist;
using GodmistWPF.Characters.Player;
using GodmistWPF.Enums;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;


namespace GodmistWPF.Items.Equippable.Armors;

[JsonConverter(typeof(EquipmentPartConverter))]
public class ArmorBinder : IEquipmentPart
{
    public int Health { get; set; }
    public double DodgeBonus { get; set; }
    public double PhysicalDefenseBonus { get; set; }
    public double MagicDefenseBonus { get; set; }
    public string Name => NameAliasHelper.GetName(Alias[..^6]);
    public string Alias { get; set; }
    public CharacterClass IntendedClass { get; set; }
    public int Tier { get; set; }
    public string Material { get; set; }
    public int MaterialCost { get; set; }

    public string DescriptionText(double costMultiplier)
    {
        return $"{Name}, {locale.Level} {Tier * 10 - 5} " +
               $"({MaterialCost * costMultiplier}x {NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.
                   Items.FirstOrDefault(x => x.Key.Alias == Material).Value}))\n{locale.HealthC}: " +
               $"{Health} | {locale.Dodge}: *{1 + DodgeBonus:P0} | {locale.Defense}: " +
               $"*{1+PhysicalDefenseBonus:P0}:{1+MagicDefenseBonus:P0}\n";
    }
}