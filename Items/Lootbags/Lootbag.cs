using GodmistWPF.Enums.Items;
using GodmistWPF.Items.Drops;
using GodmistWPF.Utilities;

namespace GodmistWPF.Items.Lootbags;

/// <summary>
/// Klasa bazowa reprezentująca worek z łupami, który można otworzyć, aby otrzymać przedmioty.
/// Implementuje interfejs IUsable, co oznacza, że można go użyć w grze.
/// </summary>
public class Lootbag : BaseItem, IUsable
{
    /// <summary>
    /// Pobiera nazwę worka z łupami na podstawie jego aliasu.
    /// </summary>
    public new string Name => NameAliasHelper.GetName(Alias);
    
    /// <summary>
    /// Pobiera wagę worka (zawsze 0, ponieważ jest to lekki przedmiot).
    /// </summary>
    public override int Weight => 0;
    
    /// <summary>
    /// Określa, czy worki mogą być składowane w stosie (zawsze true).
    /// </summary>
    public override bool Stackable => true;
    
    /// <summary>
    /// Pobiera typ przedmiotu (zawsze LootBag).
    /// </summary>
    public override ItemType ItemType => ItemType.LootBag;
    
    /// <summary>
    /// Pobiera unikalny identyfikator przedmiotu na podstawie jego aliasu.
    /// </summary>
    /// <value>
    /// Identyfikator numeryczny przypisany do każdego typu worka z łupami.
    /// </value>
    public override int ID => Alias switch
    {
        "BonySupplyBag" => 563,
        "LeafySupplyBag" => 564,
        "DemonicSupplyBag" => 565,
        "PirateSupplyBag" => 566,
        "SandySupplyBag" => 567,
        "TempleSupplyBag" => 568,
        "MountainousSupplyBag" => 569,
        "WeaponBag" => 570,
        "ArmorBag" => 571,
        "GalduriteBag" => 572,
        "SkeletonExecutionerBag" => 573,
        _ => throw new ArgumentException($"Unknown lootbag alias: {Alias}")
    };
    /// <summary>
    /// Pobiera lub ustawia tabelę przedmiotów, które mogą wypaść z worka.
    /// </summary>
    public DropTable DropTable { get; set; }
    /// <summary>
    /// Inicjalizuje nową instancję klasy Lootbag.
    /// </summary>
    /// <param name="alias">Alias worka, określający jego typ.</param>
    /// <param name="level">Poziom worka, wpływający na rzadkość przedmiotów.</param>
    /// <param name="dropTable">Tabela przedmiotów, które mogą wypaść z worka.</param>
    public Lootbag(string alias, int level, DropTable dropTable)
    {
        Alias = alias;
        Level = level;
        DropTable = dropTable;
    }

    /// <summary>
    /// Chroniony konstruktor bezparametrowy używany przez mechanizmy serializacji.
    /// </summary>
    protected Lootbag()
    {
    }

    /// <summary>
    /// Pobiera rzadkość worka na podstawie jego poziomu.
    /// </summary>
    /// <value>
    /// Rzadkość określana na podstawie poziomu:
    /// - 1-10: Common
    /// - 11-20: Uncommon
    /// - 21-30: Rare
    /// - 31-40: Ancient
    /// - 41+: Legendary
    /// </value>
    public override ItemRarity Rarity => Level switch
    {
        <=10 => ItemRarity.Common,
        >10 and <= 20 => ItemRarity.Uncommon,
        >20 and <= 30 => ItemRarity.Rare,
        >30 and <= 40 => ItemRarity.Ancient,
        _ => ItemRarity.Legendary
    };
    /// <summary>
    /// Pobiera lub ustawia poziom worka, który wpływa na jakość przedmiotów.
    /// </summary>
    public int Level { get; set; }
    /// <summary>
    /// Próbuje użyć worka, co powoduje wyświetlenie interfejsu użytkownika do jego otwarcia.
    /// </summary>
    /// <returns>Zawsze zwraca false, ponieważ obsługa interfejsu użytkownika odbywa się w warstwie WPF.</returns>
    public bool Use()
    {
        // WPF handles lootbag opening UI
        return false;
    }
}