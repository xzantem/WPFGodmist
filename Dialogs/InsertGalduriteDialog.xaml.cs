using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Items;
using GodmistWPF.Items.Equippable;
using GodmistWPF.Items.Equippable.Armors;
using GodmistWPF.Items.Equippable.Weapons;
using GodmistWPF.Items.Galdurites;

namespace GodmistWPF.Dialogs
{
    /// <summary>
    /// Okno dialogowe umożliwiające wstawianie galduritów do przedmiotów wyposażenia.
    /// Zawiera listę dostępnych galduritów i oblicza koszt wstawienia.
    /// </summary>
    public partial class InsertGalduriteDialog : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Przedmiot docelowy, do którego ma zostać wstawiony galdurit.
        /// </summary>
        private readonly IEquippable _targetItem;

        /// <summary>
        /// Modyfikator kosztu usługi wstawienia galduritu.
        /// </summary>
        private readonly double _serviceCostMod;

        /// <summary>
        /// Aktualnie wybrany galdurit do wstawienia.
        /// </summary>
        private Galdurite _selectedGaldurite;

        /// <summary>
        /// Tekst statusu wyświetlany w interfejsie.
        /// </summary>
        private string _statusText = "Select a galdurite to insert";

        /// <summary>
        /// Koszt wstawienia wybranego galduritu.
        /// </summary>
        private int _insertCost;

        /// <summary>
        /// Zdarzenie wywoływane przy zmianie wartości właściwości.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Pobiera nazwę przedmiotu docelowego.
        /// </summary>
        public string TargetItemName => _targetItem.Name;

        /// <summary>
        /// Pobiera informację o zajętych i dostępnych slotach galduritów w przedmiocie.
        /// </summary>
        public string TargetItemSlots => $"({_targetItem.Galdurites.Count}/{_targetItem.GalduriteSlots} slots used)";

        /// <summary>
        /// Kolekcja dostępnych do wstawienia galduritów.
        /// </summary>
        public ObservableCollection<Galdurite> AvailableGaldurites { get; } = new ObservableCollection<Galdurite>();

        /// <summary>
        /// Pobiera lub ustawia tekst statusu wyświetlany w interfejsie.
        /// Automatycznie powiadamia o zmianie właściwości.
        /// </summary>
        public string StatusText
        {
            get => _statusText;
            set
            {
                _statusText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Pobiera lub ustawia koszt wstawienia galduritu.
        /// Automatycznie aktualizuje wyświetlany koszt.
        /// </summary>
        public int InsertCost
        {
            get => _insertCost;
            set
            {
                _insertCost = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(InsertCostText));
            }
        }

        /// <summary>
        /// Pobiera sformatowany tekst z kosztem wstawienia galduritu.
        /// </summary>
        public string InsertCostText => $"Cost: {InsertCost:N0} Gold";

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="InsertGalduriteDialog"/>.
        /// </summary>
        /// <param name="targetItem">Przedmiot, do którego ma zostać wstawiony galdurit.</param>
        /// <param name="availableGaldurites">Lista dostępnych galduritów.</param>
        /// <param name="serviceCostMod">Modyfikator kosztu usługi.</param>
        public InsertGalduriteDialog(IEquippable targetItem, List<Galdurite> availableGaldurites, double serviceCostMod)
        {
            _targetItem = targetItem;
            _serviceCostMod = serviceCostMod;

            InitializeComponent();
            DataContext = this;
            
            // Filtruj i sortuj dostępne galdurity
            var validGaldurites = availableGaldurites
                .Where(g => g.Revealed && 
                           _targetItem.Rarity >= g.Rarity && 
                           _targetItem.RequiredLevel >= g.RequiredLevel)
                .OrderBy(g => g.Rarity)
                .ThenBy(g => g.RequiredLevel)
                .ToList();
            
            foreach (var galdurite in validGaldurites)
            {
                AvailableGaldurites.Add(galdurite);
            }
            
            if (!AvailableGaldurites.Any())
            {
                StatusText = "No compatible galdurites found";
                InsertButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Obsługuje zmianę wyboru galduritu na liście.
        /// Aktualizuje interfejs i wyświetla informacje o wybranym galduricie.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (lista galduritów).</param>
        /// <param name="e">Dane zdarzenia zmiany wyboru.</param>
        private void GalduriteList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedGaldurite = GalduriteList.SelectedItem as Galdurite;
            InsertButton.IsEnabled = _selectedGaldurite != null;
            
            if (_selectedGaldurite != null)
            {
                InsertCost = CalculateInsertCost(_selectedGaldurite);
                StatusText = $"Selected: {_selectedGaldurite.Name} - {_selectedGaldurite.ShowEffects()}";
            }
            else
            {
                InsertCost = 0;
                StatusText = "Select a galdurite to insert";
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku wstawienia galduritu.
        /// Sprawdza dostępne środki, dokonuje płatności i wstawia galdurit do przedmiotu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGaldurite == null) return;
            
            var player = PlayerHandler.player;
            var cost = CalculateInsertCost(_selectedGaldurite);
            
            if (player.Gold < cost)
            {
                StatusText = $"Not enough gold! You need {cost - player.Gold} more gold.";
                return;
            }
            
            try
            {
                // Odejmij złoto i wstaw galdurit
                player.Gold -= cost;
                if (_targetItem is Weapon weapon)
                    weapon.AddGaldurite(_selectedGaldurite);
                else if (_targetItem is Armor armor)
                    armor.AddGaldurite(_selectedGaldurite);
                else 
                    throw new Exception("Unknown equippable type.");
                
                player.Inventory.TryRemoveItem(_selectedGaldurite);
                
                // Aktualizuj interfejs użytkownika
                AvailableGaldurites.Remove(_selectedGaldurite);
                _selectedGaldurite = null;
                InsertButton.IsEnabled = false;
                
                if (AvailableGaldurites.Count == 0)
                {
                    StatusText = "No more galdurites available to insert";
                    InsertButton.IsEnabled = false;
                }
                
                // Aktualizuj wyświetlanie zajętych slotów
                OnPropertyChanged(nameof(TargetItemSlots));
                
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
                Debug.WriteLine($"Error inserting galdurite: {ex}");
            }
        }

        /// <summary>
        /// Oblicza koszt wstawienia galduritu na podstawie jego wartości i modyfikatorów.
        /// </summary>
        /// <param name="galdurite">Galdurit, dla którego obliczany jest koszt.</param>
        /// <returns>Koszt wstawienia galduritu w złocie.</returns>
        private int CalculateInsertCost(Galdurite galdurite)
        {
            return (int)(PlayerHandler.HonorDiscountModifier * _serviceCostMod * galdurite.Cost * 0.75);
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku anulowania.
        /// Zamyka okno dialogowe bez wstawiania galduritu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Wywołuje zdarzenie zmiany właściwości.
        /// </summary>
        /// <param name="propertyName">Nazwa zmienionej właściwości (automatycznie wypełniana przez kompilator).</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
