using ConsoleGodmist;
using GodmistWPF.Characters;
using GodmistWPF.Characters.Player;
using GodmistWPF.Dungeons;
using GodmistWPF.Enums;
using GodmistWPF.Items.Lootbags;
using GodmistWPF.Quests;
using GodmistWPF.Quests.Objectives;

namespace GodmistWPF.Combat.Battles;

public static class BattleManager
{
    public static Battle? CurrentBattle { get; private set; }
    
    public static void StartNewBattle(Battle battle)
    {
        battle.Escaped = false;
        var initial = battle.Users
            .ToDictionary(c => c.Key, c => c.Value);
        foreach (var user in initial.Keys)
            if (user.User.ResourceType == ResourceType.Mana)
                user.User.RegenResource((int)(user.User.MaximalResource - user.User.CurrentResource));
            else
                user.User.UseResource((int)user.User.CurrentResource);
        CurrentBattle = battle;
        battle.Interface.AddBattleLogLines($"{locale.BattleStart}: " +
                                                    $"{(battle.Users.ElementAt(^1).Key.User as EnemyCharacter).Name}");
        battle.Interface.DisplayInterface(battle.Users.ElementAt(0).Key, battle.Users.Keys.ToList(), false);
        while(CurrentBattle.CheckForResult() == -1)
            CurrentBattle.NewTurn();
        var battleResult = CurrentBattle.CheckForResult() switch
        {
            0 => true,
            1 => false,
            _ => false
        };
        if (CurrentBattle.CheckForResult() == 2) return;
        CurrentBattle.Interface.AddBattleLogLines(battleResult
            ? $"{locale.Victory}!"
            : $"{locale.Defeat}! {locale.GameOver}!");
        battle.Interface.DisplayInterface(battle.Users.ElementAt(0).Key, battle.Users.Keys.ToList());
        Thread.Sleep(2000);
        // WPF handles battle result display
        if (!battleResult)
            Environment.Exit(0);
        GenerateReward(initial);
        foreach (var user in initial.Where(u => u is { Value: 1, Key.User.CurrentHealth: <= 0 }))
        {
            QuestManager.CheckForProgress(
                new QuestObjectiveContext((user.Key.User as EnemyCharacter)?.Alias, user.Key.User.Level));
            QuestManager.CheckForProgress(
                new QuestObjectiveContext(DungeonMovementManager.CurrentDungeon.DungeonType, 
                    DungeonMovementManager.CurrentDungeon.DungeonType, user.Key.User.Level));
        }
    }

    private static void GenerateReward(Dictionary<BattleUser, int> usersTeams)
    {
        var player = usersTeams.ElementAt(0).Key.User as PlayerCharacter;
        var dungeon = DungeonMovementManager.CurrentDungeon;
        var moneyReward = 0;
        var honorReward = 0;
        var experienceReward = 0;
        const double lootBagChance = 0.5;
        const double weaponBagChance = 0.125;
        const double armorBagChance = 0.125;
        const double galduriteBagChance = 0.1;
        
        foreach (var enemy in usersTeams.Where(x => x.Value == 1).Select(x => x.Key.User as EnemyCharacter))
        {
            moneyReward += (int)(Math.Pow(4, (dungeon.DungeonLevel - 1) * 0.1) * 
                Random.Shared.Next(15, 31) * 
                (10 * dungeon.Floors.IndexOf(dungeon.CurrentFloor) + 7)) / 
                           (dungeon.Floors.IndexOf(dungeon.CurrentFloor) + 16);
            if (enemy.Level >= usersTeams.Keys.Average(x => x.User.Level))
                honorReward++;
            experienceReward += (int)Math.Max(0.1, (1 - 0.15 * Math.Abs(player!.Level - dungeon.DungeonLevel)) 
                                                   * (Math.Pow(dungeon.DungeonLevel, 1.1) + 3));
            if (Random.Shared.NextDouble() < lootBagChance)
                player.Inventory.AddItem(LootbagManager.GetSupplyBag(dungeon.DungeonType, enemy.Level));
            if (Random.Shared.NextDouble() < weaponBagChance)
                player.Inventory.AddItem(LootbagManager.GetLootbag("WeaponBag", enemy.Level));
            if (Random.Shared.NextDouble() < armorBagChance)
                player.Inventory.AddItem(LootbagManager.GetLootbag("ArmorBag", enemy.Level));
            if (Random.Shared.NextDouble() < galduriteBagChance && enemy.Level > 10)
                player.Inventory.AddItem(LootbagManager.GetLootbag("GalduriteBag", enemy.Level));
            if (enemy.EnemyType.Contains(EnemyType.Boss))
                player.Inventory.AddItem(LootbagManager.GetLootbag(enemy.Alias + "Bag", enemy.Level));
            foreach (var item in enemy.DropTable?.GetDrops(enemy.Level))
                player.Inventory.AddItem(item.Key, item.Value);
        }
        player!.GainGold(moneyReward);
        player.GainHonor(honorReward);
        player.GainExperience(experienceReward);
    }

    public static bool IsInBattle()
    {
        if (CurrentBattle == null) return false;
        return CurrentBattle.CheckForResult() != -1;
    }
}
