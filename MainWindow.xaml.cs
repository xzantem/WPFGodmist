using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using GodmistWPF.Combat.Battles;
using GodmistWPF.Dialogs;
using GodmistWPF.Towns;
using CharacterClass = GodmistWPF.Enums.CharacterClass;
using DataPersistanceManager = GodmistWPF.Utilities.DataPersistance.DataPersistanceManager;
using DungeonFieldType = GodmistWPF.Enums.Dungeons.DungeonFieldType;
using DungeonRoom = GodmistWPF.Dungeons.DungeonRoom;
using DungeonType = GodmistWPF.Enums.Dungeons.DungeonType;
using EnemyFactory = GodmistWPF.Characters.EnemyFactory;
using GameSettings = GodmistWPF.Utilities.GameSettings;
using Paladin = GodmistWPF.Characters.Player.Paladin;
using PlayerHandler = GodmistWPF.Characters.Player.PlayerHandler;
using QuestManager = GodmistWPF.Quests.QuestManager;
using SaveData = GodmistWPF.Utilities.DataPersistance.SaveData;
using Scout = GodmistWPF.Characters.Player.Scout;
using Sorcerer = GodmistWPF.Characters.Player.Sorcerer;
using Warrior = GodmistWPF.Characters.Player.Warrior;
using ResourceType = GodmistWPF.Enums.ResourceType;

namespace GodmistWPF
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> gameLog;
        private bool isGameStarted;
        
        public bool IsGameStarted
        {
            get => isGameStarted;
            set
            {
                isGameStarted = value;
                SaveButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            gameLog = new ObservableCollection<string>();
            
            // Set up data binding
            DataContext = this;
        }

        #region Main Menu Events

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            ShowNewGameDialog();
        }

        private void LoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new SaveManagerDialog();
                saveDialog.ShowDialog();
                
                // Check if a game was loaded
                if (PlayerHandler.player != null && !isGameStarted)
                {
                    StartGame();
                    ShowTownView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading game: {ex.Message}", "Load Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LanguageButton_Click(object sender, RoutedEventArgs e)
        {
            ShowLanguageDialog();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Game Events

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerHandler.player != null)
            {
                DataPersistanceManager.SaveGame(new SaveData());
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowSettingsDialog();
        }

        private void SaveManagerButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveManagerDialog();
            dialog.ShowDialog();
        }

        #endregion

        #region Dialog Methods

        private void ShowNewGameDialog()
        {
            var dialog = new NewGameDialog();
            if (dialog.ShowDialog() == true)
            {
                // Initialize new game
                TownsHandler.Arungard = new Town("Arungard");
                GameSettings.Difficulty = dialog.SelectedDifficulty;
                QuestManager.InitMainQuests();
                QuestManager.InitSideQuests(true);
                
                // Create player character
                PlayerHandler.player = dialog.SelectedCharacterClass switch
                {
                    CharacterClass.Warrior => new Warrior(dialog.CharacterName),
                    CharacterClass.Scout => new Scout(dialog.CharacterName),
                    CharacterClass.Sorcerer => new Sorcerer(dialog.CharacterName),
                    CharacterClass.Paladin => new Paladin(dialog.CharacterName),
                    _ => new Warrior(dialog.CharacterName)
                };
                
                StartGame();
                ShowTownView();
            }
        }

        private void ShowLanguageDialog()
        {
            var languages = new[] { "English", "Polish" };
            var result = MessageBox.Show("Select language:\n\n1. English\n2. Polish", "Language Selection", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("pl-PL");
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pl-PL");
            }
        }

        private void ShowSettingsDialog()
        {
            var settings = new SettingsDialog();
            settings.ShowDialog();
        }

        private void ShowInventoryDialog()
        {
            var inventory = new InventoryDialog();
            inventory.ShowDialog();
        }

        private void ShowSkillsDialog()
        {
            var skills = new SkillsDialog();
            skills.ShowDialog();
        }

        private void ShowQuestsDialog()
        {
            var quests = new QuestsDialog();
            quests.ShowDialog();
        }

        #endregion

        #region Helper Methods

        private void StartGame()
        {
            MainMenuView.Visibility = Visibility.Collapsed;
            IsGameStarted = true;
        }

        private void ShowTownView()
        {
            MainMenuView.Visibility = Visibility.Collapsed;
            TownView.Visibility = Visibility.Visible;
            InitializeTownView();
        }

        private void InventoryButton_Town_Click(object sender, RoutedEventArgs e)
        {
            ShowInventoryDialog();
        }

        private void SaveGameButton_Town_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerHandler.player != null)
            {
                DataPersistanceManager.SaveGame(new SaveData());
                StatusText_Town.Text = "Game saved successfully.";
            }
        }

        private void StartExpeditionButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement expedition start logic
            StatusText_Town.Text = "Expedition started!";
        }

        private void QuestLogButton_Click(object sender, RoutedEventArgs e)
        {
            ShowQuestsDialog();
        }

        private void CharacterButton_Click(object sender, RoutedEventArgs e)
        {
            ShowSkillsDialog();
        }

        private void AlchemistButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Show Alchemist NPC panel
        }

        private void BlacksmithButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Show Blacksmith NPC panel
        }

        private void EnchanterButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Show Enchanter NPC panel
        }

        private void DungeonsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: Update dungeon info
        }

        private void EnterDungeonButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Enter selected dungeon
        }

        private void InitializeTownView()
        {
            // Set town name, description, gold, etc.
            if (TownsHandler.Arungard != null)
            {
                TownNameText.Text = TownsHandler.Arungard.TownName;
                TownDescriptionText.Text = $"Welcome to {TownsHandler.Arungard.TownName}, a bustling town where adventurers gather to trade, craft, and prepare for their next expedition. The town offers various services from skilled NPCs who can help you on your journey.";
                PlayerGoldText.Text = $"Gold: {PlayerHandler.player?.Gold ?? 0}";
                FooterText.Text = $"Town of {TownsHandler.Arungard.TownName} - Your adventure begins here!";
                StatusText_Town.Text = "Ready";
                // TODO: Populate dungeons, NPCs, etc.
            }
        }

        #endregion
    }
} 