namespace GodmistWPF.Enums.Items;

/// <summary>
/// Określa efekt katalizatora, który może być zastosowany do mikstury.
/// </summary>
public enum PotionCatalystEffect
{
    /// <summary>
    /// Zwiększa czas trwania efektów mikstury.
    /// </summary>
    Duration,
    
    /// <summary>
    /// Zwiększa siłę efektów mikstury.
    /// </summary>
    Strength,
    
    /// <summary>
    /// Zwiększa liczbę dawek w miksturze.
    /// </summary>
    Condensation,
    
    /// <summary>
    /// Zwiększa maksymalną pojemność mikstury.
    /// </summary>
    Capacity
}