using GodmistWPF.Enums.Items;
using GodmistWPF.Utilities;

namespace GodmistWPF.Items;

/// <summary>
/// Abstrakcyjna klasa bazowa dla wszystkich przedmiotów w grze.
/// Implementuje podstawową funkcjonalność zdefiniowaną w interfejsie IItem.
/// </summary>
public abstract class BaseItem : IItem
{
    /// <summary>
    /// Pobiera lub ustawia nazwę przedmiotu.
    /// Przy pobieraniu wykorzystuje NameAliasHelper do tłumaczenia aliasu na pełną nazwę.
    /// </summary>
    public virtual string Name
    {
        get => NameAliasHelper.GetName(Alias);
        set => Alias = value;
    }

    /// <summary>
    /// Pobiera lub ustawia alias (skróconą nazwę) przedmiotu.
    /// Używany do wewnętrznej identyfikacji i tłumaczeń.
    /// </summary>
    public virtual string Alias { get; set; }

    /// <summary>
    /// Pobiera lub ustawia wagę przedmiotu.
    /// Wpływa na obciążenie ekwipunku postaci.
    /// </summary>
    public virtual int Weight { get; set; }

    /// <summary>
    /// Pobiera lub ustawia unikalny identyfikator przedmiotu.
    /// Używany do zapisu/odczytu stanu gry.
    /// </summary>
    public virtual int ID { get; set; }

    /// <summary>
    /// Pobiera lub ustawia wartość przedmiotu w walucie gry.
    /// Używane przy sprzedaży i zakupie przedmiotów.
    /// </summary>
    public virtual int Cost { get; set; }

    /// <summary>
    /// Pobiera lub ustawia rzadkość przedmiotu.
    /// Wpływa na kolor nazwy i wartość przedmiotu.
    /// </summary>
    public virtual ItemRarity Rarity { get; set; }

    /// <summary>
    /// Określa, czy przedmioty tego samego typu mogą być składane w jednym slocie ekwipunku.
    /// </summary>
    public virtual bool Stackable { get; set; }

    /// <summary>
    /// Pobiera lub ustawia opis przedmiotu.
    /// Wyświetlany w oknie inspekcji przedmiotu.
    /// </summary>
    public virtual string Description { get; set; }

    /// <summary>
    /// Pobiera lub ustawia typ przedmiotu.
    /// Określa kategorię i podstawowe przeznaczenie przedmiotu.
    /// </summary>
    public virtual ItemType ItemType { get; set; }

    /// <summary>
    /// Wyświetla szczegółowe informacje o przedmiocie.
    /// Implementacja w WPF obsługuje interfejs użytkownika.
    /// </summary>
    /// <param name="amount">Ilość przedmiotów do wyświetlenia (dla przedmiotów składalnych).</param>
    public virtual void Inspect(int amount = 1)
    {
        // WPF handles item inspection UI
    }

    /// <summary>
    /// Wyświetla nazwę przedmiotu z uwzględnieniem jego rzadkości.
    /// Implementacja w WPF obsługuje interfejs użytkownika.
    /// </summary>
    public void WriteName()
    {
        // WPF handles item name display UI
    }

    /// <summary>
    /// Wyświetla typ przedmiotu.
    /// Implementacja w WPF obsługuje interfejs użytkownika.
    /// </summary>
    public void WriteItemType()
    {
        // WPF handles item type display UI
    }

    /// <summary>
    /// Wyświetla rzadkość przedmiotu.
    /// Implementacja w WPF obsługuje interfejs użytkownika.
    /// </summary>
    public void WriteRarity()
    {
        // WPF handles item rarity display UI
    }

    /// <summary>
    /// Zwraca zlokalizowaną nazwę rzadkości przedmiotu.
    /// </summary>
    /// <returns>Zlokalizowana nazwa rzadkości.</returns>
    public string RarityName()
    {
        return Rarity switch
        {
            ItemRarity.Common => locale.Common,
            ItemRarity.Uncommon => locale.Uncommon,
            ItemRarity.Rare => locale.Rare,
            ItemRarity.Ancient => locale.Ancient,
            ItemRarity.Legendary => locale.Legendary,
            ItemRarity.Mythical => locale.Mythical,
            ItemRarity.Godly => locale.Godly,
            ItemRarity.Destroyed => locale.Destroyed,
            ItemRarity.Damaged => locale.Damaged,
            ItemRarity.Junk => locale.Junk,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// Zwraca zlokalizowaną nazwę typu przedmiotu.
    /// </summary>
    /// <returns>Zlokalizowana nazwa typu przedmiotu.</returns>
    public string ItemTypeName()
    {
        return ItemType switch
        {
            ItemType.Weapon => locale.Weapon,
            ItemType.Armor => locale.Armor,
            ItemType.Smithing => locale.Smithing,
            ItemType.Alchemy => locale.Alchemy,
            ItemType.Potion => locale.Potion,
            ItemType.Runeforging => locale.Runeforging,
            ItemType.WeaponGaldurite => locale.WeaponGaldurite,
            ItemType.ArmorGaldurite => locale.ArmorGaldurite,
            ItemType.LootBag => locale.LootBag,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}