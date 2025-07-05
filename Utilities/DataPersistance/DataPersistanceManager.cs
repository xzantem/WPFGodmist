using System.IO;
using GodmistWPF.Characters.Player;
using GodmistWPF.Quests;
using GodmistWPF.Towns;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;

namespace GodmistWPF.Utilities.DataPersistance;

public static class DataPersistanceManager
{
    private static readonly string dir = 
        $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/Godmist/saves/";
    public static void SaveGame(SaveData data)
    {
        // This method is now handled by WPF dialogs
        // For console compatibility, throw an exception
        throw new InvalidOperationException("SaveGame is now handled by WPF dialogs. Use the SettingsDialog for save management.");
    }

    public static bool LoadGame()
    {
        // This method is now handled by WPF dialogs
        // For console compatibility, return false
        return false;
    }
    
    public static void DeleteSaveFile()
    {
        // This method is now handled by WPF dialogs
        // For console compatibility, do nothing
    }

    // WPF-compatible methods that handle business logic
    public static void SaveGameToFile(SaveData data, string saveName)
    {
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        var filePath = Path.Combine(dir, $"{saveName}.json");
        File.WriteAllText(filePath, json);
    }

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

    public static void DeleteSaveFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

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

public class SaveFileInfo
{
    public string Name { get; set; } = "";
    public string FilePath { get; set; } = "";
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public long Size { get; set; }

    public override string ToString()
    {
        return $"{Name} ({ModifiedDate:g})";
    }
}