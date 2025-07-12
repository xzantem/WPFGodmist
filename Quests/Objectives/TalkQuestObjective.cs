using GodmistWPF.Towns.NPCs;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;

namespace GodmistWPF.Quests.Objectives;

/// <summary>
/// Cel zadania polegający na rozmowie z określonym NPC.
/// </summary>
[JsonConverter(typeof(QuestObjectiveConverter))]
public class TalkQuestObjective : IQuestObjective
{
    /// <inheritdoc />
    public bool IsComplete { get; set; }

    /// <summary>
    /// Lista linii dialogowych wyświetlanych podczas rozmowy z NPC.
    /// </summary>
    public List<string> Dialogue { get; set; }
    
    /// <summary>
    /// NPC, z którym gracz musi porozmawiać, aby ukończyć cel.
    /// </summary>
    public NPC NPCToTalkTo { get; set; }
    
    /// <summary>
    /// Pobiera opis celu w formacie "Porozmawiaj z [nazwa NPC]".
    /// </summary>
    public string Description => 
        $"{locale.TalkTo} {NPCToTalkTo.Name}";
    
    /// <summary>
    /// Aktualizuje postęp zadania na podstawie dostarczonego kontekstu.
    /// Oznacza cel jako ukończony, jeśli kontekst dotyczy rozmowy z odpowiednim NPC.
    /// </summary>
    /// <param name="context">Kontekst zawierający informacje o rozmowie z NPC.</param>
    public void Progress(QuestObjectiveContext context)
    {
        if (context.TalkTarget != NPCToTalkTo) return;
        IsComplete = true;
    }
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="TalkQuestObjective"/>. Używany przez serializator JSON.
    /// </summary>
    [JsonConstructor]
    public TalkQuestObjective()
    {
    }
}