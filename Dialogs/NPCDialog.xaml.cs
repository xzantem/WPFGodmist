using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GodmistWPF.Characters.Player;
using GodmistWPF.Dialogs;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Items;
using GodmistWPF.Items;
using GodmistWPF.Items.Equippable;
using GodmistWPF.Items.Equippable.Armors;
using GodmistWPF.Items.Equippable.Weapons;
using GodmistWPF.Quests;
using GodmistWPF.Towns.NPCs;
using GodmistWPF.Utilities;
using GodmistWPF.Items.Potions;
using GodmistWPF.Enums.Items;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Globalization;
using GodmistWPF.Items.Galdurites;
using IItem = GodmistWPF.Items.IItem;
using ICraftable = GodmistWPF.Items.ICraftable;

namespace GodmistWPF.Dialogs
{
    /// <summary>
    /// Okno dialogowe interakcji z NPC w grze.
    /// Umożliwia handel, craftowanie przedmiotów, akceptowanie zadań i inne interakcje z postaciami niezależnymi.
    /// </summary>
    public partial class NPCDialog : Window, INotifyPropertyChanged
    {
        /// <summary>NPC, z którym prowadzona jest interakcja.</summary>
        private readonly NPC _npc;
        
        /// <summary>Postać gracza prowadząca interakcję.</summary>
        private readonly PlayerCharacter _player;
        
        /// <summary>Kolekcja przedmiotów dostępnych w sklepie NPC.</summary>
        private ObservableCollection<ShopItemViewModel> _shopItems;
        
        /// <summary>Kolekcja przepisów rzemieślniczych dostępnych u NPC.</summary>
        private ObservableCollection<CraftingRecipeViewModel> _craftableItems;
        
        /// <summary>Flaga wskazująca, czy trwa aktualizacja wyboru przedmiotu (zapobiega cyklicznym aktualizacjom).</summary>
        private bool _isUpdatingSelection = false;
        
        /// <summary>Aktualnie wybrany przedmiot w interfejsie.</summary>
        private IItem _selectedItem = null;
        
        /// <summary>Cena aktualnie wybranego przedmiotu.</summary>
        private int _selectedItemPrice = 0;
        
        /// <summary>Maksymalna ilość przedmiotu, jaką można kupić/wyprodukować.</summary>
        private int _selectedItemMaxQuantity = 1;
        
        /// <summary>Flaga wskazująca, czy wybrano przedmiot ze sklepu (w przeciwieństwie do przedmiotu do wytworzenia).</summary>
        private bool _isShopItemSelected = false;
        
        /// <summary>Aktualnie wybrane zadanie w interfejsie.</summary>
        private Quest _selectedQuest = null;

        /// <summary>
        /// Występuje, gdy zmienia się wartość właściwości powiązanej z interfejsem użytkownika.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// Wywołuje zdarzenie PropertyChanged dla określonej właściwości.
        /// </summary>
        /// <param name="propertyName">Nazwa właściwości, która uległa zmianie.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="NPCDialog"/> dla określonego NPC.
        /// </summary>
        /// <param name="npc">Postać niezależna, z którą prowadzona jest interakcja.</param>
        /// <exception cref="ArgumentNullException">Gdy parametr npc jest null.</exception>
        /// <exception cref="InvalidOperationException">Gdy gracz nie został poprawnie zainicjalizowany.</exception>
        public NPCDialog(NPC npc)
        {
            InitializeComponent();
            
            _npc = npc ?? throw new ArgumentNullException(nameof(npc));
            _player = PlayerHandler.player ?? throw new InvalidOperationException("Player is not initialized");
            
            // Subscribe to property changes
            if (_player is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged += Player_PropertyChanged;
            }
            
            // Handle window closed event for cleanup
            this.Closed += NPCDialog_Closed;
            
            InitializeUI();
            InitializeShopTab();
            InitializeCraftingTab();
            InitializeQuestsTab();
            SetupNPCServices();
        }
        
        /// <summary>
        /// Klasa pomocnicza reprezentująca widok modelu komponentu mikstury w interfejsie użytkownika.
        /// Przechowuje informacje o komponencie i jego dostępności.
        /// </summary>
        private class PotionComponentViewModel : INotifyPropertyChanged
        {
            /// <summary>Pobiera obiekt komponentu mikstury.</summary>
            public PotionComponent Component { get; }
            
            /// <summary>Pobiera opis efektu komponentu.</summary>
            public string EffectDescription { get; }
            
            /// <summary>Pobiera informację, czy gracz posiada wymagane materiały do użycia tego komponentu.</summary>
            public bool HasRequiredMaterials { get; private set; }
            
            /// <summary>
            /// Inicjalizuje nową instancję klasy <see cref="PotionComponentViewModel"/>.
            /// </summary>
            /// <param name="component">Komponent mikstury do wyświetlenia.</param>
            /// <param name="hasMaterials">Czy gracz posiada wymagane materiały.</param>
            public PotionComponentViewModel(PotionComponent component, bool hasMaterials)
            {
                Component = component;
                EffectDescription = component.EffectDescription(null, false);
                HasRequiredMaterials = hasMaterials;
            }
            
            /// <summary>
            /// Aktualizuje stan dostępności komponentu na podstawie inwentarza gracza.
            /// Sprawdza, czy gracz posiada wymagane materiały.
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
            
            /// <summary>Występuje, gdy zmienia się wartość właściwości.</summary>
            public event PropertyChangedEventHandler PropertyChanged;
            
            /// <summary>
            /// Wywołuje zdarzenie PropertyChanged dla określonej właściwości.
            /// </summary>
            /// <param name="propertyName">Nazwa właściwości, która uległa zmianie.</param>
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        /// <summary>
        /// Klasa pomocnicza reprezentująca widok modelu katalizatora mikstury w interfejsie użytkownika.
        /// Przechowuje informacje o katalizatorze i jego dostępności.
        /// </summary>
        private class PotionCatalystViewModel : INotifyPropertyChanged
        {
            /// <summary>Pobiera obiekt katalizatora mikstury.</summary>
            public PotionCatalyst Catalyst { get; }
            
            /// <summary>Pobiera opis tekstowy katalizatora.</summary>
            public string DescriptionText { get; }
            
            /// <summary>Pobiera informację, czy gracz posiada wymagany materiał do użycia tego katalizatora.</summary>
            public bool HasRequiredMaterial { get; private set; }
            
            /// <summary>
            /// Inicjalizuje nową instancję klasy <see cref="PotionCatalystViewModel"/>.
            /// </summary>
            /// <param name="catalyst">Katalizator mikstury do wyświetlenia.</param>
            /// <param name="hasMaterial">Czy gracz posiada wymagany materiał.</param>
            public PotionCatalystViewModel(PotionCatalyst catalyst, bool hasMaterial)
            {
                Catalyst = catalyst;
                DescriptionText = catalyst.DescriptionText();
                HasRequiredMaterial = hasMaterial;
            }
            
            /// <summary>
            /// Aktualizuje stan dostępności katalizatora na podstawie inwentarza gracza.
            /// Sprawdza, czy gracz posiada wymagany materiał.
            /// </summary>
            public void UpdateAvailability()
            {
                var hasMaterial = PlayerHandler.player.Inventory.GetCount(Catalyst.Material) > 0;
                if (hasMaterial != HasRequiredMaterial)
                {
                    HasRequiredMaterial = hasMaterial;
                    OnPropertyChanged(nameof(HasRequiredMaterial));
                }
            }
            
            /// <summary>Występuje, gdy zmienia się wartość właściwości.</summary>
            public event PropertyChangedEventHandler PropertyChanged;
            
            /// <summary>
            /// Wywołuje zdarzenie PropertyChanged dla określonej właściwości.
            /// </summary>
            /// <param name="propertyName">Nazwa właściwości, która uległa zmianie.</param>
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        // Kolekcje używane w procesie tworzenia mikstur
        
        /// <summary>Dostępne komponenty mikstur, które można dodać do receptury.</summary>
        private ObservableCollection<PotionComponentViewModel> _availableComponents = new();
        
        /// <summary>Komponenty wybrane do aktualnie tworzonej mikstury.</summary>
        private ObservableCollection<PotionComponentViewModel> _selectedComponents = new();
        
        /// <summary>Dostępne katalizatory, które można dodać do mikstury.</summary>
        private ObservableCollection<PotionCatalystViewModel> _availableCatalysts = new();
        
