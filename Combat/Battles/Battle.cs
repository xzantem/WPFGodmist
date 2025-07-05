
using ConsoleGodmist;
using ConsoleGodmist.TextService;
using GodmistWPF.Characters;
using GodmistWPF.Characters.Player;
using GodmistWPF.Combat.Modifiers.PassiveEffects;
using GodmistWPF.Dungeons;
using GodmistWPF.Text;
using GodmistWPF.Utilities;

namespace GodmistWPF.Combat.Battles;

public class Battle(Dictionary<BattleUser, int> usersTeams, DungeonField location, bool canEscape = true)
{
    public Dictionary<BattleUser, int> Users { get; } = usersTeams;
    public int TurnCount { get; private set; } = 1;
    public bool Escaped { get; set; }
    public bool CanEscape { get; } = canEscape;
    private int EscapeAttempts { get; set; }
    public DungeonField Location { get; private set; } = location;

    public BattleInterface Interface { get; } = new(usersTeams.Last().Key);

    public void NewTurn()
    {
        Interface.AddBattleLogLines($"- {locale.Turn} {TurnCount} -");
        Interface.DisplayInterface(Users.ElementAt(0).Key, Users.Keys.ToList());
        foreach (var user in Users.Keys)
        {
            user.StartNewTurn();
        }
        while (Users.Keys.Any(x => !x.MovedThisTurn) && !Escaped)
        {
            foreach (var user in Users.Keys.Where(user => user.TryMove()))
            {
                if (CheckForResult() != -1)
                    return;
                Interface.AddBattleLogLines($"{locale.Moving}: {user.User.Name}");
                Interface.DisplayInterface(Users.ElementAt(0).Key, Users.Keys.ToList());
                Thread.Sleep(1000);
                if (!user.User.PassiveEffects.CanMove())
                {
                    Interface.AddBattleLogLines($"{user.User.Name} {locale.CannotMoveThisTurn}\n");
                    Thread.Sleep(1000);
                    HandleEffects(user);
                    Interface.DisplayInterface(Users.ElementAt(0).Key, Users.Keys.ToList());
                    if (CheckForResult() != -1)
                        return;
                    continue;
                }
                HandleEffects(user);
                if (CheckForResult() != -1)
                    return;
                switch (Users[user])
                {
                    case 0:
                        var hasMoved = false;
                        while (!hasMoved)
                        {
                            CheckForDead();
                            Interface.DisplayInterface(Users.ElementAt(0).Key, Users.Keys.ToList());
                            if (CheckForResult() != -1)
                                return;
                            hasMoved = PlayerMove(user, Users
                                .FirstOrDefault(x => x.Key != user).Key);
                            Interface.DisplayInterface(Users.ElementAt(0).Key, Users.Keys.ToList());
                        }
                        break;
                    case 1:
                        Thread.Sleep(1000);
                        AIMove(user, UtilityMethods
                            .RandomChoice(Users
                                .Where(x => x.Key != user)
                                .ToDictionary(x => x.Key, x => 1)));
                        if (CheckForResult() != -1)
                            return;
                        break;
                }
            }
        }
        TurnCount++;
    }
    public void HandleEffects(BattleUser user)
    {
        user.User.HandleModifiers();
        user.User.RegenResource((int)user.User.ResourceRegen);
        user.User.PassiveEffects.HandleBattleEvent(new BattleEventData("PerTurn", user));
        user.User.PassiveEffects.TickEffects();
        CheckForDead();
    }

