using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using GodmistWPF.Items;
using GodmistWPF.Items.Equippable;

namespace GodmistWPF.Dialogs
{
    /// <summary>
    /// Okno dialogowe umożliwiające ulepszanie przedmiotów w grze.
    /// Pozwala na wybór poziomu ulepszenia, szansy na sukces i wyświetla koszt operacji.
    /// </summary>
    public partial class UpgradeItemDialog : Window, INotifyPropertyChanged
    {
        /// <summary>Przedmiot poddawany ulepszeniu.</summary>
        private readonly IEquippable _item;
        
        /// <summary>Ilość złota, którą posiada gracz.</summary>
        private readonly double _playerGold;
        
        /// <summary>Nowa wartość modyfikatora ulepszenia.</summary>
        private double _newModifier;
        
        /// <summary>Szansa na udane ulepszenie (wartość od 0 do 1).</summary>
        private double _successChance;
        
        /// <summary>Koszt ulepszenia w złocie.</summary>
        private double _upgradeCost;
        
        /// <summary>Komunikat statusu wyświetlany użytkownikowi.</summary>
        private string _statusMessage = string.Empty;
        
        /// <summary>Określa, czy gracz może sobie pozwolić na ulepszenie.</summary>
        private bool _canAffordUpgrade;

        /// <summary>Pobiera nazwę przedmiotu podlegającego ulepszeniu.</summary>
        public string ItemName => _item?.Name ?? "Unknown Item";
        
        /// <summary>Pobiera aktualną wartość modyfikatora ulepszenia przedmiotu.</summary>
        public double CurrentModifier => _item?.UpgradeModifier ?? 0.0;

        /// <summary>
        /// Pobiera lub ustawia nową wartość modyfikatora ulepszenia.
        /// Automatycznie aktualizuje koszt ulepszenia przy zmianie wartości.
        /// </summary>
        public double NewModifier
        {
            get => _newModifier;
            set
            {
                if (SetField(ref _newModifier, value))
                {
                    UpdateUpgradeCost();
                }
            }
        }

        /// <summary>
        /// Pobiera lub ustawia szansę na udane ulepszenie (wartość od 0 do 1).
        /// Automatycznie aktualizuje koszt ulepszenia przy zmianie wartości.
        /// </summary>
        public double SuccessChance
        {
            get => _successChance;
            set
            {
                if (SetField(ref _successChance, value))
                {
                    UpdateUpgradeCost();
                }
            }
        }

        /// <summary>Pobiera koszt ulepszenia wyrażony w złocie.</summary>
        public double UpgradeCost
        {
            get => _upgradeCost;
            private set => SetField(ref _upgradeCost, value);
        }

        /// <summary>Pobiera informację, czy gracz może sobie pozwolić na ulepszenie.</summary>
        public bool CanAffordUpgrade
        {
            get => _canAffordUpgrade;
            private set => SetField(ref _canAffordUpgrade, value);
        }

        /// <summary>Pobiera komunikat statusu dotyczący operacji ulepszania.</summary>
        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetField(ref _statusMessage, value);
        }

        /// <summary>
        /// Inicjalizuje nową instancję okna dialogowego ulepszania przedmiotu.
        /// </summary>
        /// <param name="item">Przedmiot do ulepszenia.</param>
        /// <param name="initialModifier">Początkowa wartość modyfikatora ulepszenia.</param>
        /// <param name="initialChance">Początkowa szansa na udane ulepszenie (0-1).</param>
        /// <param name="initialCost">Początkowy koszt ulepszenia.</param>
        /// <param name="playerGold">Ilość złota, którą posiada gracz.</param>
        /// <exception cref="ArgumentNullException">Gdy parametr item jest null.</exception>
        public UpgradeItemDialog(IEquippable item, double initialModifier, double initialChance, double initialCost, double playerGold)
        {
            _item = item ?? throw new ArgumentNullException(nameof(item));
            _playerGold = playerGold;
            _newModifier = Math.Round(initialModifier, 1);
            _successChance = Math.Round(initialChance, 1);
            _upgradeCost = initialCost;
            
            InitializeComponent();
            DataContext = this;
            
            UpdateUpgradeCost();
        }

        /// <summary>
        /// Aktualizuje koszt ulepszenia na podstawie aktualnych ustawień.
        /// Sprawdza również, czy gracz może sobie pozwolić na ulepszenie
        /// i aktualizuje odpowiednie komunikaty.
        /// </summary>
        private void UpdateUpgradeCost()
        {
            if (_item == null) return;
            
            // Obliczanie kosztu ulepszenia na podstawie formuły
            UpgradeCost = (1 + _item.Cost) / 2.0 * 
                          ((7 * _newModifier + 3) / (12 - 11 * _newModifier) * ((57 - 37 * _successChance) / (76 - 75 * _successChance)));
            
            // Sprawdzenie, czy gracz może sobie pozwolić na ulepszenie
            CanAffordUpgrade = _playerGold >= UpgradeCost;
            
            // Aktualizacja komunikatu statusu
            if (_newModifier > 0.6)
            {
                StatusMessage = "Maximum upgrade level reached!";
                CanAffordUpgrade = false;
            }
            else if (!CanAffordUpgrade)
            {
                StatusMessage = "Not enough gold for this upgrade.";
            }
            else
            {
                StatusMessage = string.Empty;
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku potwierdzającego ulepszenie przedmiotu.
        /// Ustawia wynik dialogu na true i zamyka okno.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku anulowania ulepszenia.
        /// Ustawia wynik dialogu na false i zamyka okno.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #region INotifyPropertyChanged Implementation

        /// <summary>Występuje, gdy zmienia się wartość właściwości.</summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Wywołuje zdarzenie PropertyChanged dla określonej właściwości.
        /// </summary>
        /// <param name="propertyName">Nazwa właściwości, która uległa zmianie.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Ustawia wartość pola i wywołuje powiadomienie o zmianie właściwości.
        /// </summary>
        /// <typeparam name="T">Typ wartości do ustawienia.</typeparam>
        /// <param name="field">Referencja do pola.</param>
        /// <param name="value">Nowa wartość.</param>
        /// <param name="propertyName">Nazwa właściwości (ustawiana automatycznie).</param>
        /// <returns>True, jeśli wartość została zmieniona; w przeciwnym razie false.</returns>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}
