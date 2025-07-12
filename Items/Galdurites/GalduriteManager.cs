// Importy wymaganych przestrzeni nazw
using System.IO;
using System.Collections.Generic;
using System.Linq;
using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Items;
using GodmistWPF.Utilities;
using Newtonsoft.Json;

/// <summary>
/// Zawiera klasy odpowiedzialne za zarządzanie galdurytami w grze.
/// </summary>
/// <remarks>
/// <para>Przestrzeń nazw zawiera następujące główne komponenty:</para>
/// <list type="bullet">
/// <item><see cref="GalduriteManager"/> - główna klasa zarządzająca galdurytami</item>
/// <item><see cref="Galdurite"/> - klasa reprezentująca pojedynczy galduryt</item>
/// <item><see cref="GalduriteComponent"/> - klasa reprezentująca komponent galdurytu</item>
/// </list>
/// <para>Galduryty to specjalne przedmioty w grze, które mogą być używane do wzmacniania ekwipunku.</para>
/// </remarks>
namespace GodmistWPF.Items.Galdurites;

/// <summary>
/// Klasa zarządzająca komponentami galdurytów w grze.
/// </summary>
/// <remarks>
/// <para>Klasa odpowiedzialna jest za:</para>
/// <list type="bullet">
/// <item>Wczytywanie definicji komponentów z pliku JSON</item>
/// <item>Zarządzanie dostępnymi komponentami galdurytów</item>
/// <item>Udostępnianie metod do wyszukiwania i filtrowania komponentów</item>
/// <item>Konwersję między poziomem a materiałem galdurytu</item>
/// </list>
/// <para>Wykorzystuje wzorzec projektowy Singleton do zapewnienia globalnego dostępu do menedżera.</para>
/// </remarks>
public static class GalduriteManager
{
    /// <summary>
    /// Pobiera listę wszystkich dostępnych komponentów galdurytów.
    /// </summary>
    /// <value>
    /// Lista obiektów <see cref="GalduriteComponent"/> reprezentujących różne rodzaje komponentów galdurytów.
    /// </value>
    /// <remarks>
    /// Właściwość jest inicjalizowana podczas wywołania metody <see cref="InitComponents"/>.
    /// Dostęp do listy jest możliwy tylko do odczytu z zewnątrz klasy.
    /// </remarks>
    public static List<GalduriteComponent> GalduriteComponents { get; private set; }
    
    /// <summary>
    /// Inicjalizuje menedżer galdurytów, ładując dane z pliku konfiguracyjnego.
    /// </summary>
    /// <exception cref="FileNotFoundException">Wyrzucany, gdy nie znaleziono pliku konfiguracyjnego galdurytów.</exception>
    /// <exception cref="JsonException">Wyrzucany, gdy wystąpi błąd podczas deserializacji pliku JSON.</exception>
    /// <remarks>
    /// <para>Metoda wykonuje następujące operacje:</para>
    /// <list type="number">
    /// <item>Sprawdza istnienie pliku konfiguracyjnego w lokalizacji "json/galdurite-components.json"</item>
    /// <item>Wczytuje zawartość pliku JSON</item>
    /// <item>Deserializuje dane do listy obiektów <see cref="GalduriteComponent"/></item>
    /// <item>Przypisuje załadowaną listę do właściwości <see cref="GalduriteComponents"/></item>
    /// </list>
    /// <para>Metoda powinna być wywołana przed pierwszym użyciem menedżera galdurytów.</para>
    /// </remarks>
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

