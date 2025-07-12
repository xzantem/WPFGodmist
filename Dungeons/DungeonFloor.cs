using GodmistWPF.Characters;
using GodmistWPF.Characters.Player;
using GodmistWPF.Combat.Battles;
using GodmistWPF.Dungeons.Interactables;
using GodmistWPF.Enums.Dungeons;
using GodmistWPF.Utilities;

namespace GodmistWPF.Dungeons;

/// <summary>
/// Klasa reprezentująca pojedyncze piętro lochu, zawierające pokoje i korytarze.
/// </summary>
public class DungeonFloor
{
    /// <summary>
    /// Pobiera pokój startowy na piętrze.
    /// </summary>
    public DungeonRoom StarterRoom { get; private set; }
    /// <summary>
    /// Pobiera pokój końcowy na piętrze.
    /// </summary>
    public DungeonRoom EndRoom { get; private set; }
    /// <summary>
    /// Pobiera listę korytarzy na piętrze.
    /// </summary>
    /// <summary>
    /// Generuje korytarze łączące pokoje na piętrze.
    /// </summary>
    public List<DungeonCorridor> Corridor { get; private set; }
    
    /// <summary>
    /// Pobiera numer piętra w lochu.
    /// </summary>
    public int FloorNumber { get; private set; }
    public List<Trap> Traps { get; private set; }
    public List<Battle> Battles { get; private set; }
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DungeonFloor"/> o określonych parametrach.
    /// </summary>
    /// <param name="length">Długość piętra (ilość pokoi).</param>
    /// <param name="difficulty">Poziom trudności piętra.</param>
    /// <param name="dungeonType">Typ lochu.</param>
    /// <param name="level">Poziom lochu.</param>
    public DungeonFloor(int length, int difficulty, DungeonType dungeonType, int level)
    {
        Traps = [];
        Battles = [];
        var starterRoomField = UtilityMethods.RandomChoice(new Dictionary<DungeonFieldType, int> { { DungeonFieldType.Empty, 15 }, { DungeonFieldType.Battle, 9 + 3 * difficulty } });
        StarterRoom = new DungeonRoom(starterRoomField);
        if (starterRoomField == DungeonFieldType.Battle)
            Battles.Add(new Battle(new Dictionary<BattleUser, int>
            {
                {new BattleUser(PlayerHandler.player), 0}, 
                {new BattleUser(EnemyFactory.CreateEnemy(dungeonType, level)), 1}
            }, StarterRoom));
        EndRoom = new DungeonRoom(DungeonFieldType.Empty);
        Corridor = new List<DungeonCorridor>();
        Dictionary<DungeonFieldType, int> weights = new()
        {
            {DungeonFieldType.Empty, 16},
            {DungeonFieldType.Plant, 8},
            {DungeonFieldType.Battle, 2 * difficulty},
            {DungeonFieldType.Bonfire, 8},
            {DungeonFieldType.Trap, 1 + 2 * difficulty},
            {DungeonFieldType.Stash, 1}
        };
        for (var i = 0; i < length; i++)
        {
            var fieldType = UtilityMethods.RandomChoice(weights);
            Corridor.Add(new DungeonCorridor(fieldType));
            switch (fieldType)
            {
                case DungeonFieldType.Trap:
                    Traps.Add(new Trap(GameSettings.Difficulty, Corridor[^1], dungeonType));
                    break;
                case DungeonFieldType.Battle:
                    Battles.Add(new Battle(new Dictionary<BattleUser, int>
                    {
                        {new BattleUser(PlayerHandler.player), 0}, 
                        {new BattleUser(EnemyFactory.CreateEnemy(dungeonType, level)), 1}
                    }, Corridor[^1]));
                    break;
            }
        }
    }
}