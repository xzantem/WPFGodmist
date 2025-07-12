using GodmistWPF.Enums.Dungeons;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;

namespace GodmistWPF.Quests.Objectives;

/// <summary>
/// Cel zadania polegający na zejściu na określone piętro w danym lochu.
/// </summary>
[JsonConverter(typeof(QuestObjectiveConverter))]
public class DescendQuestObjective : IQuestObjective
{
    /// <inheritdoc />
    public bool IsComplete { get; set; }

    /// <summary>
    /// Typ lochu, do którego należy zejść.
    /// </summary>
    public DungeonType Target { get; set; }
    
    /// <summary>
    /// Numer piętra, na które należy zejść, aby ukończyć cel.
    /// </summary>
    public int FloorToReach { get; set; }
    
    /// <summary>
    /// Pobiera opis celu w formacie "Zejdź na [piętro] w [nazwa lochu w miejscowniku]".
    /// </summary>
    public string Description => 
        $"{locale.Descend} {FloorToReach} {locale.In} {NameAliasHelper.GetDungeonType(Target, "Locative")}";
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DescendQuestObjective"/>. Używany przez serializator JSON.
    /// </summary>
    [JsonConstructor]
    public DescendQuestObjective()
    {
    }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DescendQuestObjective"/> z określonym lochem i piętrem.
    /// </summary>
    /// <param name="dungeon">Typ lochu, do którego należy zejść.</param>
    /// <param name="floorToReach">Numer piętra, na które należy zejść.</param>
    public DescendQuestObjective(DungeonType dungeon, int floorToReach)
    {
        Target = dungeon;
        FloorToReach = floorToReach;
        IsComplete = false;
    }
    
    /// <summary>
    /// Aktualizuje postęp zadania na podstawie dostarczonego kontekstu.
    /// Oznacza cel jako ukończony, jeśli kontekst dotyczy zejścia na odpowiednie piętro we właściwym lochu.
    /// </summary>
    /// <param name="context">Kontekst zawierający informacje o zejściu do lochu.</param>
    public void Progress(QuestObjectiveContext context)
    {
        if (context.DescendTarget != null && context.DescendTarget == Target && FloorToReach == context.DescendFloor)
            
            IsComplete = true;
    }
}