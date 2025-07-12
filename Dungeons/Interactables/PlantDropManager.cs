using System.IO;
using System.Text.Json;
using DropPool = GodmistWPF.Items.Drops.DropPool;
using DungeonType = GodmistWPF.Enums.Dungeons.DungeonType;

namespace GodmistWPF.Dungeons.Interactables;

/// <summary>
/// Klasa zarządzająca bazą danych przedmiotów wypadających z roślin w lochach.
/// </summary>
public static class PlantDropManager
{
    /// <summary>
    /// Pobiera słownik zawierający pule przedmiotów dostępne w zależności od typu lochu.
    /// </summary>
    public static Dictionary<DungeonType, DropPool> DropDatabase { get; private set; }

    /// <summary>
    /// Inicjalizuje bazę danych przedmiotów wypadających z roślin, ładując dane z pliku JSON.
    /// </summary>
    /// <exception cref="FileNotFoundException">Wyrzucany, gdy nie znaleziono pliku z danymi.</exception>
    public static void InitPlantDrops()
    {
        var path = "json/plant-drop-table.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            DropDatabase = JsonSerializer.Deserialize<Dictionary<DungeonType, DropPool>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }
}