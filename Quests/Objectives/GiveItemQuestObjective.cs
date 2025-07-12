using GodmistWPF.Towns.NPCs;

namespace GodmistWPF.Quests.Objectives;

/// <summary>
/// Cel zadania polegający na oddaniu określonego przedmiotu wskazanemu NPC.
/// </summary>
public class GiveItemQuestObjective : IQuestObjective
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
    /// <param name="context">Kontekst zawierający informacje o oddanym przedmiocie.</param>
    /// <exception cref="NotImplementedException">Metoda nie jest jeszcze zaimplementowana.</exception>
    public void Progress(QuestObjectiveContext context)
    {
        throw new NotImplementedException();
    }

    
    /// <summary>
    /// Identyfikator przedmiotu, który należy oddać.
    /// </summary>
    public string ItemToGive { get; set; }
    
    /// <summary>
    /// NPC, któremu należy oddać przedmiot.
    /// </summary>
    public NPC NPCToGive { get; set; }
    
    /// <summary>
    /// Wymagana liczba przedmiotów do oddania.
    /// </summary>
    public int QuantityToGive { get; set; }
}