using System.IO;
using System.Text.Json;
using GodmistWPF.Items.MiscItems;

namespace GodmistWPF.Items;

public static class ItemManager
{
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
    public static List<BaseIngredient> BaseIngredients { get; private set; }
    public static List<CraftableIngredient> CraftableIngredients { get; private set; }

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
    
    public static IItem GetItem(string alias)
    {
        return Items.FirstOrDefault(i => i.Alias == alias);
    }
}