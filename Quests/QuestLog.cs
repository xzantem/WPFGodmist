using QuestState = GodmistWPF.Enums.QuestState;

namespace GodmistWPF.Quests;

public static class QuestLog
{
    private static readonly List<QuestState> Filters = [QuestState.Accepted, QuestState.Completed, QuestState.HandedIn];
    public static void InspectQuest(Quest quest)
    {
        // WPF handles quest inspection UI
    }

    public static void OpenLog()
    {
        // WPF handles quest log UI
    }

    private static int ChooseQuest(List<Quest> selection)
    {
        // WPF handles quest selection UI
        return -1;
    }

    private static void ChangeFilters()
    {
        // WPF handles filter change UI
    }
}