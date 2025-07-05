using GodmistWPF.Towns.NPCs;
using GodmistWPF.Enums.Dungeons;

namespace GodmistWPF.Quests.Objectives;

public class QuestObjectiveContext
{
    public string KillTarget { get; private set; }
    public NPC? TalkTarget { get; private set; }
    public DungeonType? DescendTarget { get; private set; }
    public DungeonType? KillInDungeonTarget { get; private set; }
    public DungeonType? ActivateInDungeonTarget { get; private set; }
    public int DescendFloor { get; private set; }
    public int ContextLevel { get; private set; }

    public QuestObjectiveContext(string killTarget, int contextLevel)
    {
        KillTarget = killTarget;
        ContextLevel = contextLevel;
        TalkTarget = null;
        DescendFloor = 0;
    }
    
    public QuestObjectiveContext(NPC talkTarget)
    {
        KillTarget = "";
        ContextLevel = int.MaxValue;
        TalkTarget = talkTarget;
        DescendFloor = 0;
    }
    
    public QuestObjectiveContext(DungeonType descendTarget, int descendFloor, int contextLevel)
    {
        KillTarget = "";
        ContextLevel = contextLevel;
        TalkTarget = null;
        DescendTarget = descendTarget;
        DescendFloor = descendFloor;
    }
    public QuestObjectiveContext(DungeonType killInDungeonTarget, DungeonType activateInDungeonTarget, int contextLevel)
    {
        KillTarget = "";
        ContextLevel = contextLevel;
        TalkTarget = null;
        DescendFloor = 0;
        KillInDungeonTarget = killInDungeonTarget;
        ActivateInDungeonTarget = activateInDungeonTarget;
    }
}