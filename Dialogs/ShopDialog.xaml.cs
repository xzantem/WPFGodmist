using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using GodmistWPF.Towns.NPCs;
using IItem = GodmistWPF.Items.IItem;
using ItemManager = GodmistWPF.Items.ItemManager;
using PlayerHandler = GodmistWPF.Characters.Player.PlayerHandler;

namespace GodmistWPF.Dialogs
{
    public partial class ShopDialog : Window
    {
        private ObservableCollection<ShopItemViewModel> shopItems;
        private NPC currentShopkeeper;

        public ShopDialog()
        {
            InitializeComponent();
            
            shopItems = new ObservableCollection<ShopItemViewModel>();
            ItemsListBox.ItemsSource = shopItems;
            
            // Set up event handlers
            ItemsListBox.SelectionChanged += ItemsListBox_SelectionChanged;
            
            // Load shop data
            LoadShop();
        }

        public ShopDialog(NPC shopkeeper) : this()
        {
            currentShopkeeper = shopkeeper;
            LoadShop();
        }

        private void LoadShop()
        {
            shopItems.Clear();
            
            if (currentShopkeeper != null)
            {
                // Use the actual shopkeeper's inventory
                var shopItems = currentShopkeeper.GetShopItems();
                
                foreach (var shopItem in shopItems)
                {
                    this.shopItems.Add(new ShopItemViewModel
                    {
                        Item = shopItem.Item,
                        Name = shopItem.Item.Name,
                        Cost = shopItem.Cost,
                        Type = shopItem.Item.ItemTypeName(),
                        Quantity = shopItem.Quantity,
                        DisplayName = $"{shopItem.DisplayName} - {shopItem.DisplayCost}"
                    });
                }
            }
            else
            {
                // Fallback to basic items if no shopkeeper
                var basicItems = new[]
                {
                    new { Name = "Health Potion", Cost = 50, Type = "Consumable" },
                    new { Name = "Mana Potion", Cost = 50, Type = "Consumable" },
                    new { Name = "Iron Sword", Cost = 200, Type = "Weapon" },
                    new { Name = "Leather Armor", Cost = 150, Type = "Armor" },
                    new { Name = "Antidote", Cost = 75, Type = "Consumable" }
                };
                
                foreach (var item in basicItems)
                {
                    shopItems.Add(new ShopItemViewModel
                    {
                        Name = item.Name,
                        Cost = item.Cost,
                        Type = item.Type,
                        DisplayName = $"{item.Name} - {item.Cost} Gold"
                    });
                }
            }
            
            // Update player gold display
            if (PlayerHandler.player != null)
            {
                GoldText.Text = $"{PlayerHandler.player.Gold} Gold";
            }
        }

        private void ItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemsListBox.SelectedItem is ShopItemViewModel selectedItem)
            {
                var item = selectedItem.Item ?? ItemManager.GetItem(selectedItem.Name);
                if (item != null)
                {
                    SelectedItemText.Text = $"Name: {item.Name}\n" +
                                           $"Type: {item.ItemTypeName()}\n" +
                                           $"Rarity: {item.RarityName()}\n" +
                                           $"Cost: {selectedItem.Cost} Gold\n" +
                                           $"Weight: {item.Weight}kg\n\n" +
                                           $"Description: {item.Description}";
                }
                else
                {
                    SelectedItemText.Text = $"Name: {selectedItem.Name}\n" +
                                           $"Type: {selectedItem.Type}\n" +
                                           $"Cost: {selectedItem.Cost} Gold\n\n" +
                                           $"Description: A {selectedItem.Type.ToLower()} item available for purchase.";
                }
            }
            else
            {
                SelectedItemText.Text = "No item selected";
            }
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsListBox.SelectedItem is ShopItemViewModel selectedItem)
            {
                try
                {
                    if (currentShopkeeper != null && selectedItem.Item != null)
                    {
                        // Use backend shop logic
                        if (currentShopkeeper.CanBuyItem(selectedItem.Item))
                        {
                            currentShopkeeper.BuyItem(selectedItem.Item);
                            LoadShop(); // Refresh the shop
                            MessageBox.Show($"You purchased {selectedItem.Name} for {selectedItem.Cost} Gold!", 
                                           "Purchase Successful", 
                                           MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show($"You don't have enough gold. You need {selectedItem.Cost} Gold but have {PlayerHandler.player.Gold} Gold.", 
                                           "Not Enough Gold", 
                                           MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        // Fallback to simple logic
                        if (PlayerHandler.player.Gold >= selectedItem.Cost)
                        {
                            PlayerHandler.player.Gold -= selectedItem.Cost;
                            GoldText.Text = $"{PlayerHandler.player.Gold} Gold";
                            
                            MessageBox.Show($"You purchased {selectedItem.Name} for {selectedItem.Cost} Gold!", 
                                           "Purchase Successful", 
                                           MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show($"You don't have enough gold. You need {selectedItem.Cost} Gold but have {PlayerHandler.player.Gold} Gold.", 
                                           "Not Enough Gold", 
                                           MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error purchasing item: {ex.Message}", "Purchase Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an item to purchase.", "No Item Selected", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class ShopItemViewModel
    {
        public IItem? Item { get; set; }
        public string Name { get; set; } = "";
        public int Cost { get; set; }
        public string Type { get; set; } = "";
        public int Quantity { get; set; }
        public string DisplayName { get; set; } = "";
    }
} 