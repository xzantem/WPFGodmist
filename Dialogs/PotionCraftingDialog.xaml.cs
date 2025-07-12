using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Items;
using GodmistWPF.Items.Potions;
using GodmistWPF.Utilities;
using GodmistWPF.Items;

namespace GodmistWPF.Dialogs
{
    /// <summary>
    /// Okno dialogowe umożliwiające tworzenie mikstur z dostępnych komponentów i katalizatorów.
    /// Obsługuje wybór składników, weryfikację dostępności materiałów oraz proces tworzenia mikstur.
    /// </summary>
    public partial class PotionCraftingDialog : Window, INotifyPropertyChanged
    {
        /// <summary>Dostępne komponenty do tworzenia mikstur.</summary>
        private ObservableCollection<PotionComponentViewModel> _availableComponents = new();
        
        /// <summary>Wybrane komponenty do aktualnie tworzonej mikstury.</summary>
        private ObservableCollection<PotionComponentViewModel> _selectedComponents = new();
        
        /// <summary>Dostępne katalizatory wpływające na właściwości mikstury.</summary>
        private ObservableCollection<PotionCatalystViewModel> _availableCatalysts = new();

        /// <summary>
        /// Zdarzenie wywoływane przy zmianie wartości właściwości.
        /// Wymagane przez interfejs INotifyPropertyChanged.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// Wywołuje zdarzenie PropertyChanged.
        /// </summary>
        /// <param name="propertyName">Nazwa zmienionej właściwości.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="PotionCraftingDialog">.
        /// Konfiguruje interfejs użytkownika i ładuje dostępne składniki.
        /// </summary>
        public PotionCraftingDialog()
        {
            InitializeComponent();
            DataContext = this;
            
            // Initialize components and catalysts
            InitializePotionComponents();
            InitializeCatalysts();
            
            // Set up data binding
            var availableComponentsView = CollectionViewSource.GetDefaultView(_availableComponents);
            availableComponentsView.Filter = o => 
            {
                var vm = (PotionComponentViewModel)o;
                return !_selectedComponents.Any(sc => sc.Component.Effect == vm.Component.Effect && 
                                                   sc.Component.StrengthTier == vm.Component.StrengthTier);
            };
            
            AvailableComponentsComboBox.ItemsSource = availableComponentsView;
            SelectedComponentsList.ItemsSource = _selectedComponents;
            CatalystComboBox.ItemsSource = _availableCatalysts;
            
            // Update UI state
            UpdatePotionCraftingUI();
        }
        
        /// <summary>
        /// Inicjalizuje listę dostępnych komponentów do tworzenia mikstur.
        /// Sprawdza dostępność materiałów w ekwipunku gracza.
        /// </summary>
        private void InitializePotionComponents()
        {
            _availableComponents.Clear();
            _selectedComponents.Clear();
            
            // Load available components
            foreach (var component in PotionManager.PotionComponents)
            {
                bool hasMaterials = PlayerHandler.player.Inventory.GetCount(component.Material) > 0;
                _availableComponents.Add(new PotionComponentViewModel(component, hasMaterials));
            }
        }
        
        /// <summary>
        /// Inicjalizuje listę dostępnych katalizatorów do tworzenia mikstur.
        /// Uwzględnia opcję braku katalizatora ("None") jako pierwszą pozycję.
        /// </summary>
        private void InitializeCatalysts()
        {
            _availableCatalysts.Clear();
            
            // Add "None" option as the first item
            _availableCatalysts.Add(new PotionCatalystViewModel(null, true));
            
            // Load available catalysts
            foreach (PotionCatalystEffect effect in Enum.GetValues(typeof(PotionCatalystEffect)))
            {
                for (int tier = 1; tier <= 5; tier++)
                {
                    var catalyst = new PotionCatalyst(effect, tier);
                    bool hasMaterial = PlayerHandler.player.Inventory.GetCount(catalyst.Material) > 0;
                    _availableCatalysts.Add(new PotionCatalystViewModel(catalyst, hasMaterial));
                }
            }
        }
        
