using System.Windows;
using GodmistWPF.Dialogs;
using GodmistWPF.Dungeons.Interactables;
using GodmistWPF.Items;
using GodmistWPF.Utilities;
using Battle = GodmistWPF.Combat.Battles.Battle;
using BattleManager = GodmistWPF.Combat.Battles.BattleManager;
using BattleUser = GodmistWPF.Combat.Battles.BattleUser;
using DungeonFieldType = GodmistWPF.Enums.Dungeons.DungeonFieldType;
using EnemyFactory = GodmistWPF.Characters.EnemyFactory;
using ModifierType = GodmistWPF.Enums.Modifiers.ModifierType;
using PlantDropManager = GodmistWPF.Dungeons.Interactables.PlantDropManager;
using PlayerHandler = GodmistWPF.Characters.Player.PlayerHandler;
using Stash = GodmistWPF.Dungeons.Interactables.Stash;
using StatModifier = GodmistWPF.Combat.Modifiers.StatModifier;
using StatType = GodmistWPF.Enums.Modifiers.StatType;

namespace GodmistWPF.Dungeons;

/// <summary>
/// Klasa zarządzająca ruchem gracza po lochach, obsługująca nawigację między pokojami i piętrami.
/// </summary>
public static class DungeonMovementManager
{
    /// <summary>
    /// Pobiera aktualnie eksplorowany loch.
    /// </summary>
    internal static Dungeon? CurrentDungeon { get; private set; }
    /// <summary>
    /// Pobiera aktualną lokalizację gracza w lochu.
    /// </summary>
    internal static DungeonRoom CurrentLocation { get; private set; } = null!;
    /// <summary>
    /// Pobiera indeks aktualnej lokalizacji w korytarzu.
    /// </summary>
    internal static int LocationIndex { get; private set; }
    /// <summary>
    /// Pobiera lub ustawia czas ostatniego ruchu.
    /// </summary>
    private static int LastMovement { get; set; }
    /// <summary>
    /// Pobiera informację, czy gracz opuścił loch.
    /// </summary>
    internal static bool Exited { get; private set; }

    /// <summary>
    /// Wprowadza gracza do lochu i ustawia go w pokoju startowym.
    /// </summary>
    /// <param name="dungeon">Loch, do którego wchodzi gracz.</param>
    internal static void EnterDungeon(Dungeon dungeon)
    {
        CurrentDungeon = dungeon;
        CurrentLocation = dungeon.CurrentFloor.StarterRoom;
        LocationIndex = 0;
        LastMovement = 0;
        Exited = false;
        CurrentLocation.Reveal();
        OnMove();
    }
    
    /// <summary>
    /// Wychodzi z aktualnego lochu i resetuje stan menedżera ruchu.
    /// </summary>
    public static void ExitDungeon()
    {
        Exited = true;
        CurrentDungeon = null;
    }

