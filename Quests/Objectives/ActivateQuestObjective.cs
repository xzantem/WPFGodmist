

using GodmistWPF.Enums.Dungeons;

namespace GodmistWPF.Quests.Objectives;

public class ActivateQuestObjective : IQuestObjective
{
    public bool IsComplete { get; set; }

    public DungeonType Target { get; set; }
    public int AmountToActivate { get; set; }
    public int QuestProgress { get; private set; }
    public string Description { get; }

    public void Progress(QuestObjectiveContext context)
    {
        if (context.ActivateInDungeonTarget != Target) return;
        QuestProgress++;
        if (QuestProgress >= AmountToActivate)
            IsComplete = true;
    }
}