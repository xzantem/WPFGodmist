namespace GodmistWPF.Enums;

/// <summary>
/// Określa typ przeciwnika w grze.
/// </summary>
public enum EnemyType
{
    /// <summary>
    /// Nieumarły - odporny na truciznę, słaby na ogień.
    /// </summary>
    Undead,
    
    /// <summary>
    /// Bestia - dzikie zwierzę, szybkie i agresywne.
    /// </summary>
    Beast,
    
    /// <summary>
    /// Człowiek - zrównoważony przeciwnik z różnorodnymi umiejętnościami.
    /// </summary>
    Human,
    
    /// <summary>
    /// Demon - potężny przeciwnik z odpornością na magię.
    /// </summary>
    Demon,
    
    /// <summary>
    /// Boss - wyjątkowo potężny przeciwnik, często z unikalnymi zdolnościami.
    /// </summary>
    Boss
}