using Newtonsoft.Json;
using ActiveSkillEffectConverter = GodmistWPF.Utilities.JsonConverters.ActiveSkillEffectConverter;
using Character = GodmistWPF.Characters.Character;
using SkillTarget = GodmistWPF.Enums.SkillTarget;

namespace GodmistWPF.Combat.Skills;

[JsonConverter(typeof(ActiveSkillEffectConverter))]
public interface IActiveSkillEffect
{
    public SkillTarget Target { get; set; }

    public void Execute(Character caster, Character enemy, string source);
}