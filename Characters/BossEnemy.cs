namespace GodmistWPF.Characters;

public class BossEnemy : EnemyCharacter
{
    public int CurrentPhase => CurrentHealth <= MaximalHealth / 2 ? 2 : 1;

    public BossEnemy(EnemyCharacter other, int level) : base(other, level)
    {
        
    }
}