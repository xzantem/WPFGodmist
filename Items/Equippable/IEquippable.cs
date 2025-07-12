
using GodmistWPF.Enums;
using GodmistWPF.Enums.Items;
using GodmistWPF.Items.Galdurites;

namespace GodmistWPF.Items.Equippable;

/// <summary>
/// Interfejs reprezentujący przedmiot, który może być założony przez postać.
/// Rozszerza interfejs IItem o właściwości specyficzne dla ekwipunku.
/// </summary>
public interface IEquippable : IItem
{
    /// <summary>
    /// Pobiera wymagany poziom postaci do założenia przedmiotu.
    /// </summary>
    public int RequiredLevel { get; }
    
    /// <summary>
    /// Pobiera wymaganą klasę postaci do założenia przedmiotu.
    /// </summary>
    public CharacterClass RequiredClass { get; }
    
    /// <summary>
    /// Pobiera jakość przedmiotu, która wpływa na jego statystyki.
    /// </summary>
    public Quality Quality { get; }
    
    /// <summary>
    /// Pobiera lub ustawia modyfikator ulepszenia przedmiotu.
    /// Wpływa na liczbę gniazd na galaduryty i ogólną moc przedmiotu.
    /// </summary>
    public double UpgradeModifier { get; set; }
    
    /// <summary>
    /// Pobiera bazowy koszt przedmiotu, przed zastosowaniem modyfikatorów.
    /// </summary>
    public int BaseCost { get; }
    
    /// <summary>
    /// Pobiera lub ustawia listę galadurytów osadzonych w przedmiocie.
    /// </summary>
    public List<Galdurite> Galdurites { get; set; }

    /// <summary>
    /// Pobiera modyfikator rzadkości przedmiotu, który wpływa na jego statystyki.
    /// </summary>
    /// <value>
    /// Wartość modyfikatora zależna od rzadkości przedmiotu:
    /// - Zniszczony: 0.7
    /// - Uszkodzony: 0.85
    /// - Złom: 1.05
    /// - Zwykły: 1.0
    /// - Niezwykły: 1.05
    /// - Rzadki: 1.1
    /// - Starożytny: 1.2
    /// - Legendarny: 1.3
    /// - Mityczny: 1.5
    /// - Boski: 1.75
    /// </value>
    public double RarityModifier
    {
        get
        {
            return Rarity switch
            {
                ItemRarity.Destroyed => 0.7,
                ItemRarity.Damaged => 0.85,
                ItemRarity.Junk => 1.05,
                ItemRarity.Common => 1,
                ItemRarity.Uncommon => 1.05,
                ItemRarity.Rare => 1.1,
                ItemRarity.Ancient => 1.2,
                ItemRarity.Legendary => 1.3,
                ItemRarity.Mythical => 1.5,
                ItemRarity.Godly => 1.75,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    /// <summary>
    /// Oblicza liczbę dostępnych gniazd na galaduryty na podstawie modyfikatora ulepszenia.
    /// </summary>
    /// <remarks>
    /// Każde 0.2 modyfikatora ulepszenia daje jedno dodatkowe gniazdo.
    /// Minimalna wartość modyfikatora to 1.0 (0 gniazd).
    /// </remarks>
    public int GalduriteSlots => (int)Math.Floor((UpgradeModifier - 1) / 0.2);

}