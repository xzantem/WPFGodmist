using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
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

namespace GodmistWPF.Dialogs
{
    public partial class BattleDialog : Window
    {
        private Battle? _currentBattle;
        private BattleUser? _playerUser;
        private BattleUser? _enemyUser;
        private DispatcherTimer? _battleTimer;
        private bool _isAutoBattle;
        private bool _battleEnded;

        public BattleDialog()
        {
            InitializeComponent();
            InitializeBattle();
        }

        public BattleDialog(Battle battle) : this()
        {
            _currentBattle = battle;
            InitializeBattle();
        }

        private void InitializeBattle()
        {
            if (_currentBattle == null)
            {
                // Create a test battle if none provided
                var player = PlayerHandler.player;
                if (player == null)
                {
                    MessageBox.Show("No player character found. Please start a new game first.", "No Player", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    Close();
                    return;
                }
                
                var enemy = EnemyFactory.CreateEnemy(DungeonType.Forest, player.Level);
                
                _currentBattle = new Battle(new Dictionary<BattleUser, int>
                {
                    { new BattleUser(player), 0 },
                    { new BattleUser(enemy), 1 }
                }, new DungeonRoom(DungeonFieldType.Battle));
                
                BattleManager.StartNewBattle(_currentBattle);
            }

            _playerUser = _currentBattle.Users.FirstOrDefault(x => x.Value == 0).Key;
            _enemyUser = _currentBattle.Users.FirstOrDefault(x => x.Value == 1).Key;

            if (_playerUser == null || _enemyUser == null)
            {
                MessageBox.Show("Failed to initialize battle participants.", "Battle Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            UpdateBattleDisplay();
            LoadSkills();
            LoadItems();
            UpdateTurnOrder();

            // Start battle timer
            _battleTimer = new DispatcherTimer();
            _battleTimer.Interval = TimeSpan.FromMilliseconds(100);
            _battleTimer.Tick += BattleTimer_Tick;
            _battleTimer.Start();
        }

        private void BattleTimer_Tick(object? sender, EventArgs e)
        {
            if (_battleEnded) return;

            UpdateBattleDisplay();
            
            // Check for battle end
            var result = _currentBattle?.CheckForResult();
            if (result.HasValue && result.Value != -1)
            {
                EndBattle(result.Value);
                return;
            }

            // Auto battle logic
            if (_isAutoBattle && _playerUser?.MovedThisTurn == false)
            {
                PerformAITurn();
            }
        }

        private void UpdateBattleDisplay()
        {
            if (_playerUser?.User == null || _enemyUser?.User == null) return;

            // Update player info
            PlayerName.Text = _playerUser.User.Name;
            PlayerLevel.Text = $"Level: {_playerUser.User.Level}";
            PlayerHealth.Text = $"HP: {_playerUser.User.CurrentHealth:F0}/{_playerUser.User.MaximalHealth:F0}";
            
            var resourceText = _playerUser.User.ResourceType switch
            {
                ResourceType.Mana => "MP",
                ResourceType.Fury => "RP",
                ResourceType.Momentum => "SP",
                _ => "RP"
            };
            PlayerResource.Text = $"{resourceText}: {_playerUser.User.CurrentResource:F0}/{_playerUser.User.MaximalResource:F0}";
            
            PlayerAttack.Text = $"Attack: {_playerUser.User.MinimalAttack:F0}-{_playerUser.User.MaximalAttack:F0}";
            PlayerDefense.Text = $"Defense: {_playerUser.User.PhysicalDefense:F0}/{_playerUser.User.MagicDefense:F0}";
            PlayerSpeed.Text = $"Speed: {_playerUser.User.Speed:F0}";
            
            var maxAP = _playerUser.MaxActionPoints?.Value(_playerUser.User, "MaxActionPoints") ?? 5;
            PlayerActionPoints.Text = $"AP: {_playerUser.CurrentActionPoints:F0}/{maxAP:F0}";

            // Update enemy info
            EnemyName.Text = _enemyUser.User.Name;
            EnemyLevel.Text = $"Level: {_enemyUser.User.Level}";
            EnemyHealth.Text = $"HP: {_enemyUser.User.CurrentHealth:F0}/{_enemyUser.User.MaximalHealth:F0}";
            EnemyResource.Text = $"MP: {_enemyUser.User.CurrentResource:F0}/{_enemyUser.User.MaximalResource:F0}";
            EnemyAttack.Text = $"Attack: {_enemyUser.User.MinimalAttack:F0}-{_enemyUser.User.MaximalAttack:F0}";
            EnemyDefense.Text = $"Defense: {_enemyUser.User.PhysicalDefense:F0}/{_enemyUser.User.MagicDefense:F0}";
            EnemySpeed.Text = $"Speed: {_enemyUser.User.Speed:F0}";

            // Update battle info
            TurnCounter.Text = $"Turn: {_currentBattle?.TurnCount ?? 1}";
            BattleStatus.Text = _playerUser.MovedThisTurn ? "Enemy's Turn" : "Player's Turn";
        }

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

        private void UpdateTurnOrder()
        {
            if (_currentBattle == null) return;

            var turnOrder = new List<string>();
            var users = _currentBattle.Users.Keys.ToList();
            
            // Simulate next few turns
            for (int i = 0; i < 5; i++)
            {
                foreach (var user in users)
                {
                    if (user.TryMove())
                    {
                        turnOrder.Add($"{user.User.Name} (Turn {i + 1})");
                        break;
                    }
                }
            }

            TurnOrderList.ItemsSource = turnOrder;
        }

        private void UseSkillButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBattle == null || _playerUser == null || _enemyUser == null) return;
            if (_playerUser.MovedThisTurn) return;

            var selectedSkill = SkillsList.SelectedItem as ActiveSkill;
            if (selectedSkill == null)
            {
                MessageBox.Show("Please select a skill to use.", "No Skill Selected");
                return;
            }

            // Check if skill can be used
            var resourceCost = selectedSkill.ResourceCost;
            var actionCost = selectedSkill.ActionCost * _playerUser.MaxActionPoints.Value(_playerUser.User, "MaxActionPoints");
            
            if (_playerUser.User.CurrentResource < resourceCost)
            {
                MessageBox.Show("Not enough resource to use this skill.", "Insufficient Resource");
                return;
            }

            if (_playerUser.CurrentActionPoints < actionCost)
            {
                MessageBox.Show("Not enough action points to use this skill.", "Insufficient Action Points");
                return;
            }

            // Use the skill
            selectedSkill.Use(_playerUser, _enemyUser);
            AddToBattleLog($"{_playerUser.User.Name} uses {selectedSkill.Name} on {_enemyUser.User.Name}!");
            
            UpdateBattleDisplay();
            LoadSkills();
            UpdateTurnOrder();
        }

        private void UseItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (_playerUser?.User is not PlayerCharacter player) return;

            var selectedItem = ItemsList.SelectedItem as IItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Please select an item to use.", "No Item Selected");
                return;
            }

