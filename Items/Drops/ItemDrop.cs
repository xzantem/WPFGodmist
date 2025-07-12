namespace GodmistWPF.Items.Drops;

/// <summary>
/// Klasa reprezentująca pojedynczy przedmiot, który może wypaść z przeciwnika lub skrzynki.
/// Określa parametry przedmiotu, takie jak ilość i poziomy, na których może się pojawić.
/// </summary>
public class ItemDrop
{
    /// <summary>
    /// Minimalna ilość przedmiotów, która może wypaść.
    /// </summary>
    public int MinAmount { get; set; }
    
    /// <summary>
    /// Maksymalna ilość przedmiotów, która może wypaść.
    /// </summary>
    public int MaxAmount { get; set; }
    
    /// <summary>
    /// Minimalny poziom, od którego przedmiot może się pojawić.
    /// </summary>
    public int MinLevel { get; set; }
    
    /// <summary>
    /// Maksymalny poziom, do którego przedmiot może się pojawić.
    /// </summary>
    public int MaxLevel { get; set; }
    
    /// <summary>
    /// Waga określająca szansę na wypadnięcie przedmiotu w porównaniu do innych przedmiotów w puli.
    /// </summary>
    public int Weight { get; set; }
    /// <summary>
    /// Konstruktor domyślny wymagany do deserializacji JSON.
    /// </summary>
    public ItemDrop() { }
    /// <summary>
    /// Inicjalizuje nową instancję klasy ItemDrop z określonymi parametrami.
    /// </summary>
    /// <param name="minLevel">Minimalny poziom, od którego przedmiot może się pojawić.</param>
    /// <param name="maxLevel">Maksymalny poziom, do którego przedmiot może się pojawić.</param>
    /// <param name="weight">Waga określająca szansę na wypadnięcie przedmiotu.</param>
    /// <param name="minAmount">Minimalna ilość przedmiotów (domyślnie 1).</param>
    /// <param name="maxAmount">Maksymalna ilość przedmiotów (domyślnie 1).</param>
    public ItemDrop(int minLevel, int maxLevel, int weight, int minAmount = 1, int maxAmount = 1)
    {
        MinLevel = minLevel;
        MaxLevel = maxLevel;
        MinAmount = minAmount;
        MaxAmount = maxAmount;
        Weight = weight;
    }
}