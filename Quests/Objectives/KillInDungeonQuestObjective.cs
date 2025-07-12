using GodmistWPF.Enums.Dungeons;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;

namespace GodmistWPF.Quests.Objectives;

/// <summary>
/// Cel zadania polegający na pokonaniu określonej liczby przeciwników w konkretnym lochu.
/// </summary>
[JsonConverter(typeof(QuestObjectiveConverter))]
public class KillInDungeonQuestObjective : IQuestObjective
{
    /// <inheritdoc />
    public bool IsComplete { get; set; }

    /// <summary>
    /// Pobiera opis celu w formacie "Pokonaj [liczba] przeciwników w [nazwa lochu w miejscowniku] (aktualnie/do pokonania)".
    /// </summary>
    public string Description =>
        $"{locale.Kill} {locale.EnemiesIn} {NameAliasHelper.GetDungeonType(Target, "Locative")} " +
        $"({QuestProgress}/{AmountToKill})";
    
    /// <summary>
    /// Typ lochu, w którym należy pokonać przeciwników.
    /// </summary>
    public DungeonType Target { get; set; }
    
    /// <summary>
    /// Wymagana liczba przeciwników do pokonania.
    /// </summary>
    public int AmountToKill { get; set; }
    
    /// <summary>
    /// Aktualna liczba pokonanych przeciwników.
    /// </summary>
    public int QuestProgress { get; private set; }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="KillInDungeonQuestObjective"/>. Używany przez serializator JSON.
    /// </summary>
    [JsonConstructor]
    public KillInDungeonQuestObjective()
    {
    }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="KillInDungeonQuestObjective"/> z określonym lochem i liczbą przeciwników.
    /// </summary>
    /// <param name="dungeon">Typ lochu, w którym należy pokonać przeciwników.</param>
    /// <param name="amountToKill">Wymagana liczba przeciwników do pokonania.</param>
    public KillInDungeonQuestObjective(DungeonType dungeon, int amountToKill)
    {
        Target = dungeon;
        AmountToKill = amountToKill;
        QuestProgress = 0;
        IsComplete = false;
    }

    
    /// <summary>
    /// Aktualizuje postęp zadania na podstawie dostarczonego kontekstu.
    /// Zwiększa licznik pokonanych przeciwników, jeśli kontekst dotyczy odpowiedniego lochu.
    /// </summary>
    /// <param name="context">Kontekst zawierający informacje o zabitym przeciwniku i lokacji.</param>
    public void Progress(QuestObjectiveContext context)
    {
        if (context.KillInDungeonTarget == null || context.KillInDungeonTarget != Target) return;
        QuestProgress++;
        if (QuestProgress >= AmountToKill)
            IsComplete = true;
    }
}