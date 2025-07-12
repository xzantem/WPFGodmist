using GodmistWPF.Characters.Player;
using GodmistWPF.Utilities;

namespace GodmistWPF.Items.Drops;

/// <summary>
/// Klasa reprezentująca tabelę dropu, zawierającą listę pul przedmiotów, które mogą wypaść.
/// Zapewnia metody do losowania przedmiotów z uwzględnieniem poziomu postaci i modyfikatorów szansy.
/// </summary>
public class DropTable
{
    /// <summary>
    /// Lista pul przedmiotów, z których mogą wypaść przedmioty.
    /// Każda pula ma swoją własną tablicę szans na wypadnięcie przedmiotów.
    /// </summary>
    public List<DropPool> Table { get; set; }
    /// <summary>
    /// Konstruktor domyślny wymagany do deserializacji JSON.
    /// </summary>
    public DropTable() { }
    /// <summary>
    /// Inicjalizuje nową instancję klasy DropTable z określoną listą pul przedmiotów.
    /// </summary>
    /// <param name="table">Lista pul przedmiotów.</param>
    public DropTable(List<DropPool> table)
    {
        Table = table;
    }
    /// <summary>
    /// Pobiera słownik przedmiotów, które mają wypaść, wraz z ich ilościami.
    /// Uwzględnia modyfikatory szansy na przedmioty z pasywnych umiejętności gracza.
    /// </summary>
    /// <param name="level">Poziom postaci, dla którego generowane są przedmioty.</param>
    /// <returns>Słownik zawierający przedmioty i ich ilości, które mają wypaść.</returns>
    public Dictionary<IItem, int> GetDrops(int level)
    {
        Dictionary<IItem, int> drops = new();
        foreach (var pool in Table)
        {
            if (pool.Pool == null) continue;
            var poolCopy = new DropPool(pool);
            foreach (var t in poolCopy.Chances)
            {
                if (!poolCopy.Pool.Any(x => x.Value.MinLevel <= level && x.Value.MaxLevel >= level)) break;
                var dropChance = UtilityMethods.CalculateModValue(Random.Shared.NextDouble(), 
                    PlayerHandler.player.PassiveEffects.GetModifiers("ItemChanceMod"));
                if (!(dropChance < t)) continue;
                var pair = poolCopy.Choice(level);
                drops.Add(pair.Key, Random.Shared.Next(pair.Value.MinAmount, pair.Value.MaxAmount + 1));
                poolCopy.Pool.Remove(pair.Key.Alias);
            }
        }
        return drops;
    }
}