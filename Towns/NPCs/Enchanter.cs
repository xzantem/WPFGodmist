using GodmistWPF.Enums.Items;
using GodmistWPF.Items;
using GodmistWPF.Items.Galdurites;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;


namespace GodmistWPF.Towns.NPCs;

/// <summary>
/// Reprezentuje zaklinacza - NPC odpowiedzialnego za usługi związane z zaklęciami i ulepszaniem magicznych właściwości przedmiotów.
/// </summary>
[JsonConverter(typeof(NPCConverter))]
public class Enchanter : NPC
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="Enchanter"/> z określonym aliasem.
    /// </summary>
    /// <param name="alias">Alias zaklinacza.</param>
    public Enchanter(string alias)
    {
        Alias = alias;
        Inventory = new NPCInventory([ItemType.Runeforging, ItemType.WeaponGaldurite, ItemType.ArmorGaldurite]);
        CraftableItems = ItemManager.CraftableIngredients.Where(x => x.ItemType == ItemType.Runeforging)
            .Cast<ICraftable>().ToList();
        LoyaltyLevel = 1;
        GoldSpent = 0;
    }
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="Enchanter"/> bez parametrów.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizmy deserializacji.
    /// </remarks>
    [JsonConstructor]
    public Enchanter()
    {
        Alias = "Enchanter";
    }

    /// <summary>
    /// Otwiera interfejs nasączania przedmiotów.
    /// </summary>
    /// <remarks>
    /// Umożliwia graczowi wzmocnienie przedmiotów za pomocą magicznych esencji.
    /// </remarks>
    public void Imbue()
    {
        Alias = "Enchanter";
    }
}