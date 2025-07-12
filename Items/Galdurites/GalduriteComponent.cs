using GodmistWPF.Enums.Modifiers;

namespace GodmistWPF.Items.Galdurites;

/// <summary>
/// Klasa reprezentująca pojedynczy komponent galduritu, zawierający efekt i jego parametry.
/// Każdy galdurit może zawierać wiele komponentów, z których każdy dodaje inny efekt.
/// </summary>
public class GalduriteComponent
{
    /// <summary>
    /// Pobiera lub ustawia kolor puli, z której pochodzi efekt.
    /// Określa kolorystykę i tematykę efektu.
    /// </summary>
    public string PoolColor { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia poziom (tier) efektu (A, B, C, D, S).
    /// Im wyższa litera alfabetu (z wyjątkiem S, które jest najwyższe), tym silniejszy efekt.
    /// </summary>
    public string EffectTier { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia siłę efektu, zazwyczaj jako wartość procentowa.
    /// </summary>
    public double EffectStrength { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia typ efektu, który określa jaką właściwość modyfikuje.
    /// </summary>
    public string EffectType { get; set; }
    
    /// <summary>
    /// Pobiera lub ustawia typ modyfikatora, który określa sposób aplikowania efektu.
    /// </summary>
    public ModifierType ModifierType { get; set; }

    /// <summary>
    /// Pobiera sformatowany tekst opisu efektu, gotowy do wyświetlenia w interfejsie użytkownika.
    /// </summary>
    public string EffectText
    {
        get
        {
            return EffectType switch
            {
                "DamageDealtMod" => $"Total Damage Dealt: +{EffectStrength:P1}",
                "PhysicalDamageDealtMod" => $"Physical Damage Dealt: +{EffectStrength:P1}",
                "MagicDamageDealtMod" => $"Magic Damage Dealt: +{EffectStrength:P1}",
                "CritChanceMod" => $"Critical Hit Chance: +{EffectStrength:P1}",
                "CritModMod" => $"Critical Hit Damage: +{EffectStrength:F2}x",
                "HitChanceMod" => $"Hit Chance: +{EffectStrength:P1}",
                "SuppressionChanceMod" => $"Suppression Chance: +{EffectStrength:P1}",
                "DebuffChanceMod" => $"Debuff Chance: +{EffectStrength:P1}",
                "DoTChanceMod" => $"Damage over Time Chance: +{EffectStrength:P1}",
                "ItemChanceMod" => $"Item Drop Chance: +{EffectStrength:P1}",
                "ResourceRegenMod" => $"Resource Regeneration: +{EffectStrength:P1}",
                "UndeadDamageDealtMod" => $"Undead Damage Dealt: +{EffectStrength:P1}",
                "HumanDamageDealtMod" => $"Human Damage Dealt: +{EffectStrength:P1}",
                "BeastDamageDealtMod" => $"Beast Damage Dealt: +{EffectStrength:P1}",
                "DemonDamageDealtMod" => $"Demon Damage Dealt: +{EffectStrength:P1}",
                "ExperienceGainMod" => $"Experience Gain: +{EffectStrength:P1}",
                "GoldGainMod" => $"Gold Gain: +{EffectStrength:P1}",
                "PhysicalDefensePen" => $"Physical Defense Penetration: +{EffectStrength:P1}",
                "MagicDefensePen" => $"Magic Defense Penetration: +{EffectStrength:P1}",
                "MaximalHealthMod" => $"Maximal Health: +{EffectStrength:P1}",
                "DodgeMod" => $"Dodge Chance: +{EffectStrength:P1}",
                "PhysicalDefenseMod" => $"Physical Defense: +{EffectStrength:P1}",
                "MagicDefenseMod" => $"Magic Defense: +{EffectStrength:P1}",
                "TotalDefenseMod" => $"Total Defense: +{EffectStrength:P1}",
                "SpeedMod" => $"Speed: +{EffectStrength:P1}",
                "MaxActionPointsMod" => $"Max Action Points: +{EffectStrength:P1}",
                "DoTResistanceMod" => $"Damage over Time Resistances: +{EffectStrength:P1}",
                "SuppressionResistanceMod" => $"Suppression Resistances: +{EffectStrength:P1}",
                "DebuffResistanceMod" => $"Debuff Resistances: +{EffectStrength:P1}",
                "TotalResistanceMod" => $"All Resistances: +{EffectStrength:P1}",
                "PhysicalDamageTakenMod" => $"Physical Damage Taken: {EffectStrength:P1}",
                "MagicDamageTakenMod" => $"Magic Damage Taken: {EffectStrength:P1}",
                "DamageTakenMod" => $"Total Damage Taken: {EffectStrength:P1}",
                "MaximalResourceMod" => $"Maximal Resource: +{EffectStrength:P1}",
                "BleedDamageTakenMod" => $"Bleed Damage Taken: {EffectStrength:P1}",
                "PoisonDamageTakenMod" => $"Poison Damage Taken: {EffectStrength:P1}",
                "BurnDamageTakenMod" => $"Burn Damage Taken: {EffectStrength:P1}",
                "ResourceCostMod" => $"Resource Cost Mod: {EffectStrength:P1}",
                "CritSaveChanceMod" => $"Critical Strike Negate Chance: +{EffectStrength:P1}",
                "BleedOnHit" => $"Bleed On Hit Chance: +{EffectStrength:P1}",
                "PoisonOnHit" => $"Poison On Hit Chance: +{EffectStrength:P1}",
                "BurnOnHit" => $"Burn On Hit Chance: +{EffectStrength:P1}",
                "HealOnHit" => $"Heal On Hit (Damage Dealt): +{EffectStrength:P1}",
                "ResourceOnHit" => $"Resource Restore On Hit (Of Max): +{EffectStrength:P1}",
                "AdvanceMoveOnHit" => $"Advance Move on Hit: +{EffectStrength:F0} AV",
                "StunOnHit" => $"Stun On Hit Chance: +{EffectStrength:P1}",
                "FreezeOnHit" => $"Freeze On Hit Chance: +{EffectStrength:P1}",
                "SlowOnHit" => $"Slow On Hit Chance: +{EffectStrength:P1}",
                "HealthRegenPerTurn" => $"Passive Health Regen (Of Max): +{EffectStrength:P1}",
                "ResourceRegenPerTurn" => $"Passive Resource Regen (Of Max): +{EffectStrength:P1}",
                _ => throw new ArgumentOutOfRangeException(nameof(EffectType))
            };
        }
    }

    /// <summary>
    /// Określa, czy komponent jest przeznaczony do zbroi (true) czy do broni (false).
    /// </summary>
    public bool EquipmentType { get; set; }
}