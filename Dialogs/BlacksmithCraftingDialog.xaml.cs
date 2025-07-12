using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GodmistWPF.Characters.Player;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Items;
using GodmistWPF.Items;
using GodmistWPF.Items.Equippable.Armors;
using GodmistWPF.Items.Equippable.Weapons;

namespace GodmistWPF.Dialogs
{
    /// <summary>
    /// Okno dialogowe umożliwiające tworzenie nowego ekwipunku u kowala.
    /// Zawiera interfejs do wyboru komponentów i jakości tworzonego przedmiotu.
    /// </summary>
    public partial class BlacksmithCraftingDialog : Window
    {
        /// <summary>
        /// Klasa postaci, dla której tworzony jest ekwipunek.
        /// </summary>
        private readonly CharacterClass _characterClass;
        
        /// <summary>
        /// Aktualny poziom przedmiotu obliczany na podstawie komponentów.
        /// </summary>
        private int _currentTier = 1;
        
        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="BlacksmithCraftingDialog"/>.
        /// </summary>
        /// <param name="characterClass">Klasa postaci, dla której ma być tworzony ekwipunek.</param>
        public BlacksmithCraftingDialog(CharacterClass characterClass)
        {
            InitializeComponent();
            _characterClass = characterClass;
            Loaded += OnLoaded;
            Closed += Window_Closed;
        }
        
        /// <summary>
        /// Obsługuje zdarzenie zamknięcia okna, usuwając nasłuchiwacze zdarzeń.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void Window_Closed(object sender, EventArgs e)
        {
            // Remove event handlers
            WeaponHeadCombo.SelectionChanged -= UpdateWeaponStats;
            WeaponBinderCombo.SelectionChanged -= UpdateWeaponStats;
            WeaponHandleCombo.SelectionChanged -= UpdateWeaponStats;
            WeakQualityRadio.Checked -= UpdateWeaponStats;
            NormalQualityRadio.Checked -= UpdateWeaponStats;
            ExcellentQualityRadio.Checked -= UpdateWeaponStats;
        }
        
        /// <summary>
        /// Obsługuje zdarzenie załadowania okna, inicjalizując interfejs użytkownika.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Initialize UI based on character class
            Title = $"Blacksmith's Forge - {_characterClass} Equipment";
            
            // Load available parts for the character's class
            LoadWeaponParts();
            LoadArmorParts();
            
            // Add event handlers for selection changes
            WeaponHeadCombo.SelectionChanged += UpdateWeaponStats;
            WeaponBinderCombo.SelectionChanged += UpdateWeaponStats;
            WeaponHandleCombo.SelectionChanged += UpdateWeaponStats;
            WeakQualityRadio.Checked += UpdateWeaponStats;
            NormalQualityRadio.Checked += UpdateWeaponStats;
            ExcellentQualityRadio.Checked += UpdateWeaponStats;
            
            // Set default tab based on item type selection
            ItemType_Checked(null, null);
            
            // Update stats for initial state
            UpdateWeaponStats(null, null);
        }
        
        /// <summary>
        /// Sprawdza, czy gracz posiada wystarczającą ilość materiałów do wytworzenia części.
        /// </summary>
        /// <param name="material">Nazwa wymaganego materiału.</param>
        /// <param name="baseCost">Bazowy koszt materiału.</param>
        /// <param name="quality">Jakość tworzonego przedmiotu wpływająca na koszt.</param>
        /// <returns>True, jeśli gracz posiada wystarczającą ilość materiałów; w przeciwnym razie false.</returns>
        private bool HasEnoughMaterialsForPart(string material, int baseCost, Quality quality = Quality.Normal)
        {
            var costMultiplier = quality switch
            {
                Quality.Weak => 0.5,
                Quality.Normal => 1,
                Quality.Excellent => 2,
                Quality.Masterpiece => 2,
                _ => 1
            };
            
            int requiredAmount = (int)Math.Ceiling(baseCost * costMultiplier);
            return PlayerHandler.player.Inventory.GetCount(material) >= requiredAmount;
        }
        
