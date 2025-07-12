using GodmistWPF.Characters.Player;
using GodmistWPF.Combat.Modifiers.PassiveEffects;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Items;
using GodmistWPF.Items.Galdurites;
using GodmistWPF.Utilities;
using Newtonsoft.Json;

namespace GodmistWPF.Items.Equippable.Weapons;

/// <summary>
/// Reprezentuje broń, którą może wyposażyć postać.
/// Składa się z trzech komponentów: głowicy, oprawy i rękojeści.
/// Implementuje interfejsy IEquippable i IUsable.
/// </summary>
public class Weapon : BaseItem, IEquippable, IUsable
{
    // Implementacja interfejsu IItem
    
    /// <summary>
    /// Nazwa broni.
    /// </summary>
    public override string Name { get; set; }
    
    /// <summary>
    /// Waga broni.
    /// </summary>
    [JsonIgnore]
    public override int Weight => 5;
    
    /// <summary>
    /// Unikalny identyfikator broni.
    /// </summary>
    [JsonIgnore]
    public override int ID => 559;
    
    /// <summary>
    /// Bazowy koszt broni przed uwzględnieniem modyfikatorów jakości i rzadkości.
    /// </summary>
    public int BaseCost { get; set; }

    /// <summary>
    /// Koszt broni uwzględniający modyfikator rzadkości.
    /// </summary>
    [JsonIgnore]
    public override int Cost => (int)(BaseCost * EquippableItemService.RarityPriceModifier(Rarity));
    
    /// <summary>
    /// Określa, czy przedmiot może być składowany w stosie.
    /// Dla broni zawsze zwraca false.
    /// </summary>
    [JsonIgnore]
    public override bool Stackable => false;
    
    /// <summary>
    /// Typ przedmiotu - zawsze zwraca ItemType.Weapon.
    /// </summary>
    [JsonIgnore]
    public override ItemType ItemType => ItemType.Weapon;
    
    // Implementacja interfejsu IEquippable
    
    /// <summary>
    /// Wymagany poziom postaci do założenia broni.
    /// </summary>
    public int RequiredLevel { get; set; }
    
    /// <summary>
    /// Klasa postaci, która może używać tej broni.
    /// </summary>
    public CharacterClass RequiredClass { get; set; }
    
    /// <summary>
    /// Jakość wykonania broni, wpływająca na jej statystyki.
    /// </summary>
    public Quality Quality { get; set; }
    
    /// <summary>
    /// Modyfikator ulepszenia broni, wpływający na jej statystyki.
    /// </summary>
    public double UpgradeModifier { get; set; }
    
    /// <summary>
    /// Lista galdurytów wpiętych w broń.
    /// </summary>
    public List<Galdurite> Galdurites { get; set; }
    
    /// <summary>
    /// Liczba dostępnych gniazd na galduryty, zależna od poziomu ulepszenia.
    /// </summary>
    public int GalduriteSlots => (int)Math.Floor(UpgradeModifier * 5 - 5);

    // Właściwości specyficzne dla broni
    
    /// <summary>
    /// Głowica broni, określająca podstawowe obrażenia i modyfikatory krytyczne.
    /// </summary>
    public WeaponHead Head { get; set; }
    
    /// <summary>
    /// Oprawa broni, wpływająca na dodatkowe modyfikatory obrażeń.
    /// </summary>
    public WeaponBinder Binder { get; set; }
    
    /// <summary>
    /// Rękojeść broni, wpływająca na celność i dodatkowe modyfikatory.
    /// </summary>
    public WeaponHandle Handle { get; set; }
    
    /// <summary>
    /// Minimalna wartość obrażeń zadawanych przez broń.
    /// Obliczana na podstawie głowicy, klasy postaci i jakości broni.
    /// </summary>
    [JsonIgnore]
    public int MinimalAttack
    {
        get
        {
            double value = Head.MinimalAttack;
            var multiplier = RequiredClass switch
            {
                CharacterClass.Warrior => 1,
                CharacterClass.Scout => 0.4,
                CharacterClass.Sorcerer => 0.8,
                _ => 0.4
            };
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            value += multiplier * (Head.Tier * 10 - 5);
            value *= UpgradeModifier * (Handle.AttackBonus + Binder.AttackBonus + 1);
            return (int)value;
        }
    }
    
