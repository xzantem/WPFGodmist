using ConsoleGodmist;
using GodmistWPF.Characters;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;

namespace GodmistWPF.Quests.Objectives;

[JsonConverter(typeof(QuestObjectiveConverter))]
public class KillQuestObjective : IQuestObjective
{
    public bool IsComplete { get; set; }
    public string Target { get; set; }
    public int AmountToKill { get; set; }
    public int QuestProgress { get; private set; }
    public string Description => 
        $"{locale.Kill} {EnemyFactory.EnemiesList.Find(x => x.Alias == Target).Name} " +
        $"({QuestProgress}/{AmountToKill})";
    
    [JsonConstructor]
    public KillQuestObjective()
    {
    }

    public KillQuestObjective(string target, int amountToKill)
    {
        Target = target;
        AmountToKill = amountToKill;
        IsComplete = false;
        QuestProgress = 0;
    }

    public void Progress(QuestObjectiveContext context)
    {
        if (context.KillTarget != Target) return;
        QuestProgress++;
        if (QuestProgress >= AmountToKill)
            IsComplete = true;
    }
}