

namespace GodmistWPF.Quests.Objectives;

public interface IQuestObjective
{
    public bool IsComplete { get; set; }
    public string Description { get; }
    public void Progress(QuestObjectiveContext context);
}