        /// <summary>
        /// Obsługuje zdarzenie zamknięcia okna dialogowego.
        /// Zwalnia zasoby i wykonuje niezbędne czynności porządkujące.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void NPCDialog_Closed(object sender, EventArgs e)
        {
            // Unsubscribe from property changes when window is closed
            if (_player is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged -= Player_PropertyChanged;
            }
        }

        /// <summary>
        /// Inicjalizuje podstawowe elementy interfejsu użytkownika okna dialogowego.
        /// Ustawia tytuł okna, inicjalizuje kolekcje danych i konfiguruje widżety.
        /// </summary>
        private void InitializeUI()
        {
            // Set NPC info
            NPCNameText.Text = _npc.Name;
            NPCDescriptionText.Text = GetNPCDescription(_npc);
            UpdateLoyaltyDisplay();
            UpdatePlayerGold();
            
            // Initialize collections
            _shopItems = new ObservableCollection<ShopItemViewModel>();
            ItemsListBox.ItemsSource = _shopItems;
            
            _craftableItems = new ObservableCollection<CraftingRecipeViewModel>();
            CraftingListBox.ItemsSource = _craftableItems;
            
            // Initialize potion crafting UI
            InitializePotionCraftingUI();
        }
        
        /// <summary>
        /// Inicjalizuje interfejs użytkownika do tworzenia mikstur.
        /// Ładuje dostępne komponenty i katalizatory, sprawdza ich dostępność w ekwipunku gracza
        /// i konfiguruje widok listy dostępnych elementów.
        /// </summary>
        private void InitializePotionCraftingUI()
        {
            // Initialize components collection
            _availableComponents.Clear();
            _selectedComponents.Clear();
            _availableCatalysts.Clear();
            
            // Load available components
            foreach (var component in PotionManager.PotionComponents)
            {
                bool hasMaterials = PlayerHandler.player.Inventory.GetCount(component.Material) > 0;
                _availableComponents.Add(new PotionComponentViewModel(component, hasMaterials));
            }
            
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
        /// Aktualizuje interfejs użytkownika tworzenia mikstur.
        /// Sprawdza dostępność komponentów i katalizatorów, aktualizuje liczniki
        /// i stan przycisków w zależności od dostępnych zasobów i dokonanych wyborów.
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
            var componentsText = $"Selected Components ({selectedCount}/3):";
            var componentsTextBlock = (TextBlock)FindName("SelectedComponentsText");
            if (componentsTextBlock != null)
            {
                componentsTextBlock.Text = componentsText;
            }
            
            // Update craft button state
            bool canCraft = selectedCount > 0 && selectedCount <= 3 && 
                          !string.IsNullOrWhiteSpace(PotionNameTextBox.Text);
            CraftPotionButton.IsEnabled = canCraft;
            
            // Update remove button state
            RemoveComponentButton.IsEnabled = SelectedComponentsList.SelectedItem != null;
            
            // Refresh the available components view
            CollectionViewSource.GetDefaultView(AvailableComponentsComboBox.ItemsSource).Refresh();
        }
        
        /// <summary>
        /// Inicjalizuje zakładkę rzemiosła w oknie dialogowym NPC.
        /// Ładuje dostępne przepisy rzemieślnicze i konfiguruje interfejs użytkownika.
        /// </summary>
        private void InitializeCraftingTab()
        {
            try
            {
                // Clear existing items
                _craftableItems.Clear();
                
                // Add craftable items based on NPC type
                if (_npc.CraftableItems != null && _npc.CraftableItems.Count > 0)
                {
                    foreach (var craftable in _npc.CraftableItems)
                    {
                        if (craftable != null)
                        {
                            _craftableItems.Add(new CraftingRecipeViewModel
                            {
                                Item = craftable,
                                Name = craftable.Name,
                                CanCraft = CanCraftItem(craftable)
                            });
                        }
                    }
                }
                
                // Select first item if available
                if (_craftableItems.Count > 0)
                {
                    CraftingListBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error initializing crafting tab: {ex.Message}";
            }
        }

        /// <summary>
        /// Pobiera opis NPC na podstawie jego typu.
        /// Dla znanych typów NPC (Alchemik, Kowal, Zaklinacz) zwraca odpowiedni opis,
        /// w przeciwnym razie zwraca ogólny opis.
        /// </summary>
        /// <param name="npc">Postać NPC, dla której ma zostać pobrany opis.</param>
        /// <returns>Opis postaci NPC odpowiedni dla jej typu.</returns>
        private string GetNPCDescription(NPC npc)
        {
            return npc switch
            {
                Alchemist _ => "A master of potions and alchemical concoctions. Can create powerful elixirs and refill your potions.",
                Blacksmith _ => "A skilled blacksmith who can forge and upgrade weapons and armor.",
                Enchanter _ => "A mysterious enchanter who can imbue items with magical properties.",
                _ => "A friendly NPC offering various services."
            };
        }

        /// <summary>
        /// Generuje szczegółowy opis przedmiotu na podstawie jego typu i właściwości.
        /// Uwzględnia specyficzne atrybuty dla różnych typów przedmiotów (broń, zbroja itp.).
        /// </summary>
        /// <param name="item">Przedmiot, dla którego ma zostać wygenerowany opis.</param>
        /// <returns>Sformatowany ciąg znaków zawierający opis przedmiotu.</returns>
        private string GetItemDescription(IItem item)
        {
            if (item == null)
                return string.Empty;
                
            var description = new List<string>();
            
            // Add item type and description
            description.Add($"Type: {item.ItemType}");
            if (!string.IsNullOrEmpty(item.Description))
                description.Add($"\n{item.Description}");
            
            // Add specific item properties based on type
            if (item is IEquippable equippable)
            {
                description.Add("\nEquippable");
                description.Add($"Required Level: {equippable.RequiredLevel}");
                description.Add($"Rarity: {equippable.Rarity}");
                
                if (equippable is Weapon weapon)
                {
                    description.Add($"\nDamage: {weapon.MinimalAttack}-{weapon.MaximalAttack}");
                    description.Add($"Crit Chance: {weapon.CritChance:P1} | Crit Multiplier: {weapon.CritMod:F1}x");
                    description.Add($"Accuracy: {weapon.Accuracy}");
                }
                else if (equippable is Armor armor)
                {
                    description.Add($"\nDefense: {armor.PhysicalDefense} (Physical) | {armor.MagicDefense} (Magic)");
                    description.Add($"Dodge: {armor.Dodge:P1} | Health: {armor.MaximalHealth}");
                }
            }
            
            // Add crafting info
            if (item is ICraftable craftable)
            {
                description.Add("\nCrafting Information:");
                description.Add($"Crafted Amount: {craftable.CraftedAmount}");
            }
            
            return string.Join("\n", description);
        }

        /// <summary>
        /// Aktualizuje wyświetlany poziom lojalności NPC.
        /// Pokazuje aktualny poziom lojalności oraz postęp do następnego poziomu.
        /// </summary>
        private void UpdateLoyaltyDisplay()
        {
            LoyaltyText.Text = $" {_npc.LoyaltyLevel} ({_npc.GoldSpent}/{_npc.RequiredGoldSpent})";
        }

        /// <summary>
        /// Aktualizuje wyświetlaną ilość złota gracza w interfejsie.
        /// Automatycznie aktualizuje również interfejs tworzenia mikstur, gdy zakładka usług jest aktywna.
        /// </summary>
        private void UpdatePlayerGold()
        {
            PlayerGoldText.Text = PlayerHandler.player.Gold.ToString("N0");
            
            // Also update potion crafting UI when gold changes
            if (NPCTabControl.SelectedItem == ServicesTab)
            {
                UpdatePotionCraftingUI();
            }
        }
        
        /// <summary>
        /// Obsługuje zmiany właściwości obiektu gracza.
        /// Aktualizuje interfejs użytkownika w odpowiedzi na zmiany stanu gracza.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (obiekt gracza).</param>
        /// <param name="e">Dane zdarzenia zawierające informacje o zmienionej właściwości.</param>
        private void Player_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PlayerCharacter.Gold))
            {
                Dispatcher.Invoke(UpdatePlayerGold);
            }
            else if (e.PropertyName == nameof(PlayerCharacter.Inventory))
            {
                // Update potion crafting UI when inventory changes
                Dispatcher.Invoke(UpdatePotionCraftingUI);
            }
        }

