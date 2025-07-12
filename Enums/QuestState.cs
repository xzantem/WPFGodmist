namespace GodmistWPF.Enums;

/// <summary>
/// Określa aktualny stan zadania w systemie questów.
/// </summary>
public enum QuestState
{
    /// <summary>
    /// Zadanie jest dostępne do podjęcia, ale jeszcze niezaakceptowane przez gracza.
    /// </summary>
    Available,
    
    /// <summary>
    /// Zadanie zostało zaakceptowane przez gracza i jest w trakcie wykonywania.
    /// </summary>
    Accepted,
    
    /// <summary>
    /// Zadanie zostało ukończone, ale nagroda nie została jeszcze odebrana.
    /// </summary>
    Completed,
    
    /// <summary>
    /// Zadanie zostało ukończone i nagroda została odebrana.
    /// </summary>
    HandedIn
}