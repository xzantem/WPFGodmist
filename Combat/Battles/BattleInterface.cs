using GodmistWPF.Combat.Battles;

/// <summary>
/// Klasa odpowiedzialna za zarządzanie interfejsem użytkownika podczas walki.
/// </summary>
/// <remarks>
/// Zapewnia funkcjonalność wyświetlania i aktualizacji interfejsu walki,
/// w tym kolejki turek i stanu wejścia gracza.
/// </remarks>
public class BattleInterface
{
    /// <summary>
    /// Pobiera informację, czy wejście gracza jest aktywne.
    /// </summary>
    /// <value>
    /// <c>true</c> jeśli gracz może wykonywać akcje; w przeciwnym razie, <c>false</c>.
    /// </value>
    public bool IsPlayerInputEnabled { get; private set; }

    /// <summary>
    /// Maksymalna liczba jednostek wyświetlanych w kolejce turek.
    /// </summary>
    private const int TURN_ORDER_COUNT = 10;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="BattleInterface"/>
    /// </summary>
    /// <remarks>
    /// Domyślnie wyłącza wejście gracza.
    /// </remarks>
    public BattleInterface()
    {
        IsPlayerInputEnabled = false;
    }

    /// <summary>
    /// Generuje uporządkowaną kolejkę turek dla uczestników walki.
    /// </summary>
    /// <param name="unorganized">Nieuporządkowana lista uczestników walki.</param>
    /// <returns>Lista krotek zawierających uczestnika i jego pozycję w kolejce.</returns>
    /// <remarks>
    /// Algorytm symuluje upływ czasu, aby określić kolejność ruchów
    /// na podstawie prędkości uczestników.
    /// </remarks>
    public List<(BattleUser, int)> GetTurnOrder(List<BattleUser> unorganized)
    {
        var copy = unorganized.Select(user => new BattleUser(user)).ToList();
        var organized = new List<(BattleUser, int)>();
        var index = 0;
        while (organized.Count < TURN_ORDER_COUNT)
        {
            index++;
            foreach (var user in copy.Where(user => user.TryMove()))
            {
                organized.Add((user, index));
                break;
            }
        }
        return organized.ToList();
    }
    
    /// <summary>
    /// Włącza lub wyłącza możliwość wprowadzania poleceń przez gracza.
    /// </summary>
    /// <param name="enable">
    /// <c>true</c> aby włączyć wejście gracza; w przeciwnym razie, <c>false</c>.
    /// </param>
    public void EnablePlayerInput(bool enable)
    {
        IsPlayerInputEnabled = enable;
    }
}