        /// <summary>
        /// Inicjalizuje zakładkę zadań w oknie dialogowym NPC.
        /// Ładuje dostępne zadania, ich statusy i konfiguruje interfejs użytkownika.
        /// </summary>
        private void InitializeQuestsTab()
        {
            try
            {
                // Get available quests from this NPC
                var availableQuests = QuestManager.Quests
                    .Where(q => q.QuestState == QuestState.Available && q.QuestGiver == _npc.Alias)
                    .ToList();

                // Get completed quests that can be handed in to this NPC
                var completedQuests = QuestManager.Quests
                    .Where(q => q.QuestState == QuestState.Completed && q.QuestEnder == _npc.Alias)
                    .ToList();

                // Set the quests lists as the ItemsSource
                AvailableQuestsListBox.ItemsSource = availableQuests;
                CompletedQuestsListBox.ItemsSource = completedQuests;

                // Show/hide the no quests message
                if (availableQuests.Count == 0 && completedQuests.Count == 0)
                {
                    NoQuestSelectedPanel.Visibility = Visibility.Visible;
                    QuestDetailsPanel.Visibility = Visibility.Collapsed;
                    var noQuestsText = new TextBlock
                    {
                        Text = "No quests available from this NPC.",
                        Style = (Style)FindResource("MaterialDesignBody1TextBlock"),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap
                    };
                    NoQuestSelectedPanel.Children.Clear();
                    NoQuestSelectedPanel.Children.Add(noQuestsText);
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error initializing quests tab: {ex.Message}";
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie wyboru zadania z listy dostępnych zadań.
        /// Aktualizuje panel szczegółów zadania i pokazuje przycisk akceptacji zadania.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (ListBox).</param>
        /// <param name="e">Dane zdarzenia zmiany zaznaczenia.</param>
        private void OnQuestSelected(object sender, SelectionChangedEventArgs e)
        {
            if (AvailableQuestsListBox.SelectedItem is Quest selectedQuest)
            {
                // Clear selection in completed quests list
                CompletedQuestsListBox.SelectedItem = null;
                _selectedQuest = selectedQuest;
                UpdateQuestDetails(selectedQuest);
                
                // Show accept button and hide hand in button
                if (AcceptQuestButton != null) AcceptQuestButton.Visibility = Visibility.Visible;
                if (HandInQuestButton != null) HandInQuestButton.Visibility = Visibility.Collapsed;
            }
        }
        
        /// <summary>
        /// Obsługuje zdarzenie wyboru ukończonego zadania z listy.
        /// Aktualizuje panel szczegółów zadania i pokazuje przycisk oddania zadania.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (ListBox).</param>
        /// <param name="e">Dane zdarzenia zmiany zaznaczenia.</param>
        private void OnCompletedQuestSelected(object sender, SelectionChangedEventArgs e)
        {
            if (CompletedQuestsListBox.SelectedItem is Quest selectedQuest)
            {
                // Clear selection in available quests list
                AvailableQuestsListBox.SelectedItem = null;
                _selectedQuest = selectedQuest;
                UpdateQuestDetails(selectedQuest);
                
                // Show hand in button and hide accept button
                if (AcceptQuestButton != null) AcceptQuestButton.Visibility = Visibility.Collapsed;
                if (HandInQuestButton != null) HandInQuestButton.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Aktualizuje panel szczegółów zadania na podstawie przekazanego obiektu zadania.
        /// Wyświetla tytuł, opis, cele i nagrody zadania w odpowiednich kontrolkach interfejsu użytkownika.
        /// </summary>
        /// <param name="quest">Obiekt zadania, którego szczegóły mają zostać wyświetlone.</param>
        private void UpdateQuestDetails(Quest quest)
        {
            if (quest == null)
            {
                NoQuestSelectedPanel.Visibility = Visibility.Visible;
                QuestDetailsPanel.Visibility = Visibility.Collapsed;
                return;
            }

            // Update UI elements with quest details
            QuestTitleText.Text = quest.Name;
            QuestLevelText.Text = $"(Level {quest.RecommendedLevel})";
            QuestDescriptionText.Text = quest.Description;
            
            // Build objectives text
            var objectivesText = new System.Text.StringBuilder();
            objectivesText.AppendLine("Objectives:");
            
            bool allObjectivesComplete = true;
            
            foreach (var stage in quest.Stages)
            {
                foreach (var objective in stage.Objectives)
                {
                    string status = objective.IsComplete ? "[✓] " : "[ ] ";
                    objectivesText.AppendLine($"{status}{objective.Description}");
                    
                    if (!objective.IsComplete)
                        allObjectivesComplete = false;
                }
            }
            
            QuestObjectivesText.Text = objectivesText.ToString();
            
            // Build rewards text
            var rewardsText = new System.Text.StringBuilder();
            rewardsText.AppendLine("Rewards:");
            
            if (quest.QuestReward.Gold > 0)
                rewardsText.AppendLine($"- {quest.QuestReward.Gold} Gold");
                
            if (quest.QuestReward.Experience > 0)
                rewardsText.AppendLine($"- {quest.QuestReward.Experience} XP");
                
            if (quest.QuestReward.Honor > 0)
                rewardsText.AppendLine($"- {quest.QuestReward.Honor} Honor");
                
            if (quest.QuestReward.Items.Count > 0)
            {
                foreach (var item in quest.QuestReward.Items)
                {
                    rewardsText.AppendLine($"- {item.Value}x {item.Key}");
                }
            }
            
            QuestRewardsText.Text = rewardsText.ToString();
            
            // Update quest ender text for completed quests
            if (quest.QuestState == QuestState.Completed && !string.IsNullOrEmpty(quest.QuestEnder))
            {
                if (QuestEnderText != null)
                {
                    QuestEnderText.Text = $"Return to {quest.QuestEnder} to complete this quest.";
                    QuestEnderText.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (QuestEnderText != null)
                    QuestEnderText.Visibility = Visibility.Collapsed;
            }
            
            // Update button states
            if (AcceptQuestButton != null)
                AcceptQuestButton.IsEnabled = quest.QuestState == QuestState.Available;
                
            if (HandInQuestButton != null)
                HandInQuestButton.IsEnabled = quest.QuestState == QuestState.Completed && allObjectivesComplete;
            
            // Show details panel
            NoQuestSelectedPanel.Visibility = Visibility.Collapsed;
            QuestDetailsPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku akceptacji zadania.
        /// Akceptuje wybrane zadanie, aktualizuje interfejs użytkownika i wyświetla komunikat o powodzeniu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk akceptacji zadania).</param>
        /// <param name="e">Dane zdarzenia kliknięcia.</param>
        private void OnAcceptQuestClick(object sender, RoutedEventArgs e)
        {
            if (_selectedQuest == null || _selectedQuest.QuestState != QuestState.Available) 
                return;
            
            try
            {
                // Accept the quest
                _selectedQuest.AcceptQuest();
                
                // Show success message
                StatusText.Text = $"Quest accepted: {_selectedQuest.Name}";
                
                // Refresh the quests list
                InitializeQuestsTab();
                
                // Clear selection
                AvailableQuestsListBox.SelectedItem = null;
                _selectedQuest = null;
                NoQuestSelectedPanel.Visibility = Visibility.Visible;
                QuestDetailsPanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error accepting quest: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Wyświetla panel z nagrodami otrzymanymi za wykonanie zadania.
        /// Tworzy wizualną reprezentację każdej nagrody (złoto, doświadczenie, honor, przedmioty).
        /// </summary>
        /// <param name="gold">Ilość otrzymanego złota.</param>
        /// <param name="exp">Ilość otrzymanego doświadczenia.</param>
        /// <param name="honor">Ilość otrzymanego honoru.</param>
        /// <param name="items">Lista par przedmiot-ilość otrzymanych przedmiotów.</param>
        public void ShowRewards(int gold, int exp, int honor, List<KeyValuePair<IItem, int>> items)
        {
            RewardsList.Children.Clear();
            if (gold > 0)
            {
                var goldPanel = CreateRewardItem("Gold", gold.ToString("N0"), "Gold");
                RewardsList.Children.Add(goldPanel);
            }
            if (exp > 0)
            {
                var expPanel = CreateRewardItem("Experience", exp.ToString("N0"), "Exp");
                RewardsList.Children.Add(expPanel);
            }
            if (honor > 0)
            {
                var honorPanel = CreateRewardItem("Honor", honor.ToString("N0"), "Honor");
                RewardsList.Children.Add(honorPanel);
            }
            foreach (var item in items)
            {
                var itemPanel = CreateRewardItem(item.Key.Name, item.Value.ToString(), item.Key.GetType().Name);
                RewardsList.Children.Add(itemPanel);
            }
            RewardsPanel.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Tworzy panel pojedynczej nagrody do wyświetlenia w interfejsie użytkownika.
        /// Zawiera ikonę, nazwę i ilość nagrody w odpowiednim formacie.
        /// </summary>
        /// <param name="name">Nazwa nagrody.</param>
        /// <param name="amount">Ilość (jako ciąg znaków).</param>
        /// <param name="type">Typ nagrody (Gold, Exp, Honor lub nazwa typu przedmiotu).</param>
        /// <returns>Gotowy panel z elementami interfejsu reprezentującymi nagrodę.</returns>
        private StackPanel CreateRewardItem(string name, string amount, string type)
        {
            var panel = new StackPanel 
            { 
                Orientation = Orientation.Horizontal, 
                Margin = new Thickness(0, 5, 0, 0) 
            };
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
        /// Obsługuje zdarzenie kliknięcia przycisku oddania zadania.
        /// Kończy zadanie, przyznaje nagrody i aktualizuje interfejs użytkownika.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk oddania zadania).</param>
        /// <param name="e">Dane zdarzenia kliknięcia.</param>
        private void OnHandInQuestClick(object sender, RoutedEventArgs e)
        {
            if (_selectedQuest == null || _selectedQuest.QuestState != QuestState.Completed)
                return;
                
            try
            {
                // Complete the quest and get rewards
                var rewards = _selectedQuest.HandInQuest();
                
                if (rewards != null)
                {
                    // Show rewards to the player
                    ShowRewards(rewards.Gold, rewards.Experience, rewards.Honor, rewards.Items.ToList());
                    
                    // Refresh the quests list
                    InitializeQuestsTab();
                    
                    // Clear selection
                    CompletedQuestsListBox.SelectedItem = null;
                    StatusText.Text = $"Quest completed: {_selectedQuest.Name}";
                    _selectedQuest = null;
                    NoQuestSelectedPanel.Visibility = Visibility.Visible;
                    QuestDetailsPanel.Visibility = Visibility.Collapsed;
                    
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error completing quest: {ex}";
            }
        }
        
        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku zamknięcia panelu nagród.
        /// Ukrywa panel z nagrodami i zamyka okno dialogowe.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk zamknięcia).</param>
        /// <param name="e">Dane zdarzenia kliknięcia.</param>
        private void CloseRewardsButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the rewards panel and the dialog
            RewardsPanel.Visibility = Visibility.Collapsed;
            Close();
        }

        #region Shop Tab

        /// <summary>
        /// Inicjalizuje zakładkę sklepu w oknie dialogowym NPC.
        /// Ładuje dostępne przedmioty, konfiguruje widok listy i przyciski akcji.
        /// </summary>
        private void InitializeShopTab()
        {
            try
            {
                _shopItems.Clear();
                var shopItems = _npc.GetShopItems();
                foreach (var item in shopItems)
                {
                    _shopItems.Add(new ShopItemViewModel
                    {
                        Item = item.Item,
                        Name = item.Item.Name,
                        Cost = item.Cost,
                        Quantity = item.Quantity,
                        Stackable = item.IsStackable,
                        DisplayName = item.IsStackable ? 
                            $"{item.Item.Name} (x{item.Quantity}) - {item.Cost} gold" : 
                            $"{item.Item.Name} - {item.Cost} gold"
                    });
                }
                
                ItemsListBox.ItemsSource = _shopItems;
                UpdatePlayerInventory();
                UpdatePlayerGold();
                
                // Initialize UI state
                ClearSelection();
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error loading shop items: {ex.Message}";
            }
        }

        /// <summary>
        /// Aktualizuje widok ekwipunku gracza w interfejsie sklepu.
        /// Grupuje przedmioty według ich aliasów i wyświetla ich łączną ilość.
        /// Aktualizuje również interfejs użytkownika dla aktualnie wybranego przedmiotu.
        /// </summary>
        /// <summary>
        /// Aktualizuje widok ekwipunku gracza w interfejsie sklepu.
        /// Grupuje przedmioty według ich aliasów, sumuje ilości dla przedmiotów tego samego typu
        /// i aktualizuje widok listy przedmiotów w ekwipunku gracza.
        /// </summary>
        private void UpdatePlayerInventory()
        {
            try
            {
                var playerItems = _player.Inventory.Items
                    .GroupBy(item => item.Key.Alias)
                    .Select(group => new 
                    { 
                        Item = group.First().Key, 
                        Count = group.Sum(item => item.Value) // Sum up the quantities for stackable items
                    })
                    .Where(x => x.Count > 0) // Only show items with count > 0
                    .OrderBy(x => x.Item.Name)
                    .ToList();
                
                PlayerInventoryListBox.ItemsSource = playerItems
                    .Select(x => new 
                    {
                        Item = x.Item, 
                        DisplayName = x.Count > 1 ? $"{x.Item.Name} (x{x.Count})" : x.Item.Name,
                        Count = x.Count
                    })
                    .ToList();
                
                // Update the selected item's quantity if it's from inventory
                if (!_isShopItemSelected && _selectedItem != null)
                {
                    var selectedItem = playerItems.FirstOrDefault(x => x.Item.Alias == _selectedItem.Alias);
                    if (selectedItem != null)
                    {
                        _selectedItemMaxQuantity = selectedItem.Count;
                        QuantitySlider.Maximum = _selectedItemMaxQuantity;
                        UpdateTotalPrice();
                    }
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error updating inventory: {ex.Message}";
            }
        }

        /// <summary>
        /// Aktualizuje panel szczegółów wybranego przedmiotu.
        /// Wyświetla nazwę, typ i cenę przedmiotu w zależności od tego, czy przedmiot jest ze sklepu, czy z ekwipunku gracza.
        /// </summary>
        private void UpdateItemDetails()
        {
            if (_selectedItem == null) return;
            
            SelectedItemName.Text = _selectedItem.Name;
            SelectedItemType.Text = _selectedItem.GetType().Name;
            
            if (_isShopItemSelected)
            {
                SelectedItemPrice.Text = $"Price: {_selectedItemPrice} gold";
            }
            else
            {
                SelectedItemPrice.Text = $"Sell Price: {_selectedItemPrice} gold";
            }
            
            // TODO: Uncomment and implement these if needed
            // SelectedItemStats.Text = _selectedItem.GetStats();
            // SelectedItemDescription.Text = _selectedItem.FlavorText;
        }
        
        /// <summary>
        /// Czyści aktualny wybór przedmiotu w interfejsie sklepu.
        /// Resetuje zaznaczenie na listach przedmiotów i czyści panel szczegółów.
        /// </summary>
        private void ClearSelection()
        {
            _isUpdatingSelection = true;
            
            try
            {
                ItemsListBox.SelectedIndex = -1;
                PlayerInventoryListBox.SelectedIndex = -1;
                _selectedItem = null;
                
                // Reset UI
                ClearItemDetails();
            }
            finally
            {
                _isUpdatingSelection = false;
            }
        }
        
        /// <summary>
        /// Czyści panel szczegółów przedmiotu, ustawiając domyślne wartości.
        /// Resetuje wyświetlaną nazwę, typ, cenę i inne szczegóły przedmiotu.
        /// </summary>
        private void ClearItemDetails()
        {
            SelectedItemName.Text = "No item selected";
            SelectedItemType.Text = string.Empty;
            SelectedItemPrice.Text = string.Empty;
            SelectedItemStats.Text = string.Empty;
            SelectedItemDescription.Text = string.Empty;
            SelectedItemTotalPrice.Visibility = Visibility.Collapsed;
            
            // Reset slider
            QuantitySlider.Value = 1;
            QuantitySlider.Maximum = 1;
            QuantityText.Text = "1";
            
            // Reset action UI
            ActionTitle.Text = "Select an Item";
            ActionButton.Visibility = Visibility.Collapsed;
            DeselectButton.Visibility = Visibility.Collapsed;
            QuantityLabel.Visibility = Visibility.Collapsed;
            QuantitySlider.Visibility = Visibility.Collapsed;
            QuantityText.Visibility = Visibility.Collapsed;
        }
        
        /// <summary>
        /// Sprawdza, czy gracz może wykonać dany przedmiot na podstawie posiadanych składników.
        /// Weryfikuje, czy gracz posiada wszystkie wymagane składniki w odpowiedniej ilości.
        /// </summary>
        /// <param name="item">Przedmiot do sprawdzenia.</param>
        /// <returns>True, jeśli gracz posiada wszystkie wymagane składniki; w przeciwnym razie false.</returns>
        private bool CanCraftItem(ICraftable item)
        {
            if (item?.CraftingRecipe == null)
                return false;

            return item.CraftingRecipe.All(x => _player.Inventory.GetCount(x.Key) >= x.Value);
        }
        
        /// <summary>
        /// Obsługuje zdarzenie wyboru przedmiotu z listy w interfejsie sklepu.
        /// Aktualizuje interfejs użytkownika w zależności od wybranego przedmiotu (kupno/sprzedaż).
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (ListBox z przedmiotami).</param>
        /// <param name="e">Dane zdarzenia zmiany zaznaczenia.</param>
        private void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingSelection) return;
            
            try
            {
                _isUpdatingSelection = true;
                
                // Clear the other list's selection
                if (sender == ItemsListBox)
                {
                    PlayerInventoryListBox.SelectedIndex = -1;
                    _isShopItemSelected = true;
                }
                else if (sender == PlayerInventoryListBox)
                {
                    ItemsListBox.SelectedIndex = -1;
                    _isShopItemSelected = false;
                }

                // Get the selected item
                var listBox = sender as ListBox;
                var selected = listBox?.SelectedItem;
                
                if (selected != null)
                {
                    dynamic itemData = selected;
                    _selectedItem = itemData.Item;
                    
                    // Update UI based on selection
                    if (_isShopItemSelected)
                    {
                        var shopItem = selected as ShopItemViewModel;
                        _selectedItemPrice = shopItem.Cost;
                        _selectedItemMaxQuantity = Math.Min(99, shopItem.Quantity);
                        
                        ActionTitle.Text = "Buy Item";
                        ActionButton.Content = "Buy";
                        QuantityLabel.Visibility = Visibility.Visible;
                        QuantitySlider.Visibility = Visibility.Visible;
                        QuantityText.Visibility = Visibility.Visible;
                        ActionButton.Visibility = Visibility.Visible;
                        DeselectButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        var countProperty = selected.GetType().GetProperty("Count");
                        int count = countProperty != null ? (int)countProperty.GetValue(selected) : 1;
                        _selectedItemPrice = _selectedItem.Cost / 2; // Sell for half price
                        _selectedItemMaxQuantity = count;
                        
                        ActionTitle.Text = "Sell Item";
                        ActionButton.Content = "Sell";
                        QuantityLabel.Visibility = Visibility.Visible;
                        QuantitySlider.Visibility = Visibility.Visible;
                        QuantityText.Visibility = Visibility.Visible;
                        ActionButton.Visibility = Visibility.Visible;
                        DeselectButton.Visibility = Visibility.Visible;
                    }
                    
                    // Update item details
                    UpdateItemDetails();
                    
                    // Update quantity slider
                    QuantitySlider.Maximum = _selectedItemMaxQuantity;
                    QuantitySlider.Value = 1;
                    QuantityText.Text = "1";
                    
                    // Update total price
                    UpdateTotalPrice();
                }
                else
                {
                    ClearSelection();
                }
            }
            finally
            {
                _isUpdatingSelection = false;
            }
        }
        
        /// <summary>
        /// Obsługuje zmianę wartości suwaka ilości przedmiotów.
        /// Aktualizuje wyświetlaną wartość ilości oraz łączną cenę.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (suwak).</param>
        /// <param name="e">Dane zdarzenia zawierające nową wartość suwaka.</param>
        private void Quantity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (QuantityText != null && _selectedItem != null)
            {
                int quantity = (int)e.NewValue;
                QuantityText.Text = quantity.ToString();
                UpdateTotalPrice();
            }
        }
        
        /// <summary>
        /// Aktualizuje wyświetlaną całkowitą cenę wybranych przedmiotów.
        /// Uwzględnia różne kolory tekstu w zależności od tego, czy gracz może sobie pozwolić na zakup.
        /// W przypadku sprzedaży wyświetla łączną kwotę, jaką gracz otrzyma.
        /// </summary>
        private void UpdateTotalPrice()
        {
            if (_selectedItem == null) return;
            
            int quantity = (int)QuantitySlider.Value;
            int totalPrice = _selectedItemPrice * quantity;
            
            if (_isShopItemSelected)
            {
                // For buying
                if (totalPrice > _player.Gold)
                {
                    SelectedItemTotalPrice.Foreground = Brushes.Red;
                    ActionButton.IsEnabled = false;
                }
                else
                {
                    SelectedItemTotalPrice.Foreground = Brushes.Green;
                    ActionButton.IsEnabled = true;
                }
                SelectedItemTotalPrice.Text = $"Total: {totalPrice} gold";
            }
            else
            {
                // For selling
                SelectedItemTotalPrice.Text = $"Total: {totalPrice} gold";
                SelectedItemTotalPrice.Foreground = Brushes.Goldenrod;
                ActionButton.IsEnabled = true;
            }
            
            SelectedItemTotalPrice.Visibility = Visibility.Visible;
        }
        
        /// <summary>
        /// Obsługuje kliknięcie przycisku akcji (kupno/sprzedaż).
        /// Wykonuje transakcję handlową na podstawie wybranego przedmiotu i ilości.
        /// Aktualizuje stan złota gracza i ekwipunek po zakończonej transakcji.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia kliknięcia.</param>
        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedItem == null) return;
                
                int quantity = (int)QuantitySlider.Value;
                
                if (quantity < 1)
                {
                    StatusText.Text = "Please select at least 1 item.";
                    return;
                }
                
                if (_isShopItemSelected)
                {
                    // Buy operation
                    int totalCost = _selectedItemPrice * quantity;
                    
                    // Buy the items one by one to handle stackable items correctly
                    for (int i = 0; i < quantity; i++)
                    {
                        _npc.BuyItem(_selectedItem, 1);
                    }
                    
                    StatusText.Text = $"Bought {quantity}x {_selectedItem.Name} for {totalCost} gold.";
                }
                else
                {
                    // Sell operation
                    int totalEarned = 0;
                    
                    // Sell the items one by one to handle stackable items correctly
                    for (int i = 0; i < quantity; i++)
                    {
                        _npc.SellItem(_selectedItem, 1);
                        totalEarned += _selectedItemPrice;
                    }
                    
                    StatusText.Text = $"Sold {quantity}x {_selectedItem.Name} for {totalEarned} gold.";
                }
                
                // Refresh the UI
                InitializeShopTab();
                UpdatePlayerInventory();
                UpdatePlayerGold();
                
                // Clear selection
                ClearSelection();
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Obsługuje kliknięcie przycisku odznaczenia wybranego przedmiotu.
        /// Czyści aktualny wybór i resetuje odpowiednie elementy interfejsu użytkownika.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk odznaczenia).</param>
        /// <param name="e">Dane zdarzenia kliknięcia.</param>
        private void DeselectButton_Click(object sender, RoutedEventArgs e)
        {
            ClearSelection();
        }

        /// <summary>
        /// Obsługuje zmianę zaznaczenia na liście przedmiotów do wytworzenia.
        /// Aktualizuje panel szczegółów wybranego przedmiotu oraz listę wymaganych materiałów.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (ListBox z przedmiotami do wytworzenia).</param>
        /// <param name="e">Dane zdarzenia zmiany zaznaczenia.</param>
        private void CraftingListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedItem = CraftingListBox.SelectedItem as CraftingRecipeViewModel;
                if (selectedItem == null || selectedItem.Item == null)
                {
                    ClearItemDetails();
                    return;
                }

                var item = selectedItem.Item;
                CraftingItemName.Text = item.Name;
                CraftingItemDetails.Text = GetItemDescription(item);
                
                // Update required materials
                var materials = new List<string>();
                if (item.CraftingRecipe != null)
                {
                    foreach (var material in item.CraftingRecipe)
                    {
                        var materialItem = ItemManager.GetItem(material.Key);
                        var hasCount = _player.Inventory.GetCount(material.Key);
                        var hasEnough = hasCount >= material.Value;
                        var status = hasEnough ? "✓" : "✗";
                        materials.Add($"{status} {material.Value}x {materialItem?.Name ?? material.Key} (Have: {hasCount})");
                    }
                }
                
                RequiredMaterialsList.ItemsSource = materials;
                CraftButton.IsEnabled = selectedItem.CanCraft;
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error selecting craftable item: {ex.Message}";
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku wytwarzania przedmiotu.
        /// Sprawdza dostępność składników, wykonuje wytwarzanie i aktualizuje interfejs użytkownika.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk wytwarzania).</param>
        /// <param name="e">Dane zdarzenia kliknięcia.</param>
        private void CraftButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = CraftingListBox.SelectedItem as CraftingRecipeViewModel;
                if (selectedItem?.Item == null)
                {
                    StatusText.Text = "No item selected to craft.";
                    return;
                }

                var item = selectedItem.Item;
                
                // Check if player has required materials
                if (!CanCraftItem(item))
                {
                    StatusText.Text = "You don't have all the required materials to craft this item.";
                    return;
                }

                // Remove required materials from player's inventory
                foreach (var material in item.CraftingRecipe)
                {
                    var materialItem = ItemManager.GetItem(material.Key);
                    if (materialItem != null)
                    {
                        _player.Inventory.TryRemoveItem(materialItem, material.Value);
                    }
                }

                // Add crafted item to player's inventory
                var craftedItem = ItemManager.GetItem(item.Alias);
                if (craftedItem != null)
                {
                    _player.Inventory.AddItem(craftedItem);
                    StatusText.Text = $"Successfully crafted {craftedItem.Name}!";
                    
                    // Update UI
                    UpdatePlayerGold();
                    
                    // Refresh the crafting list to update material counts
                    InitializeCraftingTab();
                }
                else
                {
                    StatusText.Text = "Failed to craft item. Please try again.";
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error crafting item: {ex.Message}";
            }
        }

        #endregion

        #region Event Handlers
        
        /// <summary>
        /// Obsługuje zmianę zaznaczenia na liście wybranych komponentów mikstury.
        /// Aktywuje przycisk usuwania komponentu, gdy jakiś komponent jest zaznaczony.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia zmiany zaznaczenia.</param>
        private void SelectedComponentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveComponentButton.IsEnabled = SelectedComponentsList.SelectedItem != null;
        }
        
        /// <summary>
        /// Obsługuje zmianę wyboru w rozwijanej liście dostępnych komponentów.
        /// Dodaje wybrany komponent do listy wybranych komponentów, jeśli nie przekroczono limitu 3 komponentów.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (ComboBox).</param>
        /// <param name="e">Dane zdarzenia zmiany zaznaczenia.</param>
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
        /// Usuwa zaznaczony komponent z listy wybranych komponentów i aktualizuje interfejs użytkownika.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia kliknięcia.</param>
        private void RemoveComponentButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedComponentsList.SelectedItem is PotionComponentViewModel selectedComponent)
            {
                _selectedComponents.Remove(selectedComponent);
                UpdatePotionCraftingUI();
            }
        }
        
        /// <summary>
        /// Obsługuje zmianę wyboru katalizatora w rozwijanej liście.
        /// Aktualizuje interfejs użytkownika po zmianie wybranego katalizatora.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (ComboBox).</param>
        /// <param name="e">Dane zdarzenia zmiany zaznaczenia.</param>
        private void CatalystComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update UI if needed when catalyst changes
            UpdatePotionCraftingUI();
        }
        
        /// <summary>
        /// Obsługuje zmianę tekstu w polu nazwy mikstury.
        /// Aktualizuje interfejs użytkownika po każdej zmianie tekstu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (TextBox).</param>
        /// <param name="e">Dane zdarzenia zmiany tekstu.</param>
        private void PotionNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePotionCraftingUI();
        }
        
        /// <summary>
        /// Obsługuje kliknięcie przycisku tworzenia mikstury.
        /// Weryfikuje poprawność danych, tworzy nową miksturę i dodaje ją do ekwipunku gracza.
        /// Wyświetla odpowiednie komunikaty o błędach w przypadku niepowodzenia.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia kliknięcia.</param>
        private void CraftPotionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate selection
                if (_selectedComponents.Count == 0 || _selectedComponents.Count > 3)
                {
                    PotionCraftingStatus.Text = "You must select between 1 and 3 components.";
                    return;
                }
                
                string potionName = PotionNameTextBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(potionName))
                {
                    PotionCraftingStatus.Text = "Please enter a name for your potion.";
                    return;
                }
                
                // Get selected components and catalyst
                var components = _selectedComponents.Select(vm => vm.Component).ToList();
                var catalyst = (CatalystComboBox.SelectedItem as PotionCatalystViewModel)?.Catalyst;
                
                // Check if player has all required materials
                foreach (var component in components)
                {
                    if (PlayerHandler.player.Inventory.GetCount(component.Material) == 0)
                    {
                        PotionCraftingStatus.Text = $"You don't have the required component: {ItemManager.GetItem(component.Material).Name}";
                        return;
                    }
                }
                
                if (catalyst != null && PlayerHandler.player.Inventory.GetCount(catalyst.Material) == 0)
                {
                    PotionCraftingStatus.Text = $"You don't have the required catalyst: {ItemManager.GetItem(catalyst.Material).Name}";
                    return;
                }
                
                // Craft the potion
                var potion = CraftingManager.CraftPotion(components, catalyst, potionName);
                
                // Update UI
                PotionCraftingStatus.Text = $"Successfully crafted {potionName}!";
                
                // Reset the form
                _selectedComponents.Clear();
                PotionNameTextBox.Text = string.Empty;
                UpdatePotionCraftingUI();
                
                // Update player inventory display
                UpdatePlayerGold();
            }
            catch (Exception ex)
            {
                PotionCraftingStatus.Text = $"Error crafting potion: {ex.Message}";
                Debug.WriteLine($"Error in CraftPotionButton_Click: {ex}");
            }
        }

