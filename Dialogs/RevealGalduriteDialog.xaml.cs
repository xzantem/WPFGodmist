using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using GodmistWPF.Characters.Player;
using GodmistWPF.Items.Galdurites;

namespace GodmistWPF.Dialogs
{
    /// <summary>
    /// Okno dialogowe umożliwiające ujawnianie właściwości nieznanych galduritów.
    /// Pozwala graczowi zapłacić za ujawnienie właściwości galduritu przed jego użyciem.
    /// </summary>
    public partial class RevealGalduriteDialog : Window, INotifyPropertyChanged
    {
        /// <summary>Kolekcja nieujawnionych jeszcze galduritów.</summary>
        private readonly ObservableCollection<Galdurite> _unrevealedGaldurites;
        
        /// <summary>Aktualnie wybrany galdurit do ujawnienia.</summary>
        private Galdurite _selectedGaldurite;
        
        /// <summary>Modyfikator kosztu usługi ujawniania galduritów.</summary>
        private readonly double _serviceCostMod;
        
        /// <summary>Tekst statusu wyświetlany użytkownikowi.</summary>
        private string _statusText;
        
        /// <summary>Ilość złota posiadanego przez gracza.</summary>
        private int _playerGold;
        
        /// <summary>Koszt ujawnienia wybranego galduritu.</summary>
        private int _revealCost;

        /// <summary>Zdarzenie wywoływane przy zmianie wartości właściwości.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Pobiera lub ustawia ilość złota posiadanego przez gracza.
        /// Automatycznie powiadamia o zmianie wartości właściwości.
        /// </summary>
        public int PlayerGold
        {
            get => _playerGold;
            private set
            {
                _playerGold = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Pobiera lub ustawia tekst statusu wyświetlany użytkownikowi.
        /// Automatycznie powiadamia o zmianie wartości właściwości.
        /// </summary>
        public string StatusText
        {
            get => _statusText;
            private set
            {
                _statusText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Pobiera lub ustawia koszt ujawnienia wybranego galduritu.
        /// Automatycznie powiadamia o zmianie wartości właściwości.
        /// </summary>
        public int RevealCost
        {
            get => _revealCost;
            private set
            {
                _revealCost = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Pobiera informację czy gracz może sobie pozwolić na ujawnienie wybranego galduritu.
        /// Wartość true oznacza, że gracz posiada wystarczającą ilość złota.
        /// </summary>
        public bool CanAffordReveal => PlayerGold >= RevealCost;

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="RevealGalduriteDialog">.
        /// </summary>
        /// <param name="unrevealedGaldurites">Lista nieujawnionych galduritów do wyświetlenia.</param>
        /// <param name="serviceCostMod">Modyfikator kosztu usługi ujawniania galduritów.</param>
        public RevealGalduriteDialog(List<Galdurite> unrevealedGaldurites, double serviceCostMod)
        {
            InitializeComponent();
            DataContext = this;

            _unrevealedGaldurites = new ObservableCollection<Galdurite>(unrevealedGaldurites);
            _serviceCostMod = serviceCostMod;
            
            // Initialize UI
            UpdatePlayerGold();
            StatusText = "Select a galdurite to reveal its properties";
            
            // Set up the list
            GalduriteList.ItemsSource = _unrevealedGaldurites;
        }

        /// <summary>
        /// Aktualizuje wartość właściwości PlayerGold na podstawie stanu konta gracza.
        /// </summary>
        private void UpdatePlayerGold()
        {
            PlayerGold = PlayerHandler.player.Gold;
        }

        /// <summary>
        /// Oblicza koszt ujawnienia właściwości galduritu na podstawie jego wartości i modyfikatorów.
        /// </summary>
        /// <param name="galdurite">Galdurit, którego koszt ujawnienia ma zostać obliczony.</param>
        /// <returns>Koszt ujawnienia galduritu w złocie.</returns>
        private int CalculateRevealCost(Galdurite galdurite)
        {
            return (int)(PlayerHandler.HonorDiscountModifier * _serviceCostMod * 0.2 * galdurite.Cost);
        }

        /// <summary>
        /// Obsługuje zmianę wyboru galduritu na liście.
        /// Aktualizuje interfejs użytkownika i oblicza koszt ujawnienia wybranego galduritu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (ListBox).</param>
        /// <param name="e">Dane zdarzenia zmiany wyboru.</param>
        private void GalduriteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedGaldurite = GalduriteList.SelectedItem as Galdurite;
            
            if (_selectedGaldurite != null)
            {
                RevealCost = CalculateRevealCost(_selectedGaldurite);
                StatusText = $"Selected: {_selectedGaldurite.Name}";
                RevealButton.IsEnabled = true;
            }
            else
            {
                RevealCost = 0;
                StatusText = "Select a galdurite to reveal its properties";
                RevealButton.IsEnabled = false;
            }
            
            // Aktualizacja właściwości CanAffordReveal
            OnPropertyChanged(nameof(CanAffordReveal));
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku ujawniającego właściwości galduritu.
        /// Sprawdza dostępność środków, pobiera opłatę i ujawnia właściwości galduritu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void RevealButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGaldurite == null) return;
            
            var player = PlayerHandler.player;
            
            if (!CanAffordReveal)
            {
                StatusText = "Not enough gold to reveal this galdurite";
                return;
            }
            
            try
            {
                // Pobranie opłaty i ujawnienie galduritu
                player.Gold -= RevealCost;
                _selectedGaldurite.Reveal();
                _selectedGaldurite.Inspect();
                
                // Aktualizacja interfejsu użytkownika
                UpdatePlayerGold();
                _unrevealedGaldurites.Remove(_selectedGaldurite);
                
                // Wyświetlenie komunikatu o sukcesie
                StatusText = $"Revealed: {_selectedGaldurite.Name}";
                _selectedGaldurite = null;
                GalduriteList.SelectedItem = null;
                
                // Zamknięcie okna, jeśli nie ma więcej galduritów do ujawnienia
                if (_unrevealedGaldurites.Count == 0)
                {
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                StatusText = "Error revealing galdurite";
                Debug.WriteLine($"Error revealing galdurite: {ex.Message}");
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku anulowania.
        /// Zamyka okno z wynikiem DialogResult ustawionym na false.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Wywołuje zdarzenie PropertyChanged.
        /// </summary>
        /// <param name="propertyName">Nazwa zmienionej właściwości. Jeśli nie podana, zostanie użyta nazwa wywołującej właściwości.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
