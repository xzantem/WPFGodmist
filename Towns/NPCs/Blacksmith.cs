using GodmistWPF.Enums.Items;
using GodmistWPF.Items;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;

namespace GodmistWPF.Towns.NPCs;

/// <summary>
/// Reprezentuje kowala - NPC odpowiedzialnego za usługi związane z naprawą i ulepszaniem ekwipunku.
/// </summary>
[JsonConverter(typeof(NPCConverter))]
public class Blacksmith : NPC
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="Blacksmith"/> z określonym aliasem.
    /// </summary>
    /// <param name="alias">Alias kowala.</param>
    public Blacksmith(string alias)
    {
        Alias = alias;
        Inventory = new NPCInventory(new List<ItemType> { ItemType.Smithing, ItemType.Weapon, ItemType.Armor });
        CraftableItems = ItemManager.CraftableIngredients.Where(x => x.ItemType == ItemType.Smithing)
            .Cast<ICraftable>().ToList();
        LoyaltyLevel = 1;
        GoldSpent = 0;
    }
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="Blacksmith"/> bez parametrów.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizmy deserializacji.
    /// </remarks>
    [JsonConstructor]
    public Blacksmith()
    {
        Alias = "Blacksmith";
    }
}