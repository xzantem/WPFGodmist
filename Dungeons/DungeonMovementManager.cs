using ConsoleGodmist;
using ConsoleGodmist.TextService;
using GodmistWPF.Utilities;
using Battle = GodmistWPF.Combat.Battles.Battle;
using BattleManager = GodmistWPF.Combat.Battles.BattleManager;
using BattleUser = GodmistWPF.Combat.Battles.BattleUser;
using DungeonFieldType = GodmistWPF.Enums.Dungeons.DungeonFieldType;
using DungeonTextService = GodmistWPF.Text.DungeonTextService;
using EnemyFactory = GodmistWPF.Characters.EnemyFactory;
using ModifierType = GodmistWPF.Enums.Modifiers.ModifierType;
using PlantDropManager = GodmistWPF.Dungeons.Interactables.PlantDropManager;
using PlayerHandler = GodmistWPF.Characters.Player.PlayerHandler;
using QuestLog = GodmistWPF.Quests.QuestLog;
using Stash = GodmistWPF.Dungeons.Interactables.Stash;
using StatModifier = GodmistWPF.Combat.Modifiers.StatModifier;
using StatType = GodmistWPF.Enums.Modifiers.StatType;

namespace GodmistWPF.Dungeons;

public static class DungeonMovementManager
{
    public static Dungeon? CurrentDungeon { get; private set; }
    private static DungeonRoom CurrentLocation { get; set; } = null!;
    private static int LocationIndex { get; set; }
    private static int LastMovement { get; set; }
    private static bool Exited { get; set; }

    public static void EnterDungeon(Dungeon dungeon)
    {
        CurrentDungeon = dungeon;
        CurrentLocation = dungeon.CurrentFloor.StarterRoom;
        LocationIndex = 0;
        LastMovement = 0;
        Exited = false;
        CurrentLocation.Reveal();
        OnMove();
    }

    public static void TraverseDungeon()
    {
        while (!Exited)
        {
            var action = ChooseAction();
            switch (action)
            {
                case 0:
                    MoveForward();
                    break;
                case 1:
                    MoveBackwards();
                    break;
                case 2:
                    //InventoryMenuHandler.OpenInventoryMenu();
                    break;
                case 3:
                    QuestLog.OpenLog();
                    break;
                case 4:
                    CharacterEventTextService.DisplayCharacterMenuText(PlayerHandler.player);
                    break;
                case 5:
                    PlayerHandler.player.Heal(PlayerHandler.player.MaximalHealth / 4);
                    var ambushed = Random.Shared.NextDouble() < 0.2;
                    if (ambushed)
                    {
                        PlayerHandler.player.AddModifier(StatType.Dodge, new StatModifier(ModifierType.Absolute, -20, locale.Ambush, 5));
                        PlayerHandler.player.AddModifier(StatType.Accuracy, new StatModifier(ModifierType.Absolute, -10, locale.Ambush, 5));
                        BattleManager.StartNewBattle(new Battle(new Dictionary<BattleUser, int>
                        {
                            { new BattleUser(PlayerHandler.player), 0 },
                            { new BattleUser(EnemyFactory.CreateEnemy(CurrentDungeon.DungeonType, CurrentDungeon.DungeonLevel)), 1 }
                        }, CurrentLocation, false));
                    }
                    CurrentLocation.Clear();
                    break;
                case 6:
                    var plant = PlantDropManager.DropDatabase[CurrentDungeon.DungeonType]
                        .GetDrop(CurrentDungeon.DungeonLevel);
                    PlayerHandler.player.Inventory.AddItem(plant.Key, plant.Value);
                    CurrentLocation.Clear();
                    break;
                case 7:
                    var stash = new Stash(CurrentDungeon.DungeonLevel, CurrentDungeon.DungeonType);
                    foreach (var item in stash.GetDrops())
                        PlayerHandler.player.Inventory.AddItem(item.Key, item.Value);
                    CurrentLocation.Clear();
                    break;
                case 8:
                    var trap = CurrentDungeon.CurrentFloor.Traps
                        .FirstOrDefault(x => x.Location == CurrentDungeon.CurrentFloor.Corridor[LocationIndex]);
                    var disarmed = trap.Activate();
                    if (disarmed)
                    {
                        trap.Disarm();
                        CurrentDungeon.CurrentFloor.Traps.Remove(trap);
                    }
                    break;
            }
        }
    }

