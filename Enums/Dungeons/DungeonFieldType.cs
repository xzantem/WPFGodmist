namespace GodmistWPF.Enums.Dungeons
{
    /// <summary>
    /// Określa typ pola w lochu, z którym może wejść w interakcję gracz.
    /// </summary>
    public enum DungeonFieldType {
        /// <summary>
        /// Puste pole - brak interakcji.
        /// </summary>
        Empty,
        
        /// <summary>
        /// Pole walki - rozpoczyna losową walkę z przeciwnikami.
        /// </summary>
        Battle,
        
        /// <summary>
        /// Skrzynia - zawiera losowe przedmioty lub złoto.
        /// </summary>
        Stash,
        
        /// <summary>
        /// Pułapka - zadaje obrażenia lub nakłada negatywne efekty.
        /// </summary>
        Trap,
        
        /// <summary>
        /// Ognisko - pozwala na odpoczynek i regenerację zdrowia.
        /// </summary>
        Bonfire,
        
        /// <summary>
        /// Roślina - może zostać zebrana, aby uzyskać surowce alchemiczne.
        /// </summary>
        Plant
    }
}