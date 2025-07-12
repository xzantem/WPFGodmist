namespace GodmistWPF.Items;

/// <summary>
/// Interfejs określający zachowanie przedmiotów, które mogą być używane przez postać.
/// </summary>
public interface IUsable
{
    /// <summary>
    /// Używa przedmiotu.
    /// </summary>
    /// <returns>Zwraca true, jeśli użycie przedmiotu się powiodło, w przeciwnym razie false.</returns>
    public bool Use();
}