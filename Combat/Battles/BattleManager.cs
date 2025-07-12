using System.Diagnostics;
using System.Windows;
using GodmistWPF.Characters;
using GodmistWPF.Characters.Player;
using GodmistWPF.Dialogs;
using GodmistWPF.Dungeons;
using GodmistWPF.Enums;
using GodmistWPF.Items;
using GodmistWPF.Items.Lootbags;
using GodmistWPF.Quests;
using GodmistWPF.Quests.Objectives;

namespace GodmistWPF.Combat.Battles;

/// <summary>
/// Statyczna klasa zarządzająca przebiegiem walk w grze.
/// </summary>
/// <remarks>
/// Odpowiada za inicjalizację walk, przetwarzanie turek,
/// obsługę nagród i komunikację z interfejsem użytkownika.
/// </remarks>
public static class BattleManager
{
    /// <summary>
    /// Pobiera aktualnie toczoną walkę.
    /// </summary>
    /// <value>
    /// Bieżąca walka lub <c>null</c> jeśli żadna walka nie jest aktywna.
    /// </value>
    public static Battle? CurrentBattle { get; private set; }
    
    /// <summary>
    /// Rozpoczyna nową walkę.
    /// </summary>
    /// <param name="battle">Obiekt walki do rozpoczęcia.</param>
    /// <remarks>
    /// Jeśli inna walka jest w toku, zostaje ona przerwana i wyczyszczona.
    /// Inicjalizuje zasoby postaci i rozpoczyna przetwarzanie turek.
    /// </remarks>
    public static void StartNewBattle(Battle battle)
    {
        if (battle == null) return;
        if (CurrentBattle != null)
        {
            var battleToCleanup = CurrentBattle;
            CurrentBattle = null;
            battleToCleanup?.Cleanup();
        }
        
        try
        {
            battle.Escaped = false;
            var initial = battle.Users.ToDictionary(c => c.Key, c => c.Value);
            
            // Initialize resources
            foreach (var user in initial.Keys)
            {
                if (user.User.ResourceType == ResourceType.Mana)
                    user.User.RegenResource((int)(user.User.MaximalResource - user.User.CurrentResource));
                else
                    user.User.UseResource((int)user.User.CurrentResource);
            }
            
            CurrentBattle = battle;

            // Start processing battle turns
            _ = ProcessBattleTurns(initial);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in StartNewBattle: {ex.Message}");
            // Ensure we clean up even if there's an error
            CurrentBattle = null;
        }
    }

    /// <summary>
    /// Przetwarza tury walki w pętli, aż do zakończenia walki.
    /// </summary>
    /// <param name="initialUsers">Początkowy słownik uczestników walki.</param>
    /// <returns>Zadanie reprezentujące asynchroniczną operację.</returns>
    private static async Task ProcessBattleTurns(Dictionary<BattleUser, int> initialUsers)
    {
        if (CurrentBattle == null) return;
        
        try
        {
            // Update UI to show battle start
            await UpdateBattleUI();
            
            while (CurrentBattle != null && CurrentBattle.CheckForResult() == -1)
            {
                await CurrentBattle.NewTurnAsync();
                await UpdateBattleUI();
                await Task.Delay(100);
            }

            // Handle battle result
            var result = CurrentBattle?.CheckForResult() ?? -1;
            if (result == 2) return;

            bool battleResult = result == 0; // Victory (0) or Defeat (1)

            if (!battleResult)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => 
                {
                    var dialog = Application.Current.Windows.OfType<BattleDialog>().FirstOrDefault();
                    if (dialog != null)
                    {
                        dialog.EndBattle(result);
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                });
                return;
            }
            
            // Handle victory
            await Application.Current.Dispatcher.InvokeAsync(() => 
            {
                var dialog = Application.Current.Windows.OfType<BattleDialog>().FirstOrDefault();
                if (dialog != null)
                {
                    dialog.EndBattle(result);
                    var rewards = GenerateReward(initialUsers);
                    dialog.ShowRewards(rewards.GoldReward, rewards.ExperienceReward, rewards.HonorReward, rewards.ItemsReward.ToList());
                }
            });

            // Update quests
            foreach (var user in initialUsers.Where(u => u is { Value: 1, Key.User.CurrentHealth: <= 0 }))
            {
                QuestManager.CheckForProgress(
                    new QuestObjectiveContext(
                        (user.Key.User as EnemyCharacter)?.Alias, 
                        user.Key.User.Level));
                QuestManager.CheckForProgress(
                    new QuestObjectiveContext(
                        DungeonMovementManager.CurrentDungeon.DungeonType, 
                        DungeonMovementManager.CurrentDungeon.DungeonType, 
                        user.Key.User.Level));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in ProcessBattleTurns: {ex.Message}");
            // Ensure we still end the battle on error
            await Application.Current.Dispatcher.InvokeAsync(() => 
            {
                var dialog = Application.Current.Windows.OfType<BattleDialog>().FirstOrDefault();
                dialog?.EndBattle(-1);
            });
        }
    }
    
