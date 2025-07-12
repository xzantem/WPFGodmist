using GodmistWPF.Characters;
using GodmistWPF.Combat.Modifiers;

namespace GodmistWPF.Combat.Battles;

/// <summary>
/// Klasa reprezentująca uczestnika walki, łącząca postać z jej stanem w trakcie bitwy.
/// </summary>
/// <remarks>
/// Zawiera informacje o dostępnych punktach akcji, wskaźniku akcji
/// i innych stanach związanych z przebiegiem walki.
/// </remarks>
public class BattleUser
{
    /// <summary>
    /// Pobiera postać powiązaną z tym uczestnikiem walki.
    /// </summary>
    public Character User { get; private set; }
    /// <summary>
    /// Pobiera informację, czy uczestnik wykonał już ruch w tej turze.
    /// </summary>
    public bool MovedThisTurn { get; private set; }
    /// <summary>
    /// Pobiera wskaźnik akcji określający, kiedy uczestnik może wykonać ruch.
    /// </summary>
    public double ActionPointer { get; private set; }
    /// <summary>
    /// Pobiera wartość akcji użytkownika, która zmniejsza się z każdym taktem.
    /// </summary>
    public int ActionValue { get; private set; }
    
    /// <summary>
    /// Pobiera maksymalną liczbę punktów akcji dostępnych w turze.
    /// </summary>
    public Stat MaxActionPoints { get; private set; }
    /// <summary>
    /// Pobiera lub ustawia bieżącą liczbę dostępnych punktów akcji.
    /// </summary>
    public double CurrentActionPoints { get; private set; }

    /// <summary>
    /// Początkowa wartość wskaźnika akcji.
    /// </summary>
    private const double ActionPointerInitial = 4;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="BattleUser"/> dla podanej postaci.
    /// </summary>
    /// <param name="user">Postać, która będzie uczestniczyć w walce.</param>
    public BattleUser(Character user)
    {
        User = user;
        MaxActionPoints = new Stat(User.Speed / 2, 0);
        ResetAction();
    }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="BattleUser"/> jako kopię istniejącego uczestnika.
    /// </summary>
    /// <param name="other">Inny uczestnik walki do skopiowania.</param>
    public BattleUser(BattleUser other)
    {
        User = other.User;
        MovedThisTurn = other.MovedThisTurn;
        ActionPointer = other.ActionPointer;
        ActionValue = other.ActionValue;
        MaxActionPoints = other.MaxActionPoints;
        CurrentActionPoints = other.CurrentActionPoints;
    }

    /// <summary>
    /// Resetuje wskaźnik i wartość akcji na podstawie prędkości postaci.
    /// </summary>
    /// <remarks>
    /// Oblicza nową wartość wskaźnika akcji i aktualizuje maksymalną liczbę punktów akcji.
    /// </remarks>
    public void ResetAction()
    {
        ActionPointer = Math.Pow(10, ActionPointerInitial);
        ActionValue += (int)(ActionPointer / User.Speed);
        MaxActionPoints = new Stat(User.Speed / 2, 0, MaxActionPoints.Modifiers);
        CurrentActionPoints = MaxActionPoints.Value(User, "MaxActionPoints");
    }

    /// <summary>
    /// Próbuje wykonać ruch, jeśli wartość akcji osiągnęła zero.
    /// </summary>
    /// <returns>
    /// <c>true</c> jeśli ruch został wykonany; w przeciwnym razie <c>false</c>.
    /// </returns>
    public bool TryMove()
    {
        ActionValue--;
        if (ActionValue != 0) return false;
        ResetAction();
        MovedThisTurn = true;
        return true;
    }

    /// <summary>
    /// Przesuwa wartość akcji o określoną liczbę jednostek.
    /// </summary>
    /// <param name="amount">Liczba jednostek, o którą ma zostać przesunięta wartość akcji.</param>
    public void AdvanceMove(int amount)
    {
        ActionValue -= amount;
    }

    /// <summary>
    /// Zużywa określoną liczbę punktów akcji.
    /// </summary>
    /// <param name="amount">Liczba punktów akcji do zużycia.</param>
    public void UseActionPoints(double amount)
    {
        CurrentActionPoints = Math.Round(Math.Max(0, CurrentActionPoints - (int)amount));
    }

    /// <summary>
    /// Rozpoczyna nową turę dla uczestnika walki.
    /// </summary>
    /// <remarks>
    /// Resetuje flagę wykonanego ruchu w turze.
    /// </remarks>
    public void StartNewTurn()
    {
        MovedThisTurn = false;
    }
}