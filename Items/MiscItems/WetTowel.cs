using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Items;
using Newtonsoft.Json;


namespace GodmistWPF.Items.MiscItems;

public class WetTowel : BaseItem, ICraftable, IUsable
{
    public override string Alias => "WetTowel";
    public override int Weight => 0;
    public override int ID => 592;
 
    public override int Cost => (int)Math.Floor((double)CraftingRecipe
        .Sum(x => x.Value * ItemManager.GetItem(x.Key).Cost) / CraftedAmount);

    public override ItemRarity Rarity => ItemRarity.Common;
    public override bool Stackable => true;
    public override string Description => "";
    public override ItemType ItemType => ItemType.Alchemy;

    [JsonIgnore]
    public Dictionary<string, int> CraftingRecipe {
        get => new() { { "CottonFabric", 2 } };
        set => throw new InvalidOperationException(); }

    [JsonIgnore]
    public int CraftedAmount
    { get => 1; set => throw new InvalidOperationException(); }

    public bool Use()
    {
        PlayerHandler.player.PassiveEffects.TimedEffects.RemoveAll(x => x.Type == "Bleed");
        return true;
    }
}