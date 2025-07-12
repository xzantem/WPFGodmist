using GodmistWPF.Combat.Battles;
using GodmistWPF.Dungeons;
using GodmistWPF.Enums.Items;
using Newtonsoft.Json;

namespace GodmistWPF.Items.MiscItems;

/// <summary>
/// Klasa reprezentująca Zwój Teleportu do Miasta, który pozwala na szybki powrót do najbliższego miasta.
/// Implementuje interfejsy ICraftable (możliwość wytworzenia) i IUsable (możliwość użycia).
/// </summary>
public class TownPortalScroll : BaseItem, ICraftable, IUsable
{
    /// <summary>
    /// Pobiera nazwę przedmiotu (zawsze "TownPortalScroll").
    /// </summary>
    public override string Alias => "TownPortalScroll";
    
    /// <summary>
    /// Pobiera wagę przedmiotu (zawsze 0, ponieważ jest to lekki przedmiot).
    /// </summary>
    public override int Weight => 0;
    
    /// <summary>
    /// Pobiera unikalny identyfikator przedmiotu (589).
    /// </summary>
    public override int ID => 589;
    
    /// <summary>
    /// Oblicza koszt przedmiotu na podstawie składników potrzebnych do wytworzenia.
    /// Koszt jest dzielony przez ilość wytworzonych sztuk.
    /// </summary>
    public override int Cost => (int)Math.Floor((double)CraftingRecipe
        .Sum(x => x.Value * ItemManager.GetItem(x.Key).Cost) / CraftedAmount);
    
    /// <summary>
    /// Pobiera rzadkość przedmiotu (zawsze Common).
    /// </summary>
    public override ItemRarity Rarity => ItemRarity.Common;
    
    /// <summary>
    /// Określa, czy przedmioty mogą być składowane w stosie (zawsze true).
    /// </summary>
    public override bool Stackable => true;
    
    /// <summary>
    /// Pobiera opis przedmiotu.
    /// </summary>
    public override string Description => "Scroll created using the ancient galdurite carving technique. " +
                                      "The inscription means \"Home\".\nTeleports you to the nearest town.";
    
    /// <summary>
    /// Pobiera typ przedmiotu (zawsze Runeforging).
    /// </summary>
    public override ItemType ItemType => ItemType.Runeforging;
    
    /// <summary>
    /// Używa zwoju, teleportując gracza do najbliższego miasta.
    /// </summary>
    /// <returns>
    /// True, jeśli teleportacja się powiodła; false, jeśli gracz jest w trakcie walki.
    /// </returns>
    public bool Use()
    {
        if (BattleManager.IsInBattle())
            return false;
        DungeonMovementManager.ExitDungeon();
        return true;
    }
    
    /// <summary>
    /// Pobiera przepis rzemieślniczy potrzebny do wytworzenia przedmiotu.
    /// </summary>
    /// <remarks>
    /// Wymagane składniki: 1x Cynkowa Płyta, 1x Czerwony Pył Energii.
    /// </remarks>
    [JsonIgnore]
    public Dictionary<string, int> CraftingRecipe {
        get => new() { { "TinPlate", 1 }, { "RedEnergyDust", 1 } };
        set => throw new InvalidOperationException(); }
    /// <summary>
    /// Pobiera ilość wytwarzanych przedmiotów w jednej partii (zawsze 1).
    /// </summary>
    [JsonIgnore]
    public int CraftedAmount { get => 1; set => throw new InvalidOperationException(); }
}