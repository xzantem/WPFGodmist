using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Items;
using GodmistWPF.Items;
using GodmistWPF.Items.Galdurites;
using GodmistWPF.Utilities;

namespace GodmistWPF.Dialogs
{
    /// <summary>
    /// Okno dialogowe umożliwiające tworzenie galduritów - specjalnych kamieni wzmacniających.
    /// Zawiera interfejs do wyboru typu, poziomu i koloru galduritu oraz podglądu dostępnych komponentów.
    /// </summary>
    public partial class GalduriteCraftingDialog : Window
    {
        /// <summary>
        /// Kolekcja komponentów wybranych do tworzenia galduritu.
        /// </summary>
        private readonly ObservableCollection<GalduriteComponent> _selectedComponents = new();

        /// <summary>
        /// Określa, czy tworzony jest galdurit do zbroi (true) czy do broni (false).
        /// </summary>
        private bool _isArmorGaldurite = false;

        /// <summary>
        /// Poziom tworzonego galduritu (1-3).
        /// </summary>
        private int _tier = 1;

        /// <summary>
        /// Kolor tworzonego galduritu.
        /// </summary>
        private string _color = "Red";

        /// <summary>
        /// Mapowanie kolorów galduritów na odpowiednie proszki.
        /// </summary>
        private readonly Dictionary<string, string> _colorToPowderMap = new()
        {
            { "Red", "RedPowder" },
            { "Blue", "BluePowder" },
            { "Green", "GreenPowder" },
            { "Yellow", "YellowPowder" },
            { "Purple", "PurplePowder" },
            { "Orange", "OrangePowder" },
            { "Pink", "PinkPowder" },
            { "White", "WhitePowder" },
            { "Black", "BlackPowder" },
            { "Golden", "GoldenPowder" }
        };

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="GalduriteCraftingDialog"/>
        /// i konfiguruje interfejs użytkownika.
        /// </summary>
        public GalduriteCraftingDialog()
        {
            InitializeComponent();
            
            // Set up data binding for selected components
            SelectedComponentsList.ItemsSource = _selectedComponents;
            
            // Initialize UI
            InitializeGalduriteTypeCombo();
            InitializeTierCombo();
            InitializeColorCombo();
            
            // Update UI state
            UpdateUIState();
        }
        
        /// <summary>
        /// Inicjalizuje listę rozwijaną z typami galduritów (do broni lub zbroi).
        /// Ustawia domyślny wybór i obsługę zmiany wyboru.
        /// </summary>
        private void InitializeGalduriteTypeCombo()
        {
            GalduriteTypeCombo.Items.Add(new ComboBoxItem { Content = "Weapon Galdurite", Tag = false });
            GalduriteTypeCombo.Items.Add(new ComboBoxItem { Content = "Armor Galdurite", Tag = true });
            GalduriteTypeCombo.SelectedIndex = 0;
            GalduriteTypeCombo.SelectionChanged += (s, e) => 
            {
                _isArmorGaldurite = ((ComboBoxItem)GalduriteTypeCombo.SelectedItem).Tag as bool? ?? false;
                UpdateUIState();
            };
        }
        
        /// <summary>
        /// Inicjalizuje listę rozwijaną z dostępnymi poziomami galduritów.
        /// Ustawia domyślny poziom i obsługę zmiany wyboru.
        /// </summary>
        private void InitializeTierCombo()
        {
            for (int i = 1; i <= 3; i++) // Tiers 1-3
            {
                GalduriteTierCombo.Items.Add(new ComboBoxItem { Content = new string('I', i), Tag = i });
            }
            GalduriteTierCombo.SelectedIndex = 0;
            GalduriteTierCombo.SelectionChanged += (s, e) => 
            {
                _tier = ((ComboBoxItem)GalduriteTierCombo.SelectedItem).Tag as int? ?? 1;
                UpdateUIState();
            };
        }
        
        /// <summary>
        /// Inicjalizuje listę rozwijaną z dostępnymi kolorami galduritów.
        /// Ustawia domyślny kolor i obsługę zmiany wyboru.
        /// </summary>
        private void InitializeColorCombo()
        {
            foreach (var color in _colorToPowderMap.Keys)
            {
                var item = new ComboBoxItem { Content = color, Tag = color };
                GalduriteColorCombo.Items.Add(item);
            }
            GalduriteColorCombo.SelectedIndex = 0;
            GalduriteColorCombo.SelectionChanged += (s, e) => 
            {
                _color = ((ComboBoxItem)GalduriteColorCombo.SelectedItem).Tag as string ?? "Red";
                UpdateUIState();
            };
        }

