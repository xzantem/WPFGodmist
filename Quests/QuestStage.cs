using GodmistWPF.Quests.Objectives;
using GodmistWPF.Utilities;
using Newtonsoft.Json;

namespace GodmistWPF.Quests;

/// <summary>
/// Reprezentuje pojedynczy etap zadania, zawierający listę celów do wykonania.
/// Każdy etap może mieć swoją nazwę i opis, które są pobierane na podstawie aliasu.
/// </summary>
public class QuestStage
{
    /// <summary>
    /// Lista celów, które muszą zostać wykonane, aby ukończyć ten etap zadania.
    /// </summary>
    public List<IQuestObjective> Objectives { get; set; }
    
    
    /// <summary>
    /// Unikalny identyfikator etapu zadania, używany do lokalizacji nazwy i opisu.
    /// </summary>
    /// <value>Alias etapu.</value>
    public string Alias { get; set; }
    /// <summary>
    /// Pobiera zlokalizowaną nazwę etapu zadania na podstawie aliasu.
    /// </summary>
    /// <value>Nazwa etapu.</value>
    [JsonIgnore]
    public string Name => NameAliasHelper.GetName(Alias);
    /// <summary>
    /// Pobiera zlokalizowany opis etapu zadania na podstawie aliasu.
    /// </summary>
    /// <value>Opis etapu.</value>
    [JsonIgnore]
    public string Description => NameAliasHelper.GetName(Alias + "Description");
    
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="QuestStage"/>. Używany przez serializator JSON.
    /// </summary>
    public QuestStage() {}

    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="QuestStage"/> z określonym aliasem i celami.
    /// </summary>
    /// <param name="alias">Unikalny identyfikator etapu.</param>
    /// <param name="objectives">Lista celów do wykonania w tym etapie.</param>
    public QuestStage(string alias, List<IQuestObjective> objectives)
    {
        Alias = alias;
        Objectives = objectives;
    }
    
}