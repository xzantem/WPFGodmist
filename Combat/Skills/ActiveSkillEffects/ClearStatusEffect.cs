using Character = GodmistWPF.Characters.Character;
using SkillTarget = GodmistWPF.Enums.SkillTarget;
using StatusEffectType = GodmistWPF.Enums.Modifiers.StatusEffectType;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

/// <summary>
/// Efekt umiejętności, który usuwa określony efekt statusowy z celu.
/// </summary>
/// <remarks>
/// Umożliwia usunięcie negatywnych efektów z sojuszników lub pozytywnych z przeciwników.
/// </remarks>
public class ClearStatusEffect : IActiveSkillEffect
{
    /// <summary>
    /// Pobiera lub ustawia cel efektu (własna postać lub przeciwnik).
    /// </summary>
    public SkillTarget Target { get; set; }
    /// <summary>
    /// Pobiera lub ustawia typ efektu statusowego do usunięcia.
    /// </summary>
    public StatusEffectType StatusEffectType { get; set; }
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="ClearStatusEffect"/>.
    /// </summary>
    /// <param name="target">Cel efektu.</param>
    /// <param name="statusEffectType">Typ efektu statusowego do usunięcia.</param>
    public ClearStatusEffect(SkillTarget target, StatusEffectType statusEffectType)
    {
        Target = target;
        StatusEffectType = statusEffectType;
    }
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="ClearStatusEffect"/>.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizm deserializacji.
    /// </remarks>
    public ClearStatusEffect() {}
    /// <summary>
    /// Wykonuje efekt usunięcia efektu statusowego.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności).</param>
    /// <remarks>
    /// Usuwa wybrany efekt statusowy z określonego celu.
    /// </remarks>
    public void Execute(Character caster, Character enemy, string source)
    {
        //TODO
    }
}