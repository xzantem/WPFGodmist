using System.Windows;
using BaseIngredient = GodmistWPF.Items.BaseIngredient;
using DamageType = GodmistWPF.Enums.DamageType;
using Dungeon = GodmistWPF.Dungeons.Dungeon;
using DungeonFieldType = GodmistWPF.Enums.Dungeons.DungeonFieldType;
using DungeonFloor = GodmistWPF.Dungeons.DungeonFloor;
using DungeonRoom = GodmistWPF.Dungeons.DungeonRoom;
using DungeonType = GodmistWPF.Enums.Dungeons.DungeonType;
using IItem = GodmistWPF.Items.IItem;
using ItemRarity = GodmistWPF.Enums.Items.ItemRarity;
using ItemType = GodmistWPF.Enums.Items.ItemType;
using PlayerHandler = GodmistWPF.Characters.Player.PlayerHandler;
using ResourceType = GodmistWPF.Enums.ResourceType;

namespace GodmistWPF.Dialogs
{
    public partial class DungeonDialog : Window
    {
        private Dungeon? _currentDungeon;
        private DungeonFloor? _currentFloor;
        private DungeonRoom? _currentRoom;
        private int _currentLocationIndex;

        public DungeonDialog()
        {
            InitializeComponent();
            InitializeDungeon();
        }

        public DungeonDialog(Dungeon dungeon) : this()
        {
            _currentDungeon = dungeon;
            InitializeDungeon();
        }

        private void InitializeDungeon()
        {
            if (_currentDungeon == null)
            {
                // Create a test dungeon
                _currentDungeon = new Dungeon(1, Enums.Dungeons.DungeonType.Forest);
            }

            _currentFloor = _currentDungeon.CurrentFloor;
            _currentRoom = _currentFloor.StarterRoom;
            _currentLocationIndex = 0;

            UpdateDungeonDisplay();
            LoadDungeonList();
            UpdatePlayerStatus();
        }

        private void LoadDungeonList()
        {
            var dungeons = new List<DungeonType>
            {
                Enums.Dungeons.DungeonType.Forest,
                Enums.Dungeons.DungeonType.Catacombs,
                Enums.Dungeons.DungeonType.ElvishRuins,
                Enums.Dungeons.DungeonType.Cove,
                Enums.Dungeons.DungeonType.Desert,
                Enums.Dungeons.DungeonType.Temple,
                Enums.Dungeons.DungeonType.Mountains,
                Enums.Dungeons.DungeonType.Swamp
            };

            DungeonsListBox.ItemsSource = dungeons;
        }

        private void UpdateDungeonDisplay()
        {
            if (_currentDungeon == null || _currentFloor == null || _currentRoom == null) return;

            // Update dungeon info
            DungeonName.Text = _currentDungeon.DungeonType.ToString();
            FloorNumber.Text = $"Floor: {_currentDungeon.Floors.IndexOf(_currentFloor) + 1}";
            RoomNumber.Text = $"Location: {_currentLocationIndex}";
            DungeonType.Text = $"Type: {_currentDungeon.DungeonType}";
            Difficulty.Text = $"Level: {_currentDungeon.DungeonLevel}";

            // Update room info
            RoomType.Text = $"Room Type: {_currentRoom.FieldType}";
            RoomDescription.Text = GetRoomDescription(_currentRoom.FieldType);

            // Update available actions based on room type
            UpdateAvailableActions();

            // Update map display
            UpdateMapDisplay();
        }

        private string GetRoomDescription(DungeonFieldType fieldType)
        {
            return fieldType switch
            {
                DungeonFieldType.Battle => "A dangerous area where enemies lurk. Be prepared for combat!",
                DungeonFieldType.Empty => "A safe area to rest and recover.",
                DungeonFieldType.Bonfire => "A safe resting place where you can recover your strength.",
                DungeonFieldType.Plant => "You notice some valuable plants growing here.",
                DungeonFieldType.Stash => "There's a hidden stash nearby that might contain valuable items.",
                DungeonFieldType.Trap => "This area seems suspicious. There might be traps here.",
                _ => "An unknown area of the dungeon."
            };
        }

