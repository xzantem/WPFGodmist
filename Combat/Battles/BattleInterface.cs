using GodmistWPF.Combat.Battles;

public class BattleInterface(BattleUser displayedUser)
{
    public string InfoDisplayMode { get; private set; } = "Resistances";
    public BattleUser DisplayedUser { get; private set; } = displayedUser;
    public List<string> BattleLog { get; private set; } = [];
    private int _battleLogCurrentIndex;

    private const int SHOWN_BATTLE_LOG_LINES = 10;

    public void ChangeDisplayMode()
    {
        // WPF handles display mode cycling
        InfoDisplayMode = InfoDisplayMode switch
        { 
            "Resistances" => "Skills", 
            "Skills" => "PassiveEffects", 
            _ => "Resistances" 
        };
    }

    public void AddBattleLogLines(params string[] lines)
    {
        BattleLog.AddRange(lines);
        _battleLogCurrentIndex = Math.Max(0, BattleLog.Count - SHOWN_BATTLE_LOG_LINES);
    }

    public void ScrollBattleLog(bool up)
    {
        _battleLogCurrentIndex = up ? Math.Max(0, _battleLogCurrentIndex - SHOWN_BATTLE_LOG_LINES) : 
            Math.Max(0, Math.Min(_battleLogCurrentIndex + SHOWN_BATTLE_LOG_LINES, BattleLog.Count - SHOWN_BATTLE_LOG_LINES));
    }

    public bool CanScroll(bool up)
    {
        if (up)
            return _battleLogCurrentIndex > Math.Max(0, _battleLogCurrentIndex - SHOWN_BATTLE_LOG_LINES);
        return 
            _battleLogCurrentIndex < Math.Max(0, Math.Min(_battleLogCurrentIndex + SHOWN_BATTLE_LOG_LINES, 
                BattleLog.Count - SHOWN_BATTLE_LOG_LINES));
    }

    public void ChangeDisplayedUser(List<BattleUser> users)
    {
        // WPF handles user selection
        var currentIndex = users.IndexOf(DisplayedUser);
        var nextIndex = (currentIndex + 1) % users.Count;
        DisplayedUser = users[nextIndex];
    }

    public void DisplayInterface(BattleUser movingUser, List<BattleUser> users, bool clear = true)
    {
        // WPF handles battle interface display
    }

    private List<(BattleUser, int)> GetTurnOrder(List<BattleUser> unorganized)
    {
        var copy = unorganized.Select(user => new BattleUser(user)).ToList();
        var organized = new List<(BattleUser, int)>();
        var index = 0;
        while (organized.Count < SHOWN_BATTLE_LOG_LINES)
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

    private object GetUserTable(BattleUser user)
    {
        // WPF handles user table display
        return new object();
    }
}