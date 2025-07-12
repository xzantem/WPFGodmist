using Character = GodmistWPF.Characters.Character;
using DamageType = GodmistWPF.Enums.DamageType;
using SkillTarget = GodmistWPF.Enums.SkillTarget;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

/// <summary>
/// Efekt umiejętności, który zamienia punkty zdrowia na punkty zasobów.
/// </summary>
/// <remarks>
/// Umożliwia poświęcenie procentowej ilości zdrowia w zamian za przywrócenie zasobów.
/// Efekt może być zastosowany zarówno do siebie, jak i do przeciwnika.
/// </remarks>
public class TradeHealthForResource : IActiveSkillEffect
{
    /// <summary>
    /// Pobiera lub ustawia cel efektu (własna postać lub przeciwnik).
    /// </summary>
    public SkillTarget Target { get; set; }
    /// <summary>
    /// Pobiera lub ustawia procent maksymalnego zdrowia do poświęcenia (wartość z przedziału 0.0-1.0).
    /// </summary>
    public double HealthSacrificed { get; set; }
    /// <summary>
    /// Pobiera lub ustawia współczynnik wymiany zdrowia na zasoby.
    /// </summary>
    /// <remarks>
    /// Określa, jak dużo zasobów zostanie przywróconych za każdy punkt utraconego zdrowia.
    /// Na przykład wartość 2.0 oznacza, że za każdy punkt zdrowia postać otrzyma 2 punkty zasobów.
    /// </remarks>
    public double ExchangeRatio { get; set; }
    /// <summary>
    /// Wykonuje efekt wymiany zdrowia na zasoby.
    /// </summary>
    /// <param name="caster">Postać rzucająca umiejętność.</param>
    /// <param name="enemy">Przeciwnik, względem którego określany jest cel.</param>
    /// <param name="source">Źródło efektu (nazwa umiejętności).</param>
    /// <remarks>
    /// Odejmuje procent maksymalnego zdrowia od celu i przywraca odpowiednią ilość zasobów,
    /// zgodnie z ustawionym współczynnikiem wymiany.
    /// </remarks>
    public void Execute(Character caster, Character enemy, string source)
    {
        double damage;
        switch (Target)
        {
            case SkillTarget.Self:
                damage = caster.TakeDamage(DamageType.True, HealthSacrificed * caster.MaximalHealth, caster);
                caster.RegenResource((int)(damage * ExchangeRatio));
                break;
            case SkillTarget.Enemy:
                damage = enemy.TakeDamage(DamageType.True, HealthSacrificed * enemy.MaximalHealth, caster);
                enemy.RegenResource((int)(damage * ExchangeRatio));
                break;
        }
    }

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="TradeHealthForResource"/>.
    /// </summary>
    /// <param name="target">Cel efektu.</param>
    /// <param name="healthSacrificed">Procent maksymalnego zdrowia do poświęcenia (0.0-1.0).</param>
    /// <param name="exchangeRatio">Współczynnik wymiany zdrowia na zasoby.</param>
    public TradeHealthForResource(SkillTarget target, double healthSacrificed, double exchangeRatio)
    {
        Target = target;
        HealthSacrificed = healthSacrificed;
        ExchangeRatio = exchangeRatio;
    }
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="TradeHealthForResource"/>.
    /// </summary>
    /// <remarks>
    /// Konstruktor używany przez mechanizm deserializacji.
    /// </remarks>
    public TradeHealthForResource() {}
}