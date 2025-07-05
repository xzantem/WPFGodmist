

using GodmistWPF.Characters.Player;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Dungeons;
using GodmistWPF.Quests;
using GodmistWPF.Towns;

namespace GodmistWPF.Utilities.DataPersistance;

public class SaveData
{
    public PlayerCharacter Player { get; set; }
    public Difficulty Difficulty { get; set; }
    public List<Quest>[] Quests { get; set; }
    public Dictionary<DungeonType,int> BossQuestProgress { get; set; }
    public Town Town { get; set; }
    
    
    public SaveData() {}

    public SaveData(PlayerCharacter player, Difficulty difficulty, List<Quest>[] quests, 
        Dictionary<DungeonType,int> bossQuestProgress, Town town)
    {
        Player = player;
        Difficulty = difficulty;
        Quests = quests;
        BossQuestProgress = bossQuestProgress;
        Town = town;
    }
}