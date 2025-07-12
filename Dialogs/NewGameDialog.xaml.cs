using System.Windows;
using System.Windows.Controls;
using CharacterClass = GodmistWPF.Enums.CharacterClass;
using Difficulty = GodmistWPF.Enums.Difficulty;

namespace GodmistWPF.Dialogs
{
    /// <summary>
    /// Okno dialogowe tworzenia nowej postaci w grze.
    /// Umożliwia wybór nazwy postaci, klasy i poziomu trudności.
    /// </summary>
    public partial class NewGameDialog : Window
    {
        /// <summary>
        /// Pobiera nazwę postaci wprowadzoną przez użytkownika.
        /// </summary>
        public string CharacterName { get; private set; } = "";

        /// <summary>
        /// Pobiera wybraną klasę postaci.
        /// Domyślnie ustawiona na Wojownika (Warrior).
        /// </summary>
        public CharacterClass SelectedCharacterClass { get; private set; } = CharacterClass.Warrior;

        /// <summary>
        /// Pobiera wybrany poziom trudności.
        /// Domyślnie ustawiony na Normalny (Normal).
        /// </summary>
        public Difficulty SelectedDifficulty { get; private set; } = Difficulty.Normal;

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="NewGameDialog">.
        /// Konfiguruje domyślne ustawienia i inicjalizuje komponenty interfejsu użytkownika.
        /// </summary>
        public NewGameDialog()
        {
            InitializeComponent();
            
            // Ustaw domyślne wybory
            ClassComboBox.SelectedIndex = 0; // Pierwsza dostępna klasa
            DifficultyComboBox.SelectedIndex = 1; // Normalny poziom trudności
            
            // Podłącz obsługę zdarzeń
            ClassComboBox.SelectionChanged += ClassComboBox_SelectionChanged;
            
            // Wyświetl początkowy opis klasy
            UpdateClassDescription();
        }

        /// <summary>
        /// Obsługuje zmianę wybranej klasy postaci na liście rozwijanej.
        /// Wywołuje aktualizację opisu klasy po zmianie wyboru.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (ComboBox klas).</param>
        /// <param name="e">Dane zdarzenia zmiany wyboru.</param>
        private void ClassComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClassDescription();
        }

        /// <summary>
        /// Aktualizuje opis wybranej klasy postaci w interfejsie użytkownika.
        /// Wyświetla odpowiedni opis w zależności od wybranej klasy.
        /// </summary>
        private void UpdateClassDescription()
        {
            if (ClassComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                var className = selectedItem.Tag.ToString();
                ClassDescriptionText.Text = className switch
                {
                    "Warrior" => "A mighty warrior skilled in close combat. High health and physical damage.",
                    "Scout" => "A nimble scout with high agility. Excellent at ranged combat and stealth.",
                    "Sorcerer" => "A powerful spellcaster with devastating magical abilities. High mana and spell damage.",
                    "Paladin" => "A holy warrior with healing abilities. Balanced combat and support capabilities.",
                    _ => "Choose your character class to see description."
                };
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku tworzenia nowej postaci.
        /// Sprawdza poprawność danych, ustawia wybrane wartości i zamyka okno z wynikiem DialogResult = true.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            // Sprawdź, czy podano nazwę postaci
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Please enter a character name.", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Sprawdź maksymalną długość nazwy
            if (NameTextBox.Text.Length > 32)
            {
                MessageBox.Show("Character name must be 32 characters or less.", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Zapisz wybrane wartości
            CharacterName = NameTextBox.Text.Trim();
            
            // Ustaw wybraną klasę postaci
            if (ClassComboBox.SelectedItem is ComboBoxItem classItem)
            {
                SelectedCharacterClass = classItem.Tag.ToString() switch
                {
                    "Warrior" => CharacterClass.Warrior,
                    "Scout" => CharacterClass.Scout,
                    "Sorcerer" => CharacterClass.Sorcerer,
                    "Paladin" => CharacterClass.Paladin,
                    _ => CharacterClass.Warrior
                };
            }
            
            // Ustaw wybrany poziom trudności
            if (DifficultyComboBox.SelectedItem is ComboBoxItem difficultyItem)
            {
                SelectedDifficulty = difficultyItem.Tag.ToString() switch
                {
                    "Easy" => Difficulty.Easy,
                    "Normal" => Difficulty.Normal,
                    "Hard" => Difficulty.Hard,
                    "Nightmare" => Difficulty.Nightmare,
                    _ => Difficulty.Normal
                };
            }

            // Zamknij okno z wynikiem powodzenia
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku anulowania.
        /// Zamyka okno z wynikiem DialogResult = false.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 