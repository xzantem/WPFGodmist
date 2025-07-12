using GodmistWPF.Enums.Items;
using GodmistWPF.Items;
using GodmistWPF.Items.Potions;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;


namespace GodmistWPF.Towns.NPCs;

/// <summary>
/// Reprezentuje alchemika - NPC odpowiedzialnego za usługi związane z alchemią i miksturami.
/// </summary>
[JsonConverter(typeof(NPCConverter))]
public class Alchemist : NPC
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="Alchemist"/> z określonym aliasem.
    /// </summary>
    /// <param name="alias">Alias alchemika.</param>
    public Alchemist(string alias)
    {
        Alias = alias;
        Inventory = new NPCInventory([ItemType.Alchemy, ItemType.Potion]);
        CraftableItems = ItemManager.CraftableIngredients.Where(x => x.ItemType == ItemType.Alchemy)
            .Cast<ICraftable>().ToList();
        LoyaltyLevel = 1;
        GoldSpent = 0;
    }
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="Alchemist"/> bez parametrów.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizmy deserializacji.
    /// </remarks>
    [JsonConstructor]
    public Alchemist()
    {
        Alias = "Alchemist";
    }
}