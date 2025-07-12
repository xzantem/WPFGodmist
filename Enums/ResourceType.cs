namespace GodmistWPF.Enums;

/// <summary>
/// Określa typ zasobów używanych przez umiejętności i przedmioty.
/// </summary>
public enum ResourceType
{
    /// <summary>
    /// Mana - zasób używany przez czarodziejów i magiczne umiejętności.
    /// Odnawia się powoli z czasem.
    /// </summary>
    Mana,
    
    /// <summary>
    /// Furia - zasób generowany przez obrażenia otrzymywane i zadawane.
    /// Używany przez wojowników i barbarzyńców.
    /// </summary>
    Fury,
    
    /// <summary>
    /// Impet - zasób generowany przez udane uniki i kontrataki.
    /// Używany przez złodziei i łotrzyków.
    /// </summary>
    Momentum
}