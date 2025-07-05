using GodmistWPF.Characters;
using GodmistWPF.Combat.Modifiers;

namespace GodmistWPF.Combat.Battles;

public class BattleUser
{
    public Character User { get; private set; }
    public bool MovedThisTurn { get; private set; }
    public double ActionPointer { get; private set; }
    public int ActionValue { get; private set; }
    
    public Stat MaxActionPoints { get; private set; }
    public double CurrentActionPoints { get; private set; }

    private const double ActionPointerInitial = 4;

    public BattleUser(Character user)
    {
        User = user;
        MaxActionPoints = new Stat(User.Speed / 2, 0);
        ResetAction();
    }

    public BattleUser(BattleUser other)
    {
        User = other.User;
        MovedThisTurn = other.MovedThisTurn;
        ActionPointer = other.ActionPointer;
        ActionValue = other.ActionValue;
        MaxActionPoints = other.MaxActionPoints;
        CurrentActionPoints = other.CurrentActionPoints;
    }

    public void ResetAction()
    {
        ActionPointer = Math.Pow(10, ActionPointerInitial);
        ActionValue += (int)(ActionPointer / User.Speed);
        MaxActionPoints = new Stat(User.Speed / 2, 0, MaxActionPoints.Modifiers);
        CurrentActionPoints = MaxActionPoints.Value(User, "MaxActionPoints");
    }

    public bool TryMove()
    {
        ActionValue--;
        if (ActionValue != 0) return false;
        ResetAction();
        MovedThisTurn = true;
        return true;
    }

    public void AdvanceMove(int amount)
    {
        ActionValue -= amount;
    }

    public void UseActionPoints(double amount)
    {
        CurrentActionPoints = Math.Round(Math.Max(0, CurrentActionPoints - (int)amount));
    }

    public void StartNewTurn()
    {
        MovedThisTurn = false;
    }
}