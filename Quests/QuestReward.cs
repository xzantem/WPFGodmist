using GodmistWPF.Items;

namespace GodmistWPF.Quests;

public class QuestReward
{
    public int Gold { get; set; }
    public int Experience { get; set; }
    public int Honor { get; set; }
    public Dictionary<IItem, int> Items { get; set; }
    
    public QuestReward() {}

    public QuestReward(int gold, int experience, int honor, Dictionary<IItem, int> items)
    {
        Gold = gold;
        Experience = experience;
        Honor = honor;
        Items = items;
    }
}