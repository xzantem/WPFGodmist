using System.IO;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Dungeons;
using GodmistWPF.Quests;
using GodmistWPF.Utilities;
using Newtonsoft.Json;

namespace GodmistWPF.Characters;

/// <summary>
/// Fabryka odpowiedzialna za tworzenie instancji przeciwników w grze.
/// </summary>
/// <remarks>
/// Klasa udostępnia metody do tworzenia przeciwników na podstawie różnych kryteriów,
/// takich jak alias, typ lokacji czy poziom trudności. Przechowuje również listę
/// wszystkich dostępnych wzorców przeciwników wczytanych z pliku JSON.
/// </remarks>
public static class EnemyFactory
{
    /// <summary>
    /// Lista przechowująca wzorce wszystkich dostępnych przeciwników w grze.
    /// </summary>
    /// <value>Lista obiektów <see cref="EnemyCharacter"/> reprezentujących wzorce przeciwników.</value>
    /// <remarks>
    /// Lista jest wczytywana raz podczas inicjalizacji gry z pliku JSON.
    /// Każdy nowy przeciwnik jest tworzony jako kopia odpowiedniego wzorca z tej listy.
    /// </remarks>
    public static List<EnemyCharacter> EnemiesList;

    /// <summary>
    /// Szansa na pojawienie się bossa, gdy warunki są spełnione.
    /// </summary>
    /// <value>Wartość z przedziału 0-1 określająca prawdopodobieństwo.</value>
    private const double BossChance = 0.2;
    
    /// <summary>
    /// Tworzy nowego przeciwnika na podstawie podanego aliasu i poziomu.
    /// </summary>
    /// <param name="alias">Unikalny identyfikator przeciwnika.</param>
    /// <param name="level">Poziom, na który ma zostać przeskalowany przeciwnik.</param>
    /// <returns>Nowa instancja <see cref="EnemyCharacter"/>.</returns>
    /// <remarks>
    /// Metoda wyszukuje wzorzec przeciwnika o podanym aliasie i tworzy jego kopię
    /// z odpowiednio przeskalowanymi statystykami do podanego poziomu.
    /// </remarks>
    public static EnemyCharacter CreateEnemy(string alias, int level)
    {
        var enemy = EnemiesList.FirstOrDefault(x => x.Alias == alias);
        return new EnemyCharacter(enemy, level);
    }
    /// <summary>
    /// Tworzy losowego przeciwnika odpowiedniego dla podanego typu lokacji i poziomu.
    /// </summary>
    /// <param name="dungeonType">Typ lokacji, z której ma pochodzić przeciwnik.</param>
    /// <param name="level">Poziom, na który ma zostać przeskalowany przeciwnik.</param>
    /// <returns>Nowa instancja <see cref="EnemyCharacter"/> lub <see cref="BossEnemy"/>.</returns>
    /// <remarks>
    /// <para>Metoda losuje przeciwnika spośród tych, które są przypisane do danej lokacji.</para>
    /// <para>Jeśli postać ukończyła odpowiednią ilość zadań w lokacji, istnieje szansa na pojawienie się bossa.</para>
    /// <para>Statystyki przeciwnika są skalowane do podanego poziomu.</para>
    /// </remarks>
    /// <summary>
    /// Tworzy losowego przeciwnika odpowiedniego dla podanego typu lokacji i poziomu.
    /// </summary>
    /// <param name="dungeonType">Typ lokacji, z której ma pochodzić przeciwnik.</param>
    /// <param name="level">Poziom, na który ma zostać przeskalowany przeciwnik.</param>
    /// <returns>Nowa instancja <see cref="EnemyCharacter"/> lub <see cref="BossEnemy"/>.</returns>
    /// <remarks>
    /// <para>Metoda działa w następujący sposób:</para>
    /// <list type="number">
    /// <item>Sprawdza, czy postać ukończyła wystarczającą liczbę zadań w danej lokacji.</item>
    /// <item>Jeśli tak, istnieje szansa (określona przez <see cref="BossChance"/>) na pojawienie się bossa.</item>
    /// <item>Jeśli warunek jest spełniony, losowany jest boss z dostępnej puli bossów dla danej lokacji.</item>
    /// <item>Jeśli boss ma aktywne zadanie do wykonania, tworzona jest jego instancja.</item>
    /// <item>W przeciwnym razie losowany jest zwykły przeciwnik z danej lokacji.</item>
    /// </list>
    /// <para>Statystyki przeciwnika są skalowane do podanego poziomu.</para>
    /// <para>Zwykli przeciwnicy są losowani spośród tych, którzy są przypisani do danej lokacji i nie są bossami.</para>
    /// </remarks>
    public static EnemyCharacter CreateEnemy(DungeonType dungeonType, int level)
    {
        if (QuestManager.BossProgress[dungeonType] >= QuestManager.ProgressTarget &&
            Random.Shared.NextDouble() < BossChance)
        {
            var target = QuestManager.GetBossQuestTarget(
                dungeonType, 
                Random.Shared.Next(1, QuestManager.BossProgress[dungeonType] / QuestManager.ProgressTarget + 1)
            );
            
            if (QuestManager.BossSideQuests
                .Where(x => x.QuestState != QuestState.Available)
                .Any(x => x.Alias == target))
            {
                var boss = EnemiesList.FirstOrDefault(x => x.Name == target);
                return new BossEnemy(boss, level);
            }
        }
        var enemy = UtilityMethods.RandomChoice(
            EnemiesList
                .Where(x => x.DefaultLocation == dungeonType && 
                           x.EnemyType.All(t => t != EnemyType.Boss))
                .ToList()
        );
        return new EnemyCharacter(enemy, level);
    }

    /// <summary>
    /// Inicjalizuje fabrykę poprzez wczytanie wzorców przeciwników z pliku JSON.
    /// </summary>
    /// <exception cref="FileNotFoundException">Wyrzucany, gdy nie znaleziono pliku z danymi przeciwników.</exception>
    /// <remarks>
    /// <para>Metoda wczytuje dane przeciwników z pliku 'json/enemies.json' i deserializuje je do listy <see cref="EnemiesList"/>.</para>
    /// <para>Musi zostać wywołana przed pierwszym użyciem metod tworzących przeciwników.</para>
    /// </remarks>
    public static void InitializeEnemies()
    {
        var path = "json/enemies.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            EnemiesList = JsonConvert.DeserializeObject<List<EnemyCharacter>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }
}