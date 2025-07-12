
using GodmistWPF.Utilities;
using Character = GodmistWPF.Characters.Character;
using ModifierType = GodmistWPF.Enums.Modifiers.ModifierType;
using SkillTarget = GodmistWPF.Enums.SkillTarget;
using StatModifier = GodmistWPF.Combat.Modifiers.StatModifier;
using StatType = GodmistWPF.Enums.Modifiers.StatType;
using StatusEffectType = GodmistWPF.Enums.Modifiers.StatusEffectType;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

/// <summary>
/// Efekt umiejętności, który zmniejsza wartość wybranej statystyki celu.
/// </summary>
/// <remarks>
/// Może być używany do osłabiania przeciwników lub samookaleczania się.
/// W przypadku używania na przeciwniku, uwzględnia jego odporność na efekty osłabiające.
/// </remarks>
public class DebuffStat : IActiveSkillEffect
{
    /// <summary>
    /// Pobiera lub ustawia cel efektu (własna postać lub przeciwnik).
    /// </summary>
    public SkillTarget Target { get; set; }
    /// <summary>
    /// Pobiera lub ustawia typ statystyki, która ma zostać zmniejszona.
    /// </summary>
    public StatType StatToDebuff { get; set; }
    /// <summary>
    /// Pobiera lub ustawia typ modyfikatora (np. addytywny, mnożnikowy).
    /// </summary>
    public ModifierType ModifierType { get; set; }
    /// <summary>
    /// Pobiera lub ustawia siłę zmniejszenia statystyki.
    /// </summary>
    /// <remarks>
    /// Wartość jest zawsze dodatnia - znak ujemny jest dodawany automatycznie.
    /// </remarks>
    public double DebuffStrength { get; set; }
    /// <summary>
    /// Pobiera lub ustawia szansę na zadziałanie efektu (0.0-1.0).
    /// </summary>
    /// <remarks>
    /// W przypadku używania na przeciwniku, szansa jest modyfikowana przez jego odporności.
    /// </remarks>
    public double DebuffChance { get; set; }
    /// <summary>
    /// Pobiera lub ustawia czas trwania efektu w turach.
    /// </summary>
    public int DebuffDuration { get; set; }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DebuffStat"/>.
    /// </summary>
    /// <param name="target">Cel efektu.</param>
    /// <param name="statToDebuff">Typ statystyki do zmniejszenia.</param>
    /// <param name="modifierType">Typ modyfikatora.</param>
    /// <param name="debuffStrength">Siła zmniejszenia statystyki.</param>
    /// <param name="debuffChance">Szansa na zadziałanie efektu (0.0-1.0).</param>
    /// <param name="debuffDuration">Czas trwania efektu w turach.</param>
    public DebuffStat(SkillTarget target, StatType statToDebuff, ModifierType modifierType, double debuffStrength,
        double debuffChance, int debuffDuration)
    {
        Target = target;
        StatToDebuff = statToDebuff;
        ModifierType = modifierType;
        DebuffStrength = debuffStrength;
        DebuffChance = debuffChance;
        DebuffDuration = debuffDuration;
    }
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DebuffStat"/>.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizm deserializacji.
    /// </remarks>
    public DebuffStat() {}

    /// <summary>
    /// Wykonuje efekt zmniejszenia statystyki.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności).</param>
    /// <remarks>
    /// W przypadku używania na przeciwniku, sprawdza szansę na trafienie,
    /// uwzględniając jego odporność na efekty osłabiające.
    /// </remarks>
    public void Execute(Character caster, Character enemy, string source)
    {
        var target = Target switch
        {
            SkillTarget.Self => caster,
            SkillTarget.Enemy => enemy
        };
        switch (Target)
        {
            case SkillTarget.Self:
                if (!(Random.Shared.NextDouble() < DebuffChance)) return;
                target.AddModifier(StatToDebuff,
                    new StatModifier(ModifierType, -DebuffStrength, source, DebuffDuration));
                break;
            case SkillTarget.Enemy:
                var chance =
                    UtilityMethods.CalculateModValue(DebuffChance, caster.PassiveEffects.GetModifiers("DebuffChanceMod"));
                if (!(Random.Shared.NextDouble() <
                      UtilityMethods.EffectChance(
                          target.Resistances[StatusEffectType.Debuff].Value(enemy, "DebuffResistance"), chance))) return;
                enemy.AddModifier(StatToDebuff,
                    new StatModifier(ModifierType, -DebuffStrength, source, DebuffDuration));
                break;
        }
        var txt1 = StatToDebuff switch
        {
            StatType.MaximalHealth => $"{locale.HealthC}",
            StatType.MinimalAttack => $"{locale.MinimalAttack}",
            StatType.MaximalAttack => $"{locale.MaximalAttack}",
            StatType.Dodge => $"{locale.Dodge}",
            StatType.PhysicalDefense => $"{locale.PhysicalDefense}",
            StatType.MagicDefense => $"{locale.MagicDefense}",
            StatType.CritChance => $"{locale.CritChance}",
            StatType.Speed => $"{locale.Speed}",
            StatType.Accuracy => $"{locale.Accuracy}",
            StatType.MaximalResource => $"{locale.MaximalResource}",
        };
        var txt2 = ModifierType switch
        {
            ModifierType.Relative or ModifierType.Multiplicative => $"{DebuffStrength:P}",
            ModifierType.Absolute or ModifierType.Additive => $"{DebuffStrength:#}",
        };
    }
}