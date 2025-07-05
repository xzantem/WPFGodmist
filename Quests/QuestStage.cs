using GodmistWPF.Quests.Objectives;
using GodmistWPF.Utilities;
using Newtonsoft.Json;

namespace GodmistWPF.Quests;

public class QuestStage
{
    public List<IQuestObjective> Objectives { get; set; }
    
    public string Alias { get; set; }
    [JsonIgnore]
    public string Name => NameAliasHelper.GetName(Alias);
    [JsonIgnore]
    public string Description => NameAliasHelper.GetName(Alias + "Description");
    
    public QuestStage() {}

    public QuestStage(string alias, List<IQuestObjective> objectives)
    {
        Alias = alias;
        Objectives = objectives;
    }
    
}