using GodmistWPF.Characters.Player;
using GodmistWPF.Enums;
using GodmistWPF.Items;
using GodmistWPF.Quests.Objectives;
using GodmistWPF.Towns;
using GodmistWPF.Utilities;
using Newtonsoft.Json;

namespace GodmistWPF.Quests;

/// <summary>
/// Reprezentuje zadanie w grze, które gracz może wykonać.
/// Zarządza stanem zadania, jego etapami, nagrodami i interakcjami z NPC.
/// </summary>
public class Quest
{
    /// <summary>
    /// Pobiera nazwę zadania na podstawie aliasu.
    /// </summary>
    [JsonIgnore]
    public string Name => NameAliasHelper.GetName(Alias);
    
    /// <summary>
    /// Pobiera opis zadania. Dla zadań pobocznych zwraca opis pierwszego celu,
    /// a dla głównych pobiera opis na podstawie aliasu.
    /// </summary>
    [JsonIgnore]
    public string Description =>
        IsSideQuest ? Stages[0].Objectives[0].Description : 
            NameAliasHelper.GetName(Alias + "Description");

    /// <summary>
    /// Unikalny identyfikator zadania.
    /// </summary>
    public string Alias { get; set; }
    
    /// <summary>
    /// Określa, czy zadanie jest zadaniem pobocznym.
    /// </summary>
    public bool IsSideQuest { get; set; }
    
    /// <summary>
    /// Zalecany poziom postaci do podjęcia zadania.
    /// </summary>
    public int RecommendedLevel { get; set; }
    
    /// <summary>
    /// Lista etapów zadania.
    /// </summary>
    public List<QuestStage> Stages { get; set; }
    
    /// <summary>
    /// Nagroda za ukończenie zadania.
    /// </summary>
    public QuestReward QuestReward { get; set; }
    
    /// <summary>
    /// Aktualny stan zadania.
    /// </summary>
    public QuestState QuestState { get; set; }
    
    /// <summary>
    /// Lista wymagań, które muszą być spełnione, aby podjąć zadanie.
    /// </summary>
    public List<string> Prerequisites { get; set; }
    
    /// <summary>
    /// Identyfikator NPC, który wydaje zadanie.
    /// </summary>
    public string QuestGiver { get; set; }
    
    /// <summary>
    /// Identyfikator NPC, u którego można zakończyć zadanie.
    /// </summary>
    public string QuestEnder { get; set; }
    
    /// <summary>
    /// Lista linii dialogowych wyświetlanych przy podejmowaniu zadania.
    /// </summary>
    public List<string> AcceptDialogue { get; set; }
    
    /// <summary>
    /// Lista linii dialogowych wyświetlanych przy oddawaniu zadania.
    /// </summary>
    public List<string> HandInDialogue { get; set; }
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="Quest"/>. Używany przez serializator JSON.
    /// </summary>
    public Quest() {}

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="Quest"/> z określonymi parametrami.
    /// </summary>
    /// <param name="alias">Unikalny identyfikator zadania.</param>
    /// <param name="level">Zalecany poziom postaci do podjęcia zadania.</param>
    /// <param name="stages">Lista etapów zadania.</param>
    /// <param name="questReward">Nagroda za ukończenie zadania.</param>
    /// <param name="questGiver">Identyfikator NPC wydającego zadanie.</param>
    /// <param name="acceptDialogue">Dialog wyświetlany przy podejmowaniu zadania.</param>
    /// <param name="handInDialogue">Dialog wyświetlany przy oddawaniu zadania.</param>
    /// <param name="isSideQuest">Określa, czy zadanie jest zadaniem pobocznym.</param>
    public Quest(string alias, int level, List<QuestStage> stages, QuestReward questReward, string questGiver,
        string acceptDialogue, string handInDialogue, bool isSideQuest = false)
    {
        Alias = alias;
        RecommendedLevel = level;
        Stages = stages;
        QuestReward = questReward;
        QuestState = QuestState.Available;
        Prerequisites = [];
        QuestGiver = questGiver;
        QuestEnder = questGiver;
        AcceptDialogue = [acceptDialogue];
        HandInDialogue = [handInDialogue];
        IsSideQuest = isSideQuest;
    }

