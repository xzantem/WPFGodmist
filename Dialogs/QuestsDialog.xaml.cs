using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GodmistWPF.Quests;
using Quest = GodmistWPF.Quests.Quest;
using QuestManager = GodmistWPF.Quests.QuestManager;
using QuestState = GodmistWPF.Enums.QuestState;

namespace GodmistWPF.Dialogs
{
    /// <summary>
    /// Okno dialogowe wyświetlające listę zadań (questów) dostępnych dla gracza.
    /// Umożliwia przeglądanie zadań w toku, ukończonych i oddanych.
    /// </summary>
    public partial class QuestsDialog : Window
    {
        /// <summary>Kolekcja zadań aktualnie w trakcie wykonywania.</summary>
        private ObservableCollection<QuestViewModel> inProgressQuests;
        
        /// <summary>Kolekcja zadań ukończonych, ale jeszcze nieoddanych.</summary>
        private ObservableCollection<QuestViewModel> completedQuests;
        
        /// <summary>Kolekcja zadań już oddanych i zakończonych.</summary>
        private ObservableCollection<QuestViewModel> handedInQuests;

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="QuestsDialog">.
        /// Konfiguruje interfejs użytkownika i ładuje dostępne zadania.
        /// </summary>
        public QuestsDialog()
        {
            InitializeComponent();
            
            inProgressQuests = new ObservableCollection<QuestViewModel>();
            completedQuests = new ObservableCollection<QuestViewModel>();
            handedInQuests = new ObservableCollection<QuestViewModel>();
            
            // Initially hide the details panel
            QuestDetailsPanel.Visibility = Visibility.Collapsed;
            NoQuestSelectedText.Visibility = Visibility.Visible;
            
            LoadQuests();
            
            // Select first tab with quests if available
            if (inProgressQuests.Count > 0)
                QuestTabs.SelectedIndex = 0;
            else if (completedQuests.Count > 0)
                QuestTabs.SelectedIndex = 1;
            else if (handedInQuests.Count > 0)
                QuestTabs.SelectedIndex = 2;
        }

        /// <summary>
        /// Ładuje zadania z menedżera zadań i grupuje je według ich stanu.
        /// Inicjalizuje źródła danych dla list zadań i ustawia domyślny wybór.
        /// </summary>
        private void LoadQuests()
        {
            inProgressQuests.Clear();
            completedQuests.Clear();
            handedInQuests.Clear();
            
            if (QuestManager.Quests == null) return;

            // Przetwarzanie wszystkich dostępnych zadań
            foreach (var quest in QuestManager.Quests)
            {
                var questVM = new QuestViewModel
                {
                    Quest = quest,
                    DisplayName = quest.Name,
                    IsMainQuest = QuestManager.MainQuests?.Contains(quest) ?? false
                };
                
                // Grupowanie zadań według ich stanu
                switch (quest.QuestState)
                {
                    case QuestState.Accepted:
                        inProgressQuests.Add(questVM);
                        break;
                    case QuestState.Completed:
                        completedQuests.Add(questVM);
                        break;
                    case QuestState.HandedIn:
                        handedInQuests.Add(questVM);
                        break;
                }
            }
            
            // Przypisanie źródeł danych do kontrolek ListBox
            InProgressQuestsList.ItemsSource = inProgressQuests;
            CompletedQuestsList.ItemsSource = completedQuests;
            HandedInQuestsList.ItemsSource = handedInQuests;
            
            // Automatyczny wybór pierwszego zadania w aktywnej zakładce
            if (QuestTabs.SelectedIndex == 0 && inProgressQuests.Count > 0)
                InProgressQuestsList.SelectedIndex = 0;
            else if (QuestTabs.SelectedIndex == 1 && completedQuests.Count > 0)
                CompletedQuestsList.SelectedIndex = 0;
            else if (QuestTabs.SelectedIndex == 2 && handedInQuests.Count > 0)
                HandedInQuestsList.SelectedIndex = 0;
        }

