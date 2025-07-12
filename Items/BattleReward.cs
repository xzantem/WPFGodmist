namespace GodmistWPF.Items;

/// <summary>
/// Klasa reprezentująca nagrody otrzymywane po zakończeniu walki.
/// Zawiera informacje o złocie, honorze, doświadczeniu i przedmiotach.
/// </summary>
/// <param name="goldReward">Ilość złota do otrzymania.</param>
/// <param name="honorReward">Ilość punktów honoru do otrzymania.</param>
/// <param name="experienceReward">Ilość doświadczenia do otrzymania.</param>
/// <param name="itemsReward">Słownik przedmiotów i ich ilości do otrzymania.</param>
public class BattleReward(
    int goldReward,
    int honorReward,
    int experienceReward,
    Dictionary<IItem, int> itemsReward)
{
    /// <summary>
    /// Pobiera ilość złota przyznaną jako nagrodę.
    /// </summary>
    public int GoldReward { get; private set; } = goldReward;
    
    /// <summary>
    /// Pobiera ilość punktów honoru przyznanych jako nagrodę.
    /// </summary>
    public int HonorReward { get; private set; } = honorReward;
    
    /// <summary>
    /// Pobiera ilość doświadczenia przyznaną jako nagrodę.
    /// </summary>
    public int ExperienceReward { get; private set; } = experienceReward;
    
    /// <summary>
    /// Pobiera słownik przedmiotów i ich ilości przyznanych jako nagroda.
    /// Kluczem jest przedmiot, a wartością ilość danego przedmiotu.
    /// </summary>
    public Dictionary<IItem, int> ItemsReward { get; private set; } = itemsReward;
}