    private static int ChooseAction()
    {
        Dictionary<string, int> choices = new();
        if (LastMovement == 0)
        {
            if (LocationIndex < CurrentDungeon.CurrentFloor.Corridor.Count + 1)
                choices.Add(locale.GoForward, 0);
            else if (LocationIndex == CurrentDungeon.CurrentFloor.Corridor.Count + 1)
                choices.Add(locale.GoDown, 0);
            
            if (LocationIndex == 0)
                choices.Add(CurrentDungeon.Floors.IndexOf(CurrentDungeon.CurrentFloor) == 0 ? 
                    locale.ExitDungeon : locale.GoUp, 1);
            else if (LocationIndex > 0)
                choices.Add(locale.GoBack, 1);
        }
        else
        {
            if (LocationIndex == 0)
                choices.Add(CurrentDungeon.Floors.IndexOf(CurrentDungeon.CurrentFloor) == 0 ? 
                    locale.ExitDungeon : locale.GoUp, 1);
            else if (LocationIndex > 0)
                choices.Add(locale.GoBack, 1);
            
            if (LocationIndex < CurrentDungeon.CurrentFloor.Corridor.Count + 1)
                choices.Add(locale.GoForward, 0);
            else if (LocationIndex == CurrentDungeon.CurrentFloor.Corridor.Count + 1)
                choices.Add(locale.GoDown, 0);
        }
        switch (CurrentLocation.FieldType)
        {
            case DungeonFieldType.Bonfire:
                choices.Add(locale.RestAtBonfire, 5);
                break;
            case DungeonFieldType.Plant:
                choices.Add(locale.CollectPlant, 6);
                break;
            case DungeonFieldType.Stash:
                choices.Add(locale.OpenStash, 7);
                break;
        }
        if (LocationIndex < CurrentDungeon.CurrentFloor.Corridor.Count)
        {
            if (CurrentDungeon.CurrentFloor.Corridor[LocationIndex].FieldType == DungeonFieldType.Trap && CurrentDungeon.CurrentFloor.Corridor[LocationIndex].Revealed)
            {
                choices.Add(locale.DisarmTrap, 8);
            }
        }
        choices.Add(locale.OpenInventory, 2);
        choices.Add(locale.QuestLog, 3);
        choices.Add(locale.ShowCharacter, 4);
        
        // WPF handles action selection UI
        var choice = choices.Keys.First(); // Default to first choice
        if (choices[choice] >= 0 && choices[choice] <= 3)
            LastMovement = choices[choice];
        UtilityMethods.ClearConsole(4);
        return choices[choice];
    }

    public static void ExitDungeon()
    {
        Exited = true;
        CurrentDungeon = null;
    }

    private static void MoveForward()
    {
        if (LocationIndex < CurrentDungeon.CurrentFloor.Corridor.Count + 1)
        { // Go Forward if not on last block
            LocationIndex++;
            CurrentLocation = LocationIndex == CurrentDungeon.CurrentFloor.Corridor.Count + 1 ? 
                CurrentDungeon.CurrentFloor.EndRoom : CurrentDungeon.CurrentFloor.Corridor[LocationIndex - 1];
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
            CurrentLocation = CurrentDungeon.CurrentFloor.StarterRoom;
            LocationIndex = 0;
            CurrentLocation.Reveal();
        }
        OnMove();
    }
    private static void MoveBackwards()
    {
        if (LocationIndex == 0)
        { // Move Up if on first block
            if (CurrentDungeon.Floors.IndexOf(CurrentDungeon.CurrentFloor) == 0)
            {
                ExitDungeon();
                return;
            }
            CurrentDungeon.Ascend();
            CurrentLocation = CurrentDungeon.CurrentFloor.EndRoom;
            LocationIndex = CurrentDungeon.CurrentFloor.Corridor.Count + 1;
        }
        else
        { // Move Back if not on first block
            LocationIndex--;
            CurrentLocation = LocationIndex == 0 ? 
                CurrentDungeon.CurrentFloor.StarterRoom : CurrentDungeon.CurrentFloor.Corridor[LocationIndex - 1];
        }
        OnMove();
    }

    private static void OnMove()
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
                var battle = CurrentDungeon.CurrentFloor.Battles.FirstOrDefault(x => x.Location == CurrentLocation);
                BattleManager.StartNewBattle(battle);
                if (BattleManager.CurrentBattle.Escaped)
                    MoveBackwards();
                else
                    CurrentLocation.Clear();
                break;
            case DungeonFieldType.Trap:
                var trap = CurrentDungeon.CurrentFloor.Traps.FirstOrDefault(x => x.Location == CurrentLocation);
                // WPF handles trap activation display
                trap.Disarm();
                CurrentDungeon.CurrentFloor.Traps.Remove(trap);
                if (!trap.Activate())
                    trap.Trigger();
                break;
        }
    }
}