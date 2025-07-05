using GodmistWPF.Characters;
using GodmistWPF.Utilities;

namespace GodmistWPF.Combat.Modifiers;

public class Stat
{
    public double BaseValue { get; set; }
    public double ScalingFactor { get; set; }
    public List<StatModifier> Modifiers { get; private set; } = [];
    
    public Stat() {}
    
    public Stat(double baseValue, double scalingFactor)
    {
        BaseValue = baseValue;
        ScalingFactor = scalingFactor;
    }
    public Stat(double baseValue, double scalingFactor, List<StatModifier> modifiers)
    {
        BaseValue = baseValue;
        ScalingFactor = scalingFactor;
        Modifiers = modifiers;
    }

    public double Value(Character owner, string statName)
    {
        var value = UtilityMethods.ScaledStat(BaseValue, ScalingFactor, owner.Level);
        var mods = new List<StatModifier>();
        mods.AddRange(Modifiers);
        mods.AddRange(owner.PassiveEffects.GetModifiers(statName));
        if (statName is "PhysicalDefense" or "MagicDefense")
            mods.AddRange(owner.PassiveEffects.GetModifiers("TotalDefense"));
        if (statName is "BleedResistance" or "PoisonResistance" or "BurnResistance")
        { mods.AddRange(owner.PassiveEffects.GetModifiers("DoTResistanceMod"));
            mods.AddRange(owner.PassiveEffects.GetModifiers("TotalResistanceMod")); }
        else if (statName.EndsWith("Resistance") && statName.StartsWith("Debuff"))
        { mods.AddRange(owner.PassiveEffects.GetModifiers("DebuffResistanceMod"));
            mods.AddRange(owner.PassiveEffects.GetModifiers("TotalResistanceMod")); }
        else if(statName.EndsWith("Resistance"))
        { mods.AddRange(owner.PassiveEffects.GetModifiers("SuppressionResistanceMod")); 
            mods.AddRange(owner.PassiveEffects.GetModifiers("TotalResistanceMod")); }
        mods.AddRange(owner.PassiveEffects.GetStatScaleModifiers(statName, owner));
        return UtilityMethods.CalculateModValue(value, mods);
    }

    public void Tick()
    {
        foreach (var modifier in Modifiers.ToList())
        {
            modifier.Tick();
            if (modifier.Duration <= 0)
                Modifiers.Remove(modifier);
        }
    }
    

    public void AddModifier(StatModifier modifier)
    {
        Modifiers.Add(modifier);
    }
}