    /// <summary>
    /// Maksymalna wartość obrażeń zadawanych przez broń.
    /// Obliczana na podstawie głowicy, klasy postaci i jakości broni.
    /// </summary>
    [JsonIgnore]
    public int MaximalAttack
    {
        get
        {
            double value = Head.MaximalAttack;
            var multiplier = RequiredClass switch
            {
                CharacterClass.Warrior => 2,
                CharacterClass.Scout => 2,
                CharacterClass.Sorcerer => 2.4,
                _ => 1.2
            };
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            value += multiplier * (Head.Tier * 10 - 5);
            value *= UpgradeModifier * (Handle.AttackBonus + Binder.AttackBonus + 1);
            return (int)value;
        }
    }
    
    /// <summary>
    /// Szansa na trafienie krytyczne (dla klasy Maga zwraca regenerację many).
    /// </summary>
    [JsonIgnore]
    public double CritChance // Mage gains Mana regen instead of CritChance
    {
        get
        {
            var value = Binder.CritChance;
            var multiplier = RequiredClass switch
            {
                CharacterClass.Warrior => 0.01,
                CharacterClass.Scout => 0.01,
                CharacterClass.Sorcerer => 0.05,
                _ => 0.01
            };
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            if (RequiredClass == CharacterClass.Sorcerer)
                multiplier *= Binder.Tier * 10 - 5;
            value += multiplier;
            value *= EquippableItemService.RarityStatModifier(Rarity) * (Head.CritChanceBonus + Handle.CritChanceBonus + 1);
            return value;
        }
    }
    
    /// <summary>
    /// Modyfikator obrażeń krytycznych.
    /// </summary>
    [JsonIgnore]
    public double CritMod
    {
        get
        {
            var value = Head.CritMod;
            var multiplier = RequiredClass switch
            {
                CharacterClass.Warrior => 0.02,
                CharacterClass.Scout => 0.03,
                CharacterClass.Sorcerer => 0.02,
                _ => 0.02
            };
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            value += multiplier * (Head.Tier * 10 - 5);
            value *= EquippableItemService.RarityStatModifier(Rarity) * (Binder.CritModBonus + Handle.CritModBonus + 1);
            return value;
        }
    }
    
    /// <summary>
    /// Celność broni (dla klasy Maga zwraca maksymalną ilość many).
    /// </summary>
    [JsonIgnore]
    public int Accuracy // Mage gains Maximal Mana instead of Accuracy
    {
        get
        {
            double value = Handle.Accuracy;
            var multiplier = RequiredClass switch
            {
                CharacterClass.Warrior => 1,
                CharacterClass.Scout => 1,
                CharacterClass.Sorcerer => 0.3,
                _ => 2
            };
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            if (RequiredClass == CharacterClass.Sorcerer)
                multiplier *= Handle.Tier * 10 - 5;
            value += (int)multiplier;
            value *= EquippableItemService.RarityStatModifier(Rarity) * (Head.AccuracyBonus + Binder.AccuracyBonus + 1);
            return (int)value;
        }
    }

