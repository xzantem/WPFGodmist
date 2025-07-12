
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using GodmistWPF.Characters;
using GodmistWPF.Characters.Player;
using GodmistWPF.Combat.Modifiers.PassiveEffects;
using GodmistWPF.Dialogs;
using GodmistWPF.Dungeons;
using GodmistWPF.Text;
using GodmistWPF.Utilities;

namespace GodmistWPF.Combat.Battles;

/// <summary>
/// Główna klasa zarządzająca przebiegiem walki w grze.
/// </summary>
/// <remarks>
/// Odpowiada za kolejność turek, przetwarzanie akcji gracza i AI,
/// oraz zarządzanie stanem walki.
/// </remarks>
public class Battle(Dictionary<BattleUser, int> usersTeams, DungeonField location, bool canEscape = true)
{
    /// <summary>
    /// Słownik zawierający uczestników walki wraz z ich przynależnością do drużyn.
    /// </summary>
    /// <value>
    /// Klucz to uczestnik walki, wartość to identyfikator drużyny.
    /// </value>
    public Dictionary<BattleUser, int> Users { get; } = usersTeams;
    /// <summary>
    /// Pobiera numer aktualnej tury walki.
    /// </summary>
    /// <value>
    /// Licznik tur, zaczynający się od 1.
    /// </value>
    public int TurnCount { get; private set; } = 1;
    /// <summary>
    /// Pobiera lub ustawia informację, czy gracz uciekł z walki.
    /// </summary>
    /// <value>
    /// <c>true</c> jeśli gracz uciekł; w przeciwnym razie <c>false</c>.
    /// </value>
    public bool Escaped { get; set; }
    /// <summary>
    /// Pobiera informację, czy ucieczka z walki jest możliwa.
    /// </summary>
    /// <value>
    /// <c>true</c> jeśli ucieczka jest możliwa; w przeciwnym razie <c>false</c>.
    /// </value>
    public bool CanEscape { get; } = canEscape;
    /// <summary>
    /// Pobiera lub ustawia liczbę prób ucieczki.
    /// </summary>
    private int EscapeAttempts { get; set; }
    /// <summary>
    /// Pobiera lokalizację, w której odbywa się walka.
    /// </summary>
    public DungeonField Location { get; private set; } = location;

    /// <summary>
    /// Pobiera interfejs użytkownika dla tej walki.
    /// </summary>
    public BattleInterface Interface { get; } = new();
    /// <summary>
    /// Źródło zadania używane do oczekiwania na zakończenie tury gracza.
    /// </summary>
    private TaskCompletionSource<bool> _playerMoveCompletionSource;

    /// <summary>
    /// Rozpoczyna nową turę walki.
    /// </summary>
    /// <returns>Zadanie reprezentujące asynchroniczną operację.</returns>
    /// <remarks>
    /// Metoda zarządza całą logiką tury, w tym:
    /// - Aktualizacją interfejsu użytkownika
    /// - Obsługą efektów początku tury
    /// - Kolejnością ruchów postaci
    /// - Obsługą akcji gracza i AI
    /// - Sprawdzaniem warunków zakończenia walki
    /// </remarks>
    public async Task NewTurnAsync()
    {
        try
        {
            // Update turn display
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (Application.Current.Windows.OfType<BattleDialog>().FirstOrDefault() is { } battleDialog)
                {
                    battleDialog.AddToBattleLog($"=== Turn {TurnCount} ===", Brushes.White);
                }
            });

            // Start new turn for all users
            foreach (var user in Users.Keys)
            {
                user.StartNewTurn();
            }

            // Process each user's turn
            while (Users.Keys.Any(x => !x.MovedThisTurn) && !Escaped)
            {
                foreach (var user in Users.Keys.ToList()) // Create a copy to avoid collection modification
                {
                    if (!user.TryMove()) continue;
                    if (CheckForResult() != -1)
                        return;

                    // Update UI to show whose turn it is
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        if (Application.Current.Windows.OfType<BattleDialog>().FirstOrDefault() is { } dialog)
                        {
                            dialog.AddToBattleLog($"{user.User.Name}'s turn", Brushes.LightBlue);
                            dialog.UpdateBattleDisplay();
                        }
                    });

                    // Small delay for UI updates
                    await Task.Delay(500);

