using System.IO;
using GodmistWPF.Utilities;
using Newtonsoft.Json;

namespace GodmistWPF.Items.Galdurites;

public static class GalduriteManager
{
    public static List<GalduriteComponent> GalduriteComponents { get; private set; }
    
    public static void InitComponents()
    {
        var path = "json/galdurite-components.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            GalduriteComponents = JsonConvert.DeserializeObject<List<GalduriteComponent>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }

    public static GalduriteComponent GetGalduriteComponent(string tier, string color, bool type, HashSet<string> excludedColors)
    {
        if (color == "Random")
        {
            return UtilityMethods.RandomChoice(GalduriteComponents.Where(x =>
                x.EffectTier == tier && x.EquipmentType == type && 
                !excludedColors.Contains(x.PoolColor)).ToList());
        }
        return UtilityMethods.RandomChoice(GalduriteComponents.Where(x =>
            x.EffectTier == tier && x.PoolColor == color && x.EquipmentType == type && 
            !excludedColors.Contains(x.PoolColor)).ToList());
    }

    public static string GetColorMaterial(int tier, string color)
    {
        return (tier, color) switch
        {
            (1, "Red") => "RedPowder",
            (2, "Red") => "CondensedRedPowder",
            (3, "Red") => "MagicalRedPowder",
            (1, "Blue") => "BluePowder",
            (2, "Blue") => "CondensedBluePowder",
            (3, "Blue") => "MagicalBluePowder",
            (1, "Purple") => "PurplePowder",
            (2, "Purple") => "CondensedPurplePowder",
            (3, "Purple") => "MagicalPurplePowder",
            (1, "Yellow") => "YellowPowder",
            (2, "Yellow") => "CondensedYellowPowder",
            (3, "Yellow") => "MagicalYellowPowder",
            (1, "Green") => "GreenPowder",
            (2, "Green") => "CondensedGreenPowder",
            (3, "Green") => "MagicalGreenPowder",
            (1, "Black") => "BlackPowder",
            (2, "Black") => "CondensedBlackPowder",
            (3, "Black") => "MagicalBlackPowder",
            (1, "Pink") => "PinkPowder",
            (2, "Pink") => "CondensedPinkPowder",
            (3, "Pink") => "MagicalPinkPowder",
            (1, "White") => "WhitePowder",
            (2, "White") => "CondensedWhitePowder",
            (3, "White") => "MagicalWhitePowder",
            (1, "Golden") => "GoldenPowder",
            (2, "Golden") => "CondensedGoldenPowder",
            (3, "Golden") => "MagicalGoldenPowder",
            (1, "Orange") => "OrangePowder",
            (2, "Orange") => "CondensedOrangePowder",
            (3, "Orange") => "MagicalOrangePowder",
            _ => ""
        };
    }
    
    public static string GetPowder(string color)
    {
        return color switch
        {
            "Red" => "RedPowder",
            "Blue" => "BluePowder",
            "Purple" => "PurplePowder",
            "Yellow" => "YellowPowder",
            "Green" => "GreenPowder",
            "Black" => "BlackPowder",
            "Pink" => "PinkPowder",
            "White" => "WhitePowder",
            "Golden" => "GoldenPowder",
            "Orange" => "OrangePowder",
            _ => ""
        };
    }
    
    public static Galdurite? ChooseGaldurite(List<Galdurite> galdurites)
    {
        // WPF handles galdurite selection UI
        return null;
    }
}