    /// <summary>
    /// Inicjalizuje nową instancję klasy Weapon z podanymi komponentami i parametrami.
    /// </summary>
    /// <param name="head">Głowica broni.</param>
    /// <param name="binder">Oprawa broni.</param>
    /// <param name="handle">Rękojeść broni.</param>
    /// <param name="requiredClass">Klasa postaci wymagana do używania broni.</param>
    /// <param name="quality">Jakość wykonania broni.</param>
    /// <param name="alias">Opcjonalny alias dla niestandardowej nazwy broni.</param>
    public Weapon(WeaponHead head, WeaponBinder binder, WeaponHandle handle, CharacterClass requiredClass, 
        Quality quality, string alias = "")
    {
        Head = head;
        Binder = binder;
        Handle = handle;
        Quality = quality;
        if (alias == "")
        {
            Name = NameAliasHelper.GetName(Head.Adjective+"Adj") + " " + requiredClass switch
            {
                CharacterClass.Warrior => locale.Longsword,
                CharacterClass.Scout => locale.SwordAndDagger,
                CharacterClass.Sorcerer => locale.Wand,
                _ => locale.Hammer
            } + quality switch
            {
                Quality.Weak => $" ({locale.Weak})",
                Quality.Excellent => $" ({locale.Excellent})",
                _ => ""
            };
            Alias = $"{head.Alias}.{binder.Alias}.{handle.Alias}";
        }
        else
        {
            Name = NameAliasHelper.GetName(alias);
            Alias = alias;
        }
        Rarity = EquippableItemService.GetRandomRarity(Quality == Quality.Masterpiece ? 7 : 0);
        BaseCost = (int)((head.MaterialCost * ItemManager.GetItem(head.Material).Cost + 
                   binder.MaterialCost * ItemManager.GetItem(binder.Material).Cost + 
                   handle.MaterialCost * ItemManager.GetItem(handle.Material).Cost) * Quality switch {
            Quality.Weak => 0.5,
            Quality.Normal => 1,
            Quality.Excellent => 2,
            Quality.Masterpiece => 4,
            _ => 1 });
        RequiredLevel = Math.Max(Math.Max(head.Tier, binder.Tier), handle.Tier) * 10 - 5 + Quality switch
        {
            Quality.Weak => -3,
            Quality.Normal => 0,
            Quality.Excellent => 3,
            Quality.Masterpiece => 5,
            _ => 0
        };
        RequiredClass = requiredClass;
        UpgradeModifier = 1;
        Galdurites = new List<Galdurite>();
    }
    /// <summary>
    /// Inicjalizuje nową instancję klasy Weapon jako broń startową dla określonej klasy postaci.
    /// </summary>
    /// <param name="requiredClass">Klasa postaci, dla której ma zostać utworzona broń startowa.</param>
    /// <exception cref="ArgumentOutOfRangeException">Wyrzucany, gdy podano nieobsługiwaną klasę postaci.</exception>
    /// <remarks>
    /// Dla każdej klasy postaci generowana jest inna broń startowa o odpowiednich właściwościach.
    /// </remarks>
    public Weapon(CharacterClass requiredClass)
    {
        switch (requiredClass)
        {
            case CharacterClass.Warrior:
                Head = EquipmentPartManager.GetPart<WeaponHead>("BrokenHead", CharacterClass.Warrior);
                Binder = EquipmentPartManager.GetPart<WeaponBinder>("BrokenBinder", CharacterClass.Warrior);
                Handle = EquipmentPartManager.GetPart<WeaponHandle>("BrokenHandle", CharacterClass.Warrior);
                Name = NameAliasHelper.GetName(Head.Adjective) + " " + locale.Longsword;
                break;
            case CharacterClass.Scout:
                Head = EquipmentPartManager.GetPart<WeaponHead>("RustyHead", CharacterClass.Scout);
                Binder = EquipmentPartManager.GetPart<WeaponBinder>("RustyBinder", CharacterClass.Scout);
                Handle = EquipmentPartManager.GetPart<WeaponHandle>("RustyHandle", CharacterClass.Scout);
                Name = NameAliasHelper.GetName(Head.Adjective) + " " + locale.SwordAndDagger;
                break;
            case CharacterClass.Sorcerer:
                Head = EquipmentPartManager.GetPart<WeaponHead>("SplinteryHead", CharacterClass.Sorcerer);
                Binder = EquipmentPartManager.GetPart<WeaponBinder>("SplinteryBinder", CharacterClass.Sorcerer);
                Handle = EquipmentPartManager.GetPart<WeaponHandle>("SplinteryHandle", CharacterClass.Sorcerer);
                Name = NameAliasHelper.GetName(Head.Adjective) + " " + locale.Wand;
                break;
            case CharacterClass.Paladin:
                Head = EquipmentPartManager.GetPart<WeaponHead>("MisshapenHead", CharacterClass.Paladin);
                Binder = EquipmentPartManager.GetPart<WeaponBinder>("MisshapenBinder", CharacterClass.Paladin);
                Handle = EquipmentPartManager.GetPart<WeaponHandle>("MisshapenHandle", CharacterClass.Paladin);
                Name = NameAliasHelper.GetName(Head.Adjective) + " " + locale.Hammer;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(requiredClass), requiredClass, null);
        }
        Alias = $"StarterWeapon.{requiredClass.ToString()}";
        BaseCost = 15;
        Rarity = ItemRarity.Junk;
        RequiredLevel = 1;
        RequiredClass = requiredClass;
        Quality = Quality.Normal;
        UpgradeModifier = 1;
        Galdurites = new List<Galdurite>();
    }
    /// <summary>
    /// Inicjalizuje nową, pustą instancję klasy Weapon.
    /// Konstruktor używany przez mechanizm deserializacji JSON.
    /// </summary>
    public Weapon() {}

    /// <summary>
    /// Próbuje wyposażyć broń na aktualnie wybranej postaci.
    /// </summary>
    /// <returns>
    /// True, jeśli udało się wyposażyć broń; w przeciwnym razie false.
    /// </returns>
    /// <remarks>
    /// Sprawdza, czy postać może używać tej broni (odpowiednia klasa i poziom).
    /// </remarks>
    public bool Use()
    {
        if (RequiredClass != PlayerHandler.player.CharacterClass)
            return false;
        if (RequiredLevel > PlayerHandler.player.Level)
            return false;
        PlayerHandler.player.SwitchWeapon(this);
        return true;
    }

    /// <summary>
    /// Aktualizuje efekty pasywne wynikające z galdurytów wpiętych w broń.
    /// </summary>
    /// <param name="player">Postać, której mają zostać zaktualizowane efekty.</param>
    /// <remarks>
    /// Usuwa stare efekty i dodaje nowe na podstawie aktualnie wpiętych galdurytów.
    /// </remarks>
    public virtual void UpdatePassives(PlayerCharacter player)
    {
        player.PassiveEffects.InnateEffects.RemoveAll(x => x.Source == "WeaponGaldurites");
        foreach (var effect in EquippableItemService.GetListenerPassiveEffects(false, player, 
                         GetEffectSums()).OfType<ListenerPassiveEffect>()) player.PassiveEffects.Add(effect);
        foreach (var effect in EquippableItemService.GetInnatePassiveEffects(false, player, 
                     GetEffectSums())) player.PassiveEffects.Add(effect);
    }
    /// <summary>
    /// Dodaje galduryt do broni, jeśli jest to możliwe.
    /// </summary>
    /// <param name="galdurite">Galduryt do dodania.</param>
    /// <remarks>
    /// Sprawdza, czy są wolne gniazda i czy typ galdurytu pasuje do broni.
    /// Po dodaniu aktualizuje efekty pasywne i ujawnia właściwości galdurytu.
    /// </remarks>
    public void AddGaldurite(Galdurite galdurite)
    {
        if (Galdurites.Count >= GalduriteSlots || galdurite.ItemType != ItemType.WeaponGaldurite) return;
        Galdurites.Add(galdurite);
        UpdatePassives(PlayerHandler.player);
        galdurite.Reveal();
    }
    /// <summary>
    /// Usuwa galduryt z broni.
    /// </summary>
    /// <param name="galdurite">Galduryt do usunięcia.</param>
    /// <remarks>
    /// Po usunięciu aktualizuje efekty pasywne postaci.
    /// </remarks>
    public void RemoveGaldurite(Galdurite galdurite)
    {
        Galdurites.Remove(galdurite);
        UpdatePassives(PlayerHandler.player);
    }

    /// <summary>
    /// Oblicza sumaryczne wartości efektów z wszystkich wpiętych galdurytów.
    /// </summary>
    /// <returns>Zbiór komponentów galdurytów z zsumowanymi wartościami efektów.</returns>
    /// <remarks>
    /// Łączy efekty tego samego typu, sumując ich wartości.
    /// </remarks>
    private HashSet<GalduriteComponent> GetEffectSums()
    {
        var result = new HashSet<GalduriteComponent>();
        foreach (var effect in Galdurites.SelectMany(gal => gal.Components))
        {
            if (result.All(x => x.EffectType != effect.EffectType))
                result.Add(effect);
            else
                result.FirstOrDefault(x => x.EffectType == effect.EffectType)!.EffectStrength += effect.EffectStrength;
        }
        return result;
    }
}