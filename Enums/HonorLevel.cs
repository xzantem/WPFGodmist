namespace GodmistWPF.Enums
{
    /// <summary>
    /// Określa poziom honoru postaci, wpływający na dostępne opcje dialogowe i reakcje NPC.
    /// </summary>
    public enum HonorLevel {
        /// <summary>
        /// Wygnaniec - całkowity brak zaufania, brak dostępu do większości usług.
        /// </summary>
        Exile,
        
        /// <summary>
        /// Bezużyteczny - bardzo niski poziom zaufania, ograniczony dostęp do usług.
        /// </summary>
        Useless,
        
        /// <summary>
        /// Haniebny - niski poziom zaufania, podstawowe usługi dostępne po podwyższonej cenie.
        /// </summary>
        Shameful,
        
        /// <summary>
        /// Niepewny - neutralny poziom zaufania, dostęp do podstawowych usług.
        /// </summary>
        Uncertain,
        
        /// <summary>
        /// Rekrut - podstawowy poziom zaufania, dostęp do większości usług.
        /// </summary>
        Recruit,
        
        /// <summary>
        /// Najemnik - wysoki poziom zaufania, dostęp do zaawansowanych usług.
        /// </summary>
        Mercenary,
        
        /// <summary>
        /// Wojownik - bardzo wysoki poziom zaufania, dostęp do elitarnych usług.
        /// </summary>
        Fighter,
        
        /// <summary>
        /// Rycerz - niemal pełne zaufanie, dostęp do wszystkich usług z rabatem.
        /// </summary>
        Knight,
        
        /// <summary>
        /// Przywódca - maksymalny poziom zaufania, pełen dostęp do wszystkich usług.
        /// </summary>
        Leader
    }
}