        /// <summary>
        /// Konfiguruje dostępne usługi świadczone przez NPC.
        /// W zależności od typu NPC aktywuje odpowiednie zakładki i funkcje.
        /// </summary>
        private void SetupNPCServices()
        {
            ServicesPanel.Children.Clear();
            
            switch (_npc)
            {
                case Alchemist alchemist:
                    AddAlchemistServices(alchemist);
                    break;
                    
                case Blacksmith blacksmith:
                    AddBlacksmithServices(blacksmith);
                    break;
                    
                case Enchanter enchanter:
                    AddEnchanterServices(enchanter);
                    break;
            }
        }

        /// <summary>
        /// Dodaje przyciski usług świadczonych przez alchemika do panelu usług.
        /// Zawiera funkcjonalność tworzenia mikstur i uzupełniania ich ładunków.
        /// </summary>
        /// <param name="alchemist">Obiekt alchemika, którego usługi mają zostać dodane.</param>
        private void AddAlchemistServices(Alchemist alchemist)
        {
            AddServiceButton("Create Potion", "Brew a custom potion with various effects", 
                () => {
                    var dialog = new PotionCraftingDialog
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };
                    
                    bool? result = dialog.ShowDialog();
                    
                    if (result == true)
                    {
                        // Refresh the UI to show the new potion in inventory
                        InitializeShopTab();
                        StatusText.Text = "Potion crafted successfully!";
                    }
                });
            
