namespace GodmistWPF.Enums
{
    /// <summary>
    /// Określa poziom trudności gry.
    /// </summary>
    public enum Difficulty
    {
        /// <summary>
        /// Łatwy poziom trudności - przeciwnicy są słabsi, a gracz otrzymuje więcej punktów zdrowia.
        /// </summary>
        Easy,
        
        /// <summary>
        /// Normalny poziom trudności - standardowe wyzwanie dla większości graczy.
        /// </summary>
        Normal,
        
        /// <summary>
        /// Trudny poziom trudności - przeciwnicy są silniejsi i bardziej agresywni.
        /// </summary>
        Hard,
        
        /// <summary>
        /// Poziom koszmaru - ekstremalne wyzwanie dla najbardziej doświadczonych graczy.
        /// Przeciwnicy są znacznie silniejsi, a gracz otrzymuje mniej punktów zdrowia.
        /// </summary>
        Nightmare
    }
}