        /// <summary>
        /// Aktualizuje interfejs użytkownika w zależności od aktualnego stanu.
        /// Sprawdza dostępność składników, aktualizuje liczniki i stan przycisków.
        /// </summary>
        private void UpdatePotionCraftingUI()
        {
            // Update components availability
            foreach (var component in _availableComponents)
            {
                component.UpdateAvailability();
            }
            
            // Update catalysts availability
            foreach (var catalyst in _availableCatalysts)
            {
                catalyst.UpdateAvailability();
            }
            
            // Update selected components count
            var selectedCount = _selectedComponents.Count;
            SelectedComponentsText.Text = $"Selected Components ({selectedCount}/3):";
            
            // Update craft button state
            bool canCraft = selectedCount > 0 && selectedCount <= 3 && 
                          !string.IsNullOrWhiteSpace(PotionNameTextBox.Text);
            CraftButton.IsEnabled = canCraft;
            
            // Update remove button state
            RemoveComponentButton.IsEnabled = SelectedComponentsList.SelectedItem != null;
            
            // Refresh the available components view
            CollectionViewSource.GetDefaultView(AvailableComponentsComboBox.ItemsSource).Refresh();
        }
        
        /// <summary>
        /// Obsługuje zmianę wyboru w liście dostępnych komponentów.
        /// Dodaje wybrany komponent do listy wybranych składników, jeśli jest to możliwe.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (ComboBox).</param>
        /// <param name="e">Dane zdarzenia zmiany wyboru.</param>
        private void AvailableComponentsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AvailableComponentsComboBox.SelectedItem is PotionComponentViewModel selectedComponent && 
                _selectedComponents.Count < 3)
            {
                // Add the selected component to the selected components list
                _selectedComponents.Add(selectedComponent);
                
                // Update UI
                UpdatePotionCraftingUI();
                
                // Clear the selection
                AvailableComponentsComboBox.SelectedIndex = -1;
            }
        }
        
        /// <summary>
        /// Obsługuje kliknięcie przycisku usuwania wybranego komponentu.
        /// Usuwa zaznaczony składnik z listy wybranych komponentów.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void RemoveComponentButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedComponentsList.SelectedItem is PotionComponentViewModel selectedComponent)
            {
                _selectedComponents.Remove(selectedComponent);
                UpdatePotionCraftingUI();
            }
        }
        
        /// <summary>
        /// Obsługuje zmianę wyboru na liście wybranych komponentów.
        /// Aktualizuje stan przycisku usuwania składnika.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (ListBox).</param>
        /// <param name="e">Dane zdarzenia zmiany wyboru.</param>
        private void SelectedComponentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveComponentButton.IsEnabled = SelectedComponentsList.SelectedItem != null;
        }
        
        /// <summary>
        /// Obsługuje zmianę wyboru katalizatora.
        /// Aktualizuje interfejs użytkownika po zmianie wyboru.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (ComboBox).</param>
        /// <param name="e">Dane zdarzenia zmiany wyboru.</param>
        private void CatalystComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update UI if needed when catalyst changes
            UpdatePotionCraftingUI();
        }
        
        /// <summary>
        /// Obsługuje zmianę tekstu w polu nazwy mikstury.
        /// Aktualizuje interfejs użytkownika po zmianie tekstu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (TextBox).</param>
        /// <param name="e">Dane zdarzenia zmiany tekstu.</param>
        private void PotionNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePotionCraftingUI();
        }
        
        /// <summary>
        /// Obsługuje kliknięcie przycisku tworzenia mikstury.
        /// Weryfikuje dane, sprawdza dostępność materiałów i tworzy miksturę.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void CraftButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate selection
                if (_selectedComponents.Count == 0 || _selectedComponents.Count > 3)
                {
                    StatusText.Text = "You must select between 1 and 3 components.";
                    return;
                }
                
                string potionName = PotionNameTextBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(potionName))
                {
                    StatusText.Text = "Please enter a name for your potion.";
                    return;
                }
                
                // Get selected components and catalyst
                var components = _selectedComponents.Select(vm => vm.Component).ToList();
                var catalystVM = CatalystComboBox.SelectedItem as PotionCatalystViewModel;
                var catalyst = catalystVM?.Catalyst; // Will be null for "None" option
                
                // Check if player has all required materials
                foreach (var component in components)
                {
                    if (PlayerHandler.player.Inventory.GetCount(component.Material) == 0)
                    {
                        StatusText.Text = $"You don't have the required component: {ItemManager.GetItem(component.Material).Name}";
                        return;
                    }
                }
                
                // Only check catalyst materials if it's not the "None" option
                if (catalyst != null && !catalystVM.IsNone && PlayerHandler.player.Inventory.GetCount(catalyst.Material) == 0)
                {
                    StatusText.Text = $"You don't have the required catalyst: {ItemManager.GetItem(catalyst.Material).Name}";
                    return;
                }
                
                // Craft the potion (pass null as catalyst if "None" is selected)
                var potion = CraftingManager.CraftPotion(components, catalyst, potionName);
                // Add the crafted potion to inventory
                PlayerHandler.player.Inventory.AddItem(potion);
                
                // Reset the form
                _selectedComponents.Clear();
                PotionNameTextBox.Text = string.Empty;
                UpdatePotionCraftingUI();
                
                StatusText.Text = $"Successfully crafted {potion.Name}!";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error crafting potion: {ex.Message}";
                Debug.WriteLine($"Error in CraftButton_Click: {ex}");
            }
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
    
    /// <summary>
    /// Klasa pomocnicza reprezentująca komponent mikstury.
    /// </summary>
    public class PotionComponentViewModel : INotifyPropertyChanged
    {
        /// <summary>Składnik mikstury.</summary>
        public PotionComponent Component { get; }
        
        /// <summary>Opis efektu składnika.</summary>
        public string EffectDescription { get; }
        
        /// <summary>Czy gracz posiada wymagane materiały.</summary>
        public bool HasRequiredMaterials { get; private set; }
        
        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="PotionComponentViewModel"/>.
        /// </summary>
        /// <param name="component">Składnik mikstury.</param>
        /// <param name="hasMaterials">Czy gracz posiada wymagane materiały.</param>
        public PotionComponentViewModel(PotionComponent component, bool hasMaterials)
        {
            Component = component;
            EffectDescription = component.EffectDescription(null, true);
            HasRequiredMaterials = hasMaterials;
        }
        
        /// <summary>
        /// Aktualizuje dostępność składnika.
        /// </summary>
        public void UpdateAvailability()
        {
            var hasMaterials = PlayerHandler.player.Inventory.GetCount(Component.Material) > 0;
            if (hasMaterials != HasRequiredMaterials)
            {
                HasRequiredMaterials = hasMaterials;
                OnPropertyChanged(nameof(HasRequiredMaterials));
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    public class PotionCatalystViewModel : INotifyPropertyChanged
    {
        public PotionCatalyst Catalyst { get; }
        public string DescriptionText { get; }
        public bool HasRequiredMaterial { get; private set; }
        public bool IsNone => Catalyst == null;
        
        public PotionCatalystViewModel(PotionCatalyst catalyst, bool hasMaterial)
        {
            Catalyst = catalyst;
            DescriptionText = catalyst?.DescriptionText() ?? "None";
            HasRequiredMaterial = hasMaterial;
        }
        
        public void UpdateAvailability()
        {
            if (Catalyst == null) return;
            var hasMaterial = PlayerHandler.player.Inventory.GetCount(Catalyst.Material) > 0;
            if (hasMaterial != HasRequiredMaterial)
            {
                HasRequiredMaterial = hasMaterial;
                OnPropertyChanged(nameof(HasRequiredMaterial));
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
