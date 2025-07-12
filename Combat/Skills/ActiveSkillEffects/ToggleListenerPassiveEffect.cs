using Character = GodmistWPF.Characters.Character;
using ListenerPassiveEffect = GodmistWPF.Combat.Modifiers.PassiveEffects.ListenerPassiveEffect;
using SkillTarget = GodmistWPF.Enums.SkillTarget;
using StatType = GodmistWPF.Enums.Modifiers.StatType;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

/// <summary>
/// Efekt umiejętności, który przełącza (włącza/wyłącza) efekt pasywny nasłuchujący zdarzeń.
/// </summary>
/// <remarks>
/// Umożliwia dynamiczne włączanie i wyłączanie efektów, które reagują na określone zdarzenia w walce.
/// Jeśli efekt jest aktywny, zostanie wyłączony, a jeśli nieaktywny - zostanie włączony.
/// </remarks>
public class ToggleListenerPassiveEffect : IActiveSkillEffect
{
    /// <summary>
    /// Pobiera lub ustawia cel efektu (własna postać lub przeciwnik).
    /// </summary>
    public SkillTarget Target { get; set; }
    /// <summary>
    /// Pobiera lub ustawia typ efektu pasywnego do przełączenia.
    /// </summary>
    /// <value>
    /// Identyfikator typu efektu, który ma zostać przełączony (np. "SlowOnHit").
    /// </value>
    public string PassiveEffect { get; set; }
    /// <summary>
    /// Pobiera lub ustawia dodatkowe parametry efektu.
    /// </summary>
    /// <value>
    /// Tablica dynamicznych obiektów zawierających parametry specyficzne dla danego typu efektu.
    /// </value>
    public dynamic[]? Effects { get; set; }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="ToggleListenerPassiveEffect"/>.
    /// </summary>
    /// <param name="target">Cel efektu.</param>
    /// <param name="passiveEffect">Typ efektu pasywnego do przełączenia.</param>
    /// <param name="effects">Opcjonalne parametry efektu.</param>
    public ToggleListenerPassiveEffect(SkillTarget target, string passiveEffect, dynamic[]? effects = null)
    {
        Target = target;
        PassiveEffect = passiveEffect;
        Effects = effects;
    }
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="ToggleListenerPassiveEffect"/>.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizm deserializacji.
    /// </remarks>
    public ToggleListenerPassiveEffect() {}

    /// <summary>
    /// Wykonuje efekt przełączania efektu pasywnego nasłuchującego.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności).</param>
    /// <remarks>
    /// Sprawdza, czy efekt jest już aktywny na celu na podstawie źródła.
    /// Jeśli tak, go usuwa, jeśli nie - dodaje nowy.
    /// </remarks>
    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                if (caster.PassiveEffects.ListenerEffects.Any(x => x.Source == source))
                    caster.PassiveEffects.ListenerEffects.RemoveAll(x => x.Source == source);
                else
                    caster.PassiveEffects.Add(GetPassiveEffect(caster, enemy, source));
                break;
            case SkillTarget.Enemy:
                if (enemy.PassiveEffects.ListenerEffects.Any(x => x.Source == source))
                    enemy.PassiveEffects.ListenerEffects.RemoveAll(x => x.Source == source);
                else
                    enemy.PassiveEffects.Add(GetPassiveEffect(caster, enemy, source));
                break;
        }
    }

    /// <summary>
    /// Tworzy nową instancję efektu pasywnego na podstawie typu.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności).</param>
    /// <returns>Nowa instancja efektu pasywnego.</returns>
    /// <remarks>
    /// Obsługuje różne typy efektów pasywnych, które mogą być przełączane.
    /// Każdy typ efektu jest tworzony z odpowiednimi parametrami.
    /// </remarks>
    private ListenerPassiveEffect GetPassiveEffect(Character caster, Character enemy, string source)
    {
        return PassiveEffect switch
        {
            "SlowOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                data => new DebuffStat(SkillTarget.Enemy, StatType.Speed, 
                    Effects[0], Effects[1], Effects[2], Effects[3])
                    .Execute(caster, enemy, source), caster, source)
        };
    }
}