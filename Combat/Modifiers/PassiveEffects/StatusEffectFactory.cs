using GodmistWPF.Characters;
using GodmistWPF.Combat.Skills.ActiveSkillEffects;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Modifiers;
using GodmistWPF.Utilities;

namespace GodmistWPF.Combat.Modifiers.PassiveEffects;

/// <summary>
/// Fabryka tworząca różne rodzaje efektów statusowych w grze.
/// </summary>
/// <remarks>
/// Zapewnia metody do łatwego tworzenia i aplikowania efektów statusowych na postaciach.
/// Obsługuje różne typy efektów, w tym obrażenia w czasie, ogłuszenia, zamrożenia i tarcze.
/// </remarks>
/// <seealso cref="TimedPassiveEffect"/>
public static class StatusEffectFactory
{
    /// <summary>
    /// Tworzy efekt obrażeń w czasie (DoT - Damage over Time).
    /// </summary>
    /// <param name="target">Cel, na który ma zostać nałożony efekt.</param>
    /// <param name="source">Źródło efektu (np. nazwa umiejętności).</param>
    /// <param name="effectType">Typ efektu (Bleed, Poison, Burn).</param>
    /// <param name="strength">Siła obrażeń w każdej turze.</param>
    /// <param name="duration">Czas trwania efektu w turach.</param>
    /// <param name="chance">Szansa na zadziałanie efektu (0.0-1.0).</param>
    /// <returns>Gotowy efekt pasywny do nałożenia na postać.</returns>
    /// <exception cref="ArgumentException">Gdy podano nieprawidłowy typ efektu.</exception>
    public static TimedPassiveEffect CreateDoTEffect(Character target, string source,
        string effectType, double strength, int duration, double chance)
    {
        return new TimedPassiveEffect(target, source,
            effectType, duration, [chance, strength], () =>
            {
                target.TakeDamage(
                    effectType switch
                    {
                        "Bleed" => DamageType.Bleed,
                        "Poison" => DamageType.Poison,
                        "Burn" => DamageType.Burn
                    }, strength, effectType);
            });
    }

    /// <summary>
    /// Tworzy efekt ogłuszenia.
    /// </summary>
    /// <param name="target">Cel, na który ma zostać nałożony efekt.</param>
    /// <param name="source">Źródło efektu.</param>
    /// <param name="duration">Czas trwania efektu w turach.</param>
    /// <param name="chance">Szansa na zadziałanie efektu (0.0-1.0).</param>
    /// <returns>Gotowy efekt ogłuszenia do nałożenia na postać.</returns>
    public static TimedPassiveEffect CreateStunEffect(Character target, string source, int duration, double chance)
    {
        return new TimedPassiveEffect(target, source, "Stun", duration, [chance]);
    }

    /// <summary>
    /// Tworzy efekt zamrożenia.
    /// </summary>
    /// <param name="target">Cel, na który ma zostać nałożony efekt.</param>
    /// <param name="source">Źródło efektu.</param>
    /// <param name="duration">Czas trwania efektu w turach.</param>
    /// <param name="chance">Szansa na zadziałanie efektu (0.0-1.0).</param>
    /// <returns>Gotowy efekt zamrożenia do nałożenia na postać.</returns>
    /// <remarks>
    /// Efekt zamrożenia dodatkowo spowalnia postać po wygaśnięciu.
    /// </remarks>
    public static TimedPassiveEffect CreateFreezeEffect(Character target, string source, int duration, double chance)
    {
        var t = new TimedPassiveEffect(target, source,
            "Freeze", duration, [chance]);
        t.UpdateOnTick(() =>
        {
            if (t.Duration == 1)
                new DebuffStat(SkillTarget.Self, StatType.Speed, ModifierType.Additive, 
                        duration * 2, 1, duration).Execute(target, target, source);
        });
        return t;
    }

    /// <summary>
    /// Tworzy efekt tarczy ochronnej.
    /// </summary>
    /// <param name="target">Cel, na który ma zostać nałożony efekt.</param>
    /// <param name="source">Źródło efektu.</param>
    /// <param name="strength">Siła tarczy (ilość pochłanianych obrażeń).</param>
    /// <param name="duration">Czas trwania efektu w turach.</param>
    /// <param name="chance">Szansa na zadziałanie efektu (0.0-1.0).</param>
    /// <returns>Gotowy efekt tarczy do nałożenia na postać.</returns>
    public static TimedPassiveEffect CreateShieldEffect(Character target, string source, double strength,
        int duration, double chance)
    {
        return new TimedPassiveEffect(target, source, "Shield", duration, [chance, strength]);
    }

    /// <summary>
    /// Próbuje dodać efekt do postaci, uwzględniając jej odporności.
    /// </summary>
    /// <param name="target">Postać, na którą ma zostać nałożony efekt.</param>
    /// <param name="effect">Efekt do nałożenia.</param>
    /// <param name="guaranteed">Czy efekt ma zostać nałożony z pominięciem odporności.</param>
    /// <returns>
    /// <c>true</c> jeśli efekt został nałożony; w przeciwnym razie <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Sprawdza odporności postaci przed nałożeniem efektu.
    /// Jeśli efekt nie jest gwarantowany, może nie zadziałać w zależności od odporności celu.
    /// </remarks>
    public static bool TryAddEffect(Character target, TimedPassiveEffect effect, bool guaranteed = false)
    {
        if (!guaranteed && target.Resistances.ContainsKey(Enum.GetValues<StatusEffectType>()
                            .FirstOrDefault(x => x.ToString() == effect.Type))
                        && Random.Shared.NextDouble() >= UtilityMethods.EffectChance(target.Resistances
                                [Enum.GetValues<StatusEffectType>().FirstOrDefault(x => x.ToString() == effect.Type)]
                            .Value(target, effect.Type + "Resistance"), effect.Effects[0])) return false;
        target.PassiveEffects.Add(effect);
        return true;

    }
}