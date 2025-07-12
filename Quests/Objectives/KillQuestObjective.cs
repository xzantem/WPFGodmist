
using GodmistWPF.Characters;
using GodmistWPF.Utilities.JsonConverters;
using Newtonsoft.Json;

namespace GodmistWPF.Quests.Objectives;

/// <summary>
/// Cel zadania polegający na pokonaniu określonej liczby przeciwników danego typu.
/// </summary>
[JsonConverter(typeof(QuestObjectiveConverter))]
public class KillQuestObjective : IQuestObjective
{
    /// <inheritdoc />
    public bool IsComplete { get; set; }
    
    /// <summary>
    /// Identyfikator typu przeciwnika, którego należy pokonać.
    /// </summary>
    public string Target { get; set; }
    
    /// <summary>
    /// Wymagana liczba pokonanych przeciwników.
    /// </summary>
    public int AmountToKill { get; set; }
    
    /// <summary>
    /// Aktualna liczba pokonanych przeciwników.
    /// </summary>
    public int QuestProgress { get; private set; }
    
    /// <summary>
    /// Pobiera opis celu w formacie "Pokonaj [nazwa przeciwnika] (aktualnie/do pokonania)".
    /// </summary>
    public string Description => 
        $"{locale.Kill} {EnemyFactory.EnemiesList.Find(x => x.Alias == Target).Name} " +
        $"({QuestProgress}/{AmountToKill})";
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="KillQuestObjective"/>. Używany przez serializator JSON.
    /// </summary>
    [JsonConstructor]
    public KillQuestObjective()
    {
    }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="KillQuestObjective"/> z określonym celem i liczbą do pokonania.
    /// </summary>
    /// <param name="target">Identyfikator typu przeciwnika.</param>
    /// <param name="amountToKill">Wymagana liczba pokonanych przeciwników.</param>
    public KillQuestObjective(string target, int amountToKill)
    {
        Target = target;
        AmountToKill = amountToKill;
        IsComplete = false;
        QuestProgress = 0;
    }

    
    /// <summary>
    /// Aktualizuje postęp zadania na podstawie dostarczonego kontekstu.
    /// Zwiększa licznik pokonanych przeciwników, jeśli kontekst dotyczy odpowiedniego celu.
    /// </summary>
    /// <param name="context">Kontekst zawierający informacje o zabitym przeciwniku.</param>
    public void Progress(QuestObjectiveContext context)
    {
        if (context.KillTarget != Target) return;
        QuestProgress++;
        if (QuestProgress >= AmountToKill)
            IsComplete = true;
    }
}