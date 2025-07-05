using GodmistWPF.Characters;

namespace GodmistWPF.Combat.Modifiers.PassiveEffects;

public class TimedPassiveEffect(Character owner, string source, string type, int duration, 
    dynamic[] effects, Action? onTick = null) : 
    InnatePassiveEffect(owner, source, type, effects)
{
    public int Duration { get; private set; } = duration;

    public void Tick()
    {
        onTick?.Invoke();
        Duration--;
        if (Duration <= 0) Owner.PassiveEffects.Remove(this);
    }

    public void Extend(int turns)
    {
        Duration += turns;
    }
    
    public void UpdateOnTick(Action newOnTick) {
        onTick = newOnTick;
    }
}