            AddServiceButton("Refill Potions", "Refill all your potions to full charges", 
                () => {
                    var potions = PlayerHandler.player.Inventory.Items
                        .Where(x => x is Potion)
                        .Cast<Potion>()
                        .Where(p => p.CurrentCharges < p.MaximalCharges)
                        .ToList();

                    if (potions.Count == 0)
                    {
                        StatusText.Text = "You don't have any potions that need refilling.";
                        return;
                    }

                    // Group potions by their tier (max component tier) and order from highest to lowest
                    var potionsByTier = potions
                        .GroupBy(p => p.Components.Max(c => c.StrengthTier))
                        .OrderByDescending(g => g.Key);

                    // Define alcohol types in order of preference (best first)
                    var alcoholTypes = new[] { "PerfectAlcohol", "StrongAlcohol", "GoodAlcohol", "Alcohol", "WeakAlcohol" };
                    
                    var alcoholUsed = new Dictionary<string, int>();
                    int totalRefilled = 0;

                    // Process each tier separately
                    foreach (var group in potionsByTier)
                    {
                        int tier = group.Key;
                        var potionsToRefill = group.ToList();
                        
                        // Try each alcohol type from best to worst
                        foreach (var alcoholType in alcoholTypes)
                        {
                            if (potionsToRefill.Count == 0) break;
                            
                            int available = PlayerHandler.player.Inventory.GetCount(alcoholType);
                            if (available <= 0) continue;
                            
                            // Process each potion in this tier with current alcohol
                            for (int i = potionsToRefill.Count - 1; i >= 0; i--)
                            {
                                var potion = potionsToRefill[i];
                                int chargesToAdd = 0;
                                
                                // Determine charges to add based on alcohol type and potion tier
                                switch (alcoholType)
                                {
                                    case "WeakAlcohol" when tier == 1:
                                        chargesToAdd = 1;
                                        break;
                                        
                                    case "Alcohol" when tier == 1:
                                        chargesToAdd = 4;
                                        break;
                                        
                                    case "GoodAlcohol" when tier <= 2:
                                        chargesToAdd = tier == 1 ? 4 : 1;
                                        break;
                                        
                                    case "StrongAlcohol" when tier <= 2:
                                        chargesToAdd = 4;
                                        break;
                                        
                                    case "PerfectAlcohol" when tier <= 3:
                                        chargesToAdd = tier <= 2 ? 4 : 1;
                                        break;
                                }
                                
                                if (chargesToAdd > 0)
                                {
                                    int needed = potion.MaximalCharges - potion.CurrentCharges;
                                    if (needed > 0)
                                    {
                                        int add = Math.Min(needed, chargesToAdd);
                                        potion.Refill(add);
                                        
                                        if (!alcoholUsed.ContainsKey(alcoholType))
                                            alcoholUsed[alcoholType] = 0;
                                        alcoholUsed[alcoholType]++;
                                        
                                        PlayerHandler.player.Inventory.TryRemoveItem(ItemManager.GetItem(alcoholType), 1);
                                        totalRefilled++;
                                        
                                        if (potion.CurrentCharges >= potion.MaximalCharges)
                                        {
                                            potionsToRefill.RemoveAt(i);
                                        }
                                    }
                                }
                                
                                // Stop if we've used all available alcohol
                                if (PlayerHandler.player.Inventory.GetCount(alcoholType) == 0)
                                    break;
                            }
                        }
                    }

                    if (alcoholUsed.Count == 0)
                    {
                        StatusText.Text = "You don't have any alcohol to refill your potions.";
                        return;
                    }

                    // Update UI with detailed alcohol usage
                    var alcoholText = string.Join(", ", alcoholUsed
                        .Select(kv => $"{kv.Value} {NameAliasHelper.GetName(kv.Key)}"));
                    
                    StatusText.Text = $"Refilled {totalRefilled} potions using {alcoholText}.";
                });
            
