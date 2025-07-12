using Character = GodmistWPF.Characters.Character;
using SkillTarget = GodmistWPF.Enums.SkillTarget;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

/// <summary>
/// Efekt umiejętności, który przedłuża czas trwania efektu DoT (Damage over Time) na celu.
/// </summary>
/// <remarks>
/// Umożliwia przedłużenie czasu trwania obrażeń odnoszonych z upływem czasu,
/// takich jak krwawienie, zatrucie czy oparzenia.
/// </remarks>
public class ExtendDoT : IActiveSkillEffect
{
    /// <summary>
    /// Pobiera lub ustawia cel efektu (własna postać lub przeciwnik).
    /// </summary>
    public SkillTarget Target { get; set; }
    /// <summary>
    /// Pobiera lub ustawia typ efektu DoT do przedłużenia.
    /// </summary>
    /// <value>Dopuszczalne wartości: "Bleed", "Poison", "Burn" (wielkość liter ma znaczenie).</value>
    public string DoTType { get; set; }
    /// <summary>
    /// Pobiera lub ustawia liczbę tur, o jaką zostanie przedłużony efekt DoT.
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="ExtendDoT"/>.
    /// </summary>
    /// <param name="target">Cel efektu.</param>
    /// <param name="doTType">Typ efektu DoT do przedłużenia (Bleed, Poison lub Burn).</param>
    /// <param name="duration">Liczba tur, o jaką zostanie przedłużony efekt.</param>
    /// <exception cref="ArgumentException">Wyrzucany, gdy podano nieprawidłowy typ efektu DoT.</exception>
    public ExtendDoT(SkillTarget target, string doTType, int duration)
    {
        if (doTType != "Bleed" && doTType != "Poison" && doTType != "Burn")
        {
            throw new ArgumentException("Invalid DoT type. Must be Bleed, Poison, or Burn.");
        }
        Target = target;
        DoTType = doTType;
        Duration = duration;
    }
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="ExtendDoT"/>.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizm deserializacji.
    /// </remarks>
    public ExtendDoT() {}

    /// <summary>
    /// Wykonuje efekt przedłużenia czasu trwania DoT.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności).</param>
    /// <remarks>
    /// Przedłuża pierwszy znaleziony efekt DoT danego typu na celu.
    /// Jeśli nie znajdzie odpowiedniego efektu, nie wykonuje żadnej akcji.
    /// </remarks>
    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                foreach (var effect in caster.PassiveEffects.TimedEffects.Where(e => e.Type == DoTType))
                {
                    effect.Extend(Duration);
                    return;
                }
                break;
            case SkillTarget.Enemy:
                foreach (var effect in caster.PassiveEffects.TimedEffects.Where(e => e.Type == DoTType))
                {
                    effect.Extend(Duration);
                    return;
                }
                break;
        }
    }
}