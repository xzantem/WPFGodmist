

namespace GodmistWPF.Quests.Objectives;

/// <summary>
/// Interfejs definiujący zachowanie celu zadania.
/// Każdy cel zadania musi implementować metody do śledzenia postępu i sprawdzania ukończenia.
/// </summary>
public interface IQuestObjective
{
    /// <summary>
    /// Określa, czy cel został ukończony.
    /// </summary>
    public bool IsComplete { get; set; }
    
    /// <summary>
    /// Pobiera opis celu zadania, który jest wyświetlany graczowi.
    /// </summary>
    public string Description { get; }
    
    /// <summary>
    /// Aktualizuje postęp celu na podstawie dostarczonego kontekstu.
    /// </summary>
    /// <param name="context">Kontekst zawierający informacje o wykonanej akcji.</param>
    public void Progress(QuestObjectiveContext context);
}