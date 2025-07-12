namespace GodmistWPF.Enums.Modifiers;

/// <summary>
/// Określa typ statystyki postaci, która może być modyfikowana przez przedmioty i umiejętności.
/// </summary>
public enum StatType
{
    /// <summary>
    /// Maksymalne zdrowie - określa maksymalną ilość punktów zdrowia postaci.
    /// </summary>
    MaximalHealth,
    
    /// <summary>
    /// Minimalny atak - określa dolną granicę generowanych obrażeń.
    /// </summary>
    MinimalAttack,
    
    /// <summary>
    /// Maksymalny atak - określa górną granicę generowanych obrażeń.
    /// </summary>
    MaximalAttack,
    
    /// <summary>
    /// Unik - określa szansę na uniknięcie ataku.
    /// </summary>
    Dodge,
    
    /// <summary>
    /// Obrona fizyczna - zmniejsza otrzymywane obrażenia fizyczne.
    /// </summary>
    PhysicalDefense,
    
    /// <summary>
    /// Obrona magiczna - zmniejsza otrzymywane obrażenia magiczne.
    /// </summary>
    MagicDefense,
    
    /// <summary>
    /// Szansa na trafienie krytyczne - zwiększa szansę na zadanie krytycznych obrażeń.
    /// </summary>
    CritChance,
    
    /// <summary>
    /// Szybkość - wpływa na kolejność wykonywania akcji w walce.
    /// </summary>
    Speed,
    
    /// <summary>
    /// Celność - zmniejsza szansę na chybienie ataku.
    /// </summary>
    Accuracy,
    
    /// <summary>
    /// Maksymalny zasób - określa maksymalną ilość punktów zasobów (many, furii itp.).
    /// </summary>
    MaximalResource
}