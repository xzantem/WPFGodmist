namespace GodmistWPF.Quests.Objectives;

/// <summary>
/// Cel zadania polegający na zebraniu określonej liczby przedmiotów.
/// Przedmioty mogą być zbierane z potworów z listy ViableMonsters.
/// </summary>
public class CollectQuestObjective : IQuestObjective
{
    /// <inheritdoc />
    public bool IsComplete { get; set; }
    
    /// <summary>
    /// Pobiera opis celu zadania.
    /// </summary>
    public string Description { get; }

    
    /// <summary>
    /// Aktualizuje postęp zadania na podstawie dostarczonego kontekstu.
    /// Ta metoda nie jest jeszcze zaimplementowana.
    /// </summary>
    /// <param name="context">Kontekst zawierający informacje o zebranym przedmiocie.</param>
    /// <exception cref="NotImplementedException">Metoda nie jest jeszcze zaimplementowana.</exception>
    public void Progress(QuestObjectiveContext context)
    {
        throw new NotImplementedException();
    }

    
    /// <summary>
    /// Lista identyfikatorów potworów, od których można zdobyć poszukiwany przedmiot.
    /// </summary>
    public List<string> ViableMonsters { get; set; }
    
    /// <summary>
    /// Identyfikator przedmiotu, który należy zebrać.
    /// </summary>
    public string ItemToCollect { get; set; }
    
    /// <summary>
    /// Wymagana liczba przedmiotów do zebrania.
    /// </summary>
    public int AmountToCollect { get; set; }
}