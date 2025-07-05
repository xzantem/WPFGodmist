

using GodmistWPF.Enums.Modifiers;

namespace GodmistWPF.Items.Galdurites;

public class GalduriteComponent
{
    public string PoolColor { get; set; }
    public string EffectTier { get; set; }
    public double EffectStrength { get; set; }
    public string EffectType { get; set; }
    
    public ModifierType ModifierType { get; set; }

    public string EffectText
    {
        get
        {
            return EffectType switch
            {
                "DamageDealtMod" => $"Total Damage Dealt: +{EffectStrength:P0}",
                "PhysicalDamageDealtMod" => $"Physical Damage Dealt: +{EffectStrength:P0}",
                "MagicDamageDealtMod" => $"Magic Damage Dealt: +{EffectStrength:P0}",
                "CritChanceMod" => $"Critical Hit Chance: +{EffectStrength:P0}",
                "CritModMod" => $"Critical Hit Damage: +{EffectStrength:F2}x",
                "HitChanceMod" => $"Hit Chance: +{EffectStrength:P0}",
                "SuppressionChanceMod" => $"Suppression Chance: +{EffectStrength:P0}",
                "DebuffChanceMod" => $"Debuff Chance: +{EffectStrength:P0}",
                "DoTChanceMod" => $"Damage over Time Chance: +{EffectStrength:P0}",
                "ItemChanceMod" => $"Item Drop Chance: +{EffectStrength:P0}",
                "ResourceRegenMod" => $"Resource Regeneration: +{EffectStrength:P0}",
                "UndeadDamageDealtMod" => $"Undead Damage Dealt: +{EffectStrength:P0}",
                "HumanDamageDealtMod" => $"Human Damage Dealt: +{EffectStrength:P0}",
                "BeastDamageDealtMod" => $"Beast Damage Dealt: +{EffectStrength:P0}",
                "DemonDamageDealtMod" => $"Demon Damage Dealt: +{EffectStrength:P0}",
                "ExperienceGainMod" => $"Experience Gain: +{EffectStrength:P0}",
                "GoldGainMod" => $"Gold Gain: +{EffectStrength:P0}",
                "PhysicalDefensePen" => $"Physical Defense Penetration: +{EffectStrength:P0}",
                "MagicDefensePen" => $"Magic Defense Penetration: +{EffectStrength:P0}",
                "MaximalHealthMod" => $"Maximal Health: +{EffectStrength:P0}",
                "DodgeMod" => $"Dodge Chance: +{EffectStrength:P0}",
                "PhysicalDefenseMod" => $"Physical Defense: +{EffectStrength:P0}",
                "MagicDefenseMod" => $"Magic Defense: +{EffectStrength:P0}",
                "TotalDefenseMod" => $"Total Defense: +{EffectStrength:P0}",
                "SpeedMod" => $"Speed: +{EffectStrength:P0}",
                "MaxActionPointsMod" => $"Max Action Points: +{EffectStrength:P0}",
                "DoTResistanceMod" => $"Damage over Time Resistances: +{EffectStrength:P0}",
                "SuppressionResistanceMod" => $"Suppression Resistances: +{EffectStrength:P0}",
                "DebuffResistanceMod" => $"Debuff Resistances: +{EffectStrength:P0}",
                "TotalResistanceMod" => $"All Resistances: +{EffectStrength:P0}",
                "PhysicalDamageTakenMod" => $"Physical Damage Taken: {EffectStrength:P0}",
                "MagicDamageTakenMod" => $"Magic Damage Taken: {EffectStrength:P0}",
                "DamageTakenMod" => $"Total Damage Taken: {EffectStrength:P0}",
                "MaximalResourceMod" => $"Maximal Resource: +{EffectStrength:P0}",
                "BleedDamageTakenMod" => $"Bleed Damage Taken: {EffectStrength:P0}",
                "PoisonDamageTakenMod" => $"Poison Damage Taken: {EffectStrength:P0}",
                "BurnDamageTakenMod" => $"Burn Damage Taken: {EffectStrength:P0}",
                "ResourceCostMod" => $"Resource Cost Mod: {EffectStrength:P0}",
                "CritSaveChanceMod" => $"Critical Strike Negate Chance: +{EffectStrength:P0}",
                "BleedOnHit" => $"Bleed On Hit Chance: +{EffectStrength:P0}",
                "PoisonOnHit" => $"Poison On Hit Chance: +{EffectStrength:P0}",
                "BurnOnHit" => $"Burn On Hit Chance: +{EffectStrength:P0}",
                "HealOnHit" => $"Heal On Hit (Damage Dealt): +{EffectStrength:P0}",
                "ResourceOnHit" => $"Resource Restore On Hit (Of Max): +{EffectStrength:P0}",
                "AdvanceMoveOnHit" => $"Advance Move on Hit: +{EffectStrength:F0} AV",
                "StunOnHit" => $"Stun On Hit Chance: +{EffectStrength:P0}",
                "FreezeOnHit" => $"Freeze On Hit Chance: +{EffectStrength:P0}",
                "SlowOnHit" => $"Slow On Hit Chance: +{EffectStrength:P0}",
                "HealthRegenPerTurn" => $"Passive Health Regen (Of Max): +{EffectStrength:P0}",
                "ResourceRegenPerTurn" => $"Passive Resource Regen (Of Max): +{EffectStrength:P0}",
                _ => throw new ArgumentOutOfRangeException(nameof(EffectType))
            };
        }
    }

    public bool EquipmentType { get; set; }
}