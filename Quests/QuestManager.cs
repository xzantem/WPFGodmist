using System.IO;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Dungeons;
using GodmistWPF.Quests.Objectives;
using GodmistWPF.Towns;
using GodmistWPF.Utilities;

namespace GodmistWPF.Quests;

/// <summary>
/// Klasa zarządzająca zadaniami w grze. Odpowiada za przechowywanie, aktualizację i generowanie zadań.
/// </summary>
public static class QuestManager
{
    /// <summary>
    /// Zdarzenie wywoływane po ukończeniu zadania.
    /// </summary>
    public static event EventHandler<Quest> QuestCompleted;
    
    /// <summary>
    /// Lista głównych zadań w grze.
    /// </summary>
    public static List<Quest>? MainQuests { get; set; }
    /// <summary>
    /// Lista losowych zadań pobocznych.
    /// </summary>
    public static List<Quest>? RandomizedSideQuests { get; set; }
    /// <summary>
    /// Lista zadań pobocznych związanych z bossami.
    /// </summary>
    public static List<Quest>? BossSideQuests { get; set; }

    /// <summary>
    /// Pobiera połączoną listę wszystkich zadań (głównych, losowych i związanych z bossami).
    /// </summary>
    public static List<Quest> Quests
    {
        get
        {
            var quests = MainQuests.ToList();
            quests.AddRange(RandomizedSideQuests);
            quests.AddRange(BossSideQuests);

            return quests;
        }
    }
    /// <summary>
    /// Słownik śledzący postęp w pokonywaniu bossów w poszczególnych lochach.
    /// </summary>
    public static Dictionary<DungeonType, int>? BossProgress { get; set; }
    
    /// <summary>
    /// Maksymalna liczba aktywnych zadań pobocznych.
    /// </summary>
    public const int QuestCount = 4;
    /// <summary>
    /// Docelowa liczba ukończeń wymagana do odblokowania zadania z bossem.
    /// </summary>
    public const int ProgressTarget = 1;

