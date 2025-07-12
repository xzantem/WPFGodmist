

using GodmistWPF.Enums.Items;

namespace GodmistWPF.Items.Potions;

/// <summary>
/// Klasa reprezentująca miksturę w grze, którą można użyć, aby uzyskać różnorodne efekty.
/// Mikstury mogą składać się z wielu komponentów i katalizatora, które wpływają na ich działanie.
/// </summary>
public class Potion : BaseItem, IUsable
{
    /// <summary>
    /// Pobiera nazwę mikstury, uwzględniającą jej komponenty i liczbę ładunków.
    /// Jeśli alias to "Potion", generowana jest nazwa na podstawie efektów komponentów.
    /// W przeciwnym razie używany jest alias z dołączoną informacją o ładunkach.
    /// </summary>
    public override string Name
    {
        get
        {
            string baseName = Alias == "Potion" 
                ? PotionManager.GetPotionName(Components.Select(x => x.Effect).ToList(), 
                    Components.Max(x => x.StrengthTier))
                : Alias;
                
            return $"{baseName} ({CurrentCharges}/{MaximalCharges})";
        }
    }
    /// <summary>
    /// Pobiera wagę mikstury (zawsze 2 jednostki).
    /// </summary>
    public override int Weight => 2;
    /// <summary>
    /// Pobiera unikalny identyfikator mikstury.
    /// </summary>
    public override int ID => 561;
    /// <summary>
    /// Określa, czy mikstury mogą być składowane w stosie (zawsze false).
    /// </summary>
    public override bool Stackable => false;

    /// <summary>
    /// Oblicza koszt mikstury na podstawie jej komponentów, katalizatora i liczby ładunków.
    /// Koszt zależy od wartości komponentów, katalizatora oraz procentowego zużycia mikstury.
    /// </summary>
    public override int Cost =>
        (int)(0.5 * (1 + (double)CurrentCharges / MaximalCharges) * (Components.Sum(x => ItemManager
            .GetItem(x.Material).Cost) * 3 + (Catalyst == null ? 0 : ItemManager.GetItem(Catalyst.Material).Cost)));
    /// <summary>
    /// Pobiera typ przedmiotu (zawsze ItemType.Potion).
    /// </summary>
    public override ItemType ItemType => ItemType.Potion;
    
    /// <summary>
    /// Lista komponentów mikstury, z których każdy dodaje określony efekt.
    /// </summary>
    public List<PotionComponent> Components { get; set; }
    
    /// <summary>
    /// Katalizator mikstury, który może modyfikować jej właściwości.
    /// Może być null, jeśli mikstura nie ma katalizatora.
    /// </summary>
    public PotionCatalyst? Catalyst { get; set; }
    /// <summary>
    /// Pobiera lub ustawia aktualną liczbę ładunków (użyć) mikstury.
    /// </summary>
    public int CurrentCharges { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia maksymalną liczbę ładunków mikstury.
    /// </summary>
    public int MaximalCharges { get; set; }

    /// <summary>
    /// Inicjalizuje nową instancję klasy Potion z określonymi parametrami.
    /// </summary>
    /// <param name="alias">Alias mikstury używany do identyfikacji.</param>
    /// <param name="components">Lista komponentów mikstury.</param>
    /// <param name="catalyst">Katalizator mikstury (może być null).</param>
    public Potion(string alias, List<PotionComponent> components, 
        PotionCatalyst catalyst)
    {
        Alias = alias;
        Rarity = components.Max(x => ItemManager.GetItem(x.Material).Rarity);
        Components = components;
        Catalyst = catalyst;
        MaximalCharges = CurrentCharges = 5 + (catalyst == null ? 0 :(Catalyst.Effect == PotionCatalystEffect.Capacity ? Catalyst.Tier : 0));
    }
    /// <summary>
    /// Konstruktor domyślny wymagany do deserializacji.
    /// </summary>
    public Potion() {}

    /// <summary>
    /// Używa mikstury, aktywując efekty jej komponentów.
    /// </summary>
    /// <returns>Zawsze zwraca false, co oznacza, że przedmiot nie jest zużywany po użyciu.</returns>
    public bool Use()
    {
        if (CurrentCharges <= 0) return false;
        CurrentCharges--;
        //Add alcohol poisoning
        foreach (var component in Components)
        {
            PotionManager.ProcessComponent(component, Catalyst);
        }
        return false;
    }

    /// <summary>
    /// Uzupełnia liczbę ładunków mikstury o określoną wartość, nie przekraczając maksymalnej liczby ładunków.
    /// </summary>
    /// <param name="amount">Liczba ładunków do dodania.</param>
    public void Refill(int amount)
    {
        CurrentCharges = Math.Min(MaximalCharges, CurrentCharges + amount);
    }

    /// <summary>
    /// Wyświetla szczegółowe informacje o miksturze.
    /// </summary>
    /// <param name="amount">Ilość przedmiotów (nieużywane).</param>
    public override void Inspect(int amount = 1)
    {
        base.Inspect(amount);
        // WPF handles potion inspection display through UI
    }
}