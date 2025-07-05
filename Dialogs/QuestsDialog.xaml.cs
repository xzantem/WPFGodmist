using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Quest = GodmistWPF.Quests.Quest;
using QuestManager = GodmistWPF.Quests.QuestManager;
using QuestState = GodmistWPF.Enums.QuestState;

namespace GodmistWPF.Dialogs
{
    public partial class QuestsDialog : Window
    {
        private ObservableCollection<QuestViewModel> availableQuests;
        private ObservableCollection<QuestViewModel> activeQuests;

        public QuestsDialog()
        {
            InitializeComponent();
            
            availableQuests = new ObservableCollection<QuestViewModel>();
            activeQuests = new ObservableCollection<QuestViewModel>();
            
            // Set up event handlers
            QuestsListBox.SelectionChanged += QuestsListBox_SelectionChanged;
            
            // Load quest data
            LoadQuests();
        }

        private void LoadQuests()
        {
            availableQuests.Clear();
            activeQuests.Clear();
            
            if (QuestManager.Quests != null)
            {
                foreach (var quest in QuestManager.Quests)
                {
                    var questVM = new QuestViewModel
                    {
                        Quest = quest,
                        DisplayName = $"{quest.Name} (Level {quest.RecommendedLevel})",
                        IsMainQuest = QuestManager.MainQuests?.Contains(quest) ?? false
                    };
                    
                    if (quest.QuestState == QuestState.Available)
                    {
                        availableQuests.Add(questVM);
                    }
                    else if (quest.QuestState == QuestState.Accepted)
                    {
                        activeQuests.Add(questVM);
                    }
                }
            }
            
            // Combine available and active quests for display
            QuestsListBox.ItemsSource = availableQuests;
        }

        private void QuestsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestsListBox.SelectedItem is QuestViewModel selectedQuest)
            {
                var quest = selectedQuest.Quest;
                var info = $"Name: {quest.Name}\n" +
                          $"Level: {quest.RecommendedLevel}\n" +
                          $"Status: {quest.QuestState}\n" +
                          $"Type: {(selectedQuest.IsMainQuest ? "Main Quest" : "Side Quest")}\n\n";
                
                if (quest.Stages != null && quest.Stages.Count > 0)
                {
                    info += "Objectives:\n";
                    foreach (var stage in quest.Stages)
                    {
                        foreach (var objective in stage.Objectives)
                        {
                            info += $"• {objective.Description}\n";
                        }
                    }
                }
                
                if (quest.QuestReward != null)
                {
                    info += $"\nRewards:\n" +
                           $"• Gold: {quest.QuestReward.Gold}\n" +
                           $"• Experience: {quest.QuestReward.Experience}\n" +
                           $"• Honor: {quest.QuestReward.Honor}";
                }
                
                QuestInfoText.Text = info;
            }
        }

        private void AcceptQuestButton_Click(object sender, RoutedEventArgs e)
        {
            if (QuestsListBox.SelectedItem is QuestViewModel selectedQuest)
            {
                if (selectedQuest.Quest.QuestState == QuestState.Available)
                {
                    selectedQuest.Quest.AcceptQuest();
                    LoadQuests(); // Refresh the list
                    MessageBox.Show($"You accepted {selectedQuest.Quest.Name}", "Quest Accepted", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("This quest is not available for acceptance.", "Quest Not Available", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select a quest to accept.", "No Quest Selected", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AbandonQuestButton_Click(object sender, RoutedEventArgs e)
        {
            if (QuestsListBox.SelectedItem is QuestViewModel selectedQuest)
            {
                if (selectedQuest.Quest.QuestState == QuestState.Accepted)
                {
                    var result = MessageBox.Show($"Are you sure you want to abandon {selectedQuest.Quest.Name}?", "Confirm Abandon", 
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                    
                    if (result == MessageBoxResult.Yes)
                    {
                        selectedQuest.Quest.QuestState = QuestState.Available;
                        LoadQuests(); // Refresh the list
                        MessageBox.Show($"You abandoned {selectedQuest.Quest.Name}", "Quest Abandoned", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("You can only abandon accepted quests.", "Cannot Abandon", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select a quest to abandon.", "No Quest Selected", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class QuestViewModel
    {
        public Quest Quest { get; set; }
        public string DisplayName { get; set; }
        public bool IsMainQuest { get; set; }
    }
} 