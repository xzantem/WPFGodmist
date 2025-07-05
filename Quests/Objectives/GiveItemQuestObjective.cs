using GodmistWPF.Towns.NPCs;

namespace GodmistWPF.Quests.Objectives;

public class GiveItemQuestObjective : IQuestObjective
{
    public bool IsComplete { get; set; }
    public string Description { get; }

    public void Progress(QuestObjectiveContext context)
    {
        throw new NotImplementedException();
    }

    public string ItemToGive { get; set; }
    public NPC NPCToGive { get; set; }
    public int QuantityToGive { get; set; }
}