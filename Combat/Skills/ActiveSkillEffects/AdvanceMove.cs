using BattleManager = GodmistWPF.Combat.Battles.BattleManager;
using Character = GodmistWPF.Characters.Character;
using SkillTarget = GodmistWPF.Enums.SkillTarget;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

/// <summary>
/// Efekt umiejętności, który przesuwa postać na osi czasu walki.
/// </summary>
/// <remarks>
/// Umożliwia przyspieszenie lub opóźnienie następnej tury postaci w systemie walki turowym.
/// </remarks>
public class AdvanceMove : IActiveSkillEffect
{
    /// <summary>
    /// Pobiera lub ustawia cel efektu (własna postać lub przeciwnik).
    /// </summary>
    public SkillTarget Target { get; set; }
    /// <summary>
    /// Pobiera lub ustawia wartość przesunięcia na osi czasu.
    /// </summary>
    /// <remarks>
    /// Wartość określa, o jaką część obecnej pozycji na osi czasu postać zostanie przesunięta.
    /// Wartość dodatnia przyspiesza postać, ujemna opóźnia.
    /// </remarks>
    public double Amount { get; set; }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="AdvanceMove"/>.
    /// </summary>
    /// <param name="target">Cel efektu.</param>
    /// <param name="amount">Wartość przesunięcia na osi czasu.</param>
    public AdvanceMove(SkillTarget target, double amount)
    {
        Target = target;
        Amount = amount;
    }
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="AdvanceMove"/>.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizm deserializacji.
    /// </remarks>
    public AdvanceMove() {}

    /// <summary>
    /// Wykonuje efekt przesunięcia na osi czasu.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności).</param>
    /// <remarks>
    /// Przesuwa wybraną postać na osi czasu walki, wpływając na kolejność wykonywania akcji.
    /// </remarks>
    public void Execute(Character caster, Character enemy, string source)
    {
        var target = Target switch
        {
            SkillTarget.Self => BattleManager.CurrentBattle!.Users
                .FirstOrDefault(x => x.Key.User == caster).Key,
            SkillTarget.Enemy => BattleManager.CurrentBattle!.Users
                .FirstOrDefault(x => x.Key.User == enemy).Key,
            _ => null
        };
        target?.AdvanceMove((int)(target.ActionPointer / target.User.Speed * Amount));
    }
}