        /// <summary>
        /// Ładuje dostępne komponenty broni do odpowiednich kontrolek ComboBox.
        /// Filtruje komponenty według klasy postaci i dostępnych materiałów.
        /// </summary>
        private void LoadWeaponParts()
        {
            // Clear existing items
            WeaponHeadCombo.Items.Clear();
            var weaponHeads = EquipmentPartManager.WeaponHeads
                .Where(x => x.IntendedClass == _characterClass && x.Material != "None")
                .OrderBy(x => x.Tier)
                .ThenBy(x => x.Alias);
            
            var costMultiplier = GetSelectedQuality() switch
            {
                Quality.Weak => 0.5,
                Quality.Normal => 1,
                Quality.Excellent => 2,
                Quality.Masterpiece => 2,
                _ => 1
            };
                
            foreach (var head in weaponHeads)
            {
                bool hasMaterials = HasEnoughMaterialsForPart(head.Material, head.MaterialCost, GetSelectedQuality());
                var item = new ComboBoxItem
                {
                    Content = head.DescriptionText(costMultiplier),
                    Tag = head,
                    IsEnabled = hasMaterials,
                    ToolTip = hasMaterials ? null : "Not enough materials"
                };
                WeaponHeadCombo.Items.Add(item);
            }
            
            WeaponBinderCombo.Items.Clear();
            var weaponBinders = EquipmentPartManager.WeaponBinders
                .Where(x => x.IntendedClass == _characterClass && x.Material != "None")
                .OrderBy(x => x.Tier)
                .ThenBy(x => x.DescriptionText(costMultiplier));
                
            foreach (var binder in weaponBinders)
            {
                bool hasMaterials = HasEnoughMaterialsForPart(binder.Material, binder.MaterialCost, GetSelectedQuality());
                var item = new ComboBoxItem
                {
                    Content = binder.DescriptionText(costMultiplier),
                    Tag = binder,
                    IsEnabled = hasMaterials,
                    ToolTip = hasMaterials ? null : "Not enough materials"
                };
                WeaponBinderCombo.Items.Add(item);
            }
            
            WeaponHandleCombo.Items.Clear();
            var weaponHandles = EquipmentPartManager.WeaponHandles
                .Where(x => x.IntendedClass == _characterClass && x.Material != "None")
                .OrderBy(x => x.Tier)
                .ThenBy(x => x.DescriptionText(costMultiplier));
                
            foreach (var handle in weaponHandles)
            {
                bool hasMaterials = HasEnoughMaterialsForPart(handle.Material, handle.MaterialCost, GetSelectedQuality());
                var item = new ComboBoxItem
                {
                    Content = handle.DescriptionText(costMultiplier),
                    Tag = handle,
                    IsEnabled = hasMaterials,
                    ToolTip = hasMaterials ? null : "Not enough materials"
                };
                WeaponHandleCombo.Items.Add(item);
            }
            
            // Select first enabled item in each combo if available
            SelectFirstEnabledItem(WeaponHeadCombo);
            SelectFirstEnabledItem(WeaponBinderCombo);
            SelectFirstEnabledItem(WeaponHandleCombo);
            
            UpdateCraftButtonState();
        }
        
