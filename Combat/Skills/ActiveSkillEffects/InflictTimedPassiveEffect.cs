using Character = GodmistWPF.Characters.Character;
using SkillTarget = GodmistWPF.Enums.SkillTarget;
using TimedPassiveEffect = GodmistWPF.Combat.Modifiers.PassiveEffects.TimedPassiveEffect;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

/// <summary>
/// Efekt umiejętności, który nakłada czasowy efekt pasywny na cel.
/// </summary>
/// <remarks>
/// Umożliwia nałożenie efektu pasywnego, który wygaśnie po określonym czasie.
/// Efekt może być zastosowany zarówno do siebie, jak i do przeciwnika.
/// </remarks>
public class InflictTimedPassiveEffect : IActiveSkillEffect
{
    /// <summary>
    /// Pobiera lub ustawia cel efektu (własna postać lub przeciwnik).
    /// </summary>
    public SkillTarget Target { get; set; }
    /// <summary>
    /// Pobiera lub ustawia typ efektu pasywnego do nałożenia.
    /// </summary>
    /// <value>
    /// Identyfikator typu efektu, który ma zostać nałożony.
    /// </value>
    public string Type { get; set; }
    /// <summary>
    /// Pobiera lub ustawia czas trwania efektu w turach.
    /// </summary>
    public int Duration { get; set; }
    /// <summary>
    /// Pobiera lub ustawia dodatkowe parametry efektu.
    /// </summary>
    /// <value>
    /// Tablica dynamicznych obiektów zawierających parametry specyficzne dla danego typu efektu.
    /// </value>
    public dynamic[]? Effects { get; set; }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="InflictTimedPassiveEffect"/>.
    /// </summary>
    /// <param name="target">Cel efektu.</param>
    /// <param name="type">Typ efektu pasywnego do nałożenia.</param>
    /// <param name="duration">Czas trwania efektu w turach.</param>
    /// <param name="effects">Opcjonalne parametry efektu.</param>
    public InflictTimedPassiveEffect(SkillTarget target, string type, int duration, dynamic[]? effects = null)
    {
        Target = target;
        Type = type;
        Duration = duration;
        Effects = effects;
    }
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="InflictTimedPassiveEffect"/>.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizm deserializacji.
    /// </remarks>
    public InflictTimedPassiveEffect() {}

    /// <summary>
    /// Wykonuje efekt nałożenia czasowego efektu pasywnego.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności).</param>
    /// <remarks>
    /// Nakłada czasowy efekt pasywny na wybrany cel.
    /// Efekt zostanie automatycznie usunięty po upływie określonej liczby tur.
    /// </remarks>
    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                caster.PassiveEffects.Add(new TimedPassiveEffect(caster, source, Type, Duration, Effects));
                break;
            case SkillTarget.Enemy:
                enemy.PassiveEffects.Add(new TimedPassiveEffect(caster, source, Type, Duration, Effects));
                break;
        }
    }
}