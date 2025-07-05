using ConsoleGodmist;
using GodmistWPF.Towns.NPCs;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;

namespace GodmistWPF.Quests.Objectives;

[JsonConverter(typeof(QuestObjectiveConverter))]
public class TalkQuestObjective : IQuestObjective
{
    public bool IsComplete { get; set; }

    public List<string> Dialogue { get; set; }
    public NPC NPCToTalkTo { get; set; }
    public string Description => 
        $"{locale.TalkTo} {NPCToTalkTo.Name}";
    public void Progress(QuestObjectiveContext context)
    {
        if (context.TalkTarget != NPCToTalkTo) return;
        IsComplete = true;
    }
    [JsonConstructor]
    public TalkQuestObjective()
    {
    }
}