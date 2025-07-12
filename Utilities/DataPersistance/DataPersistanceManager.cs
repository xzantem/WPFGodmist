using System.IO;
using GodmistWPF.Characters.Player;
using GodmistWPF.Quests;
using GodmistWPF.Towns;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;

namespace GodmistWPF.Utilities.DataPersistance;

/// <summary>
/// Klasa odpowiedzialna za zarządzanie zapisami i wczytywaniem stanu gry.
/// Zapewnia metody do zapisywania, wczytywania i usuwania plików zapisu.
/// </summary>
public static class DataPersistanceManager
{
    private static readonly string dir = 
        $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/Godmist/saves/";

    /// <summary>
    /// Metoda zachowawcza dla kompatybilności z konsolą. W wersji WPF rzuca wyjątek.
    /// </summary>
    /// <param name="data">Dane do zapisania.</param>
    /// <exception cref="InvalidOperationException">Zawsze wyrzucany, ponieważ ta metoda jest zastępowana przez funkcjonalność WPF.</exception>
    public static void SaveGame(SaveData data)
    {
        // This method is now handled by WPF dialogs
        // For console compatibility, throw an exception
        throw new InvalidOperationException("SaveGame is now handled by WPF dialogs. Use the SettingsDialog for save management.");
    }

    /// <summary>
    /// Metoda zachowawcza dla kompatybilności z konsolą. W wersji WPF zawsze zwraca false.
    /// </summary>
    /// <returns>Zawsze zwraca false, ponieważ ta metoda jest zastępowana przez funkcjonalność WPF.</returns>
    public static bool LoadGame()
    {
        // This method is now handled by WPF dialogs
        // For console compatibility, return false
        return false;
    }
    
    /// <summary>
    /// Metoda zachowawcza dla kompatybilności z konsolą. W wersji WPF nie wykonuje żadnych działań.
    /// </summary>
    public static void DeleteSaveFile()
    {
        // This method is now handled by WPF dialogs
        // For console compatibility, do nothing
    }

    // WPF-compatible methods that handle business logic
    /// <summary>
    /// Zapisuje dane gry do pliku o podanej nazwie.
    /// </summary>
    /// <param name="data">Dane gry do zapisania.</param>
    /// <param name="saveName">Nazwa pliku zapisu (bez rozszerzenia).</param>
    public static void SaveGameToFile(SaveData data, string saveName)
    {
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        var filePath = Path.Combine(dir, $"{saveName}.json");
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Wczytuje dane gry z podanej ścieżki pliku.
    /// </summary>
    /// <param name="filePath">Pełna ścieżka do pliku zapisu.</param>
    /// <returns>Zdeserializowane dane gry.</returns>
    /// <exception cref="FileNotFoundException">Gdy plik zapisu nie istnieje.</exception>
    /// <exception cref="InvalidOperationException">Gdy deserializacja danych się nie powiedzie.</exception>
    public static SaveData LoadGameFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Save file not found.", filePath);
        }

        var json = File.ReadAllText(filePath);
        var data = JsonConvert.DeserializeObject<SaveData>(json, new QuestObjectiveConverter(), new NPCConverter());
        
        if (data == null)
        {
            throw new InvalidOperationException("Failed to deserialize save data.");
        }

        return data;
    }

    /// <summary>
    /// Stosuje załadowane dane gry, aktualizując stan gry.
    /// </summary>
    /// <param name="data">Dane gry do zastosowania.</param>
    public static void ApplySaveData(SaveData data)
    {
        PlayerHandler.player = data.Player;
        GameSettings.Difficulty = data.Difficulty;
        QuestManager.MainQuests = data.Quests[0];
        QuestManager.RandomizedSideQuests = data.Quests[1];
        QuestManager.BossSideQuests = data.Quests[2];
        QuestManager.BossProgress = data.BossQuestProgress;
        TownsHandler.Arungard = data.Town;
    }

    /// <summary>
    /// Usuwa plik zapisu o podanej ścieżce, jeśli istnieje.
    /// </summary>
    /// <param name="filePath">Pełna ścieżka do pliku zapisu.</param>
    public static void DeleteSaveFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Pobiera listę dostępnych plików zapisu.
    /// </summary>
    /// <returns>Posortowana lista informacji o plikach zapisu (najnowsze pierwsze).</returns>
    public static List<SaveFileInfo> GetSaveFiles()
    {
        var saveFiles = new List<SaveFileInfo>();
        
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var files = Directory.GetFiles(dir, "*.json");
        foreach (var file in files)
        {
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var fileInfo = new FileInfo(file);
                var saveInfo = new SaveFileInfo
                {
                    Name = fileName,
                    FilePath = file,
                    CreatedDate = fileInfo.CreationTime,
                    ModifiedDate = fileInfo.LastWriteTime,
                    Size = fileInfo.Length
                };
                saveFiles.Add(saveInfo);
            }
            catch (Exception ex)
            {
                // Log error but continue loading other files
                Console.WriteLine($"Error loading save file {file}: {ex.Message}");
            }
        }

        return saveFiles.OrderByDescending(f => f.ModifiedDate).ToList();
    }

    /// <summary>
    /// Generuje nazwę pliku zapisu na podstawie nazwy postaci i znacznika czasu.
    /// </summary>
    /// <param name="characterName">Nazwa postaci gracza.</param>
    /// <returns>Wygenerowana nazwa pliku zapisu (maksymalnie 20 znaków).</returns>
    public static string GenerateSaveName(string characterName)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var saveName = $"{characterName}_{timestamp}";
        
        // Ensure the name doesn't exceed 20 characters
        if (saveName.Length > 20)
        {
            // Truncate character name if needed
            var maxCharNameLength = 20 - timestamp.Length - 1; // -1 for underscore
            if (maxCharNameLength > 0)
            {
                saveName = $"{characterName.Substring(0, maxCharNameLength)}_{timestamp}";
            }
            else
            {
                // If even timestamp is too long, use just timestamp
                saveName = timestamp.Substring(0, 20);
            }
        }

        return saveName;
    }
}

/// <summary>
/// Klasa reprezentująca informacje o pliku zapisu gry.
/// </summary>
public class SaveFileInfo
{
    /// <summary>
    /// Pobiera lub ustawia nazwę pliku zapisu (bez rozszerzenia).
    /// </summary>
    public string Name { get; set; } = "";
    
    /// <summary>
    /// Pobiera lub ustawia pełną ścieżkę do pliku zapisu.
    /// </summary>
    public string FilePath { get; set; } = "";
    
    /// <summary>
    /// Pobiera lub ustawia datę i czas utworzenia pliku.
    /// </summary>
    public DateTime CreatedDate { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia datę i czas ostatniej modyfikacji pliku.
    /// </summary>
    public DateTime ModifiedDate { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia rozmiar pliku w bajtach.
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Zwraca sformatowany ciąg reprezentujący informacje o pliku zapisu.
    /// </summary>
    /// <returns>Ciąg w formacie "Nazwa (data modyfikacji)".</returns>
    public override string ToString()
    {
        return $"{Name} ({ModifiedDate:g})";
    }
}