

using GodmistWPF.Enums;
using GodmistWPF.Utilities;

namespace GodmistWPF.Items.Equippable;

public interface IEquipmentPart
{
    public string Name => NameAliasHelper.GetName(Alias);
    public string Alias { get; set; }
    public CharacterClass IntendedClass { get; set; }
    public int Tier { get; set; }
    public string Material { get; set; }
    public int MaterialCost { get; set; }
}