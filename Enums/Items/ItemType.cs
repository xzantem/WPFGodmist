namespace GodmistWPF.Enums.Items;

/// <summary>
/// Określa typ przedmiotu w grze, który wpływa na jego przeznaczenie i użycie.
/// </summary>
public enum ItemType
{
    /// <summary>
    /// Broń - przedmioty używane do atakowania przeciwników.
    /// </summary>
    Weapon,
    
    /// <summary>
    /// Zbroja - przedmioty zapewniające ochronę przed obrażeniami.
    /// </summary>
    Armor,
    
    /// <summary>
    /// Kowalstwo - surowce i komponenty do tworzenia i ulepszania ekwipunku.
    /// </summary>
    Smithing,
    
    /// <summary>
    /// Alchemia - składniki do tworzenia mikstur i eliksirów.
    /// </summary>
    Alchemy,
    
    /// <summary>
    /// Runy - magiczne przedmioty do wzmacniania broni i zbroi.
    /// </summary>
    Runeforging,
    
    /// <summary>
    /// Mikstura - przedmioty konsumpcyjne o różnych efektach.
    /// </summary>
    Potion,
    
    /// <summary>
    /// Galduryt do broni - specjalny materiał do ulepszania broni.
    /// </summary>
    WeaponGaldurite,
    
    /// <summary>
    /// Galduryt do zbroi - specjalny materiał do ulepszania zbroi.
    /// </summary>
    ArmorGaldurite,
    
    /// <summary>
    /// Worek łupów - zawiera losowe przedmioty do zebrania.
    /// </summary>
    LootBag
}