using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using IItem = GodmistWPF.Items.IItem;
using IUsable = GodmistWPF.Items.IUsable;
using PlayerHandler = GodmistWPF.Characters.Player.PlayerHandler;

namespace GodmistWPF.Dialogs
{
    public partial class InventoryDialog : Window
    {
        private ObservableCollection<InventoryItemViewModel> inventoryItems;

        public InventoryDialog()
        {
            InitializeComponent();
            
            inventoryItems = new ObservableCollection<InventoryItemViewModel>();
            InventoryListBox.ItemsSource = inventoryItems;
            
            // Set up event handlers
            InventoryListBox.SelectionChanged += InventoryListBox_SelectionChanged;
            
            // Load inventory data
            LoadInventory();
        }

        private void LoadInventory()
        {
            inventoryItems.Clear();
            
            if (PlayerHandler.player?.Inventory != null)
            {
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
            
            // If no items, show a message
            if (inventoryItems.Count == 0)
            {
                ItemInfoText.Text = "Your inventory is empty.";
            }
        }

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

        private void UseButton_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryListBox.SelectedItem is InventoryItemViewModel selectedItem)
            {
                if (selectedItem.Item is IUsable usable)
                {
                    if (usable.Use())
                    {
                        PlayerHandler.player.Inventory.TryRemoveItem(selectedItem.Item);
                        LoadInventory(); // Refresh the list
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

        private void DropButton_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryListBox.SelectedItem is InventoryItemViewModel selectedItem)
            {
                var result = MessageBox.Show($"Are you sure you want to drop {selectedItem.Item.Name}?", "Confirm Drop", 
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    PlayerHandler.player.Inventory.TryRemoveItem(selectedItem.Item, selectedItem.Quantity);
                    LoadInventory(); // Refresh the list
                    ItemInfoText.Text = "Select an item to see its information.";
                }
            }
            else
            {
                MessageBox.Show("Please select an item to drop.", "No Item Selected", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class InventoryItemViewModel
    {
        public IItem Item { get; set; }
        public int Quantity { get; set; }
        public string DisplayName { get; set; }
    }
} 