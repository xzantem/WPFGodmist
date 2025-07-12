
using GodmistWPF.Characters.Player;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Items;
using GodmistWPF.Items.Galdurites;
using GodmistWPF.Utilities;

namespace GodmistWPF.Items.Equippable.Armors;

/// <summary>
/// Reprezentuje kompletny przedmiot zbroi, który może zostać wyposażony przez postać.
/// Składa się z trzech komponentów: płyty, oprawy i podstawy.
/// </summary>
public class Armor : BaseItem, IEquippable, IUsable
{
    /// <summary>
    /// Nazwa zbroi.
    /// </summary>
    public override string Name { get; set; }
    /// <summary>
    /// Waga zbroi.
    /// </summary>
    public override int Weight => 5;
    /// <summary>
    /// Unikalny identyfikator typu przedmiotu.
    /// </summary>
    public override int ID => 560;
    /// <summary>
    /// Bazowy koszt zbroi przed uwzględnieniem rzadkości.
    /// </summary>
    public int BaseCost { get; set; }
    /// <summary>
    /// Koszt zbroi uwzględniający jej rzadkość.
    /// </summary>
    public override int Cost => (int)(BaseCost * EquippableItemService.RarityPriceModifier(Rarity));
    /// <summary>
    /// Określa, czy przedmiot może być układany w stosy.
    /// </summary>
    public override bool Stackable => false;
    /// <summary>
    /// Typ przedmiotu - zbroja.
    /// </summary>
    public override ItemType ItemType => ItemType.Armor;
    
    /// <summary>
    /// Wymagany poziom postaci do założenia zbroi.
    /// </summary>
    public int RequiredLevel { get; set; }
    
    /// <summary>
    /// Klasa postaci, która może założyć tę zbroję.
    /// </summary>
    public CharacterClass RequiredClass { get; }
    
    /// <summary>
    /// Jakość wykonania zbroi.
    /// </summary>
    public Quality Quality { get; set; }
    
    /// <summary>
    /// Modyfikator ulepszenia zbroi.
    /// </summary>
    public double UpgradeModifier { get; set; }
    
    /// <summary>
    /// Lista galdurytów wpiętych w zbroję.
    /// </summary>
    public List<Galdurite> Galdurites { get; set; }
    
    /// <summary>
    /// Liczba dostępnych gniazd na galduryty.
    /// </summary>
    public int GalduriteSlots => (int)Math.Floor(UpgradeModifier * 5 - 5);
    
    /// <summary>
    /// Płyta zbroi, określająca jej podstawowe parametry obronne.
    /// </summary>
    public ArmorPlate Plate { get; set; }
    
    /// <summary>
    /// Oprawa zbroi, modyfikująca jej właściwości.
    /// </summary>
    public ArmorBinder Binder { get; set; }
    
    /// <summary>
    /// Podstawa zbroi, określająca dodatkowe właściwości.
    /// </summary>
    public ArmorBase Base { get; set; }
    
    /// <summary>
    /// Maksymalne zdrowie dodawane przez zbroję.
    /// Zależy od klasy postaci, jakości zbroi i jej poziomu.
    /// </summary>
    public int MaximalHealth
    {
        get
        {
            double value = Binder.Health;
            var multiplier = RequiredClass switch
            {
                CharacterClass.Warrior => 10,
                CharacterClass.Scout => 6,
                CharacterClass.Sorcerer => 4,
                _ => 14
            };
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            value += multiplier * (Binder.Tier * 10 - 5);
            value *= EquippableItemService.RarityStatModifier(Rarity) * (Plate.HealthBonus + Base.HealthBonus + 1);
            return (int)value;
        }
    }
    
    /// <summary>
    /// Wartość uników dodawana przez zbroję.
    /// Zależy od jakości zbroi i jej poziomu.
    /// </summary>
    public int Dodge
    {
        get
        {
            double value = Base.Dodge;
            var multiplier = 2;
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            value += multiplier;
            value *= EquippableItemService.RarityStatModifier(Rarity) * (Plate.DodgeBonus + Binder.DodgeBonus + 1);
            return (int)value;
        }
    }
    