                    // Check if user can move (considering status effects, etc.)
                    if (!user.User.PassiveEffects.CanMove())
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            if (Application.Current.Windows.OfType<BattleDialog>().FirstOrDefault() is { } dialog)
                            {
                                dialog.AddToBattleLog($"{user.User.Name} is unable to move!", Brushes.Yellow);
                            }
                        });

                        await Task.Delay(1000);
                        await HandleEffectsAsync(user);

                        if (CheckForResult() != -1)
                            return;

                        continue;
                    }

                    // Handle any start-of-turn effects
                    await HandleEffectsAsync(user);
                    if (CheckForResult() != -1)
                        return;

                    // Process the actual move based on user type (player or AI)
                    switch (Users[user])
                    {
                        case 0: // Player
                            // Enable player input and wait for their move
                            await PlayerMoveAsync(user, Users
                                .FirstOrDefault(x => x.Key != user).Key);
                            break;

                        case 1: // AI
                            await Task.Delay(500); // Small delay for better feel
                            await AIMoveAsync(user, UtilityMethods
                                .RandomChoice(Users
                                    .Where(x => x.Key != user)
                                    .ToDictionary(x => x.Key, x => 1)));
                            break;
                    }

                    // Update UI after the move
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        if (Application.Current.Windows.OfType<BattleDialog>().FirstOrDefault() is { } dialog)
                        {
                            dialog.UpdateBattleDisplay();
                        }
                    });

                    // Small delay between turns for better feel
                    await Task.Delay(300);

                    if (CheckForResult() != -1)
                        return;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in NewTurnAsync: {ex.Message}");
        }

        TurnCount++;
    }
    /// <summary>
    /// Obsługuje efekty i modyfikatory dla określonego użytkownika.
    /// </summary>
    /// <param name="user">Użytkownik, dla którego mają zostać obsłużone efekty.</param>
    /// <returns>Zadanie reprezentujące asynchroniczną operację.</returns>
    public async Task HandleEffectsAsync(BattleUser user)
    {
        user.User.HandleModifiers();
        user.User.RegenResource((int)user.User.ResourceRegen);
        user.User.PassiveEffects.HandleBattleEvent(new BattleEventData("PerTurn", user));
        user.User.PassiveEffects.TickEffects();
        await CheckForDeadAsync();
    }
    /// <summary>
    /// Obsługuje ruch gracza w trakcie tury.
    /// </summary>
    /// <param name="player">Gracz wykonujący ruch.</param>
    /// <param name="target">Domyślny cel ataku.</param>
    /// <returns>
    /// <c>true</c> jeśli ruch został ukończony pomyślnie; w przeciwnym razie <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Metoda aktywuje interfejs użytkownika i czeka na decyzję gracza.
    /// </remarks>
    public async Task<bool> PlayerMoveAsync(BattleUser player, BattleUser target)
    {
        try
        {
            // Create a new completion source for this move
            _playerMoveCompletionSource = new TaskCompletionSource<bool>();
            
            // Enable UI for player input
            await Application.Current.Dispatcher.InvokeAsync(() => 
            {
                Interface.EnablePlayerInput(true);
                if (Application.Current.Windows.OfType<BattleDialog>().FirstOrDefault() is { } dialog)
                {
                    dialog.UpdateBattleDisplay();
                }
                // Update UI to show it's the player's turn
            });
            
            // Wait for the player to make a move (either by using a skill/item or ending their turn)
            bool moveCompleted = await _playerMoveCompletionSource.Task;
            
            // Disable UI after move is complete
            await Application.Current.Dispatcher.InvokeAsync(() => 
            {
                Interface.EnablePlayerInput(false);
            });
            
            return moveCompleted;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in PlayerMoveAsync: {ex.Message}");
            return true; // End the turn if there's an error
        }
        finally
        {
            _playerMoveCompletionSource = null;
        }
    }
    /// <summary>
    /// Obsługuje ruch przeciwnika sterowanego przez AI.
    /// </summary>
    /// <param name="enemy">Przeciwnik wykonujący ruch.</param>
    /// <param name="target">Cel ataku.</param>
    /// <returns>Zadanie reprezentujące asynchroniczną operację.</returns>
    /// <remarks>
    /// AI wybiera losowo umiejętność z dostępnych, które może użyć,
    /// biorąc pod uwagę koszt many i punktów akcji.
    /// </remarks>
    public async Task AIMoveAsync(BattleUser enemy, BattleUser target)
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
            await Application.Current.Dispatcher.InvokeAsync(() => 
            {
                if (Application.Current.Windows.OfType<BattleDialog>().FirstOrDefault() is { } dialog)
                {
                    dialog.UpdateBattleDisplay();
                }
            });
            
            await CheckForDeadAsync();
            if (CheckForResult() != -1) break;
            
            // Small delay for better feel
            await Task.Delay(500);
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

    public async Task CheckForDeadAsync()
    {
        var deadUsers = Users.Keys.Where(x => x.User.CurrentHealth <= 0).ToList();
        if (!deadUsers.Any()) return;
        
        await Application.Current.Dispatcher.InvokeAsync(() => 
        {
            foreach (var user in deadUsers)
            {
            }
        });
    }

    public bool TryEscape(PlayerCharacter player)
    {
        if (!(Random.Shared.NextDouble() < 0.5 + EscapeAttempts * 0.1))
        {
            EscapeAttempts++;
            return false;
        }
        player.LoseHonor((int)Users.Where(x => x.Value == 1)
            .Average(x => x.Key.User.Level) / 3 + 4);
        EscapeAttempts = 0;
        Escaped = true;
        return true;
    }
    
    public void CompletePlayerMove(bool moveCompleted)
    {
        _playerMoveCompletionSource?.TrySetResult(moveCompleted);
    }
    
    public void Cleanup()
    {
        // Clean up any pending player move
        _playerMoveCompletionSource?.TrySetResult(true);
        _playerMoveCompletionSource = null;
    }
}