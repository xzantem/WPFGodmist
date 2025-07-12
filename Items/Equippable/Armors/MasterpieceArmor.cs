using GodmistWPF.Characters.Player;
using GodmistWPF.Combat.Modifiers.PassiveEffects;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Items;

namespace GodmistWPF.Items.Equippable.Armors;

/// <summary>
/// Reprezentuje arcydzieło zbroi, które oprócz standardowych właściwości
/// posiada dodatkowy efekt pasywny.
/// </summary>
/// <param name="plate">Płyta zbroi.</param>
/// <param name="binder">Oprawa zbroi.</param>
/// <param name="armorBase">Podstawa zbroi.</param>
/// <param name="requiredClass">Wymagana klasa postaci.</param>
/// <param name="quality">Jakość wykonania zbroi.</param>
/// <param name="passive">Efekt pasywny przypisany do tej zbroi.</param>
/// <param name="alias">Opcjonalny alias zbroi.</param>
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
    /// <summary>
    /// Dodatkowy efekt pasywny przypisany do tej zbroi arcydzieła.
    /// </summary>
    public PassiveEffect Passive { get; set; } = passive;

    /// <summary>
    /// Aktualizuje efekty pasywne wynikające z galdurytów oraz efekt pasywny arcydzieła.
    /// </summary>
    /// <param name="player">Postać gracza, której dotyczą efekty.</param>
    public override void UpdatePassives(PlayerCharacter player)
    {
        base.UpdatePassives(player);
        player.PassiveEffects.InnateEffects.RemoveAll(x => x.Source.EndsWith("MasterpiecePassive"));
        player.PassiveEffects.Add(Passive);
    }
}