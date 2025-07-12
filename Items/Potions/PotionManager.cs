using System.IO;
using GodmistWPF.Characters.Player;
using GodmistWPF.Combat.Modifiers;
using GodmistWPF.Combat.Modifiers.PassiveEffects;
using GodmistWPF.Enums.Items;
using GodmistWPF.Enums.Modifiers;
using GodmistWPF.Utilities;
using Newtonsoft.Json;

namespace GodmistWPF.Items.Potions;

/// <summary>
/// Statyczna klasa zarządzająca systemem mikstur w grze.
/// Odpowiada za wczytywanie komponentów, przetwarzanie efektów i generowanie losowych mikstur.
/// </summary>
public static class PotionManager
{
    /// <summary>
    /// Lista wszystkich dostępnych komponentów mikstur wczytana z pliku konfiguracyjnego.
    /// </summary>
    public static List<PotionComponent> PotionComponents { get; private set; }
    
    /// <summary>
    /// Inicjalizuje komponenty mikstur, wczytując je z pliku JSON.
    /// </summary>
    /// <exception cref="FileNotFoundException">Wyrzucany, gdy nie znaleziono pliku konfiguracyjnego.</exception>
    public static void InitComponents()
    {
        var path = "json/potion-components.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            PotionComponents = JsonConvert.DeserializeObject<List<PotionComponent>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }

    /// <summary>
    /// Przetwarza efekt komponentu mikstury, uwzględniając modyfikacje z katalizatora.
    /// </summary>
    /// <param name="component">Komponent mikstury do przetworzenia.</param>
    /// <param name="catalyst">Opcjonalny katalizator modyfikujący efekt.</param>
    public static void ProcessComponent(PotionComponent component, PotionCatalyst? catalyst)
    {
        var duration = 10 + (int)(catalyst?.Effect == PotionCatalystEffect.Duration ? catalyst.Strength : 0);
        var strength = component.EffectStrength * duration *
                       (catalyst?.Effect == PotionCatalystEffect.Strength ? 1 + catalyst.Strength : 1) / 10;
        var condensedDuration = duration - (int)(catalyst?.Effect == PotionCatalystEffect.Condensation
            ? catalyst.Strength
            : 0);
        switch (component.Effect)
        {
            case PotionEffect.HealthRegain:
                PlayerHandler.player.Heal(PlayerHandler.player.MaximalHealth * strength);
                break;
            case PotionEffect.HealthRegen:
                var totalHealthRegen = PlayerHandler.player.MaximalHealth * strength;
                var healthPerTurn = totalHealthRegen / condensedDuration;
                PlayerHandler.player.PassiveEffects.Add(new TimedPassiveEffect(
                    PlayerHandler.player, "Potion", "Regeneration", condensedDuration, [],
                    () => { PlayerHandler.player.Heal((int)healthPerTurn); }));
                break;
            case PotionEffect.ResourceRegain:
                PlayerHandler.player.RegenResource((int)(PlayerHandler.player.MaximalResource * strength));
                break;
            case PotionEffect.ResourceRegen:
                var totalResourceRegen = PlayerHandler.player.MaximalResource * strength * duration / 10;
                var resourcePerTurn = totalResourceRegen / condensedDuration;
                PlayerHandler.player.PassiveEffects.Add(new TimedPassiveEffect(
                    PlayerHandler.player, "Potion", "Regeneration", condensedDuration, [], 
                    () => { PlayerHandler.player.RegenResource((int)resourcePerTurn); }));
                break;
            case PotionEffect.MaxResourceIncrease:
                PlayerHandler.player.AddModifier(StatType.MaximalResource, new StatModifier(ModifierType.Multiplicative, 
                    strength, component.Material, duration));
                break;
            case PotionEffect.DamageDealtIncrease:
                PlayerHandler.player.PassiveEffects.Add(new TimedPassiveEffect(
                    PlayerHandler.player, "Potion", "DamageDealtMod", duration, [strength, ModifierType.Multiplicative]));
                break;
            case PotionEffect.DamageTakenDecrease:
                PlayerHandler.player.PassiveEffects.Add(new TimedPassiveEffect(
                    PlayerHandler.player, "Potion", "DamageTakenMod", duration, [-strength, ModifierType.Multiplicative]));
                break;
            case PotionEffect.ResistanceIncrease:
                foreach (var resistance in PlayerHandler.player.Resistances)
                {
                    PlayerHandler.player.AddResistanceModifier(resistance.Key, new StatModifier(ModifierType.Multiplicative, 
                        strength, component.Material, duration));
                }
                break;
            case PotionEffect.AccuracyIncrease:
                PlayerHandler.player.PassiveEffects.Add(new TimedPassiveEffect(
                    PlayerHandler.player, "Potion", "AccuracyMod", duration, [strength, ModifierType.Multiplicative]));
                break;
            case PotionEffect.SpeedIncrease:
                PlayerHandler.player.AddModifier(StatType.Speed, new StatModifier(ModifierType.Additive, 
                    strength, component.Material, duration));
                break;
            case PotionEffect.CritChanceIncrese:
                PlayerHandler.player.AddModifier(StatType.CritChance, new StatModifier(ModifierType.Additive, 
                    strength, component.Material, duration));
                break;
            case PotionEffect.DodgeIncrease:
                PlayerHandler.player.AddModifier(StatType.Dodge, new StatModifier(ModifierType.Additive, 
                    strength, component.Material, duration));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Generuje losową miksturę o określonym poziomie mocy.
    /// </summary>
    /// <param name="tier">Poziom mocy mikstury (1-3).</param>
    /// <returns>Nowa instancja losowo wygenerowanej mikstury.</returns>
    public static Potion GetRandomPotion(int tier)
    {
        var possibleComponents = PotionComponents.
            Where(c => c.StrengthTier == tier).ToList();
        var components = new List<PotionComponent>();
        for (var i = 0; i < 3; i++)
        {
            var randomized = UtilityMethods.RandomChoice(possibleComponents);
            components.Add(randomized);
            possibleComponents.Remove(randomized);
        }

        return new Potion("Potion", components,
                new PotionCatalyst(UtilityMethods.RandomChoice(Enum.GetValues<PotionCatalystEffect>().ToList()), tier));
    }

    /// <summary>
    /// Generuje nazwę mikstury na podstawie jej efektów i poziomu mocy.
    /// </summary>
    /// <param name="effects">Lista efektów mikstury.</param>
    /// <param name="tier">Poziom mocy mikstury.</param>
    /// <returns>Sformatowana nazwa mikstury.</returns>
    public static string GetPotionName(List<PotionEffect> effects, int tier)
    {
        var distinct = effects.ToHashSet().ToList();
        var txt = distinct[0] switch
        {
            PotionEffect.HealthRegain => locale.PotionHealthRegainAdv,
            PotionEffect.HealthRegen => locale.PotionHealthRegenAdv,
            PotionEffect.ResourceRegain => locale.PotionResourceRegainAdv,
            PotionEffect.ResourceRegen => locale.PotionResourceRegenAdv,
            PotionEffect.MaxResourceIncrease => locale.PotionMaxResourceIncreaseAdv,
            PotionEffect.DamageDealtIncrease => locale.PotionDamageDealtIncreaseAdv,
            PotionEffect.DamageTakenDecrease => locale.PotionDamageTakenDecreaseAdv,
            PotionEffect.ResistanceIncrease => locale.PotionResistanceIncreaseAdv,
            PotionEffect.SpeedIncrease => locale.PotionSpeedIncreaseAdv,
            PotionEffect.CritChanceIncrese => locale.PotionCritChanceIncreaseAdv,
            PotionEffect.DodgeIncrease => locale.PotionDodgeIncreaseAdv,
            PotionEffect.AccuracyIncrease => locale.PotionAccuracyIncreaseAdv
        } + " ";
        txt += tier switch
        {
            1 => locale.Lesser,
            2 => "",
            3 => locale.Greater,
            _ => throw new ArgumentOutOfRangeException()
        } + " " + locale.Potion + " " + locale.Of;
        txt += distinct[1] switch
        {
            PotionEffect.HealthRegain => locale.PotionHealthRegain,
            PotionEffect.HealthRegen => locale.PotionHealthRegen,
            PotionEffect.ResourceRegain => locale.PotionResourceRegain,
            PotionEffect.ResourceRegen => locale.PotionResourceRegen,
            PotionEffect.MaxResourceIncrease => locale.PotionMaxResourceIncrease,
            PotionEffect.DamageDealtIncrease => locale.PotionDamageDealtIncrease,
            PotionEffect.DamageTakenDecrease => locale.PotionDamageTakenDecrease,
            PotionEffect.ResistanceIncrease => locale.PotionResistanceIncrease,
            PotionEffect.SpeedIncrease => locale.PotionSpeedIncrease,
            PotionEffect.CritChanceIncrese => locale.PotionCritChanceIncrease,
            PotionEffect.DodgeIncrease => locale.PotionDodgeIncrease,
            PotionEffect.AccuracyIncrease => locale.PotionAccuracyIncrease
        };
        if (distinct.Count > 2)
        {
            txt += " " + locale.And1 + " " + distinct[2] switch
            {
                PotionEffect.HealthRegain => locale.PotionHealthRegain,
                PotionEffect.HealthRegen => locale.PotionHealthRegen,
                PotionEffect.ResourceRegain => locale.PotionResourceRegain,
                PotionEffect.ResourceRegen => locale.PotionResourceRegen,
                PotionEffect.MaxResourceIncrease => locale.PotionMaxResourceIncrease,
                PotionEffect.DamageDealtIncrease => locale.PotionDamageDealtIncrease,
                PotionEffect.DamageTakenDecrease => locale.PotionDamageTakenDecrease,
                PotionEffect.ResistanceIncrease => locale.PotionResistanceIncrease,
                PotionEffect.SpeedIncrease => locale.PotionSpeedIncrease,
                PotionEffect.CritChanceIncrese => locale.PotionCritChanceIncrease,
                PotionEffect.DodgeIncrease => locale.PotionDodgeIncrease,
                PotionEffect.AccuracyIncrease => locale.PotionAccuracyIncrease
            };
        }
        return txt;
    }

    /// <summary>
    /// Pobiera identyfikator materiału katalizatora na podstawie jego typu i poziomu.
    /// </summary>
    /// <param name="effect">Typ efektu katalizatora.</param>
    /// <param name="tier">Poziom katalizatora (1-5).</param>
    /// <returns>Identyfikator materiału katalizatora.</returns>
    public static string GetCatalystMaterial(PotionCatalystEffect effect, int tier)
    {
        return (effect, tier) switch
        {
            (PotionCatalystEffect.Duration, 1) => "HerbalDustNormal",
            (PotionCatalystEffect.Duration, 2) => "HerbalDustYellow",
            (PotionCatalystEffect.Duration, 3) => "HerbalDustHuman",
            (PotionCatalystEffect.Duration, 4) => "HerbalDustLight",
            (PotionCatalystEffect.Duration, 5) => "HerbalDustFire",
            
            (PotionCatalystEffect.Strength, 1) => "HerbalDustLumpy",
            (PotionCatalystEffect.Strength, 2) => "HerbalDustGreen",
            (PotionCatalystEffect.Strength, 3) => "HerbalDustElf",
            (PotionCatalystEffect.Strength, 4) => "HerbalDustDivinity",
            (PotionCatalystEffect.Strength, 5) => "HerbalDustAir",
            
            (PotionCatalystEffect.Condensation, 1) => "HerbalDustTacky",
            (PotionCatalystEffect.Condensation, 2) => "HerbalDustPink",
            (PotionCatalystEffect.Condensation, 3) => "HerbalDustDwarf",
            (PotionCatalystEffect.Condensation, 4) => "HerbalDustDarkness",
            (PotionCatalystEffect.Condensation, 5) => "HerbalDustWater",
            
            (PotionCatalystEffect.Capacity, 1) => "HerbalDustLoose",
            (PotionCatalystEffect.Capacity, 2) => "HerbalDustOrange",
            (PotionCatalystEffect.Capacity, 3) => "HerbalDustLizardman",
            (PotionCatalystEffect.Capacity, 4) => "HerbalDustDemon",
            (PotionCatalystEffect.Capacity, 5) => "HerbalDustEarth"
        };
    }
    /// <summary>
    /// Oblicza siłę efektu katalizatora na podstawie jego typu i poziomu.
    /// </summary>
    /// <param name="effect">Typ efektu katalizatora.</param>
    /// <param name="tier">Poziom katalizatora (1-5).</param>
    /// <returns>Wartość siły efektu katalizatora.</returns>
    public static double GetCatalystStrength(PotionCatalystEffect effect, int tier)
    {
        if (effect != PotionCatalystEffect.Strength) return tier;
        return tier switch
        {
            1 => 0.1, 2 => 0.12, 3 => 0.15, 4 => 0.2, 5 => 0.3
        };
    }

    /// <summary>
    /// Wyświetla interfejs wyboru mikstury (obsługa w interfejsie użytkownika).
    /// </summary>
    /// <param name="potions">Lista dostępnych mikstur do wyboru.</param>
    /// <param name="listMaterials">Czy wyświetlać informacje o materiałach.</param>
    /// <returns>Wybraną miksturę lub null, jeśli wybór został anulowany.</returns>
    public static Potion? ChoosePotion(List<Potion> potions, bool listMaterials)
    {
        // WPF handles potion selection UI
        return null;
    }
}