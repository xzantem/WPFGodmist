using GodmistWPF.Enums.Modifiers;

namespace GodmistWPF.Combat.Modifiers;

public class StatModifier(ModifierType modifierType, double value, string source, int duration = -1)
    // optional parameter, duration -1 represents infinite modifier
{
    public ModifierType Type { get; private set; } = modifierType;
    public double Mod { get; private set; } = value;
    public int Duration { get; private set; } = duration;
    public string Source { get; private set; } = source;

    public void Tick()
    {
        Duration--;
    }
}