

using System.IO;
using GodmistWPF.Enums;
using GodmistWPF.Items.Equippable;
using GodmistWPF.Items.Equippable.Armors;
using GodmistWPF.Items.Equippable.Weapons;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;

/// <summary>
/// Statyczna klasa zarządzająca częściami ekwipunku (głowicami, rękojeściami, elementami zbroi itp.).
/// Odpowiada za wczytywanie danych z pliku JSON i udostępnianie metod do wyszukiwania części.
/// </summary>
public static class EquipmentPartManager
{
    /// <summary>
    /// Kolekcja dostępnych głowic broni.
    /// </summary>
    public static List<WeaponHead> WeaponHeads { get; private set; } = null!;
    
    /// <summary>
    /// Kolekcja dostępnych elementów łączących w broni.
    /// </summary>
    public static List<WeaponBinder> WeaponBinders { get; private set; } = null!;
    
    /// <summary>
    /// Kolekcja dostępnych rękojeści broni.
    /// </summary>
    public static List<WeaponHandle> WeaponHandles { get; private set; } = null!;

    /// <summary>
    /// Kolekcja dostępnych płyt pancerza.
    /// </summary>
    public static List<ArmorPlate> ArmorPlates { get; private set; } = null!;
    
    /// <summary>
    /// Kolekcja dostępnych elementów łączących w zbroi.
    /// </summary>
    public static List<ArmorBinder> ArmorBinders { get; private set; } = null!;
    
    /// <summary>
    /// Kolekcja dostępnych podstaw zbroi.
    /// </summary>
    public static List<ArmorBase> ArmorBases { get; private set; } = null!;

    /// <summary>
    /// Inicjalizuje menedżera części ekwipunku, wczytując dane z pliku JSON.
    /// </summary>
    /// <exception cref="FileNotFoundException">Wyrzucany, gdy nie znaleziono pliku konfiguracyjnego.</exception>
    /// <remarks>
    /// Metoda wczytuje dane z pliku equipment-parts.json i dzieli je na odpowiednie kolekcje
    /// na podstawie typu części.
    /// </remarks>
    public static void InitItems()
    {
        var path = "json/equipment-parts.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            var equipmentParts = JsonConvert.DeserializeObject<List<IEquipmentPart>>(json, new EquipmentPartConverter());
            WeaponHeads = equipmentParts.Where(x => x.GetType() == typeof(WeaponHead)).Cast<WeaponHead>().ToList();
            WeaponBinders = equipmentParts.Where(x => x.GetType() == typeof(WeaponBinder)).Cast<WeaponBinder>()
                .ToList();
            WeaponHandles = equipmentParts.Where(x => x.GetType() == typeof(WeaponHandle)).Cast<WeaponHandle>()
                .ToList();

            ArmorPlates = equipmentParts.Where(x => x.GetType() == typeof(ArmorPlate)).Cast<ArmorPlate>().ToList();
            ArmorBinders = equipmentParts.Where(x => x.GetType() == typeof(ArmorBinder)).Cast<ArmorBinder>().ToList();
            ArmorBases = equipmentParts.Where(x => x.GetType() == typeof(ArmorBase)).Cast<ArmorBase>().ToList();
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }

