using GodmistWPF.Enums;

namespace GodmistWPF.Characters.Player
{
    /// <summary>
    /// Klasa statyczna zarządzająca główną postacią gracza i jej globalnymi właściwościami.
    /// </summary>
    /// <remarks>
    /// Zawiera referencję do aktywnej postaci gracza oraz modyfikatory zależne od poziomu honoru.
    /// </remarks>
    public static class PlayerHandler {
        /// <summary>
        /// Aktywna postać gracza.
        /// </summary>
        /// <value>Instancja <see cref="PlayerCharacter"/> reprezentująca główną postać gracza.</value>
        public static PlayerCharacter player;

        /// <summary>
        /// Pobiera modyfikator zniżki w sklepach na podstawie poziomu honoru gracza.
        /// </summary>
        /// <value>Modyfikator zniżki jako mnożnik ceny.</value>
        /// <remarks>
        /// <para>Wartości modyfikatora w zależności od poziomu honoru:</para>
        /// <list type="bullet">
        /// <item>Exile: 2.0x (wyższe ceny)</item>
        /// <item>Useless: 1.75x</item>
        /// <item>Shameful: 1.5x</item>
        /// <item>Uncertain: 1.25x</item>
        /// <item>Recruit: 1.0x (ceny standardowe)</item>
        /// <item>Mercenary: 0.95x (5% zniżki)</item>
        /// <item>Fighter: 0.9x (10% zniżki)</item>
        /// <item>Knight: 0.85x (15% zniżki)</item>
        /// <item>Leader: 0.8x (20% zniżki)</item>
        /// </list>
        /// </remarks>
        public static double HonorDiscountModifier => player.HonorLevel switch
        {
            HonorLevel.Exile => 2,
            HonorLevel.Useless => 1.75,
            HonorLevel.Shameful => 1.5,
            HonorLevel.Uncertain => 1.25,
            HonorLevel.Recruit => 1,
            HonorLevel.Mercenary => 0.95,
            HonorLevel.Fighter => 0.9,
            HonorLevel.Knight => 0.85,
            HonorLevel.Leader => 0.8,
            _ => throw new ArgumentOutOfRangeException()
        };
        /// <summary>
        /// Pobiera modyfikator doświadczenia na podstawie poziomu honoru gracza.
        /// </summary>
        /// <value>Modyfikator doświadczenia jako mnożnik.</value>
        /// <remarks>
        /// <para>Wartości modyfikatora w zależności od poziomu honoru:</para>
        /// <list type="bullet">
        /// <item>Exile do Mercenary: 1.0x (brak bonusu)</item>
        /// <item>Fighter: 1.1x (10% więcej doświadczenia)</item>
        /// <item>Knight: 1.2x (20% więcej doświadczenia)</item>
        /// <item>Leader: 1.5x (50% więcej doświadczenia)</item>
        /// </list>
        /// </remarks>
        public static double HonorExperienceModifier => player.HonorLevel switch
        {
            HonorLevel.Exile => 1,
            HonorLevel.Useless => 1,
            HonorLevel.Shameful => 1,
            HonorLevel.Uncertain => 1,
            HonorLevel.Recruit => 1,
            HonorLevel.Mercenary => 1,
            HonorLevel.Fighter => 1.1,
            HonorLevel.Knight => 1.2,
            HonorLevel.Leader => 1.5,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}