
using GodmistWPF.Characters.Player;
using GodmistWPF.Combat.Modifiers.PassiveEffects;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Items;

namespace GodmistWPF.Items.Equippable.Armors;

public class MasterpieceArmor(
    ArmorPlate plate,
    ArmorBinder binder,
    ArmorBase armorBase,
    CharacterClass requiredClass,
    Quality quality,
    PassiveEffect passive,
    string alias = "")
    : Armor(plate, binder, armorBase, requiredClass,
        quality, alias)
{
    public PassiveEffect Passive { get; set; } = passive;

    public override void UpdatePassives(PlayerCharacter player)
    {
        base.UpdatePassives(player);
        player.PassiveEffects.InnateEffects.RemoveAll(x => x.Source.EndsWith("MasterpiecePassive"));
        player.PassiveEffects.Add(Passive);
    }
}