    /// <summary>
    /// Inicjalizuje główne zadania, wczytując je z pliku JSON.
    /// </summary>
    /// <exception cref="FileNotFoundException">Wyrzucany, gdy nie znaleziono pliku z zadaniami.</exception>
    public static void InitMainQuests()
    {
        var path = "json/quests.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            MainQuests = new List<Quest>();
            //MainQuests = JsonConvert.DeserializeObject<List<Quest>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }
    
    /// <summary>
    /// Generuje nowe losowe zadania poboczne, jeśli liczba aktywnych zadań jest mniejsza niż QuestCount.
    /// </summary>
    public static void RerollSideQuests()
    {
        var randomSideQuestLevel = RandomizedSideQuests.Count == 0 ? 2 : 2 + RandomizedSideQuests
            .Count(x => x.QuestState is QuestState.Completed or QuestState.HandedIn);
        var bossSideQuestLevel = BossSideQuests.Count == 0 ? 6 : BossSideQuests.Max(x => x.RecommendedLevel);
        var level = Math.Max(Math.Min(randomSideQuestLevel, bossSideQuestLevel), 2);
        while (RandomizedSideQuests.Count(x => x.QuestState is QuestState.Accepted or QuestState.Available) < QuestCount)
        {
            var dungeonType = Enum.GetValues<DungeonType>()[Random.Shared.Next(0,8)];
            var randomQuest = Random.Shared.Next(0, 2) switch
            {
                0 => new Quest("KillDungeon" + dungeonType, level, [
                        new QuestStage("KillDungeon" + dungeonType,
                            [new KillInDungeonQuestObjective(dungeonType, Random.Shared.Next(6 / 2, 13 / 2))])
                    ], new QuestReward((int)(150 * Math.Pow(4, level / 10.0)),
                        (int)(Math.Pow(level, 1.5) + 13), 4, []), 
                    UtilityMethods.RandomChoice(TownsHandler.Arungard.NPCs.Select(x => x.Alias).ToList()),
                    "", "", true),
                1 => new Quest("Descend" + dungeonType, level, [
                        new QuestStage("Descend" + dungeonType,
                            [new DescendQuestObjective(dungeonType, Random.Shared.Next(4 / 2, 8 / 2))])
                    ], new QuestReward((int)(150 * Math.Pow(4, level / 10.0)),
                        (int)(Math.Pow(level, 1.5) + 13), 4, []), 
                    UtilityMethods.RandomChoice(TownsHandler.Arungard.NPCs.Select(x => x.Alias).ToList()),
                    "", "", true),
            };
            RandomizedSideQuests.Add(randomQuest);
        }
    }
    

    /// <summary>
    /// Sprawdza i aktualizuje postęp we wszystkich aktywnych zadaniach na podstawie dostarczonego kontekstu.
    /// </summary>
    /// <param name="context">Kontekst zawierający informacje o wykonanej akcji.</param>
    public static void CheckForProgress(QuestObjectiveContext context)
    {
        foreach (var quest in Quests.Where(q => q.QuestState == QuestState.Accepted))
        {
            var previousState = quest.QuestState;
            quest.TryProgress(context);
            if (quest.QuestState != QuestState.Completed || quest.QuestState == previousState) continue;
            OnQuestCompleted(quest);
            InitSideQuests();
        }
    }
    
    /// <summary>
    /// Wywołuje zdarzenie ukończenia zadania.
    /// </summary>
    /// <param name="quest">Zadanie, które zostało ukończone.</param>
    public static void OnQuestCompleted(Quest quest)
    {
        QuestCompleted?.Invoke(null, quest);
    }

    /// <summary>
    /// Inicjalizuje zadania poboczne, opcjonalnie czyszcząc istniejące.
    /// </summary>
    /// <param name="clear">Określa, czy wyczyścić istniejące zadania przed inicjalizacją.</param>
    public static void InitSideQuests(bool clear = false)
    {
        RandomizedSideQuests = RandomizedSideQuests == null || clear ? [] : RandomizedSideQuests;
        BossSideQuests = BossSideQuests == null || clear ? [] : BossSideQuests;
        BossProgress = BossProgress == null || clear ? new Dictionary<DungeonType, int> {
            {DungeonType.Catacombs, 0}, {DungeonType.Forest, 0}, { DungeonType.ElvishRuins, 0}, { DungeonType.Cove, 0}, 
            { DungeonType.Desert, 0}, { DungeonType.Temple, 0}, { DungeonType.Mountains, 0}, { DungeonType.Swamp, 0}, 
        } : BossProgress;
        RerollSideQuests();
    }

    /// <summary>
    /// Aktualizuje dostępne zadania związane z bossami na podstawie postępu gracza.
    /// Generuje nowe zadania, jeśli spełnione są odpowiednie warunki.
    /// </summary>
    public static void UpdateBossQuests()
    {
        foreach (var progress in BossProgress)
        {
            if (progress.Value < ProgressTarget) continue;
            var target = GetBossQuestTarget(progress.Key, progress.Value / ProgressTarget);
            if (BossSideQuests.Any(x => x.Stages.Any(s => s.Objectives
                    .Any(o => o.GetType() == typeof(KillQuestObjective) &&
                              ((KillQuestObjective)o).Target == target)))) continue;
            var quest = new Quest(target, GetLevel(BossSideQuests.Count + 1),
                [new QuestStage(target, [new KillQuestObjective(target, 1)])],
                new QuestReward((int)(300 * Math.Pow(4, GetLevel(BossSideQuests.Count + 1) / 10.0)), 
                    (int)(2 * Math.Pow(GetLevel(BossSideQuests.Count + 1), 1.5) + 26), 10, []), 
                UtilityMethods.RandomChoice(TownsHandler.Arungard.NPCs.Select(x => x.Alias).ToList()),
                "", "");
            BossSideQuests.Add(quest);
        }
        return;
        int GetLevel(int n)
        {
            if (n < 1) throw new ArgumentException("n must be greater than or equal to 1.");
            var value = 6;
            for (var i = 2; i <= n; i++)
            {
                if (i == 9) value += 6;
                else if (i % 2 == 0) value += 2;
                else value += 3;
            }
            return value;
        }
    }
    
    /// <summary>
    /// Pobiera identyfikator celu zadania związanego z bossem na podstawie typu lochu i etapu.
    /// </summary>
    /// <param name="dungeon">Typ lochu, w którym znajduje się boss.</param>
    /// <param name="stage">Etap walki z bossem (1 - pierwszy boss, 2 - drugi boss).</param>
    /// <returns>Identyfikator celu zadania.</returns>
    public static string GetBossQuestTarget(DungeonType dungeon, int stage)
    {
        return (dungeon, stage) switch
        {
            (DungeonType.Catacombs, 1) => "SkeletonAssassin",
            (DungeonType.Catacombs, 2) => "DeadKingGhost",
            (DungeonType.Forest, 1) => "InfectedEnt",
            (DungeonType.Forest, 2) => "BanditLeader",
            (DungeonType.ElvishRuins, 1) => "DemonicJarl",
            (DungeonType.ElvishRuins, 2) => "Cerberus",
            (DungeonType.Cove, 1) => "AlphaShark",
            (DungeonType.Cove, 2) => "PirateCaptain",
            (DungeonType.Desert, 1) => "AvazarLeader",
            (DungeonType.Desert, 2) => "AlphaScorpionRider",
            (DungeonType.Temple, 1) => "Hierophant",
            (DungeonType.Temple, 2) => "Java",
            (DungeonType.Mountains, 1) => "CrushedSkullBerserker",
            (DungeonType.Mountains, 2) => "Wyvern",
            (DungeonType.Swamp, 1) => "SwampEnt",
            (DungeonType.Swamp, 2) => "ManeaterPlant",
        };
    }
}