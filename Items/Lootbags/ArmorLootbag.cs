namespace GodmistWPF.Items.Lootbags;

/// <summary>
/// Specjalny rodzaj worka z łupami, który zawiera losowe elementy zbroi.
/// Dziedziczy z klasy bazowej Lootbag.
/// </summary>
public class ArmorLootbag : Lootbag
{
    /// <summary>
    /// Pobiera unikalny identyfikator worka z zbroją (zawsze 572).
    /// </summary>
    public override int ID => 572;
    
    /// <summary>
    /// Pobiera alias worka (zawsze "ArmorLootbag").
    /// </summary>
    public override string Alias => "ArmorLootbag";
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy ArmorLootbag z określonym poziomem.
    /// </summary>
    /// <param name="level">Poziom worka, który wpływa na jakość otrzymywanych przedmiotów.</param>
    public ArmorLootbag(int level)
    {
        Level = level;
    }
}