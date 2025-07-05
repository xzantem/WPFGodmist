using GodmistWPF.Characters;

namespace GodmistWPF.Combat.Modifiers.PassiveEffects;

public class PassiveEffectList
{
    public List<InnatePassiveEffect> InnateEffects { get; set; } = [];  
    public List<ListenerPassiveEffect> ListenerEffects { get; set; } = [];
    public List<TimedPassiveEffect> TimedEffects { get; set; } = [];
    
    public void Add(PassiveEffect effect)
    {
        if (effect.GetType() == typeof(InnatePassiveEffect))
            InnateEffects.Add((InnatePassiveEffect)effect);
        else if (effect.GetType() == typeof(ListenerPassiveEffect))
            ListenerEffects.Add((ListenerPassiveEffect)effect);
        else if (effect.GetType() == typeof(TimedPassiveEffect))
            TimedEffects.Add((TimedPassiveEffect)effect);
    }

    public void Remove(PassiveEffect effect)
    {
        if (effect.GetType() == typeof(InnatePassiveEffect))
            InnateEffects.Remove((InnatePassiveEffect)effect);
        else if (effect.GetType() == typeof(ListenerPassiveEffect))
            ListenerEffects.Remove((ListenerPassiveEffect)effect);
        else if (effect.GetType() == typeof(TimedPassiveEffect))
            TimedEffects.Remove((TimedPassiveEffect)effect);
    }
    

    public void HandleBattleEvent(BattleEventData eventData)
    {
        foreach (var effect in ListenerEffects)
            effect.OnTrigger(eventData);
    }

    public void TickEffects()
    {
        foreach (var effect in TimedEffects.ToList())
            effect.Tick();
    }

    public bool CanMove()
    {
        return !TimedEffects.Any(e => e.Type is "Stun" or "Freeze" or "Sleep");
    }

    public void ExtendEffects(int turns)
    {
        foreach (var effect in TimedEffects)
            effect.Extend(turns);
    }

    public List<StatModifier> GetModifiers(string ofType)
    {
        var mods = new List<StatModifier>();
        mods.AddRange(TimedEffects.Where(x => x.Type.StartsWith(ofType))
            .Select(effect => new StatModifier(effect.Effects[1], effect.Effects[0], effect.Source, effect.Duration)));
        mods.AddRange(InnateEffects.Where(x => x.Type.StartsWith(ofType))
            .Select(effect => new StatModifier(effect.Effects[1], effect.Effects[0], effect.Source, -1)));
        return mods;
    }
    public List<StatModifier> GetStatScaleModifiers(string ofType, Character owner)
    {
        var mods = new List<StatModifier>();
        mods.AddRange(TimedEffects.Where(x => x.Type.StartsWith("Scale" + ofType))
            .Select(effect => new StatModifier(effect.Effects[1], effect.Effects[0] * 
            owner.GetStat(effect.Type[(4 + ofType.Length)..]).Value(owner, effect.Type[(4 + ofType.Length)..]), effect.Source, effect.Duration)));
        mods.AddRange(TimedEffects.Where(x => x.Type.StartsWith("Scale" + ofType))
            .Select(effect => new StatModifier(effect.Effects[1], effect.Effects[0] * 
            owner.GetStat(effect.Type[(4 + ofType.Length)..]).Value(owner, effect.Type[(4 + ofType.Length)..]), effect.Source, -1)));
        return mods;
    }
}