using GodmistWPF.Characters;

namespace GodmistWPF.Combat.Modifiers.PassiveEffects;

public class ListenerPassiveEffect(
    Func<BattleEventData, bool> triggerCondition,
    Action<BattleEventData> onTrigger,
    Character owner,
    string source)
    : PassiveEffect(owner, source)
{
    public void OnTrigger(BattleEventData eventData)
    {
        if (triggerCondition(eventData))
        {
            onTrigger(eventData);
        }
    }
}