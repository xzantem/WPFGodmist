using Newtonsoft.Json;
using ActiveSkillEffectConverter = GodmistWPF.Utilities.JsonConverters.ActiveSkillEffectConverter;
using Character = GodmistWPF.Characters.Character;
using SkillTarget = GodmistWPF.Enums.SkillTarget;

namespace GodmistWPF.Combat.Skills;

/// <summary>
/// Interfejs definiujący efekt umiejętności aktywnej w systemie walki.
/// </summary>
/// <remarks>
/// Każdy efekt umiejętności musi implementować tę klasę, aby mógł być używany w systemie walki.
/// Efekty są serializowane przy użyciu konwertera ActiveSkillEffectConverter.
/// </remarks>
[JsonConverter(typeof(ActiveSkillEffectConverter))]
public interface IActiveSkillEffect
{
    /// <summary>
    /// Pobiera lub ustawia cel, na który działa efekt umiejętności.
    /// </summary>
    /// <value>
    /// Wartość wyliczeniowa określająca, czy efekt działa na gracza, przeciwnika, czy inny cel.
    /// </value>
    public SkillTarget Target { get; set; }

    /// <summary>
    /// Wykonuje efekt umiejętności na określonych postaciach.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, na którym wykonywana jest umiejętność.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności lub przedmiotu).</param>
    public void Execute(Character caster, Character enemy, string source);
}