    /// <summary>
    /// Wartość obrony fizycznej zapewnianej przez zbroję.
    /// Zależy od klasy postaci, jakości zbroi i jej poziomu.
    /// </summary>
    public int PhysicalDefense
    {
        get
        {
            double value = Plate.PhysicalDefense;
            var multiplier = RequiredClass switch
            {
                CharacterClass.Warrior => 1.2,
                CharacterClass.Scout => 0.8,
                CharacterClass.Sorcerer => 0.2,
                _ => 1.6
            };
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            value += multiplier;
            value *= UpgradeModifier * (Binder.PhysicalDefenseBonus + Base.PhysicalDefenseBonus + 1);
            return (int)value;
        }
    }
    
    /// <summary>
    /// Wartość obrony magicznej zapewnianej przez zbroję.
    /// Zależy od klasy postaci, jakości zbroi i jej poziomu.
    /// </summary>
    public int MagicDefense
    {
        get
        {
            double value = Plate.MagicDefense;
            var multiplier = RequiredClass switch
            {
                CharacterClass.Warrior => 0.8,
                CharacterClass.Scout => 0.4,
                CharacterClass.Sorcerer => 1.2,
                _ => 1.6
            };
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            value += multiplier;
            value *= UpgradeModifier * (Binder.MagicDefenseBonus + Base.MagicDefenseBonus + 1);
            return (int)value;
        }
    }
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="Armor"/> z określonymi komponentami i parametrami.
    /// </summary>
    /// <param name="plate">Płyta zbroi.</param>
    /// <param name="binder">Oprawa zbroi.</param>
    /// <param name="armBase">Podstawa zbroi.</param>
    /// <param name="requiredClass">Wymagana klasa postaci.</param>
    /// <param name="quality">Jakość wykonania zbroi.</param>
    /// <param name="alias">Opcjonalny alias zbroi. Jeśli nie podany, zostanie wygenerowany automatycznie.</param>
    public Armor(ArmorPlate plate, ArmorBinder binder, ArmorBase armBase, CharacterClass requiredClass, Quality quality, string alias = "")
    {
        Plate = plate;
        Binder = binder;
        Base = armBase;
        Quality = quality;
        if (alias == "")
        {
            Name = NameAliasHelper.GetName(Plate.Adjective+"Adj") + " " + requiredClass switch
            {
                CharacterClass.Warrior => locale.Hauberk,
                CharacterClass.Scout => locale.Tunic,
                CharacterClass.Sorcerer => locale.Robe,
                _ => locale.Cuirass
            } + quality switch
            {
                Quality.Weak => $" ({locale.Weak})",
                Quality.Excellent => $" ({locale.Excellent})",
                _ => ""
            };
            Alias = $"{plate.Alias}.{binder.Alias}.{armBase.Alias}";
        }
        else
        {
            Name = NameAliasHelper.GetName(alias);
            Alias = alias;
        }
        Rarity = EquippableItemService.GetRandomRarity(Quality == Quality.Masterpiece ? 7 : 0);
        BaseCost = (int)((plate.MaterialCost * ItemManager.GetItem(plate.Material).Cost + 
                   binder.MaterialCost * ItemManager.GetItem(binder.Material).Cost + 
                   armBase.MaterialCost * ItemManager.GetItem(armBase.Material).Cost) * quality switch {
            Quality.Weak => 0.5,
            Quality.Normal => 1,
            Quality.Excellent => 2,
            Quality.Masterpiece => 4,
            _ => 1 });
        RequiredLevel = Math.Max(Math.Max(plate.Tier, binder.Tier), armBase.Tier) * 10 - 5 + quality switch
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
    /// Inicjalizuje nową instancję zbroi startowej dla określonej klasy postaci.
    /// </summary>
    /// <param name="requiredClass">Klasa postaci, dla której ma zostać stworzona zbroja startowa.</param>
    /// <exception cref="ArgumentOutOfRangeException">Wyrzucany, gdy podano nieprawidłową klasę postaci.</exception>
    public Armor(CharacterClass requiredClass)
    {
        switch (requiredClass)
        {
            case CharacterClass.Warrior:
                Plate = EquipmentPartManager.GetPart<ArmorPlate>("ScratchedPlate", CharacterClass.Warrior);
                Binder = EquipmentPartManager.GetPart<ArmorBinder>("ScratchedBinder", CharacterClass.Warrior);
                Base = EquipmentPartManager.GetPart<ArmorBase>("ScratchedBase", CharacterClass.Warrior);
                Name = NameAliasHelper.GetName(Plate.Adjective) + " " + locale.Hauberk;
                break;
            case CharacterClass.Scout:
                Plate = EquipmentPartManager.GetPart<ArmorPlate>("HoleyPlate", CharacterClass.Scout);
                Binder = EquipmentPartManager.GetPart<ArmorBinder>("HoleyBinder", CharacterClass.Scout);
                Base = EquipmentPartManager.GetPart<ArmorBase>("HoleyBase", CharacterClass.Scout);
                Name = NameAliasHelper.GetName(Plate.Adjective) + " " + locale.Tunic;
                break;
            case CharacterClass.Sorcerer:
                Plate = EquipmentPartManager.GetPart<ArmorPlate>("TornPlate", CharacterClass.Sorcerer);
                Binder = EquipmentPartManager.GetPart<ArmorBinder>("TornBinder", CharacterClass.Sorcerer);
                Base = EquipmentPartManager.GetPart<ArmorBase>("TornBase", CharacterClass.Sorcerer);
                Name = NameAliasHelper.GetName(Plate.Adjective) + " " + locale.Robe;
                break;
            case CharacterClass.Paladin:
                Plate = EquipmentPartManager.GetPart<ArmorPlate>("PiercedPlate", CharacterClass.Paladin);
                Binder = EquipmentPartManager.GetPart<ArmorBinder>("PiercedBinder", CharacterClass.Paladin);
                Base = EquipmentPartManager.GetPart<ArmorBase>("PiercedBase", CharacterClass.Paladin);
                Name = NameAliasHelper.GetName(Plate.Adjective) + " " + locale.Cuirass;
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
    /// Inicjalizuje nową, pustą instancję klasy <see cref="Armor"/>.
    /// Używane głównie do deserializacji.
    /// </summary>
    public Armor() {}

    /// <summary>
    /// Próbuje założyć zbroję na aktualnie wybranej postaci.
    /// </summary>
    /// <returns>Prawda, jeśli udało się założyć zbroję; w przeciwnym razie fałsz.</returns>
    public bool Use()
    {
        if (RequiredClass != PlayerHandler.player.CharacterClass)
            return false;
        if (RequiredLevel > PlayerHandler.player.Level)
            return false;
        PlayerHandler.player.SwitchArmor(this);
        return true;
    }
    /// <summary>
    /// Aktualizuje efekty pasywne wynikające z galdurytów wpiętych w zbroję.
    /// </summary>
    /// <param name="player">Postać gracza, której dotyczą efekty.</param>
    public virtual void UpdatePassives(PlayerCharacter player)
    {
        player.PassiveEffects.InnateEffects.RemoveAll(x => x.Source == "ArmorGaldurites");
        foreach (var effect in EquippableItemService.GetListenerPassiveEffects(false, player, GetEffectSums()))
            player.PassiveEffects.Add(effect);
        foreach (var effect in EquippableItemService.GetInnatePassiveEffects(false, player, GetEffectSums()))
            player.PassiveEffects.Add(effect);
    }
    /// <summary>
    /// Dodaje galduryt do zbroi, jeśli jest to możliwe.
    /// </summary>
    /// <param name="galdurite">Galduryt do dodania.</param>
    public void AddGaldurite(Galdurite galdurite)
    {
        if (Galdurites.Count >= GalduriteSlots || galdurite.ItemType != ItemType.ArmorGaldurite) return;
        Galdurites.Add(galdurite);
        UpdatePassives(PlayerHandler.player);
        galdurite.Reveal();
    }
    /// <summary>
    /// Usuwa galduryt z zbroi.
    /// </summary>
    /// <param name="galdurite">Galduryt do usunięcia.</param>
    public void RemoveGaldurite(Galdurite galdurite)
    {
        Galdurites.Remove(galdurite);
        UpdatePassives(PlayerHandler.player);
    }
    
    /// <summary>
    /// Zwraca sumaryczne efekty wszystkich galdurytów wpiętych w zbroję.
    /// </summary>
    /// <returns>Zbiór komponentów galdurytów z sumowanymi wartościami efektów.</returns>
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