            if (selectedItem is IUsable usable)
            {
                usable.Use();
                AddToBattleLog($"{player.Name} uses {selectedItem.Name}!");
                LoadItems();
            }
        }

        private void EndTurnButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBattle == null || _playerUser == null) return;

            // Check if player can end turn
            var canUseSkill = _playerUser.User.ActiveSkills
                .Any(x => x != null && x.ResourceCost <= _playerUser.User.CurrentResource &&
                         x.ActionCost * _playerUser.MaxActionPoints.Value(_playerUser.User, "MaxActionPoints") <= _playerUser.CurrentActionPoints);

            if (canUseSkill)
            {
                var result = MessageBox.Show("You can still use skills. Are you sure you want to end your turn?", 
                    "End Turn", MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes) return;
            }

            // Use the battle's built-in turn management instead of directly setting properties
            _currentBattle.NewTurn();
            AddToBattleLog($"{_playerUser.User.Name} ends their turn.");

            UpdateBattleDisplay();
            UpdateTurnOrder();
        }

        private void EscapeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBattle == null || _playerUser == null) return;

            AddToBattleLog($"{_playerUser.User.Name} attempts to escape...");
            
            // 50% chance to escape
            if (new Random().NextDouble() < 0.5)
            {
                AddToBattleLog("Escape successful!");
                _currentBattle.Escaped = true;
                EndBattle(2); // Escape result
            }
            else
            {
                AddToBattleLog("Escape failed!");
                // Use the battle's turn management
                _currentBattle.NewTurn();
            }
        }

        private void AutoBattleButton_Click(object sender, RoutedEventArgs e)
        {
            _isAutoBattle = !_isAutoBattle;
            AutoBattleButton.Content = _isAutoBattle ? "Stop Auto Battle" : "Auto Battle";
            AutoBattleButton.Background = _isAutoBattle ? 
                Brushes.Orange : 
                Brushes.DodgerBlue;
        }

        private void SurrenderButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to surrender? This will end the battle in defeat.", 
                "Surrender", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                EndBattle(1); // Defeat
            }
        }

        private void PerformAITurn()
        {
            if (_currentBattle == null || _enemyUser == null) return;

            // Simple AI: use random skill or basic attack
            var availableSkills = _enemyUser.User.ActiveSkills.Where(s => s != null).ToList();
            if (availableSkills.Any())
            {
                var randomSkill = availableSkills[new Random().Next(availableSkills.Count)];
                randomSkill.Use(_enemyUser, _playerUser);
                AddToBattleLog($"{_enemyUser.User.Name} uses {randomSkill.Name}!");
            }
            else
            {
                // Basic attack
                var damage = new Random().Next((int)_enemyUser.User.MinimalAttack, (int)_enemyUser.User.MaximalAttack + 1);
                _playerUser.User.TakeDamage(DamageType.Physical, damage, _enemyUser.User);
                AddToBattleLog($"{_enemyUser.User.Name} attacks for {damage} damage!");
            }

            // Use the battle's turn management
            _currentBattle.NewTurn();
        }

        private void EndBattle(int result)
        {
            _battleEnded = true;
            _battleTimer?.Stop();

            switch (result)
            {
                case 0: // Victory
                    BattleResult.Text = "VICTORY!";
                    BattleResult.Foreground = Brushes.LightGreen;
                    AddToBattleLog("You have won the battle!");
                    break;
                case 1: // Defeat
                    BattleResult.Text = "DEFEAT!";
                    BattleResult.Foreground = Brushes.Red;
                    AddToBattleLog("You have been defeated!");
                    break;
                case 2: // Escape
                    BattleResult.Text = "ESCAPED!";
                    BattleResult.Foreground = Brushes.Yellow;
                    AddToBattleLog("You have escaped from the battle!");
                    break;
            }

            // Disable battle controls
            UseSkillButton.IsEnabled = false;
            UseItemButton.IsEnabled = false;
            EndTurnButton.IsEnabled = false;
            EscapeButton.IsEnabled = false;
            AutoBattleButton.IsEnabled = false;
        }

        private void AddToBattleLog(string message)
        {
            BattleLog.Text += $"[{DateTime.Now:HH:mm:ss}] {message}\n";
            BattleLogScroll.ScrollToEnd();
        }

        protected override void OnClosed(EventArgs e)
        {
            _battleTimer?.Stop();
            base.OnClosed(e);
        }
    }
} 