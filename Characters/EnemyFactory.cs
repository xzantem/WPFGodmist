using System.IO;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Dungeons;
using GodmistWPF.Quests;
using GodmistWPF.Utilities;
using Newtonsoft.Json;

namespace GodmistWPF.Characters;

public static class EnemyFactory
{
    public static List<EnemyCharacter> EnemiesList;

    private const double BossChance = 0.2;
    
    public static EnemyCharacter CreateEnemy(string alias, int level)
    {
        var enemy = EnemiesList.FirstOrDefault(x => x.Alias == alias);
        return new EnemyCharacter(enemy, level);
    }
    public static EnemyCharacter CreateEnemy(DungeonType dungeonType, int level)
    {
        if (QuestManager.BossProgress[dungeonType] >= QuestManager.ProgressTarget &&
            Random.Shared.NextDouble() < BossChance) // If the dungeon quest progress to boss is suitable, try to generate boss
        {
            var target = QuestManager.GetBossQuestTarget
                (dungeonType, Random.Shared.Next(1, QuestManager.BossProgress[dungeonType] / QuestManager.ProgressTarget + 1)); // Getting boss name
            if (QuestManager.BossSideQuests.Where(x => x.QuestState != QuestState.Available)
                .Any(x => x.Alias == target)) // Check if the boss kill quest is Accepted
            {
                var boss = EnemiesList.FirstOrDefault(x => x.Name == target);
                return new BossEnemy(boss, level);
            }
        }
        var enemy = UtilityMethods.RandomChoice(EnemiesList
            .Where(x => x.DefaultLocation == dungeonType && x.EnemyType.All(t => t != EnemyType.Boss))
            .ToList());
        return new EnemyCharacter(enemy, level);
    }

    public static void InitializeEnemies()
    {
        var path = "json/enemies.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            EnemiesList = JsonConvert.DeserializeObject<List<EnemyCharacter>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }
}