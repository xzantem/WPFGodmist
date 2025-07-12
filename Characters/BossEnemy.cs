/// <summary>
/// Zawiera klasy reprezentujące postacie w grze, w tym gracza, przeciwników i NPC.
/// </summary>
/// <remarks>
/// <para>Przestrzeń nazw zawiera m.in. następujące typy postaci:</para>
/// <list type="bullet">
/// <item>Gracz (<see cref="PlayerCharacter"/>)</item>
/// <item>Przeciwnicy (<see cref="EnemyCharacter"/>, <see cref="BossEnemy"/>)</item>
/// <item>Postać bazowa (<see cref="Character"/>)</item>
/// </list>
/// </remarks>
namespace GodmistWPF.Characters;

/// <summary>
/// Reprezentuje przeciwnika typu boss w grze.
/// </summary>
/// <remarks>
/// <para>Klasa dziedziczy po <see cref="EnemyCharacter"/> i rozszerza ją o funkcjonalność związaną z fazami walki z bossem.</para>
/// <para>Boss może mieć różne fazy walki, które wpływają na jego zachowanie i statystyki.</para>
/// </remarks>
public class BossEnemy : EnemyCharacter
{
    /// <summary>
    /// Pobiera aktualną fazę walki z bossem.
    /// </summary>
    /// <value>
    /// Wartość całkowita określająca aktualną fazę (1 lub 2).
    /// Faza 2 rozpoczyna się, gdy zdrowie bossa spadnie do połowy lub poniżej maksymalnej wartości.
    /// </value>
    public int CurrentPhase => CurrentHealth <= MaximalHealth / 2 ? 2 : 1;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="BossEnemy"/> na podstawie istniejącego przeciwnika.
    /// </summary>
    /// <param name="other">Przeciwnik, na podstawie którego tworzony jest boss.</param>
    /// <param name="level">Poziom, na którym ma zostać utworzony boss.</param>
    /// <remarks>
    /// Konstruktor wywołuje konstruktor klasy bazowej <see cref="EnemyCharacter"/>
    /// z przekazanymi parametrami, co pozwala na utworzenie wzmocnionej wersji przeciwnika.
    /// </remarks>
    public BossEnemy(EnemyCharacter other, int level) : base(other, level)
    {
        // Konstruktor wywołuje konstruktor klasy bazowej EnemyCharacter
    }
}