    /// <summary>
    /// Pobiera część ekwipunku o podanym aliasie i klasie postaci.
    /// </summary>
    /// <typeparam name="T">Typ części do pobrania (np. WeaponHead, ArmorPlate).</typeparam>
    /// <param name="alias">Alias części do znalezienia.</param>
    /// <param name="intendedClass">Klasa postaci, dla której przeznaczona jest część.</param>
    /// <returns>Znaleziona część ekwipunku.</returns>
    /// <exception cref="NotSupportedException">Wyrzucany, gdy podano nieobsługiwany typ części.</exception>
    public static T GetPart<T>(string alias, CharacterClass intendedClass) where T : IEquipmentPart
    {
        if (typeof(T) == typeof(WeaponHead))
            return (T)(object)WeaponHeads
                .FirstOrDefault(x => x.Alias == alias && x.IntendedClass == intendedClass)!;
        if (typeof(T) == typeof(WeaponBinder))
            return (T)(object)WeaponBinders
                .FirstOrDefault(x => x.Alias == alias && x.IntendedClass == intendedClass)!;
        if (typeof(T) == typeof(WeaponHandle))
            return (T)(object)WeaponHandles
                .FirstOrDefault(x => x.Alias == alias && x.IntendedClass == intendedClass)!;
        if (typeof(T) == typeof(ArmorPlate))
            return (T)(object)ArmorPlates
                .FirstOrDefault(x => x.Alias == alias && x.IntendedClass == intendedClass)!;
        if (typeof(T) == typeof(ArmorBinder))
            return (T)(object)ArmorBinders
                .FirstOrDefault(x => x.Alias == alias && x.IntendedClass == intendedClass)!;
        if (typeof(T) == typeof(ArmorBase))
            return (T)(object)ArmorBases
                .FirstOrDefault(x => x.Alias == alias && x.IntendedClass == intendedClass)!;
        throw new NotSupportedException();
    }
    /// <summary>
    /// Pobiera część ekwipunku na podstawie identyfikatora.
    /// </summary>
    /// <typeparam name="T">Typ części do pobrania (np. WeaponHead, ArmorPlate).</typeparam>
    /// <param name="id">Indeks części w odpowiedniej kolekcji.</param>
    /// <returns>Część ekwipunku o podanym indeksie.</returns>
    /// <exception cref="NotSupportedException">Wyrzucany, gdy podano nieobsługiwany typ części.</exception>
    /// <remarks>
    /// Uwaga: Metoda nie sprawdza zakresu indeksu.
    /// </remarks>
    public static T GetPart<T>(int id) where T : IEquipmentPart
    {
        if (typeof(T) == typeof(WeaponHead)) return (T)(object)WeaponHeads[id];
        if (typeof(T) == typeof(WeaponBinder)) return (T)(object)WeaponBinders[id];
        if (typeof(T) == typeof(WeaponHandle)) return (T)(object)WeaponHandles[id];
        if (typeof(T) == typeof(ArmorPlate)) return (T)(object)ArmorPlates[id];
        if (typeof(T) == typeof(ArmorBinder)) return (T)(object)ArmorBinders[id];
        if (typeof(T) == typeof(ArmorBase)) return (T)(object)ArmorBases[id];
        throw new NotSupportedException();
    }

    /// <summary>
    /// Losuje część ekwipunku określonego typu, poziomu i klasy postaci.
    /// </summary>
    /// <typeparam name="T">Typ części do wylosowania (np. WeaponHead, ArmorPlate).</typeparam>
    /// <param name="tier">Poziom części do wylosowania.</param>
    /// <param name="intendedClass">Klasa postaci, dla której przeznaczona ma być część.</param>
    /// <returns>Losowa część ekwipunku spełniająca podane kryteria.</returns>
    /// <exception cref="NotSupportedException">Wyrzucany, gdy podano nieobsługiwany typ części.</exception>
    /// <remarks>
    /// Metoda pomija części z materiałem "None".
    /// </remarks>
    public static T GetRandomPart<T>(int tier, CharacterClass intendedClass) where T : IEquipmentPart
    {
        if (typeof(T) == typeof(WeaponHead))
            return (T)(object)UtilityMethods.RandomChoice(WeaponHeads
                .Where(x => x.Tier == tier && x.Material != "None" && x.IntendedClass == intendedClass).ToList());
        if (typeof(T) == typeof(WeaponBinder))
            return (T)(object)UtilityMethods.RandomChoice(WeaponBinders
                .Where(x => x.Tier == tier && x.Material != "None" && x.IntendedClass == intendedClass).ToList());
        if (typeof(T) == typeof(WeaponHandle))
            return (T)(object)UtilityMethods.RandomChoice(WeaponHandles
                .Where(x => x.Tier == tier && x.Material != "None" && x.IntendedClass == intendedClass).ToList());
        if (typeof(T) == typeof(ArmorPlate))
            return (T)(object)UtilityMethods.RandomChoice(ArmorPlates
                .Where(x => x.Tier == tier && x.Material != "None" && x.IntendedClass == intendedClass).ToList());
        if (typeof(T) == typeof(ArmorBinder))
            return (T)(object)UtilityMethods.RandomChoice(ArmorBinders
                .Where(x => x.Tier == tier && x.Material != "None" && x.IntendedClass == intendedClass).ToList());
        if (typeof(T) == typeof(ArmorBase))
            return (T)(object)UtilityMethods.RandomChoice(ArmorBases
                .Where(x => x.Tier == tier && x.Material != "None" && x.IntendedClass == intendedClass).ToList());
        throw new NotSupportedException();
    }
}