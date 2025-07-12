using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Media;
using GodmistWPF.Quests;
using System.Windows.Threading;
using GodmistWPF.Characters.Player;
using GodmistWPF.Combat.Battles;
using GodmistWPF.Dungeons;
using GodmistWPF.Dungeons.Interactables;
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
    /// <summary>
    /// Okno dialogowe reprezentujące eksplorację lochów w grze.
    /// Zawiera interfejs do poruszania się po lochach, interakcji z obiektami i walki z przeciwnikami.
    /// </summary>
    public partial class DungeonDialog : Window
    {
        /// <summary>
        /// Aktualnie eksplorowany loch.
        /// </summary>
        private Dungeon? _currentDungeon;

        /// <summary>
        /// Aktualne piętro w lochu.
        /// </summary>
        private DungeonFloor? _currentFloor;

        /// <summary>
        /// Aktualna komnata, w której znajduje się gracz.
        /// </summary>
        private DungeonRoom? _currentRoom;

        /// <summary>
        /// Indeks aktualnej lokalizacji gracza w korytarzu.
        /// </summary>
        private int _currentLocationIndex;

        /// <summary>
        /// Określa, czy okno może zostać zamknięte.
        /// </summary>
        private bool _canClose = false;

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="DungeonDialog"/>
        /// i rozpoczyna eksplorację nowego losowo wygenerowanego lochu.
        /// </summary>
        public DungeonDialog()
        {
            InitializeComponent();
            InitializeDungeon();
            
            // Subscribe to quest completion events
            QuestManager.QuestCompleted += QuestManager_QuestCompleted;
        }
        
        /// <summary>
        /// Obsługuje zdarzenie zwalniania zasobów okna.
        /// Odpowiada za anulowanie subskrypcji zdarzeń.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            // Unsubscribe from events when window is closed
            QuestManager.QuestCompleted -= QuestManager_QuestCompleted;
        }
        
        /// <summary>
        /// Obsługuje zdarzenie ukończenia zadania.
        /// Wyświetla powiadomienie o ukończeniu zadania.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="quest">Ukończone zadanie.</param>
        private void QuestManager_QuestCompleted(object sender, Quest quest)
        {
            // Show the quest completed overlay
            Dispatcher.Invoke((Action)(() =>
            {
                QuestCompletedOverlay.ShowCompletedQuests(new List<Quest> { quest });
            }));
        }

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="DungeonDialog">
        /// dla określonego, istniejącego już lochu.
        /// </summary>
        /// <param name="dungeon">Loch, który ma być eksplorowany.</param>
        public DungeonDialog(Dungeon dungeon) : this()
        {
            _currentDungeon = dungeon;
            InitializeDungeon();
        }

        /// <summary>
        /// Inicjalizuje nowy loch lub ładuje istniejący.
        /// Jeśli nie podano lochu, wyświetla okno wyboru parametrów nowego lochu.
        /// </summary>
        private void InitializeDungeon()
        {
            if (_currentDungeon == null)
            {
                // Show dungeon selection dialog if no dungeon was provided
                var selectionDialog = new DungeonSelectionDialog();
                if (selectionDialog.ShowDialog() == true)
                {
                    _currentDungeon = new Dungeon(selectionDialog.SelectedLevel, selectionDialog.SelectedDungeonType);
                    DungeonMovementManager.EnterDungeon(_currentDungeon); 
                }
                else
                {
                    // User cancelled dungeon selection
                    Close();
                    return;
                }
            }

            _currentFloor = _currentDungeon.CurrentFloor;
            _currentRoom = _currentFloor.StarterRoom;
            _currentLocationIndex = 0;

            // Update UI with dungeon info
            DungeonName.Text = $"Dungeon: {_currentDungeon.DungeonType}";
            DungeonType.Text = $"Type: {_currentDungeon.DungeonType}";
            Difficulty.Text = $"Level: {_currentDungeon.DungeonLevel}";
            
            UpdateDungeonDisplay();
            UpdatePlayerStatus();
            AddToLog($"Entered {_currentDungeon.DungeonType} Dungeon (Level {_currentDungeon.DungeonLevel})");
        }

        /// <summary>
        /// Obsługuje zdarzenie załadowania okna.
        /// Ustawia fokus na oknie.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Focus the window
            Focus();
        }

        /// <summary>
        /// Aktualizuje wyświetlane informacje o lochu i aktualnej lokacji.
        /// Wywołuje aktualizację dostępnych akcji, mapy i stanu gracza.
        /// </summary>
        public void UpdateDungeonDisplay()
        {
            if (_currentDungeon == null || _currentFloor == null || _currentRoom == null) return;

            // Update UI elements
            Dispatcher.Invoke(() =>
            {
                DungeonName.Text = $"Dungeon: {_currentDungeon.DungeonType}";
                FloorNumber.Text = $"Floor: {_currentDungeon.Floors.IndexOf(_currentFloor)}";
                RoomNumber.Text = $"Location: {_currentLocationIndex}";
                DungeonType.Text = $"Type: {_currentDungeon.DungeonType}";
                Difficulty.Text = $"Level: {_currentDungeon.DungeonLevel}";
                
                // Update available actions based on room type
                UpdateAvailableActions();

                // Update map display
                UpdateMapDisplay();
                
                UpdatePlayerStatus();
                
                // Force UI update
                CommandManager.InvalidateRequerySuggested();
                this.InvalidateVisual();
            });
        }

        private void UpdateAvailableActions()
        {
            if (_currentRoom == null || _currentFloor == null || _currentDungeon == null) return;

            // Reset all button visibilities
            MoveBackButton.Visibility = Visibility.Collapsed;
            MoveForwardButton.Visibility = Visibility.Collapsed;
            RestBonfireButton.Visibility = Visibility.Collapsed;
            CollectPlantButton.Visibility = Visibility.Collapsed;
            OpenStashButton.Visibility = Visibility.Collapsed;
            DisarmTrapButton.Visibility = Visibility.Collapsed;
            DescendButton.Visibility = Visibility.Collapsed;
            AscendButton.Visibility = Visibility.Collapsed;
            ExitDungeonButton.Visibility = Visibility.Collapsed;
            PermanentButtonsSeparator.Visibility = Visibility.Collapsed;
            
            // Battle handling is now done in DungeonMovementManager.OnMove()
            
            // Show forward or descend button
            if (_currentLocationIndex <= _currentFloor.Corridor.Count)
            {
                MoveForwardButton.Visibility = Visibility.Visible;
            }
            else
            {
                DescendButton.Visibility = Visibility.Visible;
            }

            // Show movement buttons based on position
            if (_currentLocationIndex > 0)
            {
                MoveBackButton.Visibility = Visibility.Visible;
            }
            else if (_currentFloor == _currentDungeon.Floors.First())
            {
                ExitDungeonButton.Visibility = Visibility.Visible;
            }
            else
            {
                AscendButton.Visibility = Visibility.Visible;
            }
            
            // Show room-specific action
            switch (_currentRoom.FieldType)
            {
                case DungeonFieldType.Bonfire:
                    RestBonfireButton.Visibility = Visibility.Visible;
                    break;
                case DungeonFieldType.Plant:
                    CollectPlantButton.Visibility = Visibility.Visible;
                    break;
                case DungeonFieldType.Stash:
                    OpenStashButton.Visibility = Visibility.Visible;
                    break;
            }
            
            var right = _currentLocationIndex < _currentFloor.Corridor.Count ? _currentFloor.Corridor[_currentLocationIndex] : null;
            var trap = _currentFloor.Traps.FirstOrDefault(t =>
                t.Location == right);
            if (trap != null && right is { Revealed: true })
            {
                DisarmTrapButton.Visibility = Visibility.Visible;
            }

            // Show separator if any action buttons are visible
            if (MoveBackButton.Visibility == Visibility.Visible ||
                MoveForwardButton.Visibility == Visibility.Visible ||
                RestBonfireButton.Visibility == Visibility.Visible ||
                CollectPlantButton.Visibility == Visibility.Visible ||
                OpenStashButton.Visibility == Visibility.Visible ||
                DisarmTrapButton.Visibility == Visibility.Visible ||
                DescendButton.Visibility == Visibility.Visible ||
                AscendButton.Visibility == Visibility.Visible ||
                ExitDungeonButton.Visibility == Visibility.Visible)
            {
                PermanentButtonsSeparator.Visibility = Visibility.Visible;
            }
        }

        // Cache for room view models to maintain references
        private Dictionary<DungeonField, ViewModels.RoomViewModel> _roomViewModels = new();

        private void UpdateMapDisplay()
        {
            if (_currentFloor == null) return;

            var rooms = new System.Collections.ObjectModel.ObservableCollection<ViewModels.RoomViewModel>();
            
            try
            {
                string startRoomName = "Stairs Up";
                Color startRoomColor = Colors.DarkGreen;
                
                if (_currentFloor.StarterRoom.FieldType == DungeonFieldType.Battle)
                    startRoomColor = Colors.Red;
                
                var startRoom = GetOrCreateRoomViewModel(_currentFloor.StarterRoom, "S", startRoomName, startRoomName, startRoomColor);
                startRoom.IsCurrent = _currentLocationIndex == 0;
                rooms.Add(startRoom);
                for (int i = 0; i < _currentFloor.Corridor.Count; i++)
                {
                    var room = _currentFloor.Corridor[i];
                    var roomType = room.FieldType;
                    var (roomName, color) = roomType switch
                    {
                        DungeonFieldType.Bonfire => ("Bonfire", Colors.Orange),
                        DungeonFieldType.Plant => ("Plant", Colors.Green),
                        DungeonFieldType.Stash => ("Stash", Colors.Gold),
                        DungeonFieldType.Trap => ("Trap", Colors.Purple),
                        DungeonFieldType.Battle => ("Battle", Colors.Red),
                        _ => ("Empty", Colors.Gray)
                    };
                    
                    var roomVm = GetOrCreateRoomViewModel(room, roomName[0].ToString(), roomName, roomName, color);
                    roomVm.IsCurrent = _currentLocationIndex == i + 1;
                    rooms.Add(roomVm);
                }
                
                var endRoom = GetOrCreateRoomViewModel(_currentFloor.EndRoom, "S", "Stairs Down", "Stairs Down", Colors.DarkGreen);
                endRoom.IsCurrent = _currentLocationIndex == _currentFloor.Corridor.Count + 1;
                rooms.Add(endRoom);
            }
            catch (Exception ex)
            {
                AddToLog($"Error updating map: {ex.Message}");
            }
            
            // Set the items source on the UI thread
            Dispatcher.Invoke(() =>
            {
                MapDisplay.ItemsSource = null; // Force refresh
                MapDisplay.ItemsSource = rooms;
            });
        }

        private ViewModels.RoomViewModel GetOrCreateRoomViewModel(DungeonField room, string symbol, string roomInfo, string roomTypeName, Color color)
        {
            if (room == null) return null;

            // Ensure we're on the UI thread when creating/updating the view model
            ViewModels.RoomViewModel viewModel = null;
            
            // Check if we already have a view model for this room
            if (!_roomViewModels.TryGetValue(room, out viewModel) || viewModel == null)
            {
                // Create a new view model
                viewModel = new ViewModels.RoomViewModel
                {
                    Symbol = symbol,
                    RoomInfo = roomInfo,
                    RoomTypeName = roomTypeName,
                    // Don't set background color here, let UpdateDisplay handle it
                    IsRevealed = room.Revealed
                };
                // Set initial background color based on revealed state
                if (!room.Revealed)
                {
                    viewModel.BackgroundColor = new SolidColorBrush(Colors.DarkGray);
                }
                else
                {
                    viewModel.BackgroundColor = new SolidColorBrush(color);
                }
                _roomViewModels[room] = viewModel;
            }
            else
            {
                // Update the existing view model
                bool needsUpdate = false;
                
                // Only update properties if they've changed to minimize UI updates
                if (viewModel.Symbol != symbol || viewModel.RoomInfo != roomInfo || viewModel.RoomTypeName != roomTypeName)
                {
                    viewModel.Symbol = symbol;
                    viewModel.RoomInfo = roomInfo;
                    viewModel.RoomTypeName = roomTypeName;
                    needsUpdate = true;
                }
                
                // Update revealed state if changed - this will trigger UpdateDisplay()
                if (viewModel.IsRevealed != room.Revealed)
                {
                    viewModel.IsRevealed = room.Revealed;
                    needsUpdate = true;
                }
                
                // Update color if revealed state changed or color is different
                if (viewModel.IsRevealed && (viewModel.BackgroundColor == null || viewModel.BackgroundColor.Color != color))
                {
                    viewModel.BackgroundColor = new SolidColorBrush(color);
                    needsUpdate = true;
                }
                
                // Force property changed notification if anything was updated
                if (needsUpdate)
                {
                    viewModel.OnPropertyChanged(nameof(viewModel.Symbol));
                    viewModel.OnPropertyChanged(nameof(viewModel.RoomInfo));
                    viewModel.OnPropertyChanged(nameof(viewModel.RoomTypeName));
                    viewModel.OnPropertyChanged(nameof(viewModel.BackgroundColor));
                }
            }
            
            return viewModel;
        }

        /// <summary>
        /// Aktualizuje wyświetlane statystyki gracza w interfejsie użytkownika.
        /// Wyświetla imię, poziom, zdrowie, zasoby, złoto oraz statystyki postaci.
        /// </summary>
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


        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku ruchu do przodu w lochu.
        /// Przenosi gracza do następnej lokacji w korytarzu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void MoveForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDungeon == null || _currentFloor == null || _currentRoom == null) return;

            // Disable the button to prevent multiple clicks
            var button = sender as System.Windows.Controls.Button;
            if (button != null) button.IsEnabled = false;

            try
            {
                // Use DungeonMovementManager to handle movement and room revealing
                DungeonMovementManager.MoveForward();
                
                // Update our local state to match the manager's state
                _currentFloor = DungeonMovementManager.CurrentDungeon?.CurrentFloor;
                _currentRoom = DungeonMovementManager.CurrentLocation as DungeonRoom;
                _currentLocationIndex = DungeonMovementManager.LocationIndex;
                
                // If we've exited the dungeon, close the dialog
                if (DungeonMovementManager.Exited)
                {
                    DialogResult = true;
                    Close();
                    return;
                }
                
                // Update the display with the new state
                UpdateDungeonDisplay();
                AddToLog("Moved forward to the next location.");
            }
            catch (Exception ex)
            {
                AddToLog($"Error moving forward: {ex}");
            }
            finally
            {
                // Re-enable the button
                if (button != null) button.IsEnabled = true;
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku ruchu do tyłu w lochu.
        /// Przenosi gracza do poprzedniej lokacji w korytarzu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void MoveBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDungeon == null || _currentFloor == null || _currentRoom == null) return;

            // Disable the button to prevent multiple clicks
            var button = sender as System.Windows.Controls.Button;
            if (button != null) button.IsEnabled = false;

            try
            {
                // Use DungeonMovementManager to handle movement and room revealing
                DungeonMovementManager.MoveBackwards();
                
                // Update our local state to match the manager's state
                _currentFloor = DungeonMovementManager.CurrentDungeon?.CurrentFloor;
                _currentRoom = DungeonMovementManager.CurrentLocation as DungeonRoom;
                _currentLocationIndex = DungeonMovementManager.LocationIndex;
                
                // If we've exited the dungeon, close the dialog
                if (DungeonMovementManager.Exited)
                {
                    DialogResult = true;
                    Close();
                    return;
                }
                
                // Update the display with the new state
                UpdateDungeonDisplay();
                AddToLog("Moved back to the previous location.");
            }
            catch (Exception ex)
            {
                AddToLog($"Error moving back: {ex.Message}");
            }
            finally
            {
                // Re-enable the button
                if (button != null) button.IsEnabled = true;
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie interakcji z ogniskiem odpoczynku.
        /// Przywraca część zdrowia graczowi, ale może wywołać zasadzkę.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private async void RestBonfireButton_Click(object sender, RoutedEventArgs e)
        {
            var ambush = DungeonMovementManager.RestAtBonfire();
            if (ambush != null)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    var battleDialog = new BattleDialog(ambush);
                    battleDialog.Closed += async (s, e) =>
                    {
                        await Task.CompletedTask;
                    };
                    battleDialog.ShowDialog();
                    BattleManager.StartNewBattle(ambush);
                });
            }
            
            UpdateDungeonDisplay();
            AddToLog($"Rested at bonfire. Restored {(PlayerHandler.player.MaximalHealth / 4):F0} health.");
        }

        /// <summary>
        /// Obsługuje zdarzenie zbierania rośliny w lochu.
        /// Zbiera roślinę i dodaje ją do ekwipunku gracza.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void CollectPlantButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentRoom == null) return;

            var plant = DungeonMovementManager.CollectPlant();
            
            UpdateDungeonDisplay();
            if (plant != null)
                AddToLog($"Collected {plant.Value.Item1.Name} (x{plant.Value.Item2})!");
        }

        /// <summary>
        /// Obsługuje zdarzenie otwierania skrzyni w lochu.
        /// Otwiera skrzynię i dodaje jej zawartość do ekwipunku gracza.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void OpenStashButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentRoom == null) return;

            var items = DungeonMovementManager.OpenStash();
            
            UpdatePlayerStatus();
            if (items != null)
                AddToLog("Opened stash! The stash contained:" + string.Join("\n", items.Select(i => $"- {i.Key.Name} (x{i.Value})")));
        }

        /// <summary>
        /// Obsługuje zdarzenie rozbrajania pułapki w lochu.
        /// Próbuje rozbroić pułapkę, co może zakończyć się sukcesem lub porażką.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void DisarmTrapButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentRoom == null) return;
            var right = _currentLocationIndex < _currentFloor.Corridor.Count ? _currentFloor.Corridor[_currentLocationIndex] : null;
            if (right == null) return;
            var trap = _currentFloor.Traps.FirstOrDefault(t =>
                t.Location == right);
            if (trap == null) return;
            var disarmed = DungeonMovementManager.DisarmTrap(trap, false);
            
            UpdateDungeonDisplay();
            AddToLog(disarmed ? "Disarmed trap!" : "Failed to disarm trap.");
        }

        /// <summary>
        /// Obsługuje zdarzenie schodzenia na niższe piętro lochu.
        /// Przenosi gracza na niższe piętro, jeśli jest to możliwe.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void DescendButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDungeon == null || _currentFloor == null) return;

            // Use DungeonMovementManager to handle descending
            DungeonMovementManager.MoveForward();
            
            // Update our local state to match the manager's state
            _currentFloor = DungeonMovementManager.CurrentDungeon?.CurrentFloor;
            _currentRoom = DungeonMovementManager.CurrentLocation as DungeonRoom;
            _currentLocationIndex = DungeonMovementManager.LocationIndex;
            
            // Update the display with the new state
            UpdateDungeonDisplay();
            AddToLog($"Descended to floor {_currentDungeon.Floors.IndexOf(_currentFloor) + 1}.");
        }

        /// <summary>
        /// Obsługuje zdarzenie wchodzenia na wyższe piętro lochu.
        /// Przenosi gracza na wyższe piętro lub wyprowadza z lochu, jeśli to możliwe.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void AscendButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDungeon == null || _currentFloor == null) return;

            // Use DungeonMovementManager to handle ascending
            DungeonMovementManager.MoveBackwards();
            
            // Update our local state to match the manager's state
            _currentFloor = DungeonMovementManager.CurrentDungeon?.CurrentFloor;
            _currentRoom = DungeonMovementManager.CurrentLocation as DungeonRoom;
            _currentLocationIndex = DungeonMovementManager.LocationIndex;
            
            // If we've exited the dungeon, close the dialog
            if (DungeonMovementManager.Exited)
            {
                DialogResult = true;
                Close();
                return;
            }
            
            // Update the display with the new state
            UpdateDungeonDisplay();
            AddToLog($"Ascended to floor {_currentDungeon.Floors.IndexOf(_currentFloor) + 1}.");
        }

        /// <summary>
        /// Obsługuje zdarzenie zamykania okna.
        /// Zapobiega zamknięciu okna, jeśli gracz nie użył przycisku wyjścia.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia anulowania zamykania.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_canClose)
            {
                e.Cancel = true;
                MessageBox.Show("You cannot close the dungeon dialog directly. Please use the Exit button to leave the dungeon properly.", 
                    "Dungeon In Progress", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie opuszczania lochu.
        /// Wyświetla potwierdzenie i zamyka okno eksploracji lochu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void ExitDungeonButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit the dungeon?", 
                "Exit Dungeon", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                // Use DungeonMovementManager to handle exiting the dungeon
                DungeonMovementManager.ExitDungeon();
                _canClose = true;
                DialogResult = true;
                Close();
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie otwierania ekwipunku.
        /// Wyświetla okno z zawartością ekwipunku gracza.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void OpenInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            var inventoryDialog = new InventoryDialog();
            inventoryDialog.ShowDialog();
        }

        /// <summary>
        /// Obsługuje zdarzenie otwierania ekranu postaci.
        /// Wyświetla szczegółowe informacje o postaci gracza.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void OpenCharacterButton_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerHandler.player != null)
            {
                var characterWindow = new CharacterWindow(PlayerHandler.player);
                characterWindow.Owner = this;
                characterWindow.ShowDialog();
            }
            else
            {
                AddToLog("Error: Player character not found.");
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie otwierania dziennika zadań.
        /// Wyświetla listę aktywnych i ukończonych zadań.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void OpenQuestLogButton_Click(object sender, RoutedEventArgs e)
        {
            var questsDialog = new QuestsDialog();
            questsDialog.ShowDialog();
        }

        /// <summary>
        /// Dodaje nowy wpis do dziennika przygód.
        /// Automatycznie przewija zawartość do najnowszego wpisu.
        /// </summary>
        /// <param name="message">Wiadomość do dodania do dziennika.</param>
        private void AddToLog(string message)
        {
            DungeonLog.Text += $"[{DateTime.Now:HH:mm:ss}] {message}\n";
            DungeonLogScroll.ScrollToEnd();
        }
    }
} 