        /// <summary>
        /// Aktualizuje stan interfejsu użytkownika na podstawie aktualnych ustawień.
        /// Oblicza koszt materiałów, sprawdza dostępne zasoby i aktualizuje podgląd komponentów.
        /// </summary>
        private void UpdateUIState()
        {
            // Update material cost display
            int requiredPowder = _tier * 2; // 2 powder per tier
            var powderName = _colorToPowderMap.ContainsKey(_color) ? _colorToPowderMap[_color] : "RedPowder";
            var player = PlayerHandler.player;
            int playerPowder = player.Inventory.GetCount(powderName);
            
            // Update status text
            StatusText.Text = $"Crafting a {_color} {(_isArmorGaldurite ? "Armor" : "Weapon")} Galdurite (Tier {_tier})";
            
            // Update material cost display
            MaterialCostText.Text = $"Required: {requiredPowder} {_color} Powder (Have: {playerPowder})";
            
            // Enable/disable craft button based on materials
            CraftButton.IsEnabled = playerPowder >= requiredPowder;
            
            // Update preview of components that would be generated
            UpdateComponentPreview();
        }
        
        /// <summary>
        /// Aktualizuje podgląd dostępnych komponentów dla aktualnych ustawień.
        /// Pobiera komponenty odpowiednie dla wybranego typu i koloru galduritu.
        /// </summary>
        private void UpdateComponentPreview()
        {
            _selectedComponents.Clear();
            var components = new List<GalduriteComponent>();
            for (var i = 0; i < 5; i++)
            {
                components.AddRange(GalduriteManager.GetComponentsForTier(i + 1, _isArmorGaldurite).Where(x => x.PoolColor == _color));
            }
            foreach (var component in components.OrderBy(x => x.EffectType).ThenBy(x =>x.EffectStrength))
            {
                _selectedComponents.Add(component);
            }
        }

        /// <summary>
        /// Sprawdza, czy gracz posiada wystarczającą ilość materiałów do wytworzenia galduritu.
        /// </summary>
        /// <returns>True, jeśli gracz posiada wystarczającą ilość materiałów; w przeciwnym razie false.</returns>
        private bool CanCraftGaldurite()
        {
            var powderName = _colorToPowderMap.ContainsKey(_color) ? _colorToPowderMap[_color] : "RedPowder";
            int requiredPowder = _tier * 2;
            int playerPowder = PlayerHandler.player.Inventory.GetCount(powderName);
            
            return playerPowder >= requiredPowder;
        }
        
        /// <summary>
        /// Próbuje zużyć wymagane materiały z ekwipunku gracza.
        /// </summary>
        /// <returns>True, jeśli udało się zużyć materiały; w przeciwnym razie false.</returns>
        private bool TryConsumeMaterials()
        {
            var powderName = _colorToPowderMap.ContainsKey(_color) ? _colorToPowderMap[_color] : "RedPowder";
            int requiredPowder = _tier * 2;
            var player = PlayerHandler.player;
            
            if (player.Inventory.GetCount(powderName) >= requiredPowder)
            {
                player.Inventory.TryRemoveItem(ItemManager.GetItem(powderName), requiredPowder);
                return true;
            }
            
            return false;
        }

        #region Event Handlers
        
        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku tworzenia galduritu.
        /// Sprawdza dostępność materiałów, zużywa je i tworzy nowy galdurit.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void CraftButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CanCraftGaldurite())
            {
                MessageBox.Show("Not enough materials to craft this galdurite.", "Cannot Craft", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            // Consume materials
            if (!TryConsumeMaterials())
            {
                MessageBox.Show("Failed to consume materials. Please try again.", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            // Create the galdurite with random components based on tier and color
            var galdurite = new Galdurite(_isArmorGaldurite, _tier, 0, _color);
            
            // Add to player's inventory
            PlayerHandler.player.Inventory.AddItem(galdurite);
            
            // Show success message
            MessageBox.Show($"Successfully crafted a {_color} {(_isArmorGaldurite ? "Armor" : "Weapon")} Galdurite!", 
                "Crafting Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Update UI to reflect material usage
            UpdateUIState();
        }
        
        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku anulowania.
        /// Zamyka okno dialogowe bez tworzenia galduritu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #endregion
    }
}
