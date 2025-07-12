using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using GodmistWPF.Characters.Player;
using GodmistWPF.Quests;
using GodmistWPF.Items;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using ActiveSkill = GodmistWPF.Combat.Skills.ActiveSkill;
using Battle = GodmistWPF.Combat.Battles.Battle;
using BattleManager = GodmistWPF.Combat.Battles.BattleManager;
using BattleUser = GodmistWPF.Combat.Battles.BattleUser;
using DamageType = GodmistWPF.Enums.DamageType;
using DungeonFieldType = GodmistWPF.Enums.Dungeons.DungeonFieldType;
using DungeonRoom = GodmistWPF.Dungeons.DungeonRoom;
using DungeonType = GodmistWPF.Enums.Dungeons.DungeonType;
using EnemyFactory = GodmistWPF.Characters.EnemyFactory;
using IItem = GodmistWPF.Items.IItem;
using IUsable = GodmistWPF.Items.IUsable;
using PlayerCharacter = GodmistWPF.Characters.Player.PlayerCharacter;
using PlayerHandler = GodmistWPF.Characters.Player.PlayerHandler;
using ResourceType = GodmistWPF.Enums.ResourceType;

/// <summary>
/// Reprezentuje wskaźnik kolejki tury w interfejsie użytkownika walki.
/// Implementuje interfejs INotifyPropertyChanged do powiadamiania o zmianach właściwości.
/// </summary>
public class TurnOrderIndicator : INotifyPropertyChanged
{
    private string _name;
    private bool _isPlayer;
    private bool _isCurrent;

