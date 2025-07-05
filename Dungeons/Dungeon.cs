using ConsoleGodmist;
using DungeonType = GodmistWPF.Enums.Dungeons.DungeonType;
using QuestManager = GodmistWPF.Quests.QuestManager;
using QuestObjectiveContext = GodmistWPF.Quests.Objectives.QuestObjectiveContext;

namespace GodmistWPF.Dungeons
{
    // Dungeon Characteristics
    // Dungeons consist of m x n sized rooms and 3 x p or p x 3 sized corridors - implemented
    // They are procedurally generated and once you clear the first room and corridor, another one generates and "adjoins" to the duneon
    // One room may branch out to multiple corridors - implemented
    // Entering a dungeon lands you in the first room and the goal is to reach the corridor. Once in the corridor, you may return back to the room you were in, or proceed to the next room, up to infinity
    // To reach the corridor, sometimes all you need to do is walk up, but sometimes there is a lever you have to move to open the Gate, or a key you have to pick up in order to open the Door - both may require solving a puzzle
    // Dungeons are shrouded in fog of war initially, walking around the dungeon "explores" an area in a 3x3 radius around. You may also find a map which automatically reveals the whole floor (sometimes also hidden rooms!)
    // Random places in rooms may contain an enemy! When you approach an enemy in their 3x3 radius, a battle starts (kinda pokemonesque)
    // Corridors also may contain enemies, in truth they do contain them more commonly than rooms
    // You may also find rewards in rooms, such as stashes(coffins, trunk stashes, chests, etc.) or regional specialties and other alchemical ingredients
    // Exiting a dungeon is simple - simply walk back or use a town portal scroll from your inventory at any given time other than a battle

    // Different dungeon types have different enemies, alchemical ingredients, stash types, and special generation characteristics. These include:
    // Catacombs - rooms are mostly square and corridors very short (up to 3 x 3) - implemented
    // Forest - rooms are very long and thin (up to m x 3), visually indistinguishable from corridors
    // Elvish Ruins - enemies respawn every 20 moves through portals
    // Cove - more stashes exist, but also more levers/keys are required to pass
    // Desert - Corridors are only 1 tile long
    // Temple - traps exist more commonly, rooms are mostly square
    // Mountains - corridors are very long and rooms small (up to 8 x 8)
    // Swamp - same as forest, player speed is reduced by 30% while in the dungeon
    public class Dungeon {
        public DungeonType DungeonType { get; private set; }
        public int DungeonLevel { get; private set; }
        public List<DungeonFloor> Floors { get; private set; }
        
        public DungeonFloor CurrentFloor { get; private set;}

        public Dungeon(int level, DungeonType type)
        {
            DungeonLevel = level;
            DungeonType = type;
            Floors = new List<DungeonFloor>();
            AddNewFloor();
            CurrentFloor = Floors[0];
        }

        private void AddNewFloor()
        {
            var length = Random.Shared.Next(4, 9) * (1 + (Floors.Count - 1) * 0.1);
            length *= DungeonType switch
            {
                DungeonType.Catacombs => 0.6,
                DungeonType.Forest => 0.9,
                DungeonType.ElvishRuins => 0.6,
                DungeonType.Cove => 0.7,
                DungeonType.Desert => 0.3,
                DungeonType.Temple => 0.5,
                DungeonType.Mountains => 1.1,
                DungeonType.Swamp => 0.9,
                _ => throw new ArgumentOutOfRangeException()
            };
            length = Math.Clamp(length, 1, 16);
            Floors.Add(new DungeonFloor((int)length, Floors.Count, DungeonType, DungeonLevel));
            QuestManager.CheckForProgress(new QuestObjectiveContext(DungeonType, Floors.Count - 1, DungeonLevel));
        }
        public void Ascend()
        {
            if (Floors.IndexOf(CurrentFloor) > 0)
                CurrentFloor = Floors[Floors.IndexOf(CurrentFloor) - 1];
        }
        public void Descend()
        {
            if (Floors.IndexOf(CurrentFloor) == Floors.Count - 1)
            {
                AddNewFloor();
                if (Random.Shared.NextDouble() < 0.25)
                    ScoutFloor(Floors[Floors.IndexOf(CurrentFloor) + 1]);
            }
            CurrentFloor = Floors[Floors.IndexOf(CurrentFloor) + 1];
        }
        public void ScoutFloor(DungeonFloor floor)
        {
            Console.WriteLine(locale.LocationScouted + "\n");
            floor.StarterRoom.Reveal();
            floor.EndRoom.Reveal();
            foreach (var corridor in floor.Corridor)
                corridor.Reveal();
        }
        
    }
}
