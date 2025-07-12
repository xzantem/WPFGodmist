using GodmistWPF.Utilities;
using Character = GodmistWPF.Characters.Character;
using ModifierType = GodmistWPF.Enums.Modifiers.ModifierType;
using SkillTarget = GodmistWPF.Enums.SkillTarget;
using StatModifier = GodmistWPF.Combat.Modifiers.StatModifier;
using StatusEffectType = GodmistWPF.Enums.Modifiers.StatusEffectType;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

/// <summary>
/// Efekt umiejętności, który zmniejsza odporność na określony efekt statusowy.
/// </summary>
/// <remarks>
/// Umożliwia osłabienie obrony celu przed konkretnym typem efektów statusowych,
/// zwiększając skuteczność kolejnych efektów tego typu.
/// </remarks>
public class DebuffResistance : IActiveSkillEffect
{
    /// <summary>
    /// Pobiera lub ustawia cel efektu (własna postać lub przeciwnik).
    /// </summary>
    public SkillTarget Target { get; set; }
    /// <summary>
    /// Pobiera lub ustawia typ efektu statusowego, którego odporność ma zostać zmniejszona.
    /// </summary>
    public StatusEffectType ResistanceToDebuff { get; set; }
    /// <summary>
    /// Pobiera lub ustawia typ modyfikatora używany do zmniejszenia odporności.
    /// </summary>
    public ModifierType ModifierType { get; set; }
    /// <summary>
    /// Pobiera lub ustawia siłę zmniejszenia odporności.
    /// </summary>
    public double DebuffStrength { get; set; }
    /// <summary>
    /// Pobiera lub ustawia szansę na zadziałanie efektu (0.0-1.0).
    /// </summary>
    public double DebuffChance { get; set; }
    /// <summary>
    /// Pobiera lub ustawia czas trwania efektu w turach.
    /// </summary>
    public int DebuffDuration { get; set; }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DebuffResistance"/>.
    /// </summary>
    /// <param name="target">Cel efektu.</param>
    /// <param name="resistanceToDebuff">Typ efektu statusowego do osłabienia.</param>
    /// <param name="modifier">Typ modyfikatora.</param>
    /// <param name="debuffStrength">Siła zmniejszenia odporności.</param>
    /// <param name="debuffChance">Szansa na zadziałanie efektu (0.0-1.0).</param>
    /// <param name="debuffDuration">Czas trwania efektu w turach.</param>
    public DebuffResistance(SkillTarget target, StatusEffectType resistanceToDebuff,
        ModifierType modifier, double debuffStrength, double debuffChance, int debuffDuration)
    {
        Target = target;
        ResistanceToDebuff = resistanceToDebuff;
        ModifierType = modifier;
        DebuffStrength = debuffStrength;
        DebuffChance = debuffChance;
        DebuffDuration = debuffDuration;
    }
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DebuffResistance"/>.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizm deserializacji.
    /// </remarks>
    public DebuffResistance() {}

    /// <summary>
    /// Wykonuje efekt zmniejszenia odporności.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności).</param>
    /// <remarks>
    /// W przypadku celu będącego przeciwnikiem, sprawdza szansę na trafienie,
    /// uwzględniając jego odporność na efekty osłabiające.
    /// </remarks>
    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                caster.AddResistanceModifier(ResistanceToDebuff,
                    new StatModifier(ModifierType, -DebuffStrength, source, DebuffDuration));
                break;
            case SkillTarget.Enemy:
                if (Random.Shared.NextDouble() <
                    UtilityMethods.EffectChance(enemy.Resistances[StatusEffectType.Debuff].Value(enemy, "DebuffResistance"), DebuffChance))
                {
                    enemy.AddResistanceModifier(ResistanceToDebuff,
                        new StatModifier(ModifierType, -DebuffStrength, source, DebuffDuration));
                }
                break;
        }
    }
}