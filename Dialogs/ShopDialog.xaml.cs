using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using GodmistWPF.Towns.NPCs;
using IItem = GodmistWPF.Items.IItem;
using ItemManager = GodmistWPF.Items.ItemManager;
using PlayerHandler = GodmistWPF.Characters.Player.PlayerHandler;

namespace GodmistWPF.Dialogs
{
    /// <summary>
    /// Okno dialogowe sklepu umożliwiające zakup przedmiotów od NPC.
    /// Wyświetla dostępne przedmioty, ich ceny i szczegóły, a także umożliwia ich zakup.
    /// </summary>
    public partial class ShopDialog : Window
    {
        /// <summary>Kolekcja przedmiotów dostępnych w sklepie.</summary>
        private ObservableCollection<ShopItemViewModel> shopItems;
        
        /// <summary>Aktualny sklepikarz, od którego kupowane są przedmioty.</summary>
        private NPC currentShopkeeper;

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="ShopDialog"> bez określonego sklepikarza.
        /// Używa domyślnej listy podstawowych przedmiotów.
        /// </summary>
        public ShopDialog()
        {
            InitializeComponent();
            
            shopItems = new ObservableCollection<ShopItemViewModel>();
            ItemsListBox.ItemsSource = shopItems;
            
            // Ustawienie obsługi zdarzeń
            ItemsListBox.SelectionChanged += ItemsListBox_SelectionChanged;
            
            // Załadowanie danych sklepu
            LoadShop();
        }

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="ShopDialog"> z określonym sklepikarzem.
        /// </summary>
        /// <param name="shopkeeper">NPC będący właścicielem sklepu, od którego można kupować przedmioty.</param>
        public ShopDialog(NPC shopkeeper) : this()
        {
            currentShopkeeper = shopkeeper;
            LoadShop();
        }

        /// <summary>
        /// Ładuje dostępne przedmioty do sklepu z inwentarza sklepikarza lub z domyślnej listy.
        /// Aktualizuje również wyświetlaną ilość złota gracza.
        /// </summary>
        private void LoadShop()
        {
            shopItems.Clear();
            
            if (currentShopkeeper != null)
            {
                // Użycie faktycznego inwentarza sklepikarza
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
                // Domyślna lista podstawowych przedmiotów, gdy nie ma sklepikarza
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
            
            // Aktualizacja wyświetlanej ilości złota gracza
            if (PlayerHandler.player != null)
            {
                GoldText.Text = $"{PlayerHandler.player.Gold} Gold";
            }
        }

        /// <summary>
        /// Obsługuje zmianę wyboru przedmiotu na liście.
        /// Aktualizuje panel szczegółów wybranego przedmiotu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (ListBox z listą przedmiotów).</param>
        /// <param name="e">Dane zdarzenia zmiany wyboru.</param>
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

        /// <summary>
        /// Obsługuje kliknięcie przycisku zakupu przedmiotu.
        /// Weryfikuje dostępne środki, wykonuje transakcję i aktualizuje interfejs.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk Kup).</param>
        /// <param name="e">Dane zdarzenia kliknięcia.</param>
        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsListBox.SelectedItem is ShopItemViewModel selectedItem)
            {
                try
                {
                    if (currentShopkeeper != null && selectedItem.Item != null)
                    {
                        // Użycie logiki sklepu z backendu
                        if (currentShopkeeper.CanBuyItem(selectedItem.Item))
                        {
                            currentShopkeeper.BuyItem(selectedItem.Item);
                            LoadShop(); // Odświeżenie sklepu
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
                        // Prosta logika rezerwowa
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

        /// <summary>
        /// Obsługuje kliknięcie przycisku zamknięcia okna.
        /// Zamyka okno dialogowe sklepu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk Zamknij).</param>
        /// <param name="e">Dane zdarzenia kliknięcia.</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    /// <summary>
    /// Klasa reprezentująca widok modelu przedmiotu w sklepie.
    /// Używana do wyświetlania informacji o przedmiocie w interfejsie użytkownika.
    /// </summary>
    public class ShopItemViewModel
    {
        /// <summary>Pobiera lub ustawia obiekt przedmiotu (może być null dla podstawowych przedmiotów).</summary>
        public IItem? Item { get; set; }
        
        /// <summary>Pobiera lub ustawia nazwę przedmiotu.</summary>
        public string Name { get; set; } = "";
        
        /// <summary>Pobiera lub ustawia cenę przedmiotu w złocie.</summary>
        public int Cost { get; set; }
        
        /// <summary>Pobiera lub ustawia typ przedmiotu (np. Broń, Zbroja, Mikstura).</summary>
        public string Type { get; set; } = "";
        
        /// <summary>Pobiera lub ustawia ilość dostępnych sztuk przedmiotu.</summary>
        public int Quantity { get; set; }
        
        /// <summary>Pobiera lub ustawia sformatowaną nazwę do wyświetlenia w interfejsie.</summary>
        public string DisplayName { get; set; } = "";
    }
} 