        private void UpdateAvailableActions()
        {
            if (_currentRoom == null) return;

            // Enable/disable buttons based on room type
            switch (_currentRoom.FieldType)
            {
                case DungeonFieldType.Battle:
                    MoveForwardButton.IsEnabled = true;
                    MoveBackButton.IsEnabled = true;
                    RestBonfireButton.IsEnabled = false;
                    CollectPlantButton.IsEnabled = false;
                    OpenStashButton.IsEnabled = false;
                    DisarmTrapButton.IsEnabled = false;
                    break;

                case DungeonFieldType.Empty:
                    MoveForwardButton.IsEnabled = true;
                    MoveBackButton.IsEnabled = true;
                    RestBonfireButton.IsEnabled = false;
                    CollectPlantButton.IsEnabled = false;
                    OpenStashButton.IsEnabled = false;
                    DisarmTrapButton.IsEnabled = false;
                    break;

                case DungeonFieldType.Bonfire:
                    MoveForwardButton.IsEnabled = true;
                    MoveBackButton.IsEnabled = true;
                    RestBonfireButton.IsEnabled = true;
                    CollectPlantButton.IsEnabled = false;
                    OpenStashButton.IsEnabled = false;
                    DisarmTrapButton.IsEnabled = false;
                    break;

                case DungeonFieldType.Plant:
                    MoveForwardButton.IsEnabled = true;
                    MoveBackButton.IsEnabled = true;
                    RestBonfireButton.IsEnabled = false;
                    CollectPlantButton.IsEnabled = true;
                    OpenStashButton.IsEnabled = false;
                    DisarmTrapButton.IsEnabled = false;
                    break;

                case DungeonFieldType.Stash:
                    MoveForwardButton.IsEnabled = true;
                    MoveBackButton.IsEnabled = true;
                    RestBonfireButton.IsEnabled = false;
                    CollectPlantButton.IsEnabled = false;
                    OpenStashButton.IsEnabled = true;
                    DisarmTrapButton.IsEnabled = false;
                    break;

                case DungeonFieldType.Trap:
                    MoveForwardButton.IsEnabled = false;
                    MoveBackButton.IsEnabled = true;
                    RestBonfireButton.IsEnabled = false;
                    CollectPlantButton.IsEnabled = false;
                    OpenStashButton.IsEnabled = false;
                    DisarmTrapButton.IsEnabled = true;
                    break;

                default:
                    MoveForwardButton.IsEnabled = true;
                    MoveBackButton.IsEnabled = true;
                    RestBonfireButton.IsEnabled = false;
                    CollectPlantButton.IsEnabled = false;
                    OpenStashButton.IsEnabled = false;
                    DisarmTrapButton.IsEnabled = false;
                    break;
            }

            // Enable/disable floor navigation
            DescendButton.IsEnabled = _currentLocationIndex >= _currentFloor.Corridor.Count;
            AscendButton.IsEnabled = _currentDungeon.Floors.IndexOf(_currentFloor) > 0;
        }

        private void UpdateMapDisplay()
        {
            if (_currentFloor == null) return;

            // Create a simple text-based map
            var mapText = "";
            
            // Show starter room
            mapText += _currentLocationIndex == 0 ? "[X] " : "[O] ";
            
            // Show corridor
            for (int i = 0; i < _currentFloor.Corridor.Count; i++)
            {
                var corridor = _currentFloor.Corridor[i];
                var symbol = _currentLocationIndex == i + 1 ? "X" : "O";
                mapText += $"[{symbol}] ";
                
                if ((i + 2) % 5 == 0) mapText += "\n";
            }
            
            // Show end room
            if (_currentLocationIndex == _currentFloor.Corridor.Count + 1)
                mapText += "[X] ";
            else
                mapText += "[O] ";

            MapDisplay.Text = mapText;
        }

        private void UpdatePlayerStatus()
        {
            var player = PlayerHandler.player;
            if (player == null) return;
            
            PlayerName.Text = player.Name;
            PlayerLevel.Text = $"Level: {player.Level}";
            PlayerHealth.Text = $"HP: {player.CurrentHealth:F0}/{player.MaximalHealth:F0}";
            
            var resourceText = player.ResourceType switch
            {
                ResourceType.Mana => "MP",
                ResourceType.Fury => "RP",
                ResourceType.Momentum => "SP",
                _ => "MP"
            };
            PlayerResource.Text = $"{resourceText}: {player.CurrentResource:F0}/{player.MaximalResource:F0}";
            PlayerGold.Text = $"Gold: {player.Gold}";
            PlayerAttack.Text = $"Attack: {player.MinimalAttack:F0}-{player.MaximalAttack:F0}";
            PlayerDefense.Text = $"Defense: {player.PhysicalDefense:F0}/{player.MagicDefense:F0}";
            PlayerSpeed.Text = $"Speed: {player.Speed:F0}";
        }

        private void MoveForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDungeon == null || _currentFloor == null || _currentRoom == null) return;

