
using Character = GodmistWPF.Characters.Character;
using ModifierType = GodmistWPF.Enums.Modifiers.ModifierType;
using SkillTarget = GodmistWPF.Enums.SkillTarget;
using StatModifier = GodmistWPF.Combat.Modifiers.StatModifier;
using StatType = GodmistWPF.Enums.Modifiers.StatType;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

/// <summary>
/// Efekt umiejętności, który zwiększa wartość wybranej statystyki celu.
/// </summary>
/// <remarks>
/// Może być używany do wzmacniania własnych statystyk lub osłabiania przeciwnika.
/// Wspiera różne typy modyfikatorów (addytywny, mnożnikowy itp.).
/// </remarks>
public class BuffStat : IActiveSkillEffect
{
    /// <summary>
    /// Pobiera lub ustawia cel efektu (własna postać lub przeciwnik).
    /// </summary>
    public SkillTarget Target { get; set; }
    /// <summary>
    /// Pobiera lub ustawia typ statystyki, która ma zostać zmodyfikowana.
    /// </summary>
    public StatType StatToBuff { get; set; }
    /// <summary>
    /// Pobiera lub ustawia typ modyfikatora (np. addytywny, mnożnikowy).
    /// </summary>
    public ModifierType ModifierType { get; set; }
    /// <summary>
    /// Pobiera lub ustawia siłę modyfikatora.
    /// </summary>
    /// <remarks>
    /// Interpretacja wartości zależy od typu modyfikatora.
    /// Dla typów addytywnych - wartość bezwzględna, dla mnożnikowych - ułamek (np. 0.1 = +10%).
    /// </remarks>
    public double BuffStrength { get; set; }
    /// <summary>
    /// Pobiera lub ustawia szansę na zadziałanie efektu (wartość z przedziału 0.0 do 1.0).
    /// </summary>
    public double BuffChance { get; set; }
    /// <summary>
    /// Pobiera lub ustawia czas trwania efektu w turach.
    /// </summary>
    /// <remarks>
    /// Wartość -1 oznacza efekt stały (do końca walki).
    /// </remarks>
    public int BuffDuration { get; set; }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="BuffStat"/>.
    /// </summary>
    /// <param name="target">Cel efektu.</param>
    /// <param name="statToBuff">Typ statystyki do zmodyfikowania.</param>
    /// <param name="modifierType">Typ modyfikatora.</param>
    /// <param name="buffStrength">Siła modyfikatora.</param>
    /// <param name="buffChance">Szansa na zadziałanie (0.0-1.0).</param>
    /// <param name="buffDuration">Czas trwania w turach (-1 = do końca walki).</param>
    public BuffStat(SkillTarget target, StatType statToBuff, ModifierType modifierType, double buffStrength,
        double buffChance, int buffDuration)
    {
        Target = target;
        StatToBuff = statToBuff;
        ModifierType = modifierType;
        BuffStrength = buffStrength;
        BuffChance = buffChance;
        BuffDuration = buffDuration;
    }
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="BuffStat"/>.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizm deserializacji.
    /// </remarks>
    public BuffStat() {}

    /// <summary>
    /// Wykonuje efekt wzmocnienia statystyki.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności).</param>
    /// <remarks>
    /// Losowo decyduje o powodzeniu efektu na podstawie <see cref="BuffChance"/>.
    /// W przypadku powodzenia dodaje modyfikator do wybranej statystyki celu.
    /// </remarks>
    public void Execute(Character caster, Character enemy, string source)
    {
        var target = Target switch
        {
            SkillTarget.Self => caster,
            SkillTarget.Enemy => enemy
        };
        if (!(Random.Shared.NextDouble() < BuffChance)) return;
        target.AddModifier(StatToBuff,
            new StatModifier(ModifierType, BuffStrength, source, BuffDuration));
        var txt1 = StatToBuff switch
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
            ModifierType.Relative or ModifierType.Multiplicative => $"{BuffStrength:P}",
            ModifierType.Absolute or ModifierType.Additive => $"{BuffStrength:#}",
        };
    }
}