namespace GodmistWPF.Enums
{
    /// <summary>
    /// Określa typ obrażeń w systemie walki.
    /// </summary>
    public enum DamageType {
        /// <summary>
        /// Obrażenia fizyczne - podlegają redukcji przez pancerz.
        /// </summary>
        Physical,
        
        /// <summary>
        /// Obrażenia magiczne - podlegają redukcji przez odporność magiczną.
        /// </summary>
        Magic,
        
        /// <summary>
        /// Obrażenia prawdziwe - ignorują wszystkie redukcje.
        /// </summary>
        True,
        
        /// <summary>
        /// Krwawienie - obrażenia odnawialne, fizyczne.
        /// </summary>
        Bleed,
        
        /// <summary>
        /// Trucizna - obrażenia odnawialne, magiczne.
        /// </summary>
        Poison,
        
        /// <summary>
        /// Oparzenia - obrażenia odnawialne, ogniowe.
        /// </summary>
        Burn
    }
}