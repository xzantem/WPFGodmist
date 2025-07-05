

using ConsoleGodmist;
using ConsoleGodmist.TextService;
using GodmistWPF.Characters.Player;
using GodmistWPF.Dungeons;
using GodmistWPF.Enums.Dungeons;
using GodmistWPF.Quests;
using GodmistWPF.Towns.NPCs;
using GodmistWPF.Utilities;
using GodmistWPF.Utilities.DataPersistance;

namespace GodmistWPF.Towns
{
    public class Town
    {
        public List<NPC> NPCs { get; set; }
        public string TownName { get; set; }

        public Town(string name)
        {
            NPCs = new List<NPC>
            {
                new Alchemist("Alchemist"),
                new Blacksmith("Blacksmith"),
                new Enchanter("Enchanter")
            };
            TownName = name;
        }
        public Town() {}
        private Dungeon ChooseDungeon() {
            // WPF handles all UI, so all AnsiConsole calls are removed
            string[] dungeonChoices = {
                locale.Catacombs + $" ({locale.BossProgress}: {QuestManager.BossProgress[DungeonType.Catacombs] % QuestManager.ProgressTarget}/{QuestManager.ProgressTarget})", 
                locale.Forest + $" ({locale.BossProgress}: {QuestManager.BossProgress[DungeonType.Forest] % QuestManager.ProgressTarget}/{QuestManager.ProgressTarget})", 
                locale.ElvishRuins + $" ({locale.BossProgress}: {QuestManager.BossProgress[DungeonType.ElvishRuins] % QuestManager.ProgressTarget}/{QuestManager.ProgressTarget})", 
                locale.Cove + $" ({locale.BossProgress}: {QuestManager.BossProgress[DungeonType.Cove] % QuestManager.ProgressTarget}/{QuestManager.ProgressTarget})",
                locale.Desert + $" ({locale.BossProgress}: {QuestManager.BossProgress[DungeonType.Desert] % QuestManager.ProgressTarget}/{QuestManager.ProgressTarget})", 
                locale.Temple + $" ({locale.BossProgress}: {QuestManager.BossProgress[DungeonType.Temple] % QuestManager.ProgressTarget}/{QuestManager.ProgressTarget})", 
                locale.Mountains + $" ({locale.BossProgress}: {QuestManager.BossProgress[DungeonType.Mountains] % QuestManager.ProgressTarget}/{QuestManager.ProgressTarget})", 
                locale.Swamp + $" ({locale.BossProgress}: {QuestManager.BossProgress[DungeonType.Swamp] % QuestManager.ProgressTarget}/{QuestManager.ProgressTarget})"
            };
            
            // WPF handles dungeon selection UI
            var dungeonChoice = dungeonChoices[0]; // Default to first choice
            var dungeonType = Array.IndexOf(dungeonChoices, dungeonChoice) switch
            {
                0 => DungeonType.Catacombs,
                1 => DungeonType.Forest,
                2 => DungeonType.ElvishRuins,
                3 => DungeonType.Cove,
                4 => DungeonType.Desert,
                5 => DungeonType.Temple,
                6 => DungeonType.Mountains,
                7 => DungeonType.Swamp,
                _ => DungeonType.Catacombs,
            };
            
            // WPF handles level selection UI
            var level = PlayerHandler.player.Level; // Default to player level
            return new Dungeon(level, dungeonType);
        }
    }
}
