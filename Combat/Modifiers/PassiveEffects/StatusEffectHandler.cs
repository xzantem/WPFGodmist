

using GodmistWPF.Characters;

namespace GodmistWPF.Combat.Modifiers.PassiveEffects;

/// <summary>
/// Klasa pomocnicza do obsługi efektów statusowych w grze.
/// </summary>
/// <remarks>
/// Zawiera metody do zarządzania efektami specjalnymi, takimi jak tarcze ochronne.
/// </remarks>
public static class StatusEffectHandler
{

    /// <summary>
    /// Przetwarza obrażenia na tarczach ochronnych.
    /// </summary>
    /// <param name="shields">Lista aktywnych tarcz ochronnych.</param>
    /// <param name="target">Cel, na którym znajdują się tarcze.</param>
    /// <param name="damage">Ilość obrażeń do zredukowania przez tarcze.</param>
    /// <returns>Pozostała ilość obrażeń po odjęciu wartości pochłoniętej przez tarcze.</returns>
    /// <remarks>
    /// Metoda iteruje przez tarcze w kolejności ich występowania na liście.
    /// Każda tarcza pochłania obrażenia do wyczerpania swojej wytrzymałości.
    /// Tarcze są usuwane, gdy ich wytrzymałość spadnie do zera.
    /// </remarks>
    public static double TakeShieldsDamage(List<TimedPassiveEffect> shields, Character target, double damage)
    {
        foreach (var shield in shields.ToList().TakeWhile(x => !(damage <= 0)))
        {
            if (shield.Effects[1] >= damage)
            {
                shield.Effects[1] -= damage;
                damage = 0;
                if (shield.Effects[1] == 0)
                {
                    target.PassiveEffects.Remove(shield);
                }
                break;
            }

            damage -= shield.Effects[1];
            target.PassiveEffects.Remove(shield);
        }
        return damage;
    }
}