    /// <summary>
    /// Aktualizuje interfejs użytkownika walki.
    /// </summary>
    /// <returns>Zadanie reprezentujące asynchroniczną operację.</returns>
    private static async Task UpdateBattleUI()
    {
        await Application.Current.Dispatcher.InvokeAsync(() => 
        {
            var dialog = Application.Current.Windows.OfType<BattleDialog>().FirstOrDefault();
            if (dialog != null)
            {
                dialog.UpdateBattleDisplay();
                if (CurrentBattle != null)
                {
                    dialog.TurnCounter.Text = $" - Turn {CurrentBattle.TurnCount}";
                }
            }
        });
    }

    /// <summary>
    /// Generuje nagrody za pokonanie przeciwników w walce.
    /// </summary>
    /// <param name="usersTeams">Słownik uczestników walki z ich przynależnością do drużyn.</param>
    /// <returns>Obiekt zawierający wygenerowane nagrody.</returns>
    /// <remarks>
    /// Nagrody obejmują złoto, doświadczenie, honor i przedmioty.
    /// Szanse na otrzymanie przedmiotów zależą od poziomu lochu i przeciwników.
    /// </remarks>
    public static BattleReward GenerateReward(Dictionary<BattleUser, int> usersTeams)
    {
        var player = usersTeams.ElementAt(0).Key.User as PlayerCharacter;
        var dungeon = DungeonMovementManager.CurrentDungeon;
        var moneyReward = 0;
        var honorReward = 0;
        var experienceReward = 0;
        var itemReward = new Dictionary<IItem, int>();
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
            {
                var lootbag = LootbagManager.GetSupplyBag(dungeon.DungeonType, enemy.Level);
                player.Inventory.AddItem(lootbag);
                itemReward.Add(lootbag, 1);
            }

            if (Random.Shared.NextDouble() < weaponBagChance)
            {
                var lootbag = LootbagManager.GetLootbag("WeaponBag", enemy.Level);
                player.Inventory.AddItem(lootbag);
                itemReward.Add(lootbag, 1);
            }
                
            if (Random.Shared.NextDouble() < armorBagChance)
            {
                var lootbag = LootbagManager.GetLootbag("ArmorBag", enemy.Level);
                player.Inventory.AddItem(lootbag);
                itemReward.Add(lootbag, 1);
            }

            if (Random.Shared.NextDouble() < galduriteBagChance && enemy.Level > 10)
            {
                var lootbag = LootbagManager.GetLootbag("GalduriteBag", enemy.Level);
                player.Inventory.AddItem(lootbag);
                itemReward.Add(lootbag, 1);
            }

            if (enemy.EnemyType.Contains(EnemyType.Boss))
            {
                var lootbag = LootbagManager.GetLootbag(enemy.Alias + "Bag", enemy.Level);
                player.Inventory.AddItem(lootbag);
                itemReward.Add(lootbag, 1);
            }

            foreach (var item in enemy.DropTable?.GetDrops(enemy.Level))
            {
                player.Inventory.AddItem(item.Key, item.Value);
                itemReward.Add(item.Key, item.Value);
            }
        }
        player!.GainGold(moneyReward);
        player.GainHonor(honorReward);
        player.GainExperience(experienceReward);
        return new BattleReward(moneyReward, honorReward, experienceReward, itemReward);
    }

    public static bool IsInBattle()
    {
        if (CurrentBattle == null) return false;
        return CurrentBattle.CheckForResult() != -1;
    }
}
