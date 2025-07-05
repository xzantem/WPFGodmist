using ConsoleGodmist;
using GodmistWPF.Characters.Player;
using GodmistWPF.Combat.Modifiers.PassiveEffects;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Items;
using GodmistWPF.Items.Galdurites;
using GodmistWPF.Utilities;
using Newtonsoft.Json;
namespace GodmistWPF.Items.Equippable.Weapons;

public class Weapon : BaseItem, IEquippable, IUsable
{
    //Base IItem implementations
    public override string Name { get; set; }
    [JsonIgnore]
    public override int Weight => 5;
    [JsonIgnore]
    public override int ID => 559;
    public int BaseCost { get; set; }

    [JsonIgnore]
    public override int Cost => (int)(BaseCost * EquippableItemService.RarityPriceModifier(Rarity));
    [JsonIgnore]
    public override bool Stackable => false;
    [JsonIgnore]
    public override ItemType ItemType => ItemType.Weapon;
    
    //Base IEquippable implementations
    public int RequiredLevel { get; set; }
    public CharacterClass RequiredClass { get; set;  }
    public Quality Quality { get; set; }
    public double UpgradeModifier { get; set; }
    public List<Galdurite> Galdurites { get; set; }
    public int GalduriteSlots => (int)Math.Floor(UpgradeModifier * 5 - 5);

    //Weapon implementations
    
    public WeaponHead Head { get; set; }
    public WeaponBinder Binder { get; set; }
    public WeaponHandle Handle { get; set; }
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
    /// Gets Starter weapon for the specified class
    /// </summary>
    /// <param name="requiredClass"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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
    public Weapon() {}

    public bool Use()
    {
        if (RequiredClass != PlayerHandler.player.CharacterClass)
        {
            // WPF handles wrong class error display through UI
            return false;
        }
        if (RequiredLevel > PlayerHandler.player.Level)
        {
            // WPF handles level too low error display through UI
            return false;
        }
        PlayerHandler.player.SwitchWeapon(this);
        return true;
    }

    public override void Inspect(int amount = 1)
    {
        base.Inspect(amount);
        // WPF handles weapon inspection display through UI
    }

    public virtual void UpdatePassives(PlayerCharacter player)
    {
        player.PassiveEffects.InnateEffects.RemoveAll(x => x.Source == "WeaponGaldurites");
        foreach (var effect in EquippableItemService.GetListenerPassiveEffects(false, player, 
                         GetEffectSums()).OfType<ListenerPassiveEffect>()) player.PassiveEffects.Add(effect);
        foreach (var effect in EquippableItemService.GetInnatePassiveEffects(false, player, 
                     GetEffectSums())) player.PassiveEffects.Add(effect);
    }
    public void AddGaldurite(Galdurite galdurite)
    {
        if (Galdurites.Count >= GalduriteSlots || galdurite.ItemType != ItemType.WeaponGaldurite) return;
        Galdurites.Add(galdurite);
        UpdatePassives(PlayerHandler.player);
        galdurite.Reveal();
    }
    public void RemoveGaldurite(Galdurite galdurite)
    {
        Galdurites.Remove(galdurite);
        UpdatePassives(PlayerHandler.player);
    }

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