    /// <summary>
    /// Pobiera lub ustawia nazwę uczestnika walki.
    /// </summary>
    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Określa, czy uczestnik jest graczem.
    /// </summary>
    public bool IsPlayer
    {
        get => _isPlayer;
        set { _isPlayer = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Określa, czy to jest aktualny uczestnik, który wykonuje ruch.
    /// </summary>
    public bool IsCurrent
    {
        get => _isCurrent;
        set { _isCurrent = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Zdarzenie wywoływane, gdy wartość właściwości ulegnie zmianie.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Wywołuje zdarzenie PropertyChanged, powiadamiając o zmianie właściwości.
    /// </summary>
    /// <param name="propertyName">Nazwa zmienionej właściwości. Jeśli nie podano, zostanie użyta nazwa wywołującej właściwości.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

namespace GodmistWPF.Dialogs
{
    /// <summary>
    /// Okno dialogowe reprezentujące interfejs użytkownika walki.
    /// Wyświetla stan walki, umożliwia używanie umiejętności i przedmiotów oraz pokazuje rezultaty walki.
    /// </summary>
    public partial class BattleDialog : Window
    {
        private Battle _currentBattle;
        private BattleUser _playerUser;
        private BattleUser _enemyUser;
        private ObservableCollection<TurnOrderIndicator> _turnOrderIndicators;
        private bool _battleEnded;
        private bool _canClose = false;
        private IItem _selectedItem;

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="BattleDialog"/> z określoną walką.
        /// </summary>
        /// <param name="battle">Obiekt walki, który ma być wyświetlany w oknie dialogowym.</param>
        /// <exception cref="ArgumentNullException">Występuje, gdy parametr <paramref name="battle"/> jest równy null.</exception>
        public BattleDialog(Battle battle)
        {
            if (battle == null)
                throw new ArgumentNullException(nameof(battle), "Battle instance cannot be null");
                
            _currentBattle = battle;
            
            InitializeComponent();
            _turnOrderIndicators = new ObservableCollection<TurnOrderIndicator>();
            
            // Set the DataContext to this instance for binding
            this.DataContext = this;
            
            // Explicitly set the ItemsSource in code-behind
            if (TurnOrderIndicators != null)
            {
                TurnOrderIndicators.ItemsSource = _turnOrderIndicators;
            }
            
            // Initialize UI elements
            InitializeBattle();
        }

        /// <summary>
        /// Inicjalizuje stan walki, wczytując uczestników i ustawiając początkowe wartości interfejsu użytkownika.
        /// </summary>
        /// <exception cref="InvalidOperationException">Występuje, gdy nie można zainicjalizować uczestników walki.</exception>
        private void InitializeBattle()
        {
            if (_currentBattle == null)
                throw new InvalidOperationException("Battle instance is not set");
                
            _playerUser = _currentBattle.Users.FirstOrDefault(x => x.Value == 0).Key;
            _enemyUser = _currentBattle.Users.FirstOrDefault(x => x.Value == 1).Key;

            if (_playerUser == null || _enemyUser == null)
            {
                MessageBox.Show("Failed to initialize battle participants. The battle must have exactly two participants (player and enemy).", 
                    "Battle Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _canClose = true;
                Close();
                return;
            }

            UpdateBattleDisplay();
            LoadSkills();
            LoadItems();
            UpdateTurnOrderIndicators();
        }
        
        /// <summary>
        /// Aktualizuje wyświetlane informacje o stanie walki, w tym zdrowie, zasoby i punkty akcji.
        /// </summary>
        public void UpdateBattleDisplay()
        {
            Console.WriteLine("UpdateBattleDisplay called");
            
            if (_playerUser?.User == null)
            {
                Console.WriteLine("Player user is not initialized");
                return;
            }
            
            if (_enemyUser?.User == null)
            {
                Console.WriteLine("Enemy user is not initialized");
                return;
            }
            UpdateTurnOrderIndicators();
            PlayerName.Text = _playerUser.User.Name;
            double playerHealthPercent = (double)_playerUser.User.CurrentHealth / _playerUser.User.MaximalHealth;
            PlayerHealthBar.Value = playerHealthPercent * 100;
            PlayerHealthText.Text = $"{_playerUser.User.CurrentHealth:F0}/{_playerUser.User.MaximalHealth:F0}";
            string resourceText = _playerUser.User.ResourceType switch
            {
                ResourceType.Mana => "MANA",
                ResourceType.Fury => "FURY",
                ResourceType.Momentum => "MOMENTUM",
                _ => "RESOURCE"
            };
            PlayerResourceLabel.Text = resourceText;
            double resourcePercent = (double)_playerUser.User.CurrentResource / _playerUser.User.MaximalResource;
            PlayerResourceBar.Value = resourcePercent * 100;
            PlayerResourceText.Text = $"{_playerUser.User.CurrentResource:F0}/{_playerUser.User.MaximalResource:F0}";
            
            var maxAP = _playerUser.MaxActionPoints?.Value(_playerUser.User, "MaxActionPoints") ?? 5;
            PlayerActionPointsCount.Text = $"({_playerUser.CurrentActionPoints:F0}/{maxAP:F0})";
            PlayerActionPointsText.Text = new string('\u25cf', (int)_playerUser.CurrentActionPoints) + 
                                        new string('\u26ac', ((int)maxAP - (int)_playerUser.CurrentActionPoints));
            
            EnemyName.Text = _enemyUser.User.Name;
            
            double enemyHealthPercent = (double)_enemyUser.User.CurrentHealth / _enemyUser.User.MaximalHealth;
            EnemyHealthBar.Value = enemyHealthPercent * 100;
            EnemyHealthText.Text = $"{_enemyUser.User.CurrentHealth:F0}/{_enemyUser.User.MaximalHealth:F0}";
            string enemyResourceText = _enemyUser.User.ResourceType switch
            {
                ResourceType.Mana => "MANA",
                ResourceType.Fury => "FURY",
                ResourceType.Momentum => "MOMENTUM",
                _ => "RESOURCE"
            };
            EnemyResourceLabel.Text = enemyResourceText;
            double enemyResourcePercent = (double)_enemyUser.User.CurrentResource / _enemyUser.User.MaximalResource;
            EnemyResourceBar.Value = enemyResourcePercent * 100;
            EnemyResourceText.Text = $"{_enemyUser.User.CurrentResource:F0}/{_enemyUser.User.MaximalResource:F0}";
            
            TurnCounter.Text = $" - Turn {_currentBattle?.TurnCount ?? 1}";
            BattleStatus.Text = _currentBattle.Interface.IsPlayerInputEnabled ? "Player's Turn" : "Enemy's Turn";
            
            UpdateTurnOrderIndicators();
            UpdateButtonStates();
        }

        /// <summary>
        /// Wczytuje dostępne umiejętności gracza i wyświetla je w interfejsie użytkownika.
        /// </summary>
        private void LoadSkills()
        {
            if (_playerUser?.User is PlayerCharacter player)
            {
                var skills = player.ActiveSkills?.Where(s => s != null).ToList() ?? new List<ActiveSkill>();
                SkillsList.ItemsSource = skills;
            }
            else
            {
                SkillsList.ItemsSource = new List<ActiveSkill>();
            }
        }

        /// <summary>
        /// Wczytuje dostępne przedmioty z ekwipunku gracza i wyświetla je w interfejsie użytkownika.
        /// </summary>
        private void LoadItems()
        {
            if (_playerUser?.User is PlayerCharacter player && player.Inventory?.Items != null)
            {
                var usableItems = player.Inventory.Items
                    .Where(x => x.Key is IUsable)
                    .Select(x => x.Key)
                    .ToList();
                ItemsList.ItemsSource = usableItems;
            }
            else
            {
                ItemsList.ItemsSource = new List<IItem>();
            }
        }

        /// <summary>
        /// Aktualizuje wyświetlaną kolejkę tury, pokazując kolejność ruchów uczestników walki.
        /// </summary>
        private void UpdateTurnOrderIndicators()
        {
            Console.WriteLine("UpdateTurnOrderIndicators called");
            
            if (_currentBattle == null)
            {
                Console.WriteLine("_currentBattle is null");
                return;
            }
            if (_playerUser == null)
            {
                Console.WriteLine("_playerUser is null");
                return;
            }
            if (_enemyUser == null)
            {
                Console.WriteLine("_enemyUser is null");
                return;
            }
            
            try
            {
                _turnOrderIndicators.Clear();
                var users = _currentBattle.Users?.Keys?.ToList();
                
                if (users == null || !users.Any())
                {
                    Console.WriteLine("No users found in battle");
                    return;
                }
                
                Console.WriteLine($"Found {users.Count} users in battle");
                var order = _currentBattle.Interface.GetTurnOrder(_currentBattle.Users.Keys.ToList());
                foreach (var user in order)
                {
                    Console.WriteLine($"Adding to turn order: {user.Item1.User?.Name ?? "[No Name]"} (Player: {user.Item1 == _playerUser}, Current: {user.Item2 == 1})");
                    _turnOrderIndicators.Add(new TurnOrderIndicator
                    {
                        Name = user.Item1.User?.Name ?? "Unknown",
                        IsPlayer = user.Item1.User.Name == _playerUser.User.Name,
                        IsCurrent = user.Item2 == 0
                    });
                    
                }
                Console.WriteLine($"Turn order updated with {_turnOrderIndicators.Count} indicators");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateTurnOrderIndicators: {ex}");
            }
        }
        
        /// <summary>
        /// Aktualizuje stan przycisków w interfejsie użytkownika na podstawie dostępnych akcji i punktów akcji.
        /// </summary>
        private void UpdateButtonStates()
        {
            if (_playerUser == null) return;
            
            bool isPlayerTurn = _currentBattle.Interface.IsPlayerInputEnabled;
            bool hasEnoughAP = _playerUser.CurrentActionPoints > 0;
            bool hasSkills = (SkillsList.Items?.Count ?? 0) > 0;
            bool hasItems = (ItemsList.Items?.Count ?? 0) > 0;
            
            SkillsButton.IsEnabled = isPlayerTurn && hasEnoughAP && hasSkills;
            ItemsButton.IsEnabled = isPlayerTurn && hasItems;
            EndTurnButton.IsEnabled = isPlayerTurn;
            EscapeButton.IsEnabled = isPlayerTurn;
            
            // Update skill buttons
            if (SkillsList != null && SkillsList.Items != null)
            {
                foreach (var item in SkillsList.Items)
                {
                    if (item is ActiveSkill skill && item is FrameworkElement element)
                    {
                        bool canAfford = _playerUser.User.CurrentResource >= skill.ResourceCost &&
                                       _playerUser.CurrentActionPoints >= skill.ActionCost;
                        
                        element.IsEnabled = isPlayerTurn && canAfford;
                        element.Opacity = canAfford ? 1.0 : 0.6;
                    }
                }
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku umiejętności, pokazując panel umiejętności.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void SkillsButton_Click(object sender, RoutedEventArgs e)
        {
            // Show skills panel and hide items panel
            SkillsPanel.Visibility = Visibility.Visible;
            ItemsPanel.Visibility = Visibility.Collapsed;
            
            // Update button states
            SkillsButton.IsEnabled = false;
            ItemsButton.IsEnabled = true;
        }
        
        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku przedmiotów, pokazując panel przedmiotów.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void ItemsButton_Click(object sender, RoutedEventArgs e)
        {
            // Show items panel and hide skills panel
            ItemsPanel.Visibility = Visibility.Visible;
            SkillsPanel.Visibility = Visibility.Collapsed;
            
            // Update button states
            ItemsButton.IsEnabled = false;
            SkillsButton.IsEnabled = true;
        }
        
        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku powrotu, ukrywając panele umiejętności i przedmiotów.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide both panels
            SkillsPanel.Visibility = Visibility.Collapsed;
            ItemsPanel.Visibility = Visibility.Collapsed;
            
            // Re-enable main buttons
            SkillsButton.IsEnabled = true;
            ItemsButton.IsEnabled = true;
        }
        
        /// <summary>
        /// Obsługuje zdarzenie najechania kursorem na przycisk umiejętności, wyświetlając jej szczegóły.
        /// </summary>
        /// <param name="sender">Przycisk umiejętności, na który najechano kursorem.</param>
        /// <param name="e">Dane zdarzenia myszy.</param>
        private void SkillButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is Button { Tag: ActiveSkill skill })
            {
                // Update the title
                HoverInfoTitle.Text = "SKILL INFO";
                
                // Format the skill information
                var info = new System.Text.StringBuilder();
                info.AppendLine($"{skill.Name}");
                info.AppendLine();
                info.AppendLine($"{skill.GenerateDescription(_playerUser.User.ResourceType, _playerUser.MaxActionPoints.Value(_playerUser.User, "MaxActionPoints"))}");
                HoverInfoText.Text = info.ToString();
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie opuszczenia kursora z przycisku umiejętności, czyści panel informacyjny.
        /// </summary>
        /// <param name="sender">Przycisk umiejętności, z którego zjechano kursorem.</param>
        /// <param name="e">Dane zdarzenia myszy.</param>
        private void SkillButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            HoverInfoTitle.Text = "EXTRA INFO";
            HoverInfoText.Text = "";
        }
        
        /// <summary>
        /// Obsługuje zdarzenie najechania kursorem na przycisk przedmiotu, wyświetlając jego szczegóły.
        /// </summary>
        /// <param name="sender">Przycisk przedmiotu, na który najechano kursorem.</param>
        /// <param name="e">Dane zdarzenia myszy.</param>
        private void ItemButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is Button { DataContext: IItem item })
            {
                // Update the title
                HoverInfoTitle.Text = "ITEM INFO";
                
                // Format the item information
                var info = new System.Text.StringBuilder();
                info.AppendLine($"{item.Name}");
                info.AppendLine();
                info.AppendLine(item.Description);
                
                // Add item type and value if available
                info.AppendLine($"\nType: {item.ItemType}");
                
                HoverInfoText.Text = info.ToString();
            }
        }
        
        /// <summary>
        /// Obsługuje zdarzenie opuszczenia kursora z przycisku przedmiotu, czyści panel informacyjny.
        /// </summary>
        /// <param name="sender">Przycisk przedmiotu, z którego zjechano kursorem.</param>
        /// <param name="e">Dane zdarzenia myszy.</param>
        private void ItemButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            HoverInfoTitle.Text = "EXTRA INFO";
            HoverInfoText.Text = "";
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku umiejętności, używając wybranej umiejętności w walce.
        /// </summary>
        /// <param name="sender">Przycisk umiejętności, który wywołał zdarzenie.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void SkillButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: ActiveSkill skill } && _playerUser != null && _enemyUser != null)
            {
                
                // Use the backend logic to handle the skill usage
                skill.Use(_playerUser, _enemyUser);
                
                // Update UI to reflect the changes
                UpdateBattleDisplay();
                LoadSkills();
                UpdateTurnOrderIndicators();
                LoadItems();
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku przedmiotu, ustawiając wybrany przedmiot do użycia.
        /// </summary>
        /// <param name="sender">Przycisk przedmiotu, który wywołał zdarzenie.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void ItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is IItem item)
            {
                _selectedItem = item;
                UseItem();
            }
        }

        /// <summary>
        /// Wykorzystuje wybrany przedmiot w walce, jeśli jest to możliwe.
        /// Aktualizuje interfejs użytkownika po użyciu przedmiotu.
        /// </summary>
        private void UseItem()
        {
            if (_playerUser?.User is not PlayerCharacter player) return;
            if (_selectedItem == null) return;
            
            var selectedItem = _selectedItem;

            if (selectedItem is IUsable usable)
            {
                usable.Use();
                AddToBattleLog($"{player.Name} uses {selectedItem.Name}!");
                
                // Reset selection and hide panel
                _selectedItem = null;
                ItemsPanel.Visibility = Visibility.Collapsed;
                
                // Update display
                LoadItems();
                UpdateBattleDisplay();
            }
        }
        

        /// <summary>
        /// Kończy walkę z określonym rezultatem i wyświetla odpowiedni komunikat.
        /// </summary>
        /// <param name="result">Wynik walki: 0 - zwycięstwo, 1 - porażka, 2 - ucieczka.</param>
        public void EndBattle(int result)
        {
            _battleEnded = true;
            _canClose = true; // Allow closing when battle ends

            // Show result overlay
            ResultOverlay.Visibility = Visibility.Visible;
            
            switch (result)
            {
                case 0: // Victory
                    BattleResult.Text = "VICTORY!";
                    BattleResult.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Green
                    AddToBattleLog("You have won the battle!");
                    break;
                case 1: // Defeat
                    BattleResult.Text = "DEFEAT!";
                    BattleResult.Foreground = new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Red
                    AddToBattleLog("You have been defeated!");
                    break;
                case 2: // Escape
                    BattleResult.Text = "ESCAPED!";
                    BattleResult.Foreground = new SolidColorBrush(Color.FromRgb(255, 235, 59)); // Yellow
                    AddToBattleLog("You have escaped from the battle!");
                    break;
            }

            // Disable all battle controls
            SkillsButton.IsEnabled = false;
            ItemsButton.IsEnabled = false;
            EndTurnButton.IsEnabled = false;
            EscapeButton.IsEnabled = false;
            
            // Hide panels
            SkillsPanel.Visibility = Visibility.Collapsed;
            ItemsPanel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Dodaje wiadomość do dziennika walki z opcjonalnym kolorem tekstu.
        /// Automatycznie formatuje wiadomości na podstawie ich zawartości.
        /// </summary>
        /// <param name="message">Treść wiadomości do wyświetlenia.</param>
        /// <param name="color">Kolor tekstu wiadomości. Jeśli nie podano, kolor jest wybierany automatycznie.</param>
        public void AddToBattleLog(string message, Brush color = null)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => AddToBattleLog(message, color));
                return;
            }
            
            try
            {
                string timestamp = $"[{DateTime.Now:HH:mm:ss}]";
                
                // Create a new paragraph for each message
                var paragraph = new Paragraph { LineHeight = 1 };
                
                // Add timestamp
                var timestampRun = new Run(timestamp + " ");
                timestampRun.Foreground = Brushes.Gray;
                paragraph.Inlines.Add(timestampRun);
                
                // Add message with appropriate color
                var messageRun = new Run(message);
                
                // Use provided color or determine based on message content
                if (color != null)
                {
                    messageRun.Foreground = color;
                }
                else if (message.Contains("damage") || message.Contains("hit") || message.Contains("critical"))
                {
                    messageRun.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B6B"));
                }
                else if (message.Contains("heal") || message.Contains("restore") || message.Contains("recovers") || message.Contains("heals"))
                {
                    messageRun.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
                }
                else if (message.Contains("use") || message.Contains("cast") || message.Contains("activates") || message.Contains("uses"))
                {
                    messageRun.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4FC3F7"));
                }
                else if (message.Contains("dodged") || message.Contains("missed") || message.Contains("evaded") || message.Contains("blocked"))
                {
                    messageRun.Foreground = Brushes.Yellow;
                }
                else if (message.Contains("level up") || message.Contains("gains") || message.Contains("earns"))
                {
                    messageRun.Foreground = Brushes.Gold;
                }
                else
                {
                    messageRun.Foreground = Brushes.White;
                }
                
                paragraph.Inlines.Add(messageRun);
                
                // Add the paragraph to the document
                if (BattleLog.Document == null)
                {
                    BattleLog.Document = new FlowDocument();
                }
                
                // Limit the number of blocks to prevent memory issues
                const int maxBlocks = 100;
                while (BattleLog.Document.Blocks.Count >= maxBlocks)
                {
                    BattleLog.Document.Blocks.Remove(BattleLog.Document.Blocks.FirstBlock);
                }
                
                BattleLog.Document.Blocks.Add(paragraph);
                
                // Auto-scroll to bottom
                BattleLogScroll.ScrollToEnd();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in AddToBattleLog: {ex.Message}");
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie zamykania okna dialogowego walki, zapobiegając przedwczesnemu zamknięciu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia anulowania zamykania okna.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_canClose && !_battleEnded)
            {
                e.Cancel = true;
                MessageBox.Show("You cannot close the battle dialog directly. Please use the End Turn or other battle actions.", 
                    "Battle In Progress", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku zamknięcia po zakończeniu walki.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // This will be called when the battle is over and the close button is clicked
            _canClose = true;
            this.Close();
        }
        
        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku zamknięcia panelu nagród.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void CloseRewardsButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the rewards panel and the dialog
            RewardsPanel.Visibility = Visibility.Collapsed;
            Close();
        }
        
        /// <summary>
        /// Wyświetla panel z nagrodami otrzymanymi po walce.
        /// </summary>
        /// <param name="gold">Ilość otrzymanego złota.</param>
        /// <param name="exp">Ilość otrzymanego doświadczenia.</param>
        /// <param name="honor">Ilość otrzymanej chwały.</param>
        /// <param name="items">Lista otrzymanych przedmiotów wraz z ich ilościami.</param>
        public void ShowRewards(int gold, int exp, int honor, List<KeyValuePair<IItem, int>> items)
        {
            // Clear previous rewards
            RewardsList.Children.Clear();
            
            // Add gold reward
            if (gold > 0)
            {
                var goldPanel = CreateRewardItem("Gold", gold.ToString("N0"), "Gold");
                RewardsList.Children.Add(goldPanel);
            }
            
            // Add experience reward
            if (exp > 0)
            {
                var expPanel = CreateRewardItem("Experience", exp.ToString("N0"), "Exp");
                RewardsList.Children.Add(expPanel);
            }
            
            // Add honor reward
            if (honor > 0)
            {
                var honorPanel = CreateRewardItem("Honor", honor.ToString("N0"), "Honor");
                RewardsList.Children.Add(honorPanel);
            }
            
            // Add items
            foreach (var item in items)
            {
                var itemPanel = CreateRewardItem(item.Key.Name, item.Value.ToString(), item.Key.GetType().Name);
                RewardsList.Children.Add(itemPanel);
            }
            
            // Show the rewards panel
            RewardsPanel.Visibility = Visibility.Visible;
        }
        
        /// <summary>
        /// Tworzy panel reprezentujący pojedynczą nagrodę do wyświetlenia w panelu nagród.
        /// </summary>
        /// <param name="name">Nazwa nagrody.</param>
        /// <param name="amount">Ilość nagrody.</param>
        /// <param name="type">Typ nagrody (np. Gold, Exp, Honor).</param>
        /// <returns>Panel zawierający ikonę, nazwę i ilość nagrody.</returns>
        private StackPanel CreateRewardItem(string name, string amount, string type)
        {
            var panel = new StackPanel 
            { 
                Orientation = Orientation.Horizontal, 
                Margin = new Thickness(0, 5, 0, 0) 
            };
            
            // Add icon based on type
            string icon = type switch
            {
                "Gold" => "💰",
                "Exp" => "⭐",
                "Honor" => "🏆",
                _ => "🎁"
            };
            
            var iconBlock = new TextBlock 
            { 
                Text = icon, 
                FontSize = 20, 
                Margin = new Thickness(0, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            
            var nameBlock = new TextBlock 
            { 
                Text = name, 
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 200,
                TextTrimming = TextTrimming.CharacterEllipsis
            };
            
            var amountBlock = new TextBlock 
            { 
                Text = amount, 
                Foreground = Brushes.Gold,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 80
            };
            
            panel.Children.Add(iconBlock);
            panel.Children.Add(nameBlock);
            panel.Children.Add(amountBlock);
            
            return panel;
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku zakończenia tury gracza.
        /// Przekazuje kontrolę przeciwnikowi i aktualizuje interfejs użytkownika.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void EndTurnButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBattle == null) return;
            
            try
            {
                SkillsPanel.Visibility = Visibility.Collapsed;
                ItemsPanel.Visibility = Visibility.Collapsed;
                // Signal that the player's turn is complete
                _currentBattle.CompletePlayerMove(true);
                
                // Update the display
                UpdateBattleDisplay();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in EndTurnButton_Click: {ex.Message}");
                // Ensure the battle can continue even if there's an error
                _currentBattle.CompletePlayerMove(true);
            }
        }
        
        /// <summary>
        /// Obsługuje zdarzenie próby ucieczki z walki.
        /// Sprawdza możliwość ucieczki i podejmuje odpowiednie działania.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private async void EscapeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBattle == null || _playerUser == null || _playerUser.User is not PlayerCharacter player) 
                return;
            
            try
            {
                if (!_currentBattle.CanEscape)
                {
                    AddToBattleLog("You cannot escape from this battle!");
                    return;
                }
                var escaped = _currentBattle.TryEscape(player);
                if (escaped)
                {
                    AddToBattleLog($"{player.Name} successfully escaped!");
                    await Task.Delay(1000);
                    EndBattle(2); // 2 indicates escape
                }
                else
                {
                    AddToBattleLog($"{player.Name} failed to escape!");
                    // End the player's turn after a failed escape attempt
                    _currentBattle.CompletePlayerMove(true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in EscapeButton_Click: {ex.Message}");
                // Ensure the battle can continue even if there's an error
                _currentBattle.CompletePlayerMove(true);
            }
        }
        
        /// <summary>
        /// Wywoływana, gdy okno dialogowe jest zamykane.
        /// Zapewnia poprawne zakończenie walki i zwolnienie zasobów.
        /// </summary>
        /// <param name="e">Dane zdarzenia zamknięcia okna.</param>
        protected override void OnClosed(EventArgs e)
        {
            _battleEnded = true;
            
            base.OnClosed(e);
        }
    }
}