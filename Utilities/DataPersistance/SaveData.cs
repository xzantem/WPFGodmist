

using GodmistWPF.Characters.Player;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Dungeons;
using GodmistWPF.Quests;
using GodmistWPF.Towns;

namespace GodmistWPF.Utilities.DataPersistance;

/// <summary>
/// Klasa reprezentująca dane gry, które mogą być zapisane lub wczytane.
/// Zawiera wszystkie niezbędne informacje do przywrócenia stanu gry.
/// </summary>
public class SaveData
{
    /// <summary>
    /// Pobiera lub ustawia postać gracza.
    /// </summary>
    public PlayerCharacter Player { get; set; }
    /// <summary>
    /// Pobiera lub ustawia poziom trudności gry.
    /// </summary>
    public Difficulty Difficulty { get; set; }
    /// <summary>
    /// Pobiera lub ustawia tablicę list zadań.
    /// Indeks 0: Zadania główne
    /// Indeks 1: Losowe zadania poboczne
    /// Indeks 2: Zadania bossów
    /// </summary>
    public List<Quest>[] Quests { get; set; }
    /// <summary>
    /// Pobiera lub ustawia słownik śledzący postęp w zadaniach bossów.
    /// Klucz to typ lochu, a wartość to aktualny postęp.
    /// </summary>
    public Dictionary<DungeonType,int> BossQuestProgress { get; set; }
    /// <summary>
    /// Pobiera lub ustawia stan miasta (Arungard).
    /// </summary>
    public Town Town { get; set; }
    
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy SaveData z domyślnymi wartościami.
    /// </summary>
    public SaveData() {}

    /// <summary>
    /// Inicjalizuje nową instancję klasy SaveData z określonymi wartościami.
    /// </summary>
    /// <param name="player">Postać gracza do zapisania.</param>
    /// <param name="difficulty">Poziom trudności gry.</param>
    /// <param name="quests">Tablica list zadań do zapisania.</param>
    /// <param name="bossQuestProgress">Słownik postępu zadań bossów.</param>
    /// <param name="town">Stan miasta do zapisania.</param>
    public SaveData(PlayerCharacter player, Difficulty difficulty, List<Quest>[] quests, 
        Dictionary<DungeonType,int> bossQuestProgress, Town town)
    {
        Player = player;
        Difficulty = difficulty;
        Quests = quests;
        BossQuestProgress = bossQuestProgress;
        Town = town;
    }
}