        /// <summary>
        /// Wybiera pierwszy włączony element w podanym ComboBox.
        /// </summary>
        /// <param name="comboBox">Kontrolka ComboBox, w której ma zostać wybrany element.</param>
        private void SelectFirstEnabledItem(ComboBox comboBox)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.IsEnabled)
                {
                    comboBox.SelectedItem = item;
                    return;
                }
            }
            comboBox.SelectedIndex = -1;
        }
        
        /// <summary>
        /// Ładuje dostępne komponenty zbroi do odpowiednich kontrolek ComboBox.
        /// Filtruje komponenty według klasy postaci i dostępnych materiałów.
        /// </summary>
        private void LoadArmorParts()
        {
            
            // Clear existing items
            ArmorPlateCombo.Items.Clear();
            var armorPlates = EquipmentPartManager.ArmorPlates
                .Where(x => x.IntendedClass == _characterClass && x.Material != "None")
                .OrderBy(x => x.Tier)
                .ThenBy(x => x.Alias);
            
            var costMultiplier = GetSelectedQuality() switch
            {
                Quality.Weak => 0.5,
                Quality.Normal => 1,
                Quality.Excellent => 2,
                Quality.Masterpiece => 2,
                _ => 1
            };
                
            foreach (var plate in armorPlates)
            {
                bool hasMaterials = HasEnoughMaterialsForPart(plate.Material, plate.MaterialCost, GetSelectedQuality());
                var item = new ComboBoxItem
                {
                    Content = plate.DescriptionText(costMultiplier),
                    Tag = plate,
                    IsEnabled = hasMaterials,
                    ToolTip = hasMaterials ? null : "Not enough materials"
                };
                ArmorPlateCombo.Items.Add(item);
            }
            
            // Add armor binders for the character's class
            ArmorBinderCombo.Items.Clear();
            var armorBinders = EquipmentPartManager.ArmorBinders
                .Where(x => x.IntendedClass == _characterClass && x.Material != "None")
                .OrderBy(x => x.Tier)
                .ThenBy(x => x.DescriptionText(costMultiplier));
                
            foreach (var binder in armorBinders)
            {
                bool hasMaterials = HasEnoughMaterialsForPart(binder.Material, binder.MaterialCost, GetSelectedQuality());
                var item = new ComboBoxItem
                {
                    Content = binder.DescriptionText(costMultiplier),
                    Tag = binder,
                    IsEnabled = hasMaterials,
                    ToolTip = hasMaterials ? null : "Not enough materials"
                };
                ArmorBinderCombo.Items.Add(item);
            }
            
            // Add armor bases for the character's class
            ArmorBaseCombo.Items.Clear();
            var armorBases = EquipmentPartManager.ArmorBases
                .Where(x => x.IntendedClass == _characterClass && x.Material != "None")
                .OrderBy(x => x.Tier)
                .ThenBy(x => x.DescriptionText(costMultiplier));
                
            foreach (var baseItem in armorBases)
            {
                bool hasMaterials = HasEnoughMaterialsForPart(baseItem.Material, baseItem.MaterialCost, GetSelectedQuality());
                var item = new ComboBoxItem
                {
                    Content = baseItem.DescriptionText(costMultiplier),
                    Tag = baseItem,
                    IsEnabled = hasMaterials,
                    ToolTip = hasMaterials ? null : "Not enough materials"
                };
                ArmorBaseCombo.Items.Add(item);
            }
            
            // Select first enabled item in each combo if available
            SelectFirstEnabledItem(ArmorPlateCombo);
            SelectFirstEnabledItem(ArmorBinderCombo);
            SelectFirstEnabledItem(ArmorBaseCombo);
            
            UpdateCraftButtonState();
        }
        
        /// <summary>
        /// Obsługuje zmianę wybranego typu przedmiotu (broń lub zbroja).
        /// Aktualizuje widok i stan przycisku tworzenia.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void ItemType_Checked(object sender, RoutedEventArgs e)
        {
            if (WeaponPartsTab == null || ArmorPartsTab == null) return;
            
            PartsTabControl.SelectedItem = WeaponPartsTab.IsSelected ? WeaponPartsTab : ArmorPartsTab;
            
            UpdateCraftButtonState();
        }
        
        /// <summary>
        /// Obsługuje zmianę wyboru komponentu broni.
        /// Aktualizuje statystyki i stan przycisku tworzenia.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia zmiany wyboru.</param>
        private void WeaponPart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is ComboBoxItem selectedItem && !selectedItem.IsEnabled)
            {
                // Don't allow selecting disabled items
                ((ComboBox)sender).SelectedItem = e.RemovedItems.Count > 0 ? e.RemovedItems[0] : null;
                return;
            }
            
            UpdateCraftButtonState();
            UpdateTier();
            UpdateWeaponStats(sender, e);
        }
        
        /// <summary>
        /// Obsługuje zmianę wyboru komponentu zbroi.
        /// Aktualizuje statystyki i stan przycisku tworzenia.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia zmiany wyboru.</param>
        private void ArmorPart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is ComboBoxItem selectedItem && !selectedItem.IsEnabled)
            {
                // Don't allow selecting disabled items
                ((ComboBox)sender).SelectedItem = e.RemovedItems.Count > 0 ? e.RemovedItems[0] : null;
                return;
            }
            
            UpdateCraftButtonState();
            UpdateTier();
        }
        
        /// <summary>
        /// Aktualizuje poziom przedmiotu na podstawie wybranych komponentów.
        /// Dla broni oblicza średnią z poziomów komponentów, a dla zbroi wybiera najwyższy poziom.
        /// </summary>
        private void UpdateTier()
        {
            if (WeaponPartsTab.IsSelected)
            {
                var head = (WeaponHeadCombo.SelectedItem as ComboBoxItem)?.Tag as WeaponHead;
                var binder = (WeaponBinderCombo.SelectedItem as ComboBoxItem)?.Tag as WeaponBinder;
                var handle = (WeaponHandleCombo.SelectedItem as ComboBoxItem)?.Tag as WeaponHandle;
                
                if (head != null && binder != null && handle != null)
                {
                    _currentTier = (head.Tier + binder.Tier + handle.Tier) / 3;
                }
            }
            else
            {
                var plate = (ArmorPlateCombo.SelectedItem as ComboBoxItem)?.Tag as ArmorPlate;
                var binder = (ArmorBinderCombo.SelectedItem as ComboBoxItem)?.Tag as ArmorBinder;
                var baseItem = (ArmorBaseCombo.SelectedItem as ComboBoxItem)?.Tag as ArmorBase;
                
                if (plate != null && binder != null && baseItem != null)
                {
                    _currentTier = Math.Max(Math.Max(plate.Tier, binder.Tier), baseItem.Tier);
                }
            }
        }
        
        /// <summary>
        /// Aktualizuje wyświetlane statystyki broni na podstawie wybranych komponentów i jakości.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void UpdateWeaponStats(object sender, RoutedEventArgs e)
        {
            if (!WeaponPartsTab.IsSelected) return;
            
            var head = (WeaponHeadCombo.SelectedItem as ComboBoxItem)?.Tag as WeaponHead;
            var binder = (WeaponBinderCombo.SelectedItem as ComboBoxItem)?.Tag as WeaponBinder;
            var handle = (WeaponHandleCombo.SelectedItem as ComboBoxItem)?.Tag as WeaponHandle;
            
            if (head == null || binder == null || handle == null)
            {
                // Clear stats if any part is missing
                DamageText.Text = "";
                AttackSpeedText.Text = "";
                CriticalChanceText.Text = "";
                RequiredLevelText.Text = "";
                ValueText.Text = "";
                return;
            }

            // Get quality multiplier
            float qualityMultiplier = 0f;
            if (WeakQualityRadio.IsChecked == true) qualityMultiplier = -1f;
            else if (NormalQualityRadio.IsChecked == true) qualityMultiplier = 0f;
            else if (ExcellentQualityRadio.IsChecked == true) qualityMultiplier = 1f;

            // Calculate damage range
            int minDamage = CalculateMinDamage(head, binder, handle, qualityMultiplier);
            int maxDamage = CalculateMaxDamage(head, binder, handle, qualityMultiplier);
            
            // Calculate critical chance
            double critChance = CalculateCriticalChance(binder, qualityMultiplier);
            
            // Calculate critical modifier
            double critMod = CalculateCriticalModifier(head, binder, handle, qualityMultiplier);
            
            // Calculate accuracy
            int accuracy = CalculateAccuracy(handle, qualityMultiplier);
            
            // Calculate required level
            int requiredLevel = Math.Max(Math.Max(head.Tier, binder.Tier), handle.Tier) * 10 - 5;
            if (WeakQualityRadio.IsChecked == true) requiredLevel -= 3;
            else if (ExcellentQualityRadio.IsChecked == true) requiredLevel += 3;
            
            // Calculate value
            int value = (int)((head.MaterialCost * ItemManager.GetItem(head.Material).Cost + 
                              binder.MaterialCost * ItemManager.GetItem(binder.Material).Cost + 
                              handle.MaterialCost * ItemManager.GetItem(handle.Material).Cost) * 
                             (WeakQualityRadio.IsChecked == true ? 0.5 : 
                              ExcellentQualityRadio.IsChecked == true ? 2.0 : 1.0));
            
            // Update UI
            DamageText.Text = $"Damage: {minDamage}-{maxDamage}";
            CriticalChanceText.Text = $"Critical: {critChance:F1}% ({critMod:F1}x)";
            AttackSpeedText.Text = $"Accuracy: {accuracy}";
            RequiredLevelText.Text = $"Required Level: {requiredLevel}";
            ValueText.Text = $"Value: {value} gold";
        }
        
        /// <summary>
        /// Oblicza minimalne obrażenia broni na podstawie komponentów i jakości.
        /// </summary>
        /// <param name="head">Głowica broni.</param>
        /// <param name="binder">Oprawa broni.</param>
        /// <param name="handle">Rękojeść broni.</param>
        /// <param name="qualityMultiplier">Mnożnik jakości (ujemny dla słabej, dodatni dla doskonałej).</param>
        /// <returns>Wartość minimalnych obrażeń.</returns>
        private int CalculateMinDamage(WeaponHead head, WeaponBinder binder, WeaponHandle handle, float qualityMultiplier)
        {
            double value = head.MinimalAttack;
            double multiplier = _characterClass switch
            {
                CharacterClass.Warrior => 1,
                CharacterClass.Scout => 0.4,
                CharacterClass.Sorcerer => 0.8,
                _ => 0.4
            };
            multiplier *= qualityMultiplier * (head.Tier * 10 - 5);
            value += multiplier;
            value *= (handle.AttackBonus + binder.AttackBonus + 1);
            return (int)value;
        }
        
        /// <summary>
        /// Oblicza maksymalne obrażenia broni na podstawie komponentów i jakości.
        /// </summary>
        /// <param name="head">Głowica broni.</param>
        /// <param name="binder">Oprawa broni.</param>
        /// <param name="handle">Rękojeść broni.</param>
        /// <param name="qualityMultiplier">Mnożnik jakości (ujemny dla słabej, dodatni dla doskonałej).</param>
        /// <returns>Wartość maksymalnych obrażeń.</returns>
        private int CalculateMaxDamage(WeaponHead head, WeaponBinder binder, WeaponHandle handle, float qualityMultiplier)
        {
            double value = head.MaximalAttack;
            double multiplier = _characterClass switch
            {
                CharacterClass.Warrior => 2,
                CharacterClass.Scout => 2,
                CharacterClass.Sorcerer => 2.4,
                _ => 1.2
            };
            multiplier *= qualityMultiplier * (head.Tier * 10 - 5);
            value += multiplier;
            value *= (handle.AttackBonus + binder.AttackBonus + 1);
            return (int)value;
        }
        
        /// <summary>
        /// Oblicza szansę na trafienie krytyczne na podstawie oprawy i jakości.
        /// </summary>
        /// <param name="binder">Oprawa broni.</param>
        /// <param name="qualityMultiplier">Mnożnik jakości (ujemny dla słabej, dodatni dla doskonałej).</param>
        /// <returns>Szansa na trafienie krytyczne w procentach.</returns>
        private double CalculateCriticalChance(WeaponBinder binder, float qualityMultiplier)
        {
            double value = binder.CritChance;
            double multiplier = _characterClass switch
            {
                CharacterClass.Warrior => 0.01,
                CharacterClass.Scout => 0.01,
                CharacterClass.Sorcerer => 0.05,
                _ => 0.01
            };
            multiplier *= qualityMultiplier;
            if (_characterClass == CharacterClass.Sorcerer)
                multiplier *= (binder.Tier * 10 - 5);
            value += multiplier;
            return value * 100; // Convert to percentage
        }
        
        /// <summary>
        /// Oblicza mnożnik obrażeń krytycznych na podstawie komponentów i jakości.
        /// </summary>
        /// <param name="head">Głowica broni.</param>
        /// <param name="binder">Oprawa broni.</param>
        /// <param name="handle">Rękojeść broni.</param>
        /// <param name="qualityMultiplier">Mnożnik jakości (ujemny dla słabej, dodatni dla doskonałej).</param>
        /// <returns>Wartość mnożnika obrażeń krytycznych.</returns>
        private double CalculateCriticalModifier(WeaponHead head, WeaponBinder binder, WeaponHandle handle, float qualityMultiplier)
        {
            double value = head.CritMod;
            double multiplier = _characterClass switch
            {
                CharacterClass.Warrior => 0.02,
                CharacterClass.Scout => 0.03,
                CharacterClass.Sorcerer => 0.02,
                _ => 0.02
            };
            multiplier *= qualityMultiplier * (head.Tier * 10 - 5);
            value += multiplier;
            value *= (binder.CritModBonus + handle.CritModBonus + 1);
            return value;
        }
        
        /// <summary>
        /// Oblicza celność broni na podstawie rękojeści i jakości.
        /// </summary>
        /// <param name="handle">Rękojeść broni.</param>
        /// <param name="qualityMultiplier">Mnożnik jakości (ujemny dla słabej, dodatni dla doskonałej).</param>
        /// <returns>Wartość celności.</returns>
        private int CalculateAccuracy(WeaponHandle handle, float qualityMultiplier)
        {
            double value = handle.Accuracy;
            double multiplier = _characterClass switch
            {
                CharacterClass.Warrior => 1,
                CharacterClass.Scout => 1,
                CharacterClass.Sorcerer => 0.3,
                _ => 2
            };
            multiplier *= qualityMultiplier;
            if (_characterClass == CharacterClass.Sorcerer)
                multiplier *= (handle.Tier * 10 - 5);
            value += multiplier;
            return (int)value;
        }
        
        /// <summary>
        /// Aktualizuje stan przycisku tworzenia przedmiotu.
        /// Przycisk jest aktywny tylko gdy wybrano wszystkie wymagane komponenty.
        /// </summary>
        private void UpdateCraftButtonState()
        {
            bool canCraft = false;
            
            if (WeaponPartsTab.IsSelected)
            {
                canCraft = WeaponHeadCombo.SelectedItem is ComboBoxItem headItem && headItem.IsEnabled &&
                          WeaponBinderCombo.SelectedItem is ComboBoxItem binderItem && binderItem.IsEnabled &&
                          WeaponHandleCombo.SelectedItem is ComboBoxItem handleItem && handleItem.IsEnabled;
            }
            else
            {
                canCraft = ArmorPlateCombo.SelectedItem is ComboBoxItem plateItem && plateItem.IsEnabled &&
                          ArmorBinderCombo.SelectedItem is ComboBoxItem binderItem && binderItem.IsEnabled &&
                          ArmorBaseCombo.SelectedItem is ComboBoxItem baseItem && baseItem.IsEnabled;
            }
            
            CraftButton.IsEnabled = canCraft;
        }
        
        /// <summary>
        /// Pobiera wybraną jakość przedmiotu z kontrolek radiowych.
        /// </summary>
        /// <returns>Wybrana jakość przedmiotu.</returns>
        private Quality GetSelectedQuality()
        {
            if (WeakQualityRadio.IsChecked == true) return Quality.Weak;
            if (ExcellentQualityRadio.IsChecked == true) return Quality.Excellent;
            return Quality.Normal;
        }
        
        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku tworzenia przedmiotu.
        /// Tworzy nowy przedmiot na podstawie wybranych komponentów i jakości.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void CraftButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var quality = GetSelectedQuality();
                var itemName = string.IsNullOrWhiteSpace(ItemNameTextBox.Text) ? "Custom Item" : ItemNameTextBox.Text;
                
                if (WeaponPartsTab.IsSelected)
                {
                    var head = (WeaponHeadCombo.SelectedItem as ComboBoxItem)?.Tag as WeaponHead;
                    var binder = (WeaponBinderCombo.SelectedItem as ComboBoxItem)?.Tag as WeaponBinder;
                    var handle = (WeaponHandleCombo.SelectedItem as ComboBoxItem)?.Tag as WeaponHandle;
                    
                    if (head != null && binder != null && handle != null)
                    {
                        var weapon = CraftingManager.CraftWeapon(head, binder, handle, quality, itemName);
                        StatusText.Text = $"Successfully crafted {weapon.Name}!";
                        StatusText.Foreground = System.Windows.Media.Brushes.Green;
                        
                        // Close the dialog after a short delay
                        var timer = new System.Windows.Threading.DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(1.5);
                        timer.Tick += (s, args) =>
                        {
                            timer.Stop();
                            DialogResult = true;
                            Close();
                        };
                        timer.Start();
                    }
                }
                else
                {
                    var plate = (ArmorPlateCombo.SelectedItem as ComboBoxItem)?.Tag as ArmorPlate;
                    var binder = (ArmorBinderCombo.SelectedItem as ComboBoxItem)?.Tag as ArmorBinder;
                    var baseItem = (ArmorBaseCombo.SelectedItem as ComboBoxItem)?.Tag as ArmorBase;
                    
                    if (plate != null && binder != null && baseItem != null)
                    {
                        var armor = CraftingManager.CraftArmor(plate, binder, baseItem, quality, itemName);
                        StatusText.Text = $"Successfully crafted {armor.Name}!";
                        StatusText.Foreground = System.Windows.Media.Brushes.Green;
                        
                        // Close the dialog after a short delay
                        var timer = new System.Windows.Threading.DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(1.5);
                        timer.Tick += (s, args) =>
                        {
                            timer.Stop();
                            DialogResult = true;
                            Close();
                        };
                        timer.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error crafting item: {ex.Message}";
                StatusText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
    }
}
