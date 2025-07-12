using GodmistWPF.Characters;
using GodmistWPF.Utilities;

namespace GodmistWPF.Combat.Modifiers;

/// <summary>
/// Klasa reprezentująca statystykę postaci, która może być modyfikowana przez różne efekty.
/// </summary>
/// <remarks>
/// Zawiera wartość bazową, współczynnik skalowania i listę modyfikatorów.
/// Wartość końcowa jest obliczana na podstawie poziomu postaci i aktywnych modyfikatorów.
/// </remarks>
public class Stat
{
    /// <summary>
    /// Pobiera lub ustawia bazową wartość statystyki.
    /// </summary>
    public double BaseValue { get; set; }
    /// <summary>
    /// Pobiera lub ustawia współczynnik skalowania statystyki w zależności od poziomu.
    /// </summary>
    public double ScalingFactor { get; set; }
    /// <summary>
    /// Pobiera listę modyfikatorów wpływających na tę statystykę.
    /// </summary>
    public List<StatModifier> Modifiers { get; private set; } = [];
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="Stat"/> z domyślnymi wartościami.
    /// </summary>
    public Stat() {}
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="Stat"/> z określonymi wartościami.
    /// </summary>
    /// <param name="baseValue">Wartość bazowa statystyki.</param>
    /// <param name="scalingFactor">Współczynnik skalowania w zależności od poziomu.</param>
    public Stat(double baseValue, double scalingFactor)
    {
        BaseValue = baseValue;
        ScalingFactor = scalingFactor;
    }
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="Stat"/> z określonymi wartościami i modyfikatorami.
    /// </summary>
    /// <param name="baseValue">Wartość bazowa statystyki.</param>
    /// <param name="scalingFactor">Współczynnik skalowania w zależności od poziomu.</param>
    /// <param name="modifiers">Lista modyfikatorów do zastosowania.</param>
    public Stat(double baseValue, double scalingFactor, List<StatModifier> modifiers)
    {
        BaseValue = baseValue;
        ScalingFactor = scalingFactor;
        Modifiers = modifiers;
    }

    /// <summary>
    /// Oblicza końcową wartość statystyki, uwzględniając modyfikatory i skalowanie.
    /// </summary>
    /// <param name="owner">Właściciel statystyki (postać).</param>
    /// <param name="statName">Nazwa statystyki używana do wyszukiwania odpowiednich modyfikatorów.</param>
    /// <returns>Końcowa wartość statystyki po zastosowaniu wszystkich modyfikatorów.</returns>
    /// <remarks>
    /// Uwzględnia specjalne przypadki dla obrony i odporności.
    /// </remarks>
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

    /// <summary>
    /// Aktualizuje czas trwania wszystkich modyfikatorów statystyki.
    /// </summary>
    /// <remarks>
    /// Usuwa modyfikatory, których czas trwania wygasł.
    /// </remarks>
    public void Tick()
    {
        foreach (var modifier in Modifiers.ToList())
        {
            modifier.Tick();
            if (modifier.Duration <= 0)
                Modifiers.Remove(modifier);
        }
    }
    

    /// <summary>
    /// Dodaje nowy modyfikator do statystyki.
    /// </summary>
    /// <param name="modifier">Modyfikator do dodania.</param>
    public void AddModifier(StatModifier modifier)
    {
        Modifiers.Add(modifier);
    }
}