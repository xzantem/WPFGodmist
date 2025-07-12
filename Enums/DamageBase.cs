namespace GodmistWPF.Enums;

/// <summary>
/// Określa podstawę obliczania obrażeń w systemie walki.
/// </summary>
public enum DamageBase
{
    /// <summary>
    /// Stała wartość obrażeń.
    /// </summary>
    Flat,
    
    /// <summary>
    /// Minimalna możliwa wartość losowych obrażeń.
    /// </summary>
    Minimal,
    
    /// <summary>
    /// Losowa wartość obrażeń z zadanego przedziału.
    /// </summary>
    Random,
    
    /// <summary>
    /// Maksymalna możliwa wartość losowych obrażeń.
    /// </summary>
    Maximal,
    
    /// <summary>
    /// Procent maksymalnego zdrowia rzucającego.
    /// </summary>
    CasterMaxHealth,
    
    /// <summary>
    /// Procent maksymalnego zdrowia celu.
    /// </summary>
    TargetMaxHealth,
    
    /// <summary>
    /// Procent aktualnego zdrowia rzucającego.
    /// </summary>
    CasterCurrentHealth,
    
    /// <summary>
    /// Procent aktualnego zdrowia celu.
    /// </summary>
    TargetCurrentHealth,
    
    /// <summary>
    /// Procent brakującego zdrowia rzucającego.
    /// </summary>
    CasterMissingHealth,
    
    /// <summary>
    /// Procent brakującego zdrowia celu.
    /// </summary>
    TargetMissingHealth
}