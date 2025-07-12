using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace GodmistWPF.ViewModels
{
    /// <summary>
    /// ViewModel reprezentujący pojedyncze pomieszczenie w widoku lochów.
    /// </summary>
    /// <remarks>
    /// Zapewnia dane i logikę dla widoku pokoju, w tym zarządzanie stanem widoczności,
    /// aktualizację wyświetlanych informacji oraz powiadamianie o zmianach właściwości.
    /// </remarks>
    public class RoomViewModel : INotifyPropertyChanged
    {
        private string _symbol;
        private string _roomInfo;
        private string _roomTypeName;
        private bool _isCurrent;
        private bool _isRevealed;
        private SolidColorBrush _backgroundColor;
        private string _displayText;
        private int _fontSize;

        /// <summary>
        /// Pobiera lub ustawia symbol reprezentujący pokój.
        /// </summary>
        public string Symbol
        {
            get => _symbol;
            set { _symbol = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Pobiera lub ustawia dodatkowe informacje o pokoju.
        /// </summary>
        public string RoomInfo
        {
            get => _roomInfo;
            set { _roomInfo = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Pobiera lub ustawia nazwę typu pokoju.
        /// </summary>
        /// <remarks>
        /// Ustawienie tej właściwości powoduje aktualizację wyświetlanego tekstu.
        /// </remarks>
        public string RoomTypeName
        {
            get => _roomTypeName;
            set { _roomTypeName = value; 
                UpdateDisplay();
                OnPropertyChanged(); }
        }

        /// <summary>
        /// Pobiera lub ustawia wartość wskazującą, czy pokój jest aktualnie wybrany.
        /// </summary>
        public bool IsCurrent
        {
            get => _isCurrent;
            set { _isCurrent = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Pobiera lub ustawia wartość wskazującą, czy pokój został odkryty.
        /// </summary>
        /// <remarks>
        /// Zmiana tej właściwości powoduje aktualizację wyglądu pokoju.
        /// </remarks>
        public bool IsRevealed
        {
            get => _isRevealed;
            set 
            { 
                if (_isRevealed != value)
                {
                    _isRevealed = value; 
                    OnPropertyChanged();
                    UpdateDisplay();
                }
            }
        }

        /// <summary>
        /// Pobiera lub ustawia kolor tła pokoju.
        /// </summary>
        public SolidColorBrush BackgroundColor
        {
            get => _backgroundColor;
            set { _backgroundColor = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Występuje, gdy zmienia się wartość właściwości.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Wywołuje zdarzenie PropertyChanged.
        /// </summary>
        /// <param name="propertyName">Nazwa zmienionej właściwości. Jeśli nie podano, używana jest nazwa wywołującej właściwości.</param>
        internal void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Pobiera tekst wyświetlany w pokoju.
        /// </summary>
        /// <remarks>
        /// Zależy od stanu pokoju (odkryty/nieodkryty) i typu pokoju.
        /// </remarks>
        public string DisplayText
        {
            get => _displayText;
            private set { _displayText = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Pobiera rozmiar czcionki używany do wyświetlania tekstu w pokoju.
        /// </summary>
        public int FontSize
        {
            get => _fontSize;
            private set { _fontSize = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="RoomViewModel"/>.
        /// </summary>
        /// <remarks>
        /// Ustawia domyślne wartości właściwości, w tym przezroczyste tło i ukryty stan pokoju.
        /// </remarks>
        public RoomViewModel()
        {
            BackgroundColor = new SolidColorBrush(Colors.Transparent);
            IsRevealed = false; // Start with rooms hidden by default
            UpdateDisplay();
        }

        /// <summary>
        /// Przechowuje oryginalny kolor tła przed zmianą na ciemnoszary (dla nieodkrytych pokoi).
        /// </summary>
        private Color _originalColor = Colors.Transparent;

        /// <summary>
        /// Aktualizuje wygląd pokoju na podstawie jego stanu.
        /// </summary>
        /// <remarks>
        /// Dla nieodkrytych pokoi wyświetla znak zapytania i ciemnoszare tło.
        /// Dla odkrytych pokoi wyświetla nazwę typu pokoju i przywraca oryginalny kolor tła.
        /// </remarks>
        private void UpdateDisplay()
        {
            if (IsRevealed)
            {
                DisplayText = RoomTypeName;
                FontSize = 12;
                // Restore original color when revealed
                if (BackgroundColor == null || BackgroundColor.Color == Colors.DarkGray)
                {
                    BackgroundColor = new SolidColorBrush(_originalColor);
                }
            }
            else
            {
                DisplayText = "?";
                FontSize = 20;
                // Store the current color before changing to dark gray
                if (BackgroundColor != null && BackgroundColor.Color != Colors.DarkGray)
                {
                    _originalColor = BackgroundColor.Color;
                }
                // Set background to dark grey when not revealed
                BackgroundColor = new SolidColorBrush(Colors.DarkGray);
            }
        }
    }
}
