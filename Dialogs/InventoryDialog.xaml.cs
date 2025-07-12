using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using IItem = GodmistWPF.Items.IItem;
using IUsable = GodmistWPF.Items.IUsable;
using PlayerHandler = GodmistWPF.Characters.Player.PlayerHandler;

namespace GodmistWPF.Dialogs
{
    /// <summary>
    /// Okno dialogowe wyświetlające zawartość ekwipunku gracza.
    /// Umożliwia przeglądanie, używanie i usuwanie przedmiotów.
    /// </summary>
    public partial class InventoryDialog : Window
    {
        /// <summary>
        /// Kolekcja przedmiotów w ekwipunku powiązana z interfejsem użytkownika.
        /// </summary>
        private ObservableCollection<InventoryItemViewModel> inventoryItems;

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="InventoryDialog">.
        /// Konfiguruje interfejs użytkownika i ładuje zawartość ekwipunku gracza.
        /// </summary>
        public InventoryDialog()
        {
            InitializeComponent();
            
            inventoryItems = new ObservableCollection<InventoryItemViewModel>();
            InventoryListBox.ItemsSource = inventoryItems;
            
            // Konfiguruj obsługę zdarzeń
            InventoryListBox.SelectionChanged += InventoryListBox_SelectionChanged;
            
            // Załaduj zawartość ekwipunku
            LoadInventory();
        }

        /// <summary>
        /// Ładuje zawartość ekwipunku gracza do widoku.
        /// Aktualizuje wyświetlaną ilość złota oraz listę przedmiotów.
        /// </summary>
        private void LoadInventory()
        {
            inventoryItems.Clear();
            
            if (PlayerHandler.player?.Inventory != null)
            {
                // Aktualizuj wyświetlaną ilość złota
                GoldText.Text = PlayerHandler.player.Gold.ToString("N0");
                
                // Dodaj wszystkie przedmioty do widoku
                foreach (var item in PlayerHandler.player.Inventory.Items)
                {
                    var displayName = item.Value > 1 ? $"{item.Key.Name} (x{item.Value})" : item.Key.Name;
                    inventoryItems.Add(new InventoryItemViewModel
                    {
                        Item = item.Key,
                        Quantity = item.Value,
                        DisplayName = displayName
                    });
                }
            }
            
            // Jeśli brak przedmiotów, wyświetl odpowiedni komunikat
            if (inventoryItems.Count == 0)
            {
                ItemInfoText.Text = "Your inventory is empty.";
            }
        }

        /// <summary>
        /// Obsługuje zmianę wybranego przedmiotu na liście.
        /// Aktualizuje panel informacyjny z danymi wybranego przedmiotu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (lista przedmiotów).</param>
        /// <param name="e">Dane zdarzenia zmiany wyboru.</param>
        private void InventoryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InventoryListBox.SelectedItem is InventoryItemViewModel selectedItem)
            {
                var item = selectedItem.Item;
                ItemInfoText.Text = $"Name: {item.Name}\n" +
                                   $"Type: {item.ItemTypeName()}\n" +
                                   $"Rarity: {item.RarityName()}\n" +
                                   $"Weight: {item.Weight}kg\n" +
                                   $"Cost: {item.Cost} Gold\n" +
                                   $"Description: {item.Description}";
            }
            else
            {
                ItemInfoText.Text = "Select an item to see its information.";
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku użycia przedmiotu.
        /// Sprawdza, czy przedmiot może być użyty, a następnie wywołuje jego funkcjonalność.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void UseButton_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryListBox.SelectedItem is InventoryItemViewModel selectedItem)
            {
                if (selectedItem.Item is IUsable usable)
                {
                    if (usable.Use())
                    {
                        PlayerHandler.player.Inventory.TryRemoveItem(selectedItem.Item);
                        LoadInventory(); // Odśwież listę
                        MessageBox.Show($"You used {selectedItem.Item.Name}", "Item Used", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("This item cannot be used.", "Cannot Use Item", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select an item to use.", "No Item Selected", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku wyrzucenia przedmiotu.
        /// Wyświetla potwierdzenie, a następnie usuwa przedmiot z ekwipunku.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void DropButton_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryListBox.SelectedItem is InventoryItemViewModel selectedItem)
            {
                var result = MessageBox.Show($"Are you sure you want to drop {selectedItem.Item.Name}?", "Confirm Drop", 
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    PlayerHandler.player.Inventory.TryRemoveItem(selectedItem.Item, selectedItem.Quantity);
                    LoadInventory(); // Odśwież listę
                    ItemInfoText.Text = "Select an item to see its information.";
                }
            }
            else
            {
                MessageBox.Show("Please select an item to drop.", "No Item Selected", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku zamknięcia okna.
        /// Zamyka okno dialogowe.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    /// <summary>
    /// Klasa pomocnicza reprezentująca przedmiot w widoku listy ekwipunku.
    /// Przechowuje referencję do przedmiotu, jego ilość i sformatowaną nazwę do wyświetlenia.
    /// </summary>
    public class InventoryItemViewModel
    {
        /// <summary>
        /// Pobiera lub ustawia referencję do przedmiotu.
        /// </summary>
        public IItem Item { get; set; }

        /// <summary>
        /// Pobiera lub ustawia ilość danego przedmiotu w ekwipunku.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Pobiera lub ustawia sformatowaną nazwę przedmiotu do wyświetlenia w interfejsie.
        /// W przypadku większej ilości niż 1, dodaje liczbę sztuk w nawiasie.
        /// </summary>
        public string DisplayName { get; set; }
    }
} 