            if (_currentLocationIndex < _currentFloor.Corridor.Count + 1)
            {
                _currentLocationIndex++;
                if (_currentLocationIndex == _currentFloor.Corridor.Count + 1)
                {
                    _currentRoom = _currentFloor.EndRoom;
                }
                else
                {
                    _currentRoom = _currentFloor.Corridor[_currentLocationIndex - 1];
                }
                UpdateDungeonDisplay();
                AddToLog("Moved forward to the next location.");
            }
            else
            {
                // Descend to next floor
                _currentDungeon.Descend();
                _currentFloor = _currentDungeon.CurrentFloor;
                _currentRoom = _currentFloor.StarterRoom;
                _currentLocationIndex = 0;
                UpdateDungeonDisplay();
                AddToLog($"Descended to floor {_currentDungeon.Floors.IndexOf(_currentFloor) + 1}.");
            }
        }

        private void MoveBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDungeon == null || _currentFloor == null || _currentRoom == null) return;

            if (_currentLocationIndex == 0)
            {
                // Move up to previous floor
                if (_currentDungeon.Floors.IndexOf(_currentFloor) > 0)
                {
                    _currentDungeon.Ascend();
                    _currentFloor = _currentDungeon.CurrentFloor;
                    _currentRoom = _currentFloor.EndRoom;
                    _currentLocationIndex = _currentFloor.Corridor.Count + 1;
                    UpdateDungeonDisplay();
                    AddToLog($"Ascended to floor {_currentDungeon.Floors.IndexOf(_currentFloor) + 1}.");
                }
                else
                {
                    AddToLog("Cannot ascend further. This is the top floor.");
                }
            }
            else
            {
                _currentLocationIndex--;
                if (_currentLocationIndex == 0)
                {
                    _currentRoom = _currentFloor.StarterRoom;
                }
                else
                {
                    _currentRoom = _currentFloor.Corridor[_currentLocationIndex - 1];
                }
                UpdateDungeonDisplay();
                AddToLog("Moved back to the previous location.");
            }
        }

        private void RestBonfireButton_Click(object sender, RoutedEventArgs e)
        {
            var player = PlayerHandler.player;
            var healAmount = player.MaximalHealth * 0.3f;
            player.Heal(healAmount);
            
            UpdatePlayerStatus();
            AddToLog($"Rested at bonfire. Restored {healAmount:F0} health.");
        }

        private void CollectPlantButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentRoom == null) return;

            // Simulate plant collection
            var random = new Random();
            var items = new List<IItem>
            {
                new BaseIngredient("Herb", 1, 1, ItemRarity.Common, "A common herb", ItemType.Alchemy),
                new BaseIngredient("Rare Flower", 5, 1, ItemRarity.Uncommon, "A rare flower", ItemType.Alchemy),
                new BaseIngredient("Mystical Plant", 10, 1, ItemRarity.Rare, "A mystical plant", ItemType.Alchemy)
            };

            var collectedItem = items[random.Next(items.Count)];
            PlayerHandler.player.Inventory.AddItem(collectedItem);
            
            UpdatePlayerStatus();
            AddToLog($"Collected {collectedItem.Name}!");
        }

        private void OpenStashButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentRoom == null) return;

            // Simulate stash opening
            var random = new Random();
            var gold = random.Next(10, 50);
            PlayerHandler.player.Gold += gold;
            
            UpdatePlayerStatus();
            AddToLog($"Found {gold} gold in the stash!");
        }

        private void DisarmTrapButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentRoom == null) return;

            // Simulate trap disarming
            var random = new Random();
            if (random.NextDouble() < 0.7) // 70% success rate
            {
                AddToLog("Successfully disarmed the trap!");
                _currentRoom.Clear();
                UpdateDungeonDisplay();
            }
            else
            {
                var damage = random.Next(10, 30);
                PlayerHandler.player.TakeDamage(DamageType.Physical, damage, null);
                AddToLog($"Failed to disarm trap! Took {damage} damage.");
                UpdatePlayerStatus();
            }
        }

        private void DescendButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDungeon == null || _currentFloor == null) return;

            _currentDungeon.Descend();
            _currentFloor = _currentDungeon.CurrentFloor;
            _currentRoom = _currentFloor.StarterRoom;
            _currentLocationIndex = 0;
            UpdateDungeonDisplay();
            AddToLog($"Descended to floor {_currentDungeon.Floors.IndexOf(_currentFloor) + 1}.");
        }

        private void AscendButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDungeon == null || _currentFloor == null) return;

            if (_currentDungeon.Floors.IndexOf(_currentFloor) > 0)
            {
                _currentDungeon.Ascend();
                _currentFloor = _currentDungeon.CurrentFloor;
                _currentRoom = _currentFloor.EndRoom;
                _currentLocationIndex = _currentFloor.Corridor.Count + 1;
                UpdateDungeonDisplay();
                AddToLog($"Ascended to floor {_currentDungeon.Floors.IndexOf(_currentFloor) + 1}.");
            }
            else
            {
                AddToLog("Cannot ascend further. This is the top floor.");
            }
        }

        private void ExitDungeonButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit the dungeon?", 
                "Exit Dungeon", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private void OpenInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            var inventoryDialog = new InventoryDialog();
            inventoryDialog.ShowDialog();
        }

        private void OpenQuestLogButton_Click(object sender, RoutedEventArgs e)
        {
            var questsDialog = new QuestsDialog();
            questsDialog.ShowDialog();
        }

        private void AddToLog(string message)
        {
            DungeonLog.Text += $"[{DateTime.Now:HH:mm:ss}] {message}\n";
            DungeonLogScroll.ScrollToEnd();
        }
    }
} 