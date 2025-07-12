using System.IO;
using System.Text.Json;
using GodmistWPF.Items.MiscItems;

namespace GodmistWPF.Items;

/// <summary>
/// Klasa zarządzająca wszystkimi przedmiotami w grze.
/// Odpowiada za ładowanie, przechowywanie i udostępnianie dostępu do przedmiotów.
/// </summary>
public static class ItemManager
{
    /// <summary>
    /// Pobiera listę wszystkich dostępnych przedmiotów w grze.
    /// Zawiera zarówno podstawowe składniki, jak i składniki wytwarzalne oraz przedmioty specjalne.
    /// </summary>
    public static List<IItem> Items
    {
        get
        {
            var temp = new List<IItem>();
            temp = temp.Concat(BaseIngredients).ToList();
            temp = temp.Concat(CraftableIngredients).ToList();
            temp.Add(new TownPortalScroll());
            temp.Add(new Bandage());
            temp.Add(new Antidote());
            temp.Add(new WetTowel());
            return temp;
        }
    }
    
    /// <summary>
    /// Pobiera listę podstawowych składników w grze.
    /// Składniki te są ładowane z pliku JSON podczas inicjalizacji.
    /// </summary>
    public static List<BaseIngredient> BaseIngredients { get; private set; }
    
    /// <summary>
    /// Pobiera listę składników wytwarzalnych w grze.
    /// Składniki te są ładowane z pliku JSON podczas inicjalizacji.
    /// </summary>
    public static List<CraftableIngredient> CraftableIngredients { get; private set; }

    /// <summary>
    /// Inicjalizuje menedżer przedmiotów, ładując dane z plików JSON.
    /// Metoda musi zostać wywołana przed pierwszym użyciem menedżera.
    /// </summary>
    /// <exception cref="FileNotFoundException">Wyrzucany, gdy nie znaleziono wymaganego pliku JSON.</exception>
    public static void InitItems()
    {
        var path = "json/base-ingredients.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            BaseIngredients = JsonSerializer.Deserialize<List<BaseIngredient>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
        path = "json/craftable-ingredients.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            CraftableIngredients = JsonSerializer.Deserialize<List<CraftableIngredient>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }
    
    
    /// <summary>
    /// Znajduje przedmiot na podstawie jego aliasu.
    /// </summary>
    /// <param name="alias">Alias (skrócona nazwa) szukanego przedmiotu.</param>
    /// <returns>Znaleziony przedmiot lub null, jeśli przedmiot o podanym aliasie nie istnieje.</returns>
    public static IItem GetItem(string alias)
    {
        return Items.FirstOrDefault(i => i.Alias == alias);
    }
}