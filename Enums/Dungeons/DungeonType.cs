namespace GodmistWPF.Enums.Dungeons
{
    /// <summary>
    /// Określa typ lochu, który może zostać odwiedzony przez gracza.
    /// Każdy typ ma unikalny motyw, przeciwników i nagrody.
    /// </summary>
    public enum DungeonType
    {
        /// <summary>
        /// Katakumby - podziemne korytarze pełne nieumarłych i pułapek.
        /// </summary>
        Catacombs,
        
        /// <summary>
        /// Las - gęste zarośla zamieszkane przez bestie i leśne duchy.
        /// </summary>
        Forest,
        
        /// <summary>
        /// Elficze ruiny - starożytne budowle pełne magicznych artefaktów i strażników.
        /// </summary>
        ElvishRuins,
        
        /// <summary>
        /// Zatoka - nadmorskie jaskinie zamieszkałe przez piratów i morskie potwory.
        /// </summary>
        Cove,
        
        /// <summary>
        /// Pustynia - spalone słońcem ruiny i piaskowe grobowce.
        /// </summary>
        Desert,
        
        /// <summary>
        /// Świątynia - starożytne miejsce kultu, pełne pułapek i strażników.
        /// </summary>
        Temple,
        
        /// <summary>
        /// Góry - strome szczyty zamieszkane przez orki i smoki.
        /// </summary>
        Mountains,
        
        /// <summary>
        /// Bagno - mroczne mokradła pełne trujących stworzeń i złowrogiej magii.
        /// </summary>
        Swamp
    }
}
