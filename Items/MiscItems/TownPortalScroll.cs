using GodmistWPF.Combat.Battles;
using GodmistWPF.Dungeons;
using GodmistWPF.Enums.Items;
using Newtonsoft.Json;

namespace GodmistWPF.Items.MiscItems;

public class TownPortalScroll : BaseItem, ICraftable, IUsable
{
    public override string Alias => "TownPortalScroll";
    public override int Weight => 0;
    public override int ID => 589;
    public override int Cost => (int)Math.Floor((double)CraftingRecipe
        .Sum(x => x.Value * ItemManager.GetItem(x.Key).Cost) / CraftedAmount);
    public override ItemRarity Rarity => ItemRarity.Common;
    public override bool Stackable => true;
    public override string Description => "Scroll created using the ancient galdurite carving technique. " +
                                          "The inscription means \"Home\".\nTeleports you to the nearest town.";
    public override ItemType ItemType => ItemType.Runeforging;
    
    public bool Use()
    {
        if (BattleManager.IsInBattle())
            return false;
        DungeonMovementManager.ExitDungeon();
        return true;
    }
    
    [JsonIgnore]
    public Dictionary<string, int> CraftingRecipe {
        get => new() { { "TinPlate", 1 }, { "RedEnergyDust", 1 } };
        set => throw new InvalidOperationException(); }
    [JsonIgnore]
    public int CraftedAmount { get => 1; set => throw new InvalidOperationException(); }
}