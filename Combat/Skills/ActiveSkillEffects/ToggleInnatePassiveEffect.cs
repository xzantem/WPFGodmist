using Character = GodmistWPF.Characters.Character;
using InnatePassiveEffect = GodmistWPF.Combat.Modifiers.PassiveEffects.InnatePassiveEffect;
using SkillTarget = GodmistWPF.Enums.SkillTarget;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

/// <summary>
/// Efekt umiejętności, który przełącza (włącza/wyłącza) wrodzony efekt pasywny na celu.
/// </summary>
/// <remarks>
/// Pozwala na dynamiczne włączanie i wyłączanie efektów pasywnych w trakcie walki.
/// Jeśli efekt jest aktywny, zostanie wyłączony, a jeśli nieaktywny - zostanie włączony.
/// </remarks>
public class ToggleInnatePassiveEffect : IActiveSkillEffect
{
    /// <summary>
    /// Pobiera lub ustawia cel efektu (własna postać lub przeciwnik).
    /// </summary>
    public SkillTarget Target { get; set; }
    /// <summary>
    /// Pobiera lub ustawia typ efektu pasywnego do przełączenia.
    /// </summary>
    /// <value>
    /// Identyfikator typu efektu, który ma zostać przełączony.
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
    /// Inicjalizuje nową instancję klasy <see cref="ToggleInnatePassiveEffect"/>.
    /// </summary>
    /// <param name="target">Cel efektu.</param>
    /// <param name="passiveEffect">Typ efektu pasywnego do przełączenia.</param>
    /// <param name="effects">Opcjonalne parametry efektu.</param>
    public ToggleInnatePassiveEffect(SkillTarget target, string passiveEffect, dynamic[]? effects = null)
    {
        Target = target;
        PassiveEffect = passiveEffect;
        Effects = effects;
    }
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="ToggleInnatePassiveEffect"/>.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizm deserializacji.
    /// </remarks>
    public ToggleInnatePassiveEffect() {}

    /// <summary>
    /// Wykonuje efekt przełączania efektu pasywnego.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności).</param>
    /// <remarks>
    /// Sprawdza, czy efekt jest już aktywny na celu. Jeśli tak, go usuwa, jeśli nie - dodaje nowy.
    /// </remarks>
    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                if (caster.PassiveEffects.InnateEffects.Any(x => x.Type == PassiveEffect))
                    caster.PassiveEffects.InnateEffects.RemoveAll(x => x.Type == PassiveEffect);
                else
                    caster.PassiveEffects.Add(new InnatePassiveEffect(caster, source, PassiveEffect, Effects));
                break;
            case SkillTarget.Enemy:
                if (enemy.PassiveEffects.InnateEffects.Any(x => x.Type == PassiveEffect))
                    enemy.PassiveEffects.InnateEffects.RemoveAll(x => x.Type == PassiveEffect);
                else
                    enemy.PassiveEffects.Add(new InnatePassiveEffect(enemy, source, PassiveEffect, Effects));
                break;
        }
    }
}