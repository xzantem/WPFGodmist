

using GodmistWPF.Enums.Dungeons;

namespace GodmistWPF.Quests.Objectives;

/// <summary>
/// Cel zadania polegający na aktywowaniu określonej liczby obiektów w danym lochu.
/// </summary>
public class ActivateQuestObjective : IQuestObjective
{
    /// <inheritdoc />
    public bool IsComplete { get; set; }

    
    /// <summary>
    /// Typ lochu, w którym należy aktywować obiekty.
    /// </summary>
    public DungeonType Target { get; set; }
    
    /// <summary>
    /// Wymagana liczba aktywacji do ukończenia celu.
    /// </summary>
    public int AmountToActivate { get; set; }
    
    /// <summary>
    /// Aktualna liczba dokonanych aktywacji.
    /// </summary>
    public int QuestProgress { get; private set; }
    
    /// <summary>
    /// Pobiera opis celu zadania.
    /// </summary>
    public string Description { get; }

    
    /// <summary>
    /// Aktualizuje postęp zadania na podstawie dostarczonego kontekstu.
    /// Zwiększa licznik aktywacji, jeśli kontekst dotyczy odpowiedniego lochu.
    /// </summary>
    /// <param name="context">Kontekst zawierający informacje o aktywacji obiektu.</param>
    public void Progress(QuestObjectiveContext context)
    {
        if (context.ActivateInDungeonTarget != Target) return;
        QuestProgress++;
        if (QuestProgress >= AmountToActivate)
            IsComplete = true;
    }
}