using GodmistWPF.Characters.Player;
using GodmistWPF.Enums;
using GodmistWPF.Quests.Objectives;
using GodmistWPF.Towns;
using GodmistWPF.Utilities;
using Newtonsoft.Json;

namespace GodmistWPF.Quests;

public class Quest
{
    [JsonIgnore]
    public string Name => NameAliasHelper.GetName(Alias);
    [JsonIgnore]
    public string Description =>
        IsSideQuest ? Stages[0].Objectives[0].Description : 
            NameAliasHelper.GetName(Alias + "Description");

    public string Alias { get; set; }
    
    public bool IsSideQuest { get; set; }
    public int RecommendedLevel { get; set; }
    public List<QuestStage> Stages { get; set; }
    public QuestReward QuestReward { get; set; }
    public QuestState QuestState { get; set; }
    public List<string> Prerequisites { get; set; }
    public string QuestGiver { get; set; }
    public string QuestEnder { get; set; }
    public List<string> AcceptDialogue { get; set; }
    public List<string> HandInDialogue { get; set; }
    
    public Quest() {}

    public Quest(string alias, int level, List<QuestStage> stages, QuestReward questReward, string questGiver,
        string acceptDialogue, string handInDialogue, bool isSideQuest = false)
    {
        Alias = alias;
        RecommendedLevel = level;
        Stages = stages;
        QuestReward = questReward;
        QuestState = QuestState.Available;
        Prerequisites = [];
        QuestGiver = questGiver;
        QuestEnder = questGiver;
        AcceptDialogue = [acceptDialogue];
        HandInDialogue = [handInDialogue];
        IsSideQuest = isSideQuest;
    }

    public void TryProgress(QuestObjectiveContext context)
    {
        if (context.ContextLevel < RecommendedLevel || QuestState != QuestState.Accepted) return;
        foreach (var objective in GetCurrentStage().Objectives.Where(objective => !objective.IsComplete))
        {
            objective.Progress(context);
            if (!objective.IsComplete || GetCurrentStage() == default) continue;
            // WPF handles quest progress UI
        }
        if (GetCurrentStage() == default)
            TryCompleteQuest();
    }

    public QuestStage GetCurrentStage()
    {
        return Stages.FirstOrDefault(stage => stage.Objectives.Any(objective => !objective.IsComplete))!;
    }

    public void AcceptQuest()
    {
        QuestState = QuestState.Accepted;
    }

    public void TryCompleteQuest()
    {
        if (QuestState != QuestState.Accepted || Stages.Count <= 0 || 
            !Stages.All(s => s.Objectives.All(o => o.IsComplete))) return;
        QuestState = QuestState.Completed;
        // WPF handles quest completion UI
    }

    public void HandInQuest()
    {
        GetRewards();
        QuestState = QuestState.HandedIn;
        if (QuestManager.RandomizedSideQuests.Contains(this))
        {
            foreach (var objective in Stages.SelectMany(stage => stage.Objectives))
            {
                switch (objective)
                {
                    case KillInDungeonQuestObjective killObjective:
                        QuestManager.BossProgress[killObjective.Target]++;
                        break;
                    case DescendQuestObjective descendObjective:
                        QuestManager.BossProgress[descendObjective.Target]++;
                        break;
                }
            }
            QuestManager.UpdateBossQuests();
        }
    }

    private void GetRewards()
    {
        var player = PlayerHandler.player;
        if (QuestReward.Gold != 0) player.GainGold(QuestReward.Gold);
        if (QuestReward.Experience != 0) player.GainExperience(QuestReward.Experience);
        if (QuestReward.Honor !=  0) player.GainHonor(QuestReward.Honor);
        if (QuestReward.Items.Count == 0) return;
        foreach (var item in QuestReward.Items)
            player.Inventory.AddItem(item.Key, item.Value);
    }
}