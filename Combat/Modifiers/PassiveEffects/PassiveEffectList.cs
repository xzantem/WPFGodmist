using GodmistWPF.Characters;

namespace GodmistWPF.Combat.Modifiers.PassiveEffects;

/// <summary>
/// Klasa zarządzająca listą efektów pasywnych przypisanych do postaci.
/// </summary>
/// <remarks>
/// Przechowuje i zarządza różnymi typami efektów: wrodzonymi, nasłuchującymi i czasowymi.
/// Zapewnia metody do obsługi zdarzeń, aktualizacji stanu i modyfikacji efektów.
/// </remarks>
public class PassiveEffectList
{
    /// <summary>
    /// Pobiera lub ustawia listę wrodzonych efektów pasywnych.
    /// </summary>
    /// <value>
    /// Lista efektów, które są stale aktywne dla postaci.
    /// </value>
    public List<InnatePassiveEffect> InnateEffects { get; set; } = [];
    
    /// <summary>
    /// Pobiera lub ustawia listę efektów nasłuchujących zdarzenia.
    /// </summary>
    /// <value>
    /// Lista efektów, które aktywują się w odpowiedzi na określone zdarzenia.
    /// </value>
    public List<ListenerPassiveEffect> ListenerEffects { get; set; } = [];
    
    /// <summary>
    /// Pobiera lub ustawia listę efektów czasowych.
    /// </summary>
    /// <value>
    /// Lista efektów, które mają ograniczony czas działania.
    /// </value>
    public List<TimedPassiveEffect> TimedEffects { get; set; } = [];
    
    /// <summary>
    /// Dodaje nowy efekt pasywny do odpowiedniej listy.
    /// </summary>
    /// <param name="effect">Efekt do dodania.</param>
    /// <remarks>
    /// Automatycznie rozpoznaje typ efektu i dodaje go do odpowiedniej kolekcji.
    /// </remarks>
    public void Add(PassiveEffect effect)
    {
        if (effect.GetType() == typeof(InnatePassiveEffect))
            InnateEffects.Add((InnatePassiveEffect)effect);
        else if (effect.GetType() == typeof(ListenerPassiveEffect))
            ListenerEffects.Add((ListenerPassiveEffect)effect);
        else if (effect.GetType() == typeof(TimedPassiveEffect))
            TimedEffects.Add((TimedPassiveEffect)effect);
    }

    /// <summary>
    /// Usuwa efekt pasywny z odpowiedniej listy.
    /// </summary>
    /// <param name="effect">Efekt do usunięcia.</param>
    public void Remove(PassiveEffect effect)
    {
        if (effect.GetType() == typeof(InnatePassiveEffect))
            InnateEffects.Remove((InnatePassiveEffect)effect);
        else if (effect.GetType() == typeof(ListenerPassiveEffect))
            ListenerEffects.Remove((ListenerPassiveEffect)effect);
        else if (effect.GetType() == typeof(TimedPassiveEffect))
            TimedEffects.Remove((TimedPassiveEffect)effect);
    }
    

    /// <summary>
    /// Przetwarza zdarzenie walki, aktywując odpowiednie efekty nasłuchujące.
    /// </summary>
    /// <param name="eventData">Dane zdarzenia do przetworzenia.</param>
    public void HandleBattleEvent(BattleEventData eventData)
    {
        foreach (var effect in ListenerEffects)
            effect.OnTrigger(eventData);
    }

    /// <summary>
    /// Aktualizuje stan wszystkich efektów czasowych.
    /// </summary>
    /// <remarks>
    /// Powinno być wywoływane na początku każdej tury.
    /// </remarks>
    public void TickEffects()
    {
        foreach (var effect in TimedEffects.ToList())
            effect.Tick();
    }

    /// <summary>
    /// Sprawdza, czy postać może się poruszyć.
    /// </summary>
    /// <returns>
    /// <c>true</c> jeśli postać może się poruszyć; w przeciwnym razie <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Sprawdza, czy na postaci nie ma efektów uniemożliwiających ruch (ogłuszenie, zamrożenie, sen).
    /// </remarks>
    public bool CanMove()
    {
        return !TimedEffects.Any(e => e.Type is "Stun" or "Freeze" or "Sleep");
    }

    /// <summary>
    /// Wydłuża czas trwania wszystkich efektów czasowych.
    /// </summary>
    /// <param name="turns">Liczba tur, o którą zostaną wydłużone efekty.</param>
    public void ExtendEffects(int turns)
    {
        foreach (var effect in TimedEffects)
            effect.Extend(turns);
    }

    /// <summary>
    /// Pobiera modyfikatory statystyk określonego typu.
    /// </summary>
    /// <param name="ofType">Typ modyfikatora do pobrania (np. "Buff", "Debuff").</param>
    /// <returns>Lista pasujących modyfikatorów statystyk.</returns>
    public List<StatModifier> GetModifiers(string ofType)
    {
        var mods = new List<StatModifier>();
        mods.AddRange(TimedEffects.Where(x => x.Type.StartsWith(ofType))
            .Select(effect => new StatModifier(effect.Effects[1], effect.Effects[0], effect.Source, effect.Duration)));
        mods.AddRange(InnateEffects.Where(x => x.Type.StartsWith(ofType))
            .Select(effect => new StatModifier(effect.Effects[1], effect.Effects[0], effect.Source, -1)));
        return mods;
    }
    /// <summary>
    /// Pobiera modyfikatory skalowania statystyk.
    /// </summary>
    /// <param name="ofType">Typ statystyki do skalowania (np. "Strength").</param>
    /// <param name="owner">Właściciel efektów (postać).</param>
    /// <returns>Lista modyfikatorów skalowania dla określonej statystyki.</returns>
    /// <remarks>
    /// Używane do efektów, które modyfikują wartość w procentach innej statystyki.
    /// </remarks>
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