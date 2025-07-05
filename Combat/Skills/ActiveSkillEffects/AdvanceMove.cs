using ConsoleGodmist;
using BattleManager = GodmistWPF.Combat.Battles.BattleManager;
using Character = GodmistWPF.Characters.Character;
using SkillTarget = GodmistWPF.Enums.SkillTarget;

namespace GodmistWPF.Combat.Skills.ActiveSkillEffects;

public class AdvanceMove : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public double Amount { get; set; }

    public AdvanceMove(SkillTarget target, double amount)
    {
        Target = target;
        Amount = amount;
    }
    public AdvanceMove() {}

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
        BattleManager.CurrentBattle?.Interface.AddBattleLogLines(
            $"{target} {locale.AdvancesMove} {locale.By} {Amount:P}");
    }
}