            AddServiceButton("Heal Wounds", "Fully restore your health", 
                () => {
                    if (_player.CurrentHealth >= _player.MaximalHealth)
                    {
                        StatusText.Text = "You are already at full health.";
                        return;
                    }
                    
                    var cost = CalculateHealingCost();
                    if (_player.Gold < cost)
                    {
                        StatusText.Text = $"You need {cost} Gold to heal, but only have {_player.Gold} Gold.";
                        return;
                    }
                    
                    _player.LoseGold(cost);
                    _player.Heal(_player.MaximalHealth - _player.CurrentHealth);
                    alchemist.SpendGold(cost);
                    UpdatePlayerGold();
                    UpdateLoyaltyDisplay();
                    StatusText.Text = $"You have been fully healed for {cost} Gold.";
                });
        }

        /// <summary>
        /// Oblicza koszt leczenia na podstawie poziomu i aktualnego stanu zdrowia gracza.
        /// Uwzględnia modyfikator zniżki honorowej i skalowanie kosztu wraz z poziomem postaci.
        /// </summary>
        /// <returns>Koszt leczenia wyrażony w złocie.</returns>
        private int CalculateHealingCost()
        {
            var player = PlayerHandler.player;
            return (int)(PlayerHandler.HonorDiscountModifier * 
                Math.Pow(4, (player.Level - 1) / 10.0) * 0.1 * player.MaximalHealth * 
                (player.MaximalHealth - player.CurrentHealth) / (2 * player.MaximalHealth - player.CurrentHealth));
        }

        /// <summary>
        /// Dodaje przyciski usług świadczonych przez kowala do panelu usług.
        /// Zawiera funkcjonalność tworzenia ekwipunku oraz ulepszania broni i zbroi.
        /// </summary>
        /// <param name="blacksmith">Obiekt kowala, którego usługi mają zostać dodane.</param>
        private void AddBlacksmithServices(Blacksmith blacksmith)
        {
            AddServiceButton("Craft Equipment", "Forge a new weapon or armor from components", 
                () => 
                {
                    var dialog = new BlacksmithCraftingDialog(PlayerHandler.player.CharacterClass)
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };
                    
                    dialog.ShowDialog();
                });
            
            AddServiceButton("Upgrade Weapon", "Improve your weapon's damage and stats", 
                () => ShowUpgradeDialog(isWeapon: true));
            
            AddServiceButton("Upgrade Armor", "Improve your armor's defense and stats", 
                () => ShowUpgradeDialog(isWeapon: false));
            
            AddServiceButton("Reforge Weapon", "Reroll your weapon's rarity (chance to increase or decrease)", 
                () => ShowReforgeDialog(blacksmith, isWeapon: true));
            
            AddServiceButton("Reforge Armor", "Reroll your armor's rarity (chance to increase or decrease)", 
                () => ShowReforgeDialog(blacksmith, isWeapon: false));
        }

        /// <summary>
        /// Wyświetla okno dialogowe ulepszania przedmiotu (broni lub zbroi).
        /// Sprawdza wymagania i umożliwia ulepszenie przedmiotu za odpowiednią opłatą.
        /// </summary>
        /// <param name="isWeapon">Określa, czy ulepszana jest broń (true) czy zbroja (false).</param>
        private void ShowUpgradeDialog(bool isWeapon)
        {
            try
            {
                var item = isWeapon ? (IEquippable)_player.Weapon : _player.Armor;
                if (item == null)
                {
                    StatusText.Text = isWeapon 
                        ? "You don't have a weapon equipped!" 
                        : "You don't have armor equipped!";
                    return;
                }

                if (item.UpgradeModifier >= 1.6)
                {
                    StatusText.Text = $"Your {item.Name} is already at maximum upgrade level!";
                    return;
                }
                
                double minModifier = Math.Round(item.UpgradeModifier - 0.99, 2);
                double maxModifier = 0.6;
                double initialModifier = Math.Min(minModifier, maxModifier);
                var dialog = new UpgradeItemDialog(
                    item, 
                    initialModifier, 
                    0.5, 
                    0,
                    PlayerHandler.player.Gold)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                dialog.ShowDialog();
                if (dialog.DialogResult == true)
                {
                    double chosenModifier = dialog.NewModifier;
                    double successChance = dialog.SuccessChance;
                    double cost = dialog.UpgradeCost;
                    
                    if (chosenModifier < minModifier || chosenModifier > maxModifier)
                    {
                        StatusText.Text = "Invalid modifier selected.";
                        return;
                    }
                    if (PlayerHandler.player.Gold < cost)
                    {
                        StatusText.Text = "Not enough gold for this upgrade.";
                        return;
                    }
                    PlayerHandler.player.Gold -= (int)cost;
                    item.UpgradeModifier = chosenModifier + 1;
                    bool success = Random.Shared.NextDouble() <= successChance;
                    
                    if (success)
                        StatusText.Text = $"Successfully upgraded {item.Name} to x{chosenModifier + 1:F2}!";
                    else
                        StatusText.Text = $"Upgrade failed!.";
                    SetupNPCServices();
                    UpdateLoyaltyDisplay();
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error during upgrade: {ex.Message}";
            }
        }

        /// <summary>
        /// Wyświetla okno dialogowe przekuwania przedmiotu (broni lub zbroi).
        /// Umożliwia zmianę rzadkości przedmiotu z odpowiednimi szansami na sukces.
        /// </summary>
        /// <param name="blacksmith">Obiekt kowala, który wykonuje przekucie.</param>
        /// <param name="isWeapon">Określa, czy przekuwana jest broń (true) czy zbroja (false).</param>
        private void ShowReforgeDialog(Blacksmith blacksmith, bool isWeapon)
        {
            try
            {
                var item = isWeapon ? (IEquippable)_player.Weapon : _player.Armor;
                if (item == null)
                {
                    StatusText.Text = isWeapon 
                        ? "You don't have a weapon equipped!" 
                        : "You don't have armor equipped!";
                    return;
                }

                if (item.Rarity == ItemRarity.Godly || item.Rarity == ItemRarity.Junk)
                {
                    StatusText.Text = $"Your {item.Name} cannot be reforged!";
                    return;
                }

                int cost = (int)(item.Cost * blacksmith.ServiceCostMod * PlayerHandler.HonorDiscountModifier / 4.0);
                
                var result = MessageBox.Show(
                    $"Reforge {item.Name}?\n" +
                    $"Current Rarity: {item.Rarity}\n" +
                    $"Cost: {cost} Gold\n\n" +
                    "Chances:\n" +
                    "- 10%: Item destroyed (reduced to Junk)\n" +
                    "- 20%: No change\n" +
                    "- 50%: +1 rarity level\n" +
                    "- 20%: +2 rarity levels (or +1 if Legendary)",
                    "Confirm Reforging",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_player.Gold < cost)
                    {
                        StatusText.Text = $"You need {cost} Gold for reforging, but only have {_player.Gold} Gold.";
                        return;
                    }

                    _player.LoseGold(cost);
                    blacksmith.SpendGold(cost);
                    
                    var success = UtilityMethods.RandomChoice(new Dictionary<int, double>
                    {
                        { -1, 0.1 }, // Critical failure
                        { 0, 0.2 },  // Failure
                        { 1, 0.5 },  // Success
                        { 2, 0.2 }   // Critical success
                    });

                    string message;
                    ItemRarity oldRarity = item.Rarity;
                    
                    switch (success)
                    {
                        case -1:
                            item.Rarity = ItemRarity.Junk;
                            message = $"Disaster! {item.Name} was reduced to Junk quality!";
                            break;
                        case 0:
                            message = $"The reforge had no effect. {item.Name} remains {oldRarity}.";
                            break;
                        case 1:
                            item.Rarity += 1;
                            message = $"Success! {item.Name} is now {item.Rarity} (was {oldRarity})!";
                            break;
                        case 2:
                            item.Rarity += item.Rarity == ItemRarity.Legendary ? 1 : 2;
                            message = $"Amazing! {item.Name} is now {item.Rarity} (was {oldRarity})!";
                            break;
                        default:
                            message = "Something unexpected happened during reforge.";
                            break;
                    }
                    
                    StatusText.Text = message;
                    UpdatePlayerGold();
                    UpdateLoyaltyDisplay();
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error during reforge: {ex.Message}";
            }
        }

        /// <summary>
        /// Dodaje przyciski usług świadczonych przez zaklinacza do panelu usług.
        /// Zawiera funkcjonalność tworzenia i wstawiania galdurytów do ekwipunku.
        /// </summary>
        /// <param name="enchanter">Obiekt zaklinacza, którego usługi mają zostać dodane.</param>
        private void AddEnchanterServices(Enchanter enchanter)
        {
            AddServiceButton("Craft Galdurite", "Create powerful magical gems with random properties", 
                () => {
                    try
                    {
                        var dialog = new GalduriteCraftingDialog
                        {
                            Owner = this,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                        };
                        
                        dialog.ShowDialog();
                        StatusText.Text = "Galdurite crafting complete.";
                    }
                    catch (Exception ex)
                    {
                        StatusText.Text = $"Error opening Galdurite crafting: {ex.Message}";
                        Debug.WriteLine($"Error in Galdurite crafting: {ex}");
                    }
                });
            
            AddServiceButton("Insert Galdurite to Weapon", "Add magical properties to your weapon", 
                () => {
                    try
                    {
                        var galdurites = PlayerHandler.player.Inventory.Items
                            .Where(x => x.Key.ItemType == ItemType.WeaponGaldurite)
                            .Select(x => x.Key).Cast<Galdurite>().ToList();
                        var dialog = new InsertGalduriteDialog(PlayerHandler.player.Weapon, galdurites, 1)
                        {
                            Owner = this,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                        };
                        
                        dialog.ShowDialog();
                        StatusText.Text = "Galdurite inserting complete.";
                    }
                    catch (Exception ex)
                    {
                        StatusText.Text = $"Error opening Galdurite inserting: {ex.Message}";
                        Debug.WriteLine($"Error in Galdurite inserting: {ex}");
                    }
                });
            
            AddServiceButton("Insert Galdurite into Armor", "Add magical properties to your armor", 
                () => {
                    try
                    {
                        var galdurites = PlayerHandler.player.Inventory.Items
                            .Where(x => x.Key.ItemType == ItemType.ArmorGaldurite)
                            .Select(x => x.Key).Cast<Galdurite>().ToList();
                        var dialog = new InsertGalduriteDialog(PlayerHandler.player.Armor, galdurites, 1)
                        {
                            Owner = this,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                        };
                        
                        dialog.ShowDialog();
                        StatusText.Text = "Galdurite inserting complete.";
                    }
                    catch (Exception ex)
                    {
                        StatusText.Text = $"Error opening Galdurite inserting: {ex.Message}";
                        Debug.WriteLine($"Error in Galdurite inserting: {ex}");
                    }
                });
            
            AddServiceButton("Reveal Galdurite", "Reveal the properties of unidentified galdurites", 
                () => {
                    try
                    {
                        var dialog = new RevealGalduriteDialog(PlayerHandler.player.Inventory.Items
                            .Where(x => x.Key is Galdurite).Select(x => x.Key)
                            .Cast<Galdurite>().ToList(), 1)
                        {
                            Owner = this,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                        };
                        
                        dialog.ShowDialog();
                        StatusText.Text = "Galdurite revealing complete.";
                    }
                    catch (Exception ex)
                    {
                        StatusText.Text = $"Error opening Galdurite inserting: {ex.Message}";
                        Debug.WriteLine($"Error in Galdurite inserting: {ex}");
                    }
                });
        }

        /// <summary>
        /// Dodaje przycisk usługi do panelu usług NPC.
        /// Tworzy przycisk z tytułem i opisem, który po kliknięciu wykonuje określoną akcję.
        /// </summary>
        /// <param name="title">Tytuł przycisku usługi.</param>
        /// <param name="description">Opis usługi wyświetlany pod tytułem.</param>
        /// <param name="action">Akcja do wykonania po kliknięciu przycisku.</param>
        private void AddServiceButton(string title, string description, Action action)
        {
            var button = new Button
            {
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBlock
                        {
                            Text = title,
                            Style = (Style)FindResource("MaterialDesignHeadline6TextBlock"),
                            TextWrapping = System.Windows.TextWrapping.Wrap,
                            FontWeight = FontWeights.SemiBold
                        },
                        new TextBlock
                        {
                            Text = description,
                            Style = (Style)FindResource("MaterialDesignBody2TextBlock"),
                            TextWrapping = System.Windows.TextWrapping.Wrap,
                            Margin = new Thickness(0, 5, 0, 5)
                        }
                    }
                },
                Style = (Style)FindResource("MaterialDesignOutlinedButton"),
                Margin = new Thickness(0, 0, 0, 15),
                Padding = new Thickness(20, 15, 20, 15),
                MinHeight = 80,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            
            button.Click += (s, e) => action();
            ServicesPanel.Children.Add(button);
        }

        #endregion

        #region Helper Classes

        public class ShopItemViewModel
        {
            public IItem? Item { get; set; }
            public string Name { get; set; } = string.Empty;
            public int Cost { get; set; }
            public string Type { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public string DisplayName { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public bool Stackable { get; set; }
        }

        public class CraftingRecipeViewModel : INotifyPropertyChanged
        {
            private ICraftable? _item;
            private string _name = string.Empty;
            private bool _canCraft;

            public ICraftable? Item
            {
                get => _item;
                set
                {
                    _item = value;
                    OnPropertyChanged();
                }
            }

            public string Name
            {
                get => _name;
                set
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }

            public bool CanCraft
            {
                get => _canCraft;
                set
                {
                    _canCraft = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
