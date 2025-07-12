namespace GodmistWPF.Items.Lootbags;

/// <summary>
/// Specjalny rodzaj worka z łupami, który zawiera losową broń.
/// Dziedziczy z klasy bazowej Lootbag.
/// </summary>
public class WeaponLootbag : Lootbag
{
    /// <summary>
    /// Pobiera alias worka (zawsze "WeaponLootbag").
    /// </summary>
    public override string Alias => "WeaponLootbag";
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy WeaponLootbag z określonym poziomem.
    /// </summary>
    /// <param name="level">Poziom worka, który wpływa na jakość otrzymywanej broni.</param>
    public WeaponLootbag(int level)
    {
        Level = level;
    }
}