    /// <summary>
    /// Próbuje wykonać postęp w zadaniu na podstawie dostarczonego kontekstu.
    /// </summary>
    /// <param name="context">Kontekst zawierający informacje o wykonanej akcji.</param>
    public void TryProgress(QuestObjectiveContext context)
    {
        if (context.ContextLevel < RecommendedLevel || QuestState != QuestState.Accepted) return;
        foreach (var objective in GetCurrentStage().Objectives.Where(objective => !objective.IsComplete))
        {
            objective.Progress(context);
            if (!objective.IsComplete || GetCurrentStage() == default) continue;
            // WPF handles quest progress UI
        }
        if (GetCurrentStage() == default)
            TryCompleteQuest();
    }

    /// <summary>
    /// Pobiera aktualny etap zadania (pierwszy, który ma nieukończone cele).
    /// </summary>
    /// <returns>Bieżący etap zadania lub null, jeśli wszystkie etapy są ukończone.</returns>
    public QuestStage GetCurrentStage()
    {
        return Stages.FirstOrDefault(stage => stage.Objectives.Any(objective => !objective.IsComplete))!;
    }

    /// <summary>
    /// Oznacza zadanie jako zaakceptowane przez gracza.
    /// </summary>
    public void AcceptQuest()
    {
        QuestState = QuestState.Accepted;
    }

    /// <summary>
    /// Próbuje oznaczyć zadanie jako ukończone, jeśli wszystkie cele zostały wykonane.
    /// Wywołuje zdarzenie ukończenia zadania, jeśli stan się zmienił.
    /// </summary>
    public void TryCompleteQuest()
    {
        if (QuestState != QuestState.Accepted || Stages.Count <= 0 || 
            !Stages.All(s => s.Objectives.All(o => o.IsComplete))) return;
                
        var previousState = QuestState;
        QuestState = QuestState.Completed;
            
        // Only raise the event if the state actually changed to completed
        if (previousState != QuestState.Completed)
        {
            QuestManager.OnQuestCompleted(this);
        }
    }

    /// <summary>
    /// Realizuje nagrodę za zadanie i oznacza je jako oddane.
    /// W przypadku zadań pobocznych aktualizuje postęp w zabijaniu bossów.
    /// </summary>
    /// <returns>Nagroda za ukończenie zadania.</returns>
    public QuestReward HandInQuest()
    {
        GetRewards();
        QuestState = QuestState.HandedIn;
        if (QuestManager.RandomizedSideQuests.Contains(this))
        {
            foreach (var objective in Stages.SelectMany(stage => stage.Objectives))
            {
                switch (objective)
                {
                    case KillInDungeonQuestObjective killObjective:
                        QuestManager.BossProgress[killObjective.Target]++;
                        break;
                    case DescendQuestObjective descendObjective:
                        QuestManager.BossProgress[descendObjective.Target]++;
                        break;
                }
            }
            QuestManager.UpdateBossQuests();
        }

        return QuestReward;
    }

    /// <summary>
    /// Przyznaje graczowi nagrody za ukończenie zadania.
    /// Dodaje złoto, doświadczenie, honor i przedmioty do ekwipunku gracza.
    /// </summary>
    private void GetRewards()
    {
        var player = PlayerHandler.player;
        if (QuestReward.Gold != 0) player.GainGold(QuestReward.Gold);
        if (QuestReward.Experience != 0) player.GainExperience(QuestReward.Experience);
        if (QuestReward.Honor !=  0) player.GainHonor(QuestReward.Honor);
        if (QuestReward.Items.Count == 0) return;
        foreach (var item in QuestReward.Items)
            player.Inventory.AddItem(item.Key, item.Value);
    }
}