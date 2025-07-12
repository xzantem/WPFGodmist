namespace GodmistWPF.Enums;

/// <summary>
/// Określa cel umiejętności w systemie walki.
/// </summary>
public enum SkillTarget
{
    /// <summary>
    /// Umiejętność działa na postać, która ją rzuca.
    /// Używane np. do leczenia lub wzmocnień.
    /// </summary>
    Self,
    /// <summary>
    /// Umiejętność działa na wrogą postać.
    /// Używane np. do ataków i przekleństw.
    /// </summary>
    Enemy
}