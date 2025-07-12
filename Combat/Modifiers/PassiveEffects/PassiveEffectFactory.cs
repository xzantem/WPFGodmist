using GodmistWPF.Combat.Skills.ActiveSkillEffects;
using GodmistWPF.Characters.Player;

namespace GodmistWPF.Combat.Modifiers.PassiveEffects;

/// <summary>
/// Fabryka tworząca gotowe efekty pasywne do wykorzystania w grze.
/// </summary>
/// <remarks>
/// Zawiera metody pomocnicze do tworzenia często używanych efektów pasywnych.
/// Upraszcza proces tworzenia i konfigurowania efektów.
/// </remarks>
public static class PassiveEffectFactory
{
    /// <summary>
    /// Tworzy efekt pasywny ogłuszający przeciwnika przy trafieniu.
    /// </summary>
    /// <param name="duration">Czas trwania efektu ogłuszenia w turach.</param>
    /// <param name="chance">Szansa na zadziałanie efektu (wartość z przedziału 0.0 do 1.0).</param>
    /// <param name="source">Źródło efektu (np. nazwa umiejętności lub przedmiotu).</param>
    /// <returns>Gotowy do użycia efekt pasywny.</returns>
    /// <remarks>
    /// Efekt aktywuje się przy każdym trafieniu przeciwnika.
    /// Ma określoną szansę na nałożenie efektu ogłuszenia.
    /// </remarks>
    public static ListenerPassiveEffect StunOnHit(int duration, double chance, string source)
    {
        return new ListenerPassiveEffect(data => data.EventType == "OnHit",
            data => new InflictGenericStatusEffect("Stun", duration, chance,
                source).Execute(PlayerHandler.player, data.Target?.User, source), 
            PlayerHandler.player, source);
    }
}