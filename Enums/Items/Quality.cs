namespace GodmistWPF.Enums.Items;

/// <summary>
/// Określa jakość przedmiotu, która wpływa na jego statystyki i wartość.
/// </summary>
public enum Quality
{
    /// <summary>
    /// Słaby - przedmiot o obniżonych statystykach.
    /// </summary>
    Weak,
    
    /// <summary>
    /// Normalny - standardowe statystyki przedmiotu.
    /// </summary>
    Normal,
    
    /// <summary>
    /// Doskonały - przedmiot o zwiększonych statystykach.
    /// </summary>
    Excellent,
    
    /// <summary>
    /// Arcydzieło - przedmiot o maksymalnych możliwych statystykach.
    /// </summary>
    Masterpiece
}