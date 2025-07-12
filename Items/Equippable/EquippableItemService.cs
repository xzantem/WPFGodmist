using GodmistWPF.Characters.Player;
using GodmistWPF.Combat.Modifiers.PassiveEffects;
using GodmistWPF.Combat.Skills.ActiveSkillEffects;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Items;
using GodmistWPF.Enums.Modifiers;
using GodmistWPF.Items.Equippable.Armors;
using GodmistWPF.Items.Equippable.Weapons;
using GodmistWPF.Items.Galdurites;
using GodmistWPF.Utilities;

namespace GodmistWPF.Items.Equippable;

/// <summary>
/// Statyczna klasa serwisu odpowiedzialnego za tworzenie i zarządzanie przedmiotami wyposażenia.
/// Zawiera metody do generowania losowych przedmiotów, określania ich właściwości i efektów.
/// </summary>
public static class EquippableItemService
{
    /// <summary>
    /// Oblicza modyfikator ceny na podstawie rzadkości przedmiotu.
    /// </summary>
    /// <param name="rarity">Rzadkość przedmiotu.</param>
    /// <returns>Modyfikator ceny jako wartość zmiennoprzecinkowa.</returns>
    /// <remarks>
    /// Wartości modyfikatorów:
    /// - Zniszczony: 0.7
    /// - Uszkodzony: 0.85
    /// - Niezwykły: 1.15
    /// - Rzadki: 1.25
    /// - Starożytny: 1.5
    /// - Legendarny: 1.75
    /// - Mityczny: 2.0
    /// - Boski: 2.5
    /// - Domyślnie: 1.0
    /// </remarks>
    public static double RarityPriceModifier(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Destroyed => 0.7,
            ItemRarity.Damaged => 0.85,
            ItemRarity.Uncommon => 1.15,
            ItemRarity.Rare => 1.25,
            ItemRarity.Ancient => 1.5,
            ItemRarity.Legendary => 1.75,
            ItemRarity.Mythical => 2,
            ItemRarity.Godly => 2.5,
            _ => 1
        };
    }
    /// <summary>
    /// Oblicza modyfikator statystyk na podstawie rzadkości przedmiotu.
    /// </summary>
    /// <param name="rarity">Rzadkość przedmiotu.</param>
    /// <returns>Modyfikator statystyk jako wartość zmiennoprzecinkowa.</returns>
    /// <remarks>
    /// Wartości modyfikatorów:
    /// - Zniszczony: 0.6
    /// - Uszkodzony: 0.7
    /// - Niezwykły: 1.025
    /// - Rzadki: 1.05
    /// - Starożytny: 1.125
    /// - Legendarny: 1.225
    /// - Mityczny: 1.35
    /// - Boski: 1.5
    /// - Domyślnie: 1.0
    /// </remarks>
    public static double RarityStatModifier(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Destroyed => 0.6,
            ItemRarity.Damaged => 0.7,
            ItemRarity.Uncommon => 1.025,
            ItemRarity.Rare => 1.05,
            ItemRarity.Ancient => 1.125,
            ItemRarity.Legendary => 1.225,
            ItemRarity.Mythical => 1.35,
            ItemRarity.Godly => 1.5,
            _ => 1
        };
    }

    /// <summary>
    /// Losuje rzadkość przedmiotu z uwzględnieniem przesunięcia.
    /// </summary>
    /// <param name="bias">Przesunięcie wpływające na szanse wylosowania wyższych rzadkości.</param>
    /// <returns>Wylosowana rzadkość przedmiotu.</returns>
    /// <remarks>
    /// Im wyższe przesunięcie, tym większa szansa na rzadsze przedmioty.
    /// Domyślne szanse (przy bias=0):
    /// - Zniszczony: 40%
    /// - Uszkodzony: 60%
    /// - Zwykły: 200%
    /// - Niezwykły: 40%
    /// - Rzadki: 20%
    /// - Starożytny: 10%
    /// - Legendarny: 5%
    /// - Mityczny: 2%
    /// - Boski: 1%
    /// </remarks>
    public static ItemRarity GetRandomRarity(int bias = 0)
    {
        var rarities = new Dictionary<ItemRarity, int>
        {
            {ItemRarity.Destroyed, bias > 1 ? 0 : 40 },
            {ItemRarity.Damaged, bias > 2 ? 0 : 60 },
            {ItemRarity.Common, bias > 3 ? 0 : 200 },
            {ItemRarity.Uncommon, bias > 4 ? 0 : 40 },
            {ItemRarity.Rare, bias > 5 ? 0 : 20 },
            {ItemRarity.Ancient, bias > 6 ? 0 : 10 },
            {ItemRarity.Legendary, bias > 7 ? 0 : 5 },
            {ItemRarity.Mythical, bias > 8 ? 0 : 2 },
            {ItemRarity.Godly, 1 }
        };
        return UtilityMethods.RandomChoice(rarities);
    }
    /// <summary>
    /// Losuje rzadkość galadurytu z uwzględnieniem przesunięcia.
    /// </summary>
    /// <param name="bias">Przesunięcie wpływające na szanse wylosowania wyższych rzadkości.</param>
    /// <returns>Wylosowana rzadkość galadurytu.</returns>
    /// <remarks>
    /// Specjalna wersja dla galadurytów, która nie uwzględnia zniszczonych i uszkodzonych przedmiotów.
    /// Domyślne szanse (przy bias=0):
    /// - Zwykły: 50%
    /// - Niezwykły: 35%
    /// - Rzadki: 25%
    /// - Starożytny: 20%
    /// - Legendarny: 15%
    /// - Mityczny: 10%
    /// - Boski: 5%
    /// </remarks>
    public static ItemRarity GetRandomGalduriteRarity(int bias = 0)
    {
        var rarities = new Dictionary<ItemRarity, int>
        {
            {ItemRarity.Common, bias > 3 ? 0 : 50 },
            {ItemRarity.Uncommon, bias > 4 ? 0 : 35 },
            {ItemRarity.Rare, bias > 5 ? 0 : 25 },
            {ItemRarity.Ancient, bias > 6 ? 0 : 20 },
            {ItemRarity.Legendary, bias > 7 ? 0 : 15 },
            {ItemRarity.Mythical, bias > 8 ? 0 : 10 },
            {ItemRarity.Godly, 5 }
        };
        return UtilityMethods.RandomChoice(rarities);
    }

    /// <summary>
    /// Tworzy losową broń określonego poziomu i klasy postaci.
    /// </summary>
    /// <param name="tier">Poziom broni.</param>
    /// <param name="requiredClass">Wymagana klasa postaci.</param>
    /// <returns>Nowa instancja losowej broni.</returns>
    /// <remarks>
    /// Jakość broni jest losowana z wyłączeniem jakości "Arcydzieło".
    /// </remarks>
    public static Weapon GetRandomWeapon(int tier, CharacterClass requiredClass)
    {
        return new Weapon(EquipmentPartManager.GetRandomPart<WeaponHead>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<WeaponBinder>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<WeaponHandle>(tier, requiredClass),
            requiredClass,
            UtilityMethods.RandomChoice(Enum.GetValues<Quality>()
                .Where(x => x != Quality.Masterpiece).ToList()));
    }
    /// <summary>
    /// Tworzy losową broń określonego poziomu i losowej klasy postaci.
    /// </summary>
    /// <param name="tier">Poziom broni.</param>
    /// <returns>Nowa instancja losowej broni.</returns>
    /// <remarks>
    /// Klasa postaci jest losowana spośród wszystkich dostępnych klas.
    /// Jakość broni jest losowana z wyłączeniem jakości "Arcydzieło".
    /// </remarks>
    public static Weapon GetRandomWeapon(int tier)
    {
        var requiredClass = UtilityMethods.RandomChoice(Enum.GetValues<CharacterClass>().ToList());
        return new Weapon(EquipmentPartManager.GetRandomPart<WeaponHead>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<WeaponBinder>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<WeaponHandle>(tier, requiredClass),
            requiredClass,
            UtilityMethods.RandomChoice(Enum.GetValues<Quality>()
                .Where(x => x != Quality.Masterpiece).ToList()));
    }
    /// <summary>
    /// Tworzy specjalny przedmiot upuszczany przez bossa.
    /// </summary>
    /// <param name="tier">Poziom przedmiotu.</param>
    /// <param name="bossAlias">Alias bossa, który upuszcza przedmiot.</param>
    /// <returns>Specjalny przedmiot arcydzieło powiązany z danym bossem.</returns>
    /// <remarks>
    /// Dla każdego bossa zdefiniowane są 2 możliwe przedmioty arcydzieła.
    /// Przedmioty mają specjalne unikalne właściwości i są zawsze najwyższej jakości.
    /// </remarks>
    public static IEquippable GetBossDrop(int tier, string bossAlias)
    {
        var random = Random.Shared.Next(1, 3);
        return (bossAlias, random) switch {
            ("SkeletonExecutioner", 1) => new MasterpieceWeapon
            (EquipmentPartManager.GetRandomPart<WeaponHead>(tier, CharacterClass.Paladin),
                EquipmentPartManager.GetRandomPart<WeaponBinder>(tier, CharacterClass.Paladin),
                EquipmentPartManager.GetRandomPart<WeaponHandle>(tier, CharacterClass.Paladin),
                CharacterClass.Paladin, Quality.Masterpiece, 
                new InnatePassiveEffect(PlayerHandler.player , "", "", []), "ExecutionersHammer"),
            ("SkeletonExecutioner", 2) => new MasterpieceArmor
            (EquipmentPartManager.GetRandomPart<ArmorPlate>(tier, CharacterClass.Sorcerer),
                EquipmentPartManager.GetRandomPart<ArmorBinder>(tier, CharacterClass.Sorcerer),
                EquipmentPartManager.GetRandomPart<ArmorBase>(tier, CharacterClass.Sorcerer),
                CharacterClass.Sorcerer, Quality.Masterpiece, 
                new InnatePassiveEffect(PlayerHandler.player , "", "", []), "RunicRobes"),
            ("DeadKingSpirit", 1) => new MasterpieceArmor
            (EquipmentPartManager.GetRandomPart<ArmorPlate>(tier, CharacterClass.Warrior),
                EquipmentPartManager.GetRandomPart<ArmorBinder>(tier, CharacterClass.Warrior),
                EquipmentPartManager.GetRandomPart<ArmorBase>(tier, CharacterClass.Warrior),
                CharacterClass.Warrior, Quality.Masterpiece, 
                new InnatePassiveEffect(PlayerHandler.player , "", "", []), "HeraldsHauberk"),
            ("DeadKingSpirit", 2) => new MasterpieceArmor
            (EquipmentPartManager.GetRandomPart<ArmorPlate>(tier, CharacterClass.Paladin),
                EquipmentPartManager.GetRandomPart<ArmorBinder>(tier, CharacterClass.Paladin),
                EquipmentPartManager.GetRandomPart<ArmorBase>(tier, CharacterClass.Paladin),
                CharacterClass.Paladin, Quality.Masterpiece, 
                new InnatePassiveEffect(PlayerHandler.player , "", "", []),"DeadKingsArmor"),
            ("InfectedEnt", 1) => new MasterpieceArmor
            (EquipmentPartManager.GetRandomPart<ArmorPlate>(tier, CharacterClass.Sorcerer),
                EquipmentPartManager.GetRandomPart<ArmorBinder>(tier, CharacterClass.Sorcerer),
                EquipmentPartManager.GetRandomPart<ArmorBase>(tier, CharacterClass.Sorcerer),
                CharacterClass.Sorcerer, Quality.Masterpiece, 
                new InnatePassiveEffect(PlayerHandler.player , "", "", []), "EntsFinger"),
            ("InfectedEnt", 2) => new MasterpieceWeapon
            (EquipmentPartManager.GetRandomPart<WeaponHead>(tier, CharacterClass.Sorcerer),
                EquipmentPartManager.GetRandomPart<WeaponBinder>(tier, CharacterClass.Sorcerer),
                EquipmentPartManager.GetRandomPart<WeaponHandle>(tier, CharacterClass.Sorcerer),
                CharacterClass.Sorcerer, Quality.Masterpiece, 
                new InnatePassiveEffect(PlayerHandler.player , "", "", []), "LeafyTunic"),
        };
    }
    /// <summary>
    /// Tworzy losową zbroję określonego poziomu i klasy postaci.
    /// </summary>
    /// <param name="tier">Poziom zbroi.</param>
    /// <param name="requiredClass">Wymagana klasa postaci.</param>
    /// <returns>Nowa instancja losowej zbroi.</returns>
    /// <remarks>
    /// Jakość zbroi jest losowana z wyłączeniem jakości "Arcydzieło".
    /// </remarks>
    public static Armor GetRandomArmor(int tier, CharacterClass requiredClass)
    {
        return new Armor(EquipmentPartManager.GetRandomPart<ArmorPlate>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<ArmorBinder>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<ArmorBase>(tier, requiredClass),
            requiredClass,
            UtilityMethods.RandomChoice(Enum.GetValues<Quality>()
                .Where(x => x != Quality.Masterpiece).ToList()));
    }

    /// <summary>
    /// Tworzy losową zbroję określonego poziomu i losowej klasy postaci.
    /// </summary>
    /// <param name="tier">Poziom zbroi.</param>
    /// <returns>Nowa instancja losowej zbroi.</returns>
    /// <remarks>
    /// Klasa postaci jest losowana spośród wszystkich dostępnych klas.
    /// Jakość zbroi jest losowana z wyłączeniem jakości "Arcydzieło".
    /// </remarks>
    public static Armor GetRandomArmor(int tier)
    {
        var requiredClass = UtilityMethods.RandomChoice(Enum.GetValues<CharacterClass>().ToList());
        return new Armor(EquipmentPartManager.GetRandomPart<ArmorPlate>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<ArmorBinder>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<ArmorBase>(tier, requiredClass),
            requiredClass,
            UtilityMethods.RandomChoice(Enum.GetValues<Quality>()
                .Where(x => x != Quality.Masterpiece).ToList()));
    }

    /// <summary>
    /// Tworzy listę efektów pasywnych na podstawie komponentów galadurytów.
    /// </summary>
    /// <param name="equipmentType">Typ ekwipunku (true dla zbroi, false dla broni).</param>
    /// <param name="player">Postać gracza, do której będą przypisane efekty.</param>
    /// <param name="components">Zbiór komponentów galadurytów.</param>
    /// <returns>Lista efektów pasywnych.</returns>
    /// <remarks>
    /// Obsługiwane typy efektów:
    /// - Modyfikatory obrażeń, szansy na trafienie, krytyki, itp.
    /// - Modyfikatory obrony, uników, zdrowia, itp.
    /// - Modyfikatory odporności na różne typy obrażeń i efektów.
    /// </remarks>
    public static List<InnatePassiveEffect> GetInnatePassiveEffects(bool equipmentType, PlayerCharacter player,
        HashSet<GalduriteComponent> components)
    {
        var allowedTypes = new List<string>
        {
            "DamageDealtMod", "PhysicalDamageDealtMod", "MagicDamageDealtMod", "CritChanceMod", "CritModMod",
            "HitChanceMod", "SuppressionChanceMod","DebuffChanceMod", "DoTChanceMod", "ItemChanceMod", 
            "ResourceRegenMod", "UndeadDamageDealtMod", "HumanDamageDealtMod", "BeastDamageDealtMod",
            "DemonDamageDealtMod", "ExperienceGainMod", "GoldGainMod", "PhysicalDefensePen", "MagicDefensePen", 
            "MaximalHealthMod", "DodgeMod", "PhysicalDefenseMod", "MagicDefenseMod", "TotalDefenseMod", "SpeedMod", 
            "MaxActionPointsMod", "DoTResistanceMod", "SuppressionResistanceMod", "DebuffResistanceMod", 
            "TotalResistanceMod", "PhysicalDamageTakenMod", "MagicDamageTakenMod", "DamageTakenMod", "MaximalResourceMod",
            "BleedDamageTakenMod", "PoisonDamageTakenMod", "BurnDamageTakenMod", "ResourceCostMod", "CritSaveChanceMod"
        };
        return (from component in components
            where allowedTypes.Contains(component.EffectType)
            select new InnatePassiveEffect(player, equipmentType ? "ArmorGaldurites" : "WeaponGaldurites",
                component.EffectType, [component.EffectStrength, ModifierType.Multiplicative])).ToList();
    }

    /// <summary>
    /// Tworzy listę efektów nasłuchujących na podstawie komponentów galadurytów.
    /// </summary>
    /// <param name="equipmentType">Typ ekwipunku (true dla zbroi, false dla broni).</param>
    /// <param name="player">Postać gracza, do której będą przypisane efekty.</param>
    /// <param name="components">Zbiór komponentów galadurytów.</param>
    /// <returns>Lista efektów nasłuchujących.</returns>
    /// <remarks>
    /// Obsługiwane typy efektów:
    /// - Efekty aktywowane przy trafieniu (np. krwawienie, trucizna, ogień)
    /// - Leczenie przy trafieniu
    /// - Regeneracja zdrowia i zasobów
    /// - Efekty kontroli tłumu (ogłuszenie, spowolnienie, zamrożenie)
    /// </remarks>
    public static List<ListenerPassiveEffect?> GetListenerPassiveEffects(bool equipmentType, PlayerCharacter player,
        HashSet<GalduriteComponent> components)
    {
        var source = equipmentType ? "ArmorGaldurites" : "WeaponGaldurites";
        return components.Select(component => component.EffectType switch
            {
                "BleedOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                    data => new InflictDoTStatusEffect(SkillTarget.Enemy, 2, 0.5,
                            "Bleed", component.EffectStrength)
                        .Execute(player, data.Target?.User, source), player, source),
                "PoisonOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                    data => new InflictDoTStatusEffect(SkillTarget.Enemy, 2, 0.5,
                            "Poison", component.EffectStrength)
                        .Execute(player, data.Target?.User, source), player, source),
                "BurnOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                    data => new InflictDoTStatusEffect(SkillTarget.Enemy, 2, 0.5,
                            "Burn", component.EffectStrength)
                        .Execute(player, data.Target?.User, source), player, source),
                "HealOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                    data => new HealTarget(SkillTarget.Self, component.EffectStrength, DamageBase.Random).Execute(
                        player, player, source), player, source),
                "ResourceOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                    data => new RegenResource(SkillTarget.Self, component.EffectStrength, DamageBase.CasterMaxHealth)
                        .Execute(player, player, source), player, source),
                "AdvanceMoveOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                    data => data.Source.AdvanceMove((int)component.EffectStrength), player, source),
                "StunOnHit" => PassiveEffectFactory.StunOnHit(1, component.EffectStrength, source),
                "FreezeOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                    data => new InflictGenericStatusEffect("Freeze", 1, component.EffectStrength,
                        source).Execute(player, data.Target?.User, source), player,
                    source),
                "SlowOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                    data => new DebuffStat(SkillTarget.Enemy, StatType.Speed, ModifierType.Additive, 20,
                        component.EffectStrength, 3).Execute(player, data.Target?.User, source), player,
                    source),
                "HealthRegenPerTurn" => new ListenerPassiveEffect(data => data.EventType == "PerTurn",
                    data => new HealTarget(SkillTarget.Self, component.EffectStrength, DamageBase.CasterMaxHealth)
                        .Execute(player, player, source), player, source),
                "ResourceRegenPerTurn" => new ListenerPassiveEffect(data => data.EventType == "PerTurn",
                    data => new RegenResource(SkillTarget.Self, component.EffectStrength, DamageBase.CasterMaxHealth)
                        .Execute(player, player, source), player, source),
                _ => null,
            })
            .ToList();
    }
}