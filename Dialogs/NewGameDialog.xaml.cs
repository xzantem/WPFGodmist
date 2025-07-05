using System.Windows;
using System.Windows.Controls;
using CharacterClass = GodmistWPF.Enums.CharacterClass;
using Difficulty = GodmistWPF.Enums.Difficulty;

namespace GodmistWPF.Dialogs
{
    public partial class NewGameDialog : Window
    {
        public string CharacterName { get; private set; } = "";
        public CharacterClass SelectedCharacterClass { get; private set; } = CharacterClass.Warrior;
        public Difficulty SelectedDifficulty { get; private set; } = Difficulty.Normal;

        public NewGameDialog()
        {
            InitializeComponent();
            
            // Set default selections
            ClassComboBox.SelectedIndex = 0;
            DifficultyComboBox.SelectedIndex = 1; // Normal
            
            // Set up event handlers
            ClassComboBox.SelectionChanged += ClassComboBox_SelectionChanged;
            
            // Show initial class description
            UpdateClassDescription();
        }

        private void ClassComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClassDescription();
        }

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

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Please enter a character name.", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NameTextBox.Text.Length > 32)
            {
                MessageBox.Show("Character name must be 32 characters or less.", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get selected values
            CharacterName = NameTextBox.Text.Trim();
            
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

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 