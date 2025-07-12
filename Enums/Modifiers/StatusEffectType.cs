namespace GodmistWPF.Enums.Modifiers;

/// <summary>
/// Określa typ efektu statusowego, który może być nałożony na postać.
/// </summary>
public enum StatusEffectType
{
    /// <summary>
    /// Pozytywny efekt zwiększający statystyki postaci.
    /// </summary>
    Buff,
    
    /// <summary>
    /// Negatywny efekt zmniejszający statystyki postaci.
    /// </summary>
    Debuff,
    
    /// <summary>
    /// Krwawienie - zadaje obrażenia przez określony czas.
    /// </summary>
    Bleed,
    
    /// <summary>
    /// Trucizna - zadaje obrażenia przez określony czas, silniejsze niż krwawienie.
    /// </summary>
    Poison,
    
    /// <summary>
    /// Podpalenie - zadaje obrażenia przez określony czas, może się rozprzestrzeniać.
    /// </summary>
    Burn,
    
    /// <summary>
    /// Ogłuszenie - uniemożliwia wykonywanie akcji przez określony czas.
    /// </summary>
    Stun,
    
    /// <summary>
    /// Zamrożenie - uniemożliwia wykonywanie akcji i spowalnia po zakończeniu efektu.
    /// </summary>
    Freeze,
    
    /// <summary>
    /// Odmrożenie - znacząco zmniejsza szybkość postaci.
    /// </summary>
    Frostbite,
    
    /// <summary>
    /// Sen - unieruchamia postać, ale regeneruje jej zdrowie.
    /// </summary>
    Sleep,
    
    /// <summary>
    /// Niewidzialność - postać nie może być celem umiejętności.
    /// </summary>
    Invisible,
    
    /// <summary>
    /// Paraliż - całkowicie unieruchamia postać i czyni ją nietykalną.
    /// </summary>
    Paralysis,
    
    /// <summary>
    /// Prowokacja - zmusza postać do atakowania prowokującego.
    /// </summary>
    Provocation,
    
    /// <summary>
    /// Tarcza - pochłania określoną ilość obrażeń.
    /// </summary>
    Shield,
    
    /// <summary>
    /// Regeneracja - przywraca punkty zdrowia lub zasobów co turę.
    /// </summary>
    Regeneration
}