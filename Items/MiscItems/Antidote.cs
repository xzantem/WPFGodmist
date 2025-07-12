using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Items;
using Newtonsoft.Json;

namespace GodmistWPF.Items.MiscItems;

/// <summary>
/// Klasa reprezentująca przedmiot Antidotum, które leczy zatrucie.
/// Implementuje interfejsy ICraftable (możliwość wytworzenia) i IUsable (możliwość użycia).
/// </summary>
public class Antidote : BaseItem, ICraftable, IUsable
{
    /// <summary>
    /// Pobiera nazwę przedmiotu (zawsze "Antidote").
    /// </summary>
    public override string Alias => "Antidote";
    
    /// <summary>
    /// Pobiera wagę przedmiotu (zawsze 0, ponieważ jest to lekki przedmiot).
    /// </summary>
    public override int Weight => 0;
    
    /// <summary>
    /// Pobiera unikalny identyfikator przedmiotu (591).
    /// </summary>
    public override int ID => 591;
    
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
    /// Pobiera opis przedmiotu (pusty, ponieważ nie jest używany).
    /// </summary>
    public override string Description => "";
    
    /// <summary>
    /// Pobiera typ przedmiotu (zawsze Alchemy).
    /// </summary>
    public override ItemType ItemType => ItemType.Alchemy;

    /// <summary>
    /// Pobiera przepis rzemieślniczy potrzebny do wytworzenia przedmiotu.
    /// </summary>
    /// <remarks>
    /// Wymagane składniki: 1x Słaby Alkohol, 1x Aloes Meda.
    /// </remarks>
    [JsonIgnore]
    public Dictionary<string, int> CraftingRecipe {
        get => new() { { "WeakAlcohol", 1 }, { "AloeMeda", 1 } };
        set => throw new InvalidOperationException(); }

    /// <summary>
    /// Pobiera ilość wytwarzanych przedmiotów w jednej partii (zawsze 2).
    /// </summary>
    [JsonIgnore]
    public int CraftedAmount { get => 2; set => throw new InvalidOperationException(); }

    /// <summary>
    /// Używa antidotum, usuwając wszystkie efekty zatrucia z postaci gracza.
    /// </summary>
    /// <returns>Zawsze zwraca true, co oznacza, że przedmiot został zużyty.</returns>
    public bool Use()
    {
        PlayerHandler.player.PassiveEffects.TimedEffects.RemoveAll(x => x.Type == "Poison");
        return true;
    }
}