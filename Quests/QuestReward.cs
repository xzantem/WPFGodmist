using GodmistWPF.Items;

namespace GodmistWPF.Quests;

/// <summary>
/// Reprezentuje nagrodę za ukończenie zadania.
/// Zawiera informacje o złocie, doświadczeniu, honorze i przedmiotach przyznawanych graczowi.
/// </summary>
public class QuestReward
{
    /// <summary>
    /// Ilość złota przyznawanego jako nagroda.
    /// </summary>
    public int Gold { get; set; }
    
    /// <summary>
    /// Ilość doświadczenia przyznawanego jako nagroda.
    /// </summary>
    public int Experience { get; set; }
    
    /// <summary>
    /// Ilość punktów honoru przyznawanych jako nagroda.
    /// </summary>
    public int Honor { get; set; }
    
    /// <summary>
    /// Słownik przedmiotów przyznawanych jako nagroda, gdzie klucz to przedmiot, a wartość to ilość.
    /// </summary>
    public Dictionary<IItem, int> Items { get; set; }
    
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="QuestReward"/>. Używany przez serializator JSON.
    /// </summary>
    public QuestReward() {}

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="QuestReward"/> z określonymi wartościami.
    /// </summary>
    /// <param name="gold">Ilość złota w nagrodzie.</param>
    /// <param name="experience">Ilość doświadczenia w nagrodzie.</param>
    /// <param name="honor">Ilość punktów honoru w nagrodzie.</param>
    /// <param name="items">Słownik przedmiotów w nagrodzie.</param>
    public QuestReward(int gold, int experience, int honor, Dictionary<IItem, int> items)
    {
        Gold = gold;
        Experience = experience;
        Honor = honor;
        Items = items;
    }
}