    public void ChooseSkill(BattleUser player, BattleUser target)
    {
        var activeSkills = (player.User as PlayerCharacter)?.ActiveSkills;  
        var possibleSkills = activeSkills
            .Where(x => (x.ResourceCost <= player.User.CurrentResource ||
                         Math.Abs(player.User.MaximalResource - player.User.CurrentResource) < 0.01)
                        && x.ActionCost * player.MaxActionPoints.Value(player.User, "MaxActionPoints") <= player.CurrentActionPoints)
            .ToArray();
        var possibleSkillsNames = possibleSkills
            .Select(x => x.Name + $" ({(int)UtilityMethods.CalculateModValue(x.ResourceCost, player.User.PassiveEffects.GetModifiers("ResourceCost"))} {BattleTextService.ResourceShortText(player.User as PlayerCharacter)}, " +
                         $"{(int)(x.ActionCost * player.MaxActionPoints.BaseValue)} {locale.ActionPointsShort})").ToArray();
        var unselectableSkillsNames = activeSkills.Where(x => !possibleSkills.Contains(x))
            .Select(x => BattleTextService.UnselectableSkillMarkup(x, player)).ToArray();
        var choices = possibleSkillsNames.Append(locale.Return).ToArray();
        // WPF handles skill selection UI
        var choice = choices[0]; // Default to first choice
        if (choice == locale.Return) return;
        if (!(possibleSkills[Array.IndexOf(choices, choice)]
                .ActionCost * player.MaxActionPoints.BaseValue > player.CurrentActionPoints))
            possibleSkills[Array.IndexOf(choices, choice)].Use(player, target);
    }
    public bool PlayerMove(BattleUser player, BattleUser target)
    {
        var choices = new Dictionary<string, int>
        {
            {locale.EndTurn, 0},
            {locale.UseSkill, 1},
            {locale.UseItem, 2}
        };
        if (CanEscape)
            choices.Add(locale.Escape, 7);
        var displayChoices = new Dictionary<string, int>
        {
            {locale.ChangeAdditionalCharacterInfo, 3},
            {locale.ChangeDisplayedCharacter, 4},
        };
        if (Interface.CanScroll(true))
            displayChoices.Add(locale.ScrollBattleLogUp, 5);
        if (Interface.CanScroll(false))
            displayChoices.Add(locale.ScrollBattleLogDown, 6);

        // WPF handles player move selection UI
        var choice = choices.Keys.First(); // Default to first choice
        switch (choices.Concat(displayChoices).ToDictionary()[choice])
        {
            case 0:
                var activeSkills = (player.User as PlayerCharacter)?.ActiveSkills;
                var canUseSkill = activeSkills
                    .Any(x => (x.ResourceCost <= player.User.CurrentResource ||
                               Math.Abs(player.User.MaximalResource - player.User.CurrentResource) < 0.01)
                              && x.ActionCost * player.MaxActionPoints.Value(player.User, "MaxActionPoints") <=
                              player.CurrentActionPoints && x is not { ResourceCost: 0, ActionCost: 0 });
                var ended = !canUseSkill || UtilityMethods.Confirmation(locale.ConfirmEndTurn);
                if (canUseSkill) 
                    UtilityMethods.ClearConsole();
                return ended;
            case 1:
                ChooseSkill(player, target);
                CheckForDead();
                return false;
            case 2:
                //(player.User as PlayerCharacter).Inventory.UseItem(InventoryMenuHandler.Choose;
                return false;
            case 3:
                Interface.ChangeDisplayMode();
                return false;
            case 4:
                Interface.ChangeDisplayedUser(Users.Keys.ToList());
                return false;
            case 5:
                Interface.ScrollBattleLog(true);
                return false;
            case 6:
                Interface.ScrollBattleLog(false);
                return false;
            case 7:
                Interface.AddBattleLogLines($"{locale.TryEscape}...");
                Interface.DisplayInterface(player, Users.Keys.ToList());
                Thread.Sleep(2000);
                if (!(Random.Shared.NextDouble() < 0.5 + EscapeAttempts * 0.1))
                {
                    Interface.AddBattleLogLines($"{locale.EscapeFail}!");
                    Interface.DisplayInterface(player, Users.Keys.ToList());
                    EscapeAttempts++;
                    return true;
                }
                Interface.AddBattleLogLines($"{locale.EscapeSuccess}!");
                Interface.DisplayInterface(player, Users.Keys.ToList());
                (player.User as PlayerCharacter).LoseHonor((int)Users.Where(x => x.Value == 1)
                    .Average(x => x.Key.User.Level) / 3 + 4);
                EscapeAttempts = 0;
                Escaped = true;
                return true;
        }
        return false;
    }
    public void AIMove(BattleUser enemy, BattleUser target)
    {
        // WPF handles AI move display
        while (CheckForResult() == -1)
        {
            var activeSkills = enemy.User is BossEnemy boss
                ? boss.CurrentPhase == 1
                    ? [boss.ActiveSkills[0], boss.ActiveSkills[1], boss.ActiveSkills[2]]
                    : [boss.ActiveSkills[3], boss.ActiveSkills[4], boss.ActiveSkills[5]] 
                : enemy.User.ActiveSkills;
            var possibleSkills = activeSkills
                .Where(x => x.ResourceCost <= enemy.User.CurrentResource &&
                           x.ActionCost * enemy.MaxActionPoints.Value(enemy.User, "MaxActionPoints") <= enemy.CurrentActionPoints)
                .ToArray();
            if (possibleSkills.Length == 0) break;
            var skill = UtilityMethods.RandomChoice(possibleSkills.ToList());
            skill.Use(enemy, target);
            CheckForDead();
            if (CheckForResult() != -1) break;
        }
    }

    public int CheckForResult()
    {
        var aliveUsers = Users.Keys.Where(x => x.User.CurrentHealth > 0).ToList();
        if (aliveUsers.Count == 0) return 2; // Draw
        if (aliveUsers.All(x => Users[x] == 0)) return 0; // Player victory
        if (aliveUsers.All(x => Users[x] == 1)) return 1; // Enemy victory
        return -1; // Battle continues
    }

    public void CheckForDead()
    {
        foreach (var user in Users.Keys.Where(x => x.User.CurrentHealth <= 0))
        {
            Interface.AddBattleLogLines($"{user.User.Name} {locale.Dies}!");
            Interface.DisplayInterface(Users.ElementAt(0).Key, Users.Keys.ToList());
        }
    }
}