using GodmistWPF.Enums.Items;
using GodmistWPF.Items.Equippable;
using GodmistWPF.Utilities;

namespace GodmistWPF.Items.Galdurites;

/// <summary>
/// Klasa reprezentująca kamień galdurytowy, który może zostać osadzony w przedmiotach.
/// Zawiera komponenty z efektami, które są aktywowane po osadzeniu w przedmiocie.
/// </summary>
public class Galdurite : BaseItem
{
    /// <summary>
    /// Pobiera nazwę galduritu z systemu lokalizacji.
    /// </summary>
    public new string Name => NameAliasHelper.GetName(Alias);
    
    /// <summary>
    /// Pobiera wagę galduritu (zawsze 1).
    /// </summary>
    public override int Weight => 1;
    
    /// <summary>
    /// Pobiera unikalny identyfikator galduritu.
    /// </summary>
    public override int ID => 562;
    
    /// <summary>
    /// Określa, czy galdurity mogą być składowane w stosie (zawsze false).
    /// </summary>
    public override bool Stackable => false;
    
    /// <summary>
    /// Tablica komponentów galduritu, z których każdy zawiera efekt i jego parametry.
    /// </summary>
    public GalduriteComponent[] Components { get; set; }
    
    /// <summary>
    /// Pobiera koszt galduritu, uwzględniający rzadkość, poziom ujawnienia i jakość efektów.
    /// </summary>
    public override int Cost => (int)(BaseCost * EquippableItemService.RarityPriceModifier(Rarity) * (Revealed ? 
                                      Components.Aggregate(1.0, (x, y) => 
                                          x * ("DCBAS".IndexOf(y.EffectTier) + 18) / 20) : 1));
    
    /// <summary>
    /// Pobiera lub ustawia bazowy koszt galduritu bez modyfikatorów.
    /// </summary>
    public int BaseCost { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia poziom (tier) galduritu (1-3).
    /// </summary>
    public int Tier { get; set; }
    
    /// <summary>
    /// Pobiera wymagany poziom postaci do użycia galduritu.
    /// Tier 1: 11 poziom, Tier 2: 21 poziom, Tier 3: 41 poziom.
    /// </summary>
    public int RequiredLevel => Tier == 3 ? 41 : Tier * 10 + 1;
    
    /// <summary>
    /// Określa, czy efekty galduritu zostały ujawnione.
    /// </summary>
    public bool Revealed { get; set; }
    
    /// <summary>
    /// Konstruktor domyślny wymagany do serializacji.
    /// </summary>
    public Galdurite()
    {
    }

    /// <summary>
    /// Inicjalizuje nową instancję klasy Galdurite z losowymi właściwościami.
    /// </summary>
    /// <param name="equipmentType">Typ przedmiotu, do którego pasuje galdurit (true = zbroja, false = broń).</param>
    /// <param name="tier">Poziom (tier) galduritu (1-3).</param>
    /// <param name="bias">Modyfikator szansy na lepszą rzadkość galduritu.</param>
    /// <param name="color">Kolor galduritu (domyślnie losowy).</param>
    /// <exception cref="ArgumentOutOfRangeException">Występuje, gdy podano nieprawidłowy tier.</exception>
    public Galdurite(bool equipmentType, int tier, int bias, string color = "Random")
    {
        ItemType = equipmentType ? ItemType.ArmorGaldurite : ItemType.WeaponGaldurite;
        Alias = (equipmentType, tier) switch
        {
            (false, 1) => "MeagreWeaponGaldurite",
            (false, 2) => "FairWeaponGaldurite",
            (false, 3) => "PotentWeaponGaldurite",
            (true, 1) => "MeagreArmorGaldurite",
            (true, 2) => "FairArmorGaldurite",
            (true, 3) => "PotentArmorGaldurite",
            _ => throw new ArgumentOutOfRangeException(nameof(equipmentType), equipmentType, null)
        };
        Revealed = false;
        Rarity = EquippableItemService.GetRandomGalduriteRarity(bias);
        Tier = tier;
        BaseCost = Tier switch
        {
            1 => 500,
            2 => 1000,
            3 => 12000,
            _ => throw new ArgumentOutOfRangeException(nameof(tier), tier, null)
        };
        var componentCount = Random.Shared.Next(1, 3) + Tier;
        Components = new GalduriteComponent[componentCount];
        var tiers = new List<string>();
        for (var i = 0; i < componentCount; i++)
        {
            tiers.Add(Rarity switch
            {
                ItemRarity.Common => UtilityMethods.RandomChoice(new Dictionary<string, double>
                    { { "D", 0.5 }, { "C", 0.5 } }),
                ItemRarity.Uncommon => UtilityMethods.RandomChoice(new Dictionary<string, double>
                    { { "D", 0.25 }, { "C", 0.5 }, { "B", 0.25 } }),
                ItemRarity.Rare => UtilityMethods.RandomChoice(new Dictionary<string, double>
                    { { "C", 0.5 }, { "B", 0.25 }, { "A", 0.25 } }),
                ItemRarity.Ancient => UtilityMethods.RandomChoice(new Dictionary<string, double>
                    { { "C", 0.25 }, { "B", 0.5 }, { "A", 0.25 } }),
                ItemRarity.Legendary => UtilityMethods.RandomChoice(new Dictionary<string, double>
                    { { "B", 0.5 }, { "A", 0.25 }, { "S", 0.25 } }),
                ItemRarity.Mythical => UtilityMethods.RandomChoice(new Dictionary<string, double>
                    { { "B", 0.25 }, { "A", 0.5 }, { "S", 0.25 } }),
                ItemRarity.Godly => UtilityMethods.RandomChoice(new Dictionary<string, double>
                    { { "A", 0.5 }, { "S", 0.5 } }),
            });
        }
        tiers.Sort((a, b) => "SABCD".IndexOf(a).CompareTo("SABCD".IndexOf(b)));
        var excludedColors = new HashSet<string>();
        for (var i = 0; i < componentCount; i++)
        {
            Components[i] = GalduriteManager.GetGalduriteComponent(tiers[i], i == 0 ? color : "Random", equipmentType, excludedColors);
            excludedColors.Add(Components[i].PoolColor);
        }
    }

    /// <summary>
    /// Ujawnia efekty galduritu, jeśli nie były wcześniej ujawnione.
    /// </summary>
    public void Reveal()
    {
        Revealed = true;
    }

    /// <summary>
    /// Wyświetla szczegółowe informacje o galduricie, w tym jego efekty.
    /// </summary>
    /// <param name="amount">Ilość przedmiotów (nieużywane).</param>
    public override void Inspect(int amount = 1)
    {
        base.Inspect(amount);
        ShowEffects();
    }

    /// <summary>
    /// Generuje tekst z opisem wszystkich efektów galduritu.
    /// </summary>
    /// <returns>Tekst zawierający opisy wszystkich efektów, oddzielone znakami nowej linii.</returns>
    public string ShowEffects()
    {
        return Components.Aggregate(string.Empty, (current, component) => current + (component.EffectText + "\n"));
    }
}