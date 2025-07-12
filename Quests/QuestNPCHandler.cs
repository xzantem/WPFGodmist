using GodmistWPF.Enums;

namespace GodmistWPF.Quests;

/// <summary>
/// Klasa pomocnicza do obsługi interakcji z NPC związanymi z zadaniami.
/// Zapewnia metody do zarządzania dostępnymi i gotowymi do oddania zadaniami.
/// </summary>
public static class QuestNPCHandler
{
    /// <summary>
    /// Pobiera listę zadań dostępnych do podjęcia u danego NPC.
    /// Uwzględnia wymagania wstępne zadań.
    /// </summary>
    /// <param name="questGiver">Identyfikator NPC, u którego sprawdzamy dostępne zadania.</param>
    /// <returns>Lista zadań dostępnych do podjęcia.</returns>
    public static List<Quest> GetAvailableQuests(string questGiver)
    {
        var quests = new List<Quest>();
        foreach (var quest in QuestManager.Quests
                     .Where(quest => quest.QuestGiver == questGiver && quest.QuestState == QuestState.Available))
        {
            if (quest.Prerequisites.Count == 0 && quest.QuestState == QuestState.Available)
            {
                quests.Add(quest);
                continue;
            }
            quests.AddRange(QuestManager.Quests.Where(prerequisite => quest.Prerequisites
                    .Contains(prerequisite.Alias))
                .TakeWhile(prerequisite => prerequisite.QuestState is QuestState.Completed or QuestState.HandedIn)
                .Select(_ => quest));
        }
        return quests;
    }
    /// <summary>
    /// Pobiera listę zadań gotowych do oddania u danego NPC.
    /// </summary>
    /// <param name="questEnder">Identyfikator NPC, u którego można oddać zadanie.</param>
    /// <returns>Lista zadań gotowych do oddania.</returns>
    public static List<Quest> GetReturnableQuests(string questEnder)
    {
        return QuestManager.Quests
            .Where(quest => quest.QuestEnder == questEnder && quest.QuestState == QuestState.Completed).ToList();
    }

    /// <summary>
    /// Wywołuje interfejs użytkownika do wyboru zadania do przyjęcia.
    /// W wersji WPF obsługa odbywa się po stronie interfejsu użytkownika.
    /// </summary>
    /// <param name="questGiver">Identyfikator NPC, u którego można przyjąć zadanie.</param>
    public static void SelectQuestToAccept(string questGiver)
    {
        // WPF handles quest acceptance UI
    }
    /// <summary>
    /// Wywołuje interfejs użytkownika do oddania ukończonego zadania.
    /// W wersji WPF obsługa odbywa się po stronie interfejsu użytkownika.
    /// </summary>
    /// <param name="questEnder">Identyfikator NPC, u którego można oddać zadanie.</param>
    public static void SelectQuestToReturn(string questEnder)
    {
        // WPF handles quest return UI
    }
}