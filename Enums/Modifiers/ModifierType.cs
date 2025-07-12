namespace GodmistWPF.Enums.Modifiers;

/// <summary>
/// Określa typ modyfikatora, który wpływa na sposób obliczania wartości statystyk.
/// </summary>
public enum ModifierType
{
    /// <summary>
    /// Względny - modyfikator jest procentem bazowej wartości.
    /// Przykład: +10% do siły.
    /// </summary>
    Relative,
    
    /// <summary>
    /// Dodatkowy - modyfikator dodaje stałą wartość do wyniku.
    /// Przykład: +5 do siły.
    /// </summary>
    Additive,
    
    /// <summary>
    /// Mnożący - modyfikator mnoży końcową wartość przez podany współczynnik.
    /// Stosowane po wszystkich innych modyfikatorach.
    /// </summary>
    Multiplicative,
    
    /// <summary>
    /// Bezwzględny - ustawia wartość na dokładnie określoną, ignorując inne modyfikatory.
    /// Przykład: ustawia siłę na dokładnie 20, niezależnie od innych modyfikatorów.
    /// </summary>
    Absolute
}