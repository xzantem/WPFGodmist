using GodmistWPF.Towns.NPCs;
using GodmistWPF.Enums.Dungeons;

namespace GodmistWPF.Quests.Objectives;

/// <summary>
/// Klasa przechowująca kontekst akcji, która może wpłynąć na postęp celu zadania.
/// Zawiera informacje o wykonanej akcji, takie jak zabity przeciwnik, rozmówca lub odwiedzony loch.
/// </summary>
public class QuestObjectiveContext
{
    /// <summary>
    /// Identyfikator zabitego przeciwnika, jeśli dotyczy.
    /// </summary>
    public string KillTarget { get; private set; }
    
    /// <summary>
    /// Cel rozmowy, jeśli dotyczy.
    /// </summary>
    public NPC? TalkTarget { get; private set; }
    
    /// <summary>
    /// Typ lochu, do którego się zeszło, jeśli dotyczy.
    /// </summary>
    public DungeonType? DescendTarget { get; private set; }
    
    /// <summary>
    /// Typ lochu, w którym należy pokonać przeciwników, jeśli dotyczy.
    /// </summary>
    public DungeonType? KillInDungeonTarget { get; private set; }
    
    /// <summary>
    /// Typ lochu, w którym należy aktywować obiekt, jeśli dotyczy.
    /// </summary>
    public DungeonType? ActivateInDungeonTarget { get; private set; }
    
    /// <summary>
    /// Poziom, na który się zeszło w lochu, jeśli dotyczy.
    /// </summary>
    public int DescendFloor { get; private set; }
    
    /// <summary>
    /// Poziom kontekstu, używany do określenia ważności lub skali akcji.
    /// </summary>
    public int ContextLevel { get; private set; }

    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="QuestObjectiveContext"/> dla zabicia przeciwnika.
    /// </summary>
    /// <param name="killTarget">Identyfikator zabitego przeciwnika.</param>
    /// <param name="contextLevel">Poziom kontekstu.</param>
    public QuestObjectiveContext(string killTarget, int contextLevel)
    {
        KillTarget = killTarget;
        ContextLevel = contextLevel;
        TalkTarget = null;
        DescendFloor = 0;
    }
    
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="QuestObjectiveContext"/> dla rozmowy z NPC.
    /// </summary>
    /// <param name="talkTarget">NPC, z którym przeprowadzono rozmowę.</param>
    public QuestObjectiveContext(NPC talkTarget)
    {
        KillTarget = "";
        ContextLevel = int.MaxValue;
        TalkTarget = talkTarget;
        DescendFloor = 0;
    }
    
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="QuestObjectiveContext"/> dla zejścia do lochu.
    /// </summary>
    /// <param name="descendTarget">Typ lochu, do którego się zeszło.</param>
    /// <param name="descendFloor">Poziom, na który się zeszło.</param>
    /// <param name="contextLevel">Poziom kontekstu.</param>
    public QuestObjectiveContext(DungeonType descendTarget, int descendFloor, int contextLevel)
    {
        KillTarget = "";
        ContextLevel = contextLevel;
        TalkTarget = null;
        DescendTarget = descendTarget;
        DescendFloor = descendFloor;
    }
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="QuestObjectiveContext"/> dla akcji w lochu.
    /// </summary>
    /// <param name="killInDungeonTarget">Typ lochu, w którym należy pokonać przeciwników.</param>
    /// <param name="activateInDungeonTarget">Typ lochu, w którym należy aktywować obiekt.</param>
    /// <param name="contextLevel">Poziom kontekstu.</param>
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