namespace GodmistWPF.Enums.Items;

/// <summary>
/// Określa efekt, który może zostać zastosowany przez miksturę.
/// </summary>
public enum PotionEffect
{
    /// <summary>
    /// Natychmiast przywraca określoną ilość zdrowia.
    /// </summary>
    HealthRegain,
    
    /// <summary>
    /// Zwiększa regenerację zdrowia przez określony czas.
    /// </summary>
    HealthRegen,
    
    /// <summary>
    /// Natychmiast przywraca określoną ilość zasobów (many, furii itp.).
    /// </summary>
    ResourceRegain,
    
    /// <summary>
    /// Zwiększa regenerację zasobów przez określony czas.
    /// </summary>
    ResourceRegen,
    
    /// <summary>
    /// Zwiększa maksymalną ilość zasobów na określony czas.
    /// </summary>
    MaxResourceIncrease,
    
    /// <summary>
    /// Zwiększa zadawane obrażenia przez określony czas.
    /// </summary>
    DamageDealtIncrease,
    
    /// <summary>
    /// Zmniejsza otrzymywane obrażenia przez określony czas.
    /// </summary>
    DamageTakenDecrease,
    
    /// <summary>
    /// Zwiększa odporność na określony typ obrażeń.
    /// </summary>
    ResistanceIncrease,
    
    /// <summary>
    /// Zwiększa szybkość poruszania się i szybkość ataku.
    /// </summary>
    SpeedIncrease,
    
    /// <summary>
    /// Zwiększa szansę na trafienie krytyczne.
    /// </summary>
    CritChanceIncrese,
    
    /// <summary>
    /// Zwiększa szansę na uniknięcie ataku.
    /// </summary>
    DodgeIncrease,
    
    /// <summary>
    /// Zwiększa celność, zmniejszając szansę na chybienie.
    /// </summary>
    AccuracyIncrease
}