    /// <summary>
    /// Pobiera losowy komponent galdurytu spełniający podane kryteria.
    /// </summary>
    /// <param name="tier">Poziom efektu galdurytu (np. "Minor", "Major").</param>
    /// <param name="color">Kolor galdurytu (np. "Red", "Blue") lub "Random" dla losowego koloru.</param>
    /// <param name="type">Typ ekwipunku, dla którego przeznaczony jest galduryt.</param>
    /// <param name="excludedColors">Zbiór kolorów, które mają być wykluczone z wyszukiwania.</param>
    /// <returns>Losowo wybrany komponent galdurytu spełniający podane kryteria.</returns>
    /// <exception cref="InvalidOperationException">Wyrzucany, gdy nie znaleziono żadnego pasującego komponentu.</exception>
    /// <remarks>
    /// <para>Metoda wykonuje następujące kroki:</para>
    /// <list type="number">
    /// <item>Jeśli parametr <paramref name="color"> ma wartość "Random", wybiera losowy kolor z wyjątkiem tych w <paramref name="excludedColors"/></item>
    /// <item>Filtruje dostępne komponenty według podanych kryteriów (poziom, kolor, typ ekwipunku, wykluczone kolory)</item>
    /// <item>Zwraca losowy element z przefiltrowanej listy</item>
    /// </list>
    /// <para>Metoda używa <see cref="UtilityMethods.RandomChoice"/> do losowego wyboru elementu z listy.</para>
    /// </remarks>
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

    /// <summary>
    /// Konwertuje poziom i kolor galdurytu na nazwę materiału.
    /// </summary>
    /// <param name="tier">Poziom galdurytu (1-3).</param>
    /// <param name="color">Kolor galdurytu (np. "Red", "Blue").</param>
    /// <returns>Nazwa materiału odpowiadająca podanym parametrom.</returns>
    /// <remarks>
    /// <para>Metoda zwraca odpowiednią nazwę materiału na podstawie poziomu i koloru galdurytu:</para>
    /// <list type="bullet">
    /// <item>Poziom 1: [Kolor]Powder (np. RedPowder)</item>
    /// <item>Poziom 2: Condensed[Kolor]Powder (np. CondensedRedPowder)</item>
    /// <item>Poziom 3: Magical[Kolor]Powder (np. MagicalRedPowder)</item>
    /// </list>
    /// <para>Obsługiwane kolory: Red, Blue, Purple, Yellow, Green, Black, Pink, Orange, White, Golden</para>
    /// <para>W przypadku nieznanego koloru lub poziomu, metoda może zwrócić nieokreśloną wartość.</para>
    /// </remarks>
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
    
    public static bool CanCraftGaldurite(int tier, string color, bool isArmorGaldurite, out string errorMessage)
    {
        errorMessage = string.Empty;
        var player = PlayerHandler.player;
        var powder = GetPowder(color);
        
        if (string.IsNullOrEmpty(powder))
        {
            errorMessage = "Invalid color selected.";
            return false;
        }
        
        int requiredPowder = tier * 2; // Base cost: 2 powder per tier
        
        if (player.Inventory.GetCount(powder) < requiredPowder)
        {
            errorMessage = $"Not enough {color} powder. Need {requiredPowder} but have {player.Inventory.GetCount(powder)}.";
            return false;
        }
        
        return true;
    }
    
    public static Galdurite CraftGaldurite(int tier, string color, bool isArmorGaldurite, string name)
    {
        var player = PlayerHandler.player;
        var powder = GetPowder(color);
        int requiredPowder = tier * 2;
        
        // Remove the required powder from inventory
        player.Inventory.TryRemoveItem(ItemManager.GetItem(powder), requiredPowder);
        
        // Create and return the new Galdurite
        var galdurite = new Galdurite(isArmorGaldurite, tier, tier, color);
        player.Inventory.AddItem(galdurite);
        
        return galdurite;
    }
    
    public static List<GalduriteComponent> GetComponentsForTier(int tier, bool isArmor)
    {
        var tierLetter = tier switch
        {
            1 => "D",
            2 => "C",
            3 => "B",
            4 => "A",
            5 => "S",
            _ => "D"
        };
        
        return GalduriteComponents
            .Where(c => c.EffectTier == tierLetter && c.EquipmentType == isArmor)
            .ToList();
    }
    
    public static Galdurite? ChooseGaldurite(List<Galdurite> galdurites)
    {
        // WPF handles galdurite selection UI
        return null;
    }
}