        /// <summary>
        /// Obsługuje zmianę wyboru zadania na liście zadań.
        /// Aktualizuje panel szczegółów wybranego zadania.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (ListBox).</param>
        /// <param name="e">Dane zdarzenia zmiany wyboru.</param>
        private void QuestList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (ListBox)sender;
            if (listBox.SelectedItem is QuestViewModel selectedQuest)
            {
                UpdateQuestDetails(selectedQuest.Quest);
                // Show details panel and hide no selection message
                QuestDetailsPanel.Visibility = Visibility.Visible;
                NoQuestSelectedText.Visibility = Visibility.Collapsed;
            }
            else
            {
                // No quest selected, show message and hide details
                QuestDetailsPanel.Visibility = Visibility.Collapsed;
                NoQuestSelectedText.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Aktualizuje panel szczegółów wybranego zadania.
        /// Wyświetla informacje o zadaniu, cele, nagrody i inne szczegóły.
        /// </summary>
        /// <param name="quest">Zadanie, którego szczegóły mają zostać wyświetlone.</param>
        private void UpdateQuestDetails(Quest quest)
        {
            // Podstawowe informacje o zadaniu
            QuestTitle.Text = quest.Name;
            QuestLevel.Text = $"(Level {quest.RecommendedLevel})";
            QuestStatus.Text = GetQuestStatusText(quest.QuestState);
            QuestType.Text = QuestManager.MainQuests?.Contains(quest) == true ? "Main Quest" : "Side Quest";
            
            // Opis zadania
            QuestDescription.Text = quest.Description == "" ? quest.Description : "No description available.";
            
            // Cele zadania
            ObjectivesList.ItemsSource = null;
            if (quest is { Stages: { Count: > 0 }, QuestState: QuestState.Accepted })
            {
                var currentStage = quest.GetCurrentStage();
                ObjectivesList.ItemsSource = currentStage.Objectives;
                ObjectivesHeader.Visibility = Visibility.Visible;
            }
            else
            {
                ObjectivesHeader.Visibility = Visibility.Collapsed;
            }
            
            // Nagrody za zadanie
            if (quest.QuestReward != null)
            {
                RewardsHeader.Visibility = Visibility.Visible;
                RewardsPanel.Visibility = Visibility.Visible;
                
                GoldReward.Text = $"• Gold: {quest.QuestReward.Gold}";
                ExpReward.Text = $"• Experience: {quest.QuestReward.Experience}";
                
                if (quest.QuestReward.Items is { Count: > 0 })
                {
                    ItemReward.Text = $"• Items: {string.Join(", ", quest.QuestReward.Items.Select(i => i.Key.Name))}";
                    ItemReward.Visibility = Visibility.Visible;
                }
                else
                {
                    ItemReward.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                RewardsHeader.Visibility = Visibility.Collapsed;
                RewardsPanel.Visibility = Visibility.Collapsed;
            }
            
            // Informacja o zleceniodawcy
            QuestGiver.Text = $"From: {quest.QuestGiver}";
            
            // Informacja o oddawaniu zadań dla ukończonych zadań
            if (quest.QuestState == QuestState.Completed && !string.IsNullOrEmpty(quest.QuestEnder))
            {
                QuestEnderText.Text = $"Report back to {quest.QuestEnder} to collect your rewards!";
                QuestEnderText.Visibility = Visibility.Visible;
            }
            else
            {
                QuestEnderText.Visibility = Visibility.Collapsed;
            }
        }
        
        /// <summary>
        /// Zwraca tekstową reprezentację stanu zadania.
        /// </summary>
        /// <param name="state">Stan zadania do przetłumaczenia na tekst.</param>
        /// <returns>Przyjazna dla użytkownika nazwa stanu zadania.</returns>
        private string GetQuestStatusText(QuestState state)
        {
            return state switch
            {
                QuestState.Available => "Available",
                QuestState.Accepted => "In Progress",
                QuestState.Completed => "Completed",
                QuestState.HandedIn => "Handed In",
                _ => "Unknown"
            };
        }
    }

    /// <summary>
    /// Klasa reprezentująca widok modelu zadania (questa) do wyświetlania w interfejsie użytkownika.
    /// </summary>
    public class QuestViewModel
    {
        /// <summary>Pobiera lub ustawia obiekt zadania.</summary>
        public Quest Quest { get; set; }
        
        /// <summary>Pobiera lub ustawia wyświetlaną nazwę zadania.</summary>
        public string DisplayName { get; set; }
        
        /// <summary>Pobiera lub ustawia wartość wskazującą, czy jest to zadanie główne.</summary>
        public bool IsMainQuest { get; set; }
    }
}