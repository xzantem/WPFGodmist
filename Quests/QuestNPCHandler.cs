using GodmistWPF.Enums;

namespace GodmistWPF.Quests;

public static class QuestNPCHandler
{
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
    public static List<Quest> GetReturnableQuests(string questEnder)
    {
        return QuestManager.Quests
            .Where(quest => quest.QuestEnder == questEnder && quest.QuestState == QuestState.Completed).ToList();
    }

    public static void SelectQuestToAccept(string questGiver)
    {
        // WPF handles quest acceptance UI
    }
    public static void SelectQuestToReturn(string questEnder)
    {
        // WPF handles quest return UI
    }
}