    /// <summary>
    /// Przesuwa gracza do przodu w lochu - do następnego pokoju lub na niższe piętro.
    /// </summary>
    /// <exception cref="InvalidOperationException">Wyrzucany, gdy nie można wykonać ruchu.</exception>
    public static void MoveForward()
    {
        try
        {
            if (CurrentDungeon == null)
                throw new InvalidOperationException("No active dungeon. Please enter a dungeon first.");
                
            if (CurrentDungeon.CurrentFloor == null)
                throw new InvalidOperationException("Current floor is not initialized.");
                
            if (LocationIndex < CurrentDungeon.CurrentFloor.Corridor.Count + 1)
            { // Go Forward if not on last block
                LocationIndex++;
                if (CurrentDungeon.CurrentFloor.Corridor == null)
                    throw new InvalidOperationException("Corridor is not initialized for the current floor.");
                    
                CurrentLocation = LocationIndex == CurrentDungeon.CurrentFloor.Corridor.Count + 1 ? 
                    CurrentDungeon.CurrentFloor.EndRoom : CurrentDungeon.CurrentFloor.Corridor[LocationIndex - 1];
                    
                if (CurrentLocation == null)
                    throw new InvalidOperationException("Failed to determine next location.");
                    
                if (!CurrentLocation.Revealed)
                {
                    CurrentLocation.Reveal();
                    if (Random.Shared.NextDouble() < 0.125)
                        CurrentDungeon.ScoutFloor(CurrentDungeon.CurrentFloor);
                }
            }
            else
            { // Go Down if on last block
                CurrentDungeon.Descend();
                if (CurrentDungeon.CurrentFloor == null)
                    throw new InvalidOperationException("Failed to descend to next floor.");
                    
                CurrentLocation = CurrentDungeon.CurrentFloor.StarterRoom;
                LocationIndex = 0;
                if (CurrentLocation == null)
                    throw new InvalidOperationException("Starter room is not initialized for the current floor.");
                    
                CurrentLocation.Reveal();
            }
            OnMove();
        }
        catch (Exception ex)
        {
            // Log the error details for debugging
            System.Diagnostics.Debug.WriteLine($"Error in MoveForward: {ex}");
            throw; // Re-throw to allow UI to handle the error
        }
    }
    /// <summary>
    /// Przesuwa gracza do tyłu w lochu - do poprzedniego pokoju lub na wyższe piętro.
    /// </summary>
    /// <exception cref="InvalidOperationException">Wyrzucany, gdy nie można wykonać ruchu.</exception>
    public static void MoveBackwards()
    {
        try
        {
            if (CurrentDungeon == null)
                throw new InvalidOperationException("No active dungeon. Please enter a dungeon first.");
                
            if (CurrentDungeon.CurrentFloor == null)
                throw new InvalidOperationException("Current floor is not initialized.");

            if (LocationIndex == 0)
            { // Move Up if on first block
                if (CurrentDungeon.Floors.IndexOf(CurrentDungeon.CurrentFloor) == 0)
                {
                    ExitDungeon();
                    return;
                }
                
                CurrentDungeon.Ascend();
                if (CurrentDungeon.CurrentFloor == null)
                    throw new InvalidOperationException("Failed to ascend to previous floor.");
                    
                CurrentLocation = CurrentDungeon.CurrentFloor.EndRoom;
                if (CurrentLocation == null)
                    throw new InvalidOperationException("End room is not initialized for the current floor.");
                    
                LocationIndex = CurrentDungeon.CurrentFloor.Corridor?.Count + 1 ?? 0;
            }
            else
            { // Move Back if not on first block
                LocationIndex--;
                if (LocationIndex == 0)
                {
                    CurrentLocation = CurrentDungeon.CurrentFloor.StarterRoom;
                }
                else
                {
                    if (CurrentDungeon.CurrentFloor.Corridor == null || LocationIndex - 1 >= CurrentDungeon.CurrentFloor.Corridor.Count)
                        throw new InvalidOperationException("Invalid corridor index when moving backwards.");
                        
                    CurrentLocation = CurrentDungeon.CurrentFloor.Corridor[LocationIndex - 1];
                }
                
                if (CurrentLocation == null)
                    throw new InvalidOperationException("Failed to determine previous location.");
            }
            OnMove();
        }
        catch (Exception ex)
        {
            // Log the error details for debugging
            System.Diagnostics.Debug.WriteLine($"Error in MoveBackwards: {ex}");
            throw; // Re-throw to allow UI to handle the error
        }
    }

    /// <summary>
    /// Wywoływana po każdym ruchu gracza, obsługuje zdarzenia związane z nową lokalizacją.
    /// </summary>
    private static async void OnMove()
    {
        try
        {
            PlayerHandler.player.HandleModifiers(); 
            PlayerHandler.player.PassiveEffects.TickEffects();
            if (PlayerHandler.player.CurrentHealth <= 0)
            {
                // WPF handles player death display
                Environment.Exit(0);
            }

            switch (CurrentLocation.FieldType)
            {
                case DungeonFieldType.Battle:
                    var battle = CurrentDungeon?.CurrentFloor?.Battles?.FirstOrDefault(x => x.Location == CurrentLocation);
                    if (battle != null)
                    {
                        // Start the battle first
                        BattleManager.StartNewBattle(battle);
                        
                        // Then show the dialog
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            var battleDialog = new BattleDialog(battle);
                            battleDialog.Closed += (s, e) =>
                            {
                                if (battle.Escaped)
                                    MoveBackwards();
                                else
                                {
                                    if (CurrentLocation != CurrentDungeon?.CurrentFloor?.StarterRoom)
                                        CurrentLocation.Clear();
                                    else
                                        CurrentDungeon.CurrentFloor.Battles.Remove(battle);
                                }
                            };
                            battleDialog.ShowDialog();
                        });
                    }
                    break;

                case DungeonFieldType.Trap:
                    var trap = CurrentDungeon?.CurrentFloor?.Traps?.FirstOrDefault(x => x.Location == CurrentLocation);
                    if (trap != null)
                    {
                        // Show trap minigame dialog
                        DisarmTrap(trap, true);
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            // Log error and continue
            System.Diagnostics.Debug.WriteLine($"Error in OnMove: {ex}");
        }
    }

    public static bool DisarmTrap(Trap trap, bool isSteppedOn)
    {
        var success = false;
        Action showTrapDialog = () =>
        {
            var trapDialog = new TrapMinigameDialog(trap.Difficulty, (disarmed) =>
            {
                success = disarmed;
                if (!disarmed && isSteppedOn)
                    trap.Trigger();
                if (disarmed || isSteppedOn)
                {
                    CurrentDungeon?.CurrentFloor.Traps.Remove(trap);
                    trap.Location.Clear();
                }
                                
                // Update the dungeon display after trap is handled
                if (Application.Current.MainWindow is DungeonDialog dungeonDialog)
                {
                    dungeonDialog.UpdateDungeonDisplay();
                }
            });
            trapDialog.Owner = Application.Current.MainWindow;
            trapDialog.ShowDialog();
        };
                        
        // Explicitly specify the delegate type to resolve ambiguity
        Application.Current.Dispatcher.Invoke((Action)showTrapDialog);
        return success;
    }

    public static (IItem, int)? CollectPlant()
    {
        if (CurrentDungeon != null)
        {
            var plant = PlantDropManager.DropDatabase[CurrentDungeon.DungeonType]
                .GetDrop(CurrentDungeon.DungeonLevel);
            PlayerHandler.player.Inventory.AddItem(plant.Key, plant.Value);
            CurrentLocation.Clear();
            return (plant.Key, plant.Value);
        }

        return null;
    }

    public static Battle? RestAtBonfire()
    {
        PlayerHandler.player.Heal(PlayerHandler.player.MaximalHealth / 4);
        var ambushed = Random.Shared.NextDouble() < 0.2;
        CurrentLocation.Clear();
        if (ambushed)
        {
            PlayerHandler.player.AddModifier(StatType.Dodge, new StatModifier(ModifierType.Absolute, -20, locale.Ambush, 5));
            PlayerHandler.player.AddModifier(StatType.Accuracy, new StatModifier(ModifierType.Absolute, -10, locale.Ambush, 5));
            if (CurrentDungeon != null)
                return new Battle(new Dictionary<BattleUser, int>
                {
                    { new BattleUser(PlayerHandler.player), 0 },
                    {
                        new BattleUser(
                            EnemyFactory.CreateEnemy(CurrentDungeon.DungeonType, CurrentDungeon.DungeonLevel)),
                        1
                    }
                }, CurrentLocation, false);
        }
        return null;
    }

    public static Dictionary<IItem,int>? OpenStash()
    {
        if (CurrentDungeon != null)
        {
            var stash = new Stash(CurrentDungeon.DungeonLevel, CurrentDungeon.DungeonType);
            var items = new Dictionary<IItem, int>();
            foreach (var item in stash.GetDrops())
            {
                items.Add(item.Key, item.Value);
                PlayerHandler.player.Inventory.AddItem(item.Key, item.Value);
            }
            CurrentLocation.Clear();
            return items;
        }

        return null;
    }
}