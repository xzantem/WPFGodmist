namespace GodmistWPF.Enums.Items;

/// <summary>
/// Określa rzadkość przedmiotu, która wpływa na jego moc i wartość.
/// </summary>
public enum ItemRarity
{
    /// <summary>
    /// Śmieć - bezużyteczne przedmioty o nikłej wartości.
    /// </summary>
    Junk,
    
    /// <summary>
    /// Zniszczony - przedmioty w bardzo złym stanie, prawie bezużyteczne.
    /// </summary>
    Destroyed,
    
    /// <summary>
    /// Uszkodzony - przedmioty w złym stanie, z obniżoną skutecznością.
    /// </summary>
    Damaged,
    
    /// <summary>
    /// Zwykły - podstawowe przedmioty bez specjalnych właściwości.
    /// </summary>
    Common,
    
    /// <summary>
    /// Niezwykły - przedmioty z jedną dodatkową właściwością.
    /// </summary>
    Uncommon,
    
    /// <summary>
    /// Rzadki - potężne przedmioty z kilkoma dodatkowymi właściwościami.
    /// </summary>
    Rare,
    
    /// <summary>
    /// Starożytny - bardzo rzadkie przedmioty z unikalnymi właściwościami.
    /// </summary>
    Ancient,
    
    /// <summary>
    /// Legendarny - potężne przedmioty o mitycznej mocy.
    /// </summary>
    Legendary,
    
    /// <summary>
    /// Mityczny - przedmioty o boskiej mocy, niezwykle rzadkie.
    /// </summary>
    Mythical,
    
    /// <summary>
    /// Boski - najrzadsze i najpotężniejsze przedmioty w grze.
    /// </summary>
    Godly
}