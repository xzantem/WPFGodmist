using System.IO;
using System.Windows;
using System.Windows.Controls;
using GodmistWPF.Towns;
using GodmistWPF.Utilities.DataPersistance;
using DataPersistanceManager = GodmistWPF.Utilities.DataPersistance.DataPersistanceManager;
using Difficulty = GodmistWPF.Enums.Difficulty;
using GameSettings = GodmistWPF.Utilities.GameSettings;
using PlayerHandler = GodmistWPF.Characters.Player.PlayerHandler;
using QuestManager = GodmistWPF.Quests.QuestManager;
using SaveData = GodmistWPF.Utilities.DataPersistance.SaveData;

namespace GodmistWPF.Dialogs
{
    public partial class SettingsDialog : Window
    {
        public SettingsDialog()
        {
            InitializeComponent();
        }

        private void SetupEventHandlers()
        {
            DifficultyComboBox.SelectionChanged += DifficultyComboBox_SelectionChanged;
            MasterVolumeSlider.ValueChanged += MasterVolumeSlider_ValueChanged;
        }

        private void LoadSettings()
        {
            // Load difficulty settings
            DifficultyComboBox.ItemsSource = Enum.GetValues(typeof(Difficulty));
            DifficultyComboBox.SelectedItem = GameSettings.Difficulty;

            // Load display settings
            FullscreenCheckBox.IsChecked = false; // Default to windowed
            ShowTooltipsCheckBox.IsChecked = true;
            ShowAnimationsCheckBox.IsChecked = true;
            HighContrastCheckBox.IsChecked = false;

            // Load gameplay settings
            AutoSaveCheckBox.IsChecked = true;
            ConfirmActionsCheckBox.IsChecked = true;
            ShowDamageNumbersCheckBox.IsChecked = true;
            ShowCombatLogCheckBox.IsChecked = true;

            // Load sound settings
            SoundEnabledCheckBox.IsChecked = true;
            MusicEnabledCheckBox.IsChecked = true;
            MasterVolumeSlider.Value = 50;
            VolumeText.Text = "50%";
        }

        #region Event Handlers

        private void DifficultyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDifficultyDescription();
        }

        private void MasterVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VolumeText.Text = $"{(int)MasterVolumeSlider.Value}%";
        }

        private void UpdateDifficultyDescription()
        {
            if (DifficultyComboBox.SelectedItem is Difficulty difficulty)
            {
                DifficultyDescriptionText.Text = difficulty switch
                {
                    Difficulty.Easy => "Enemies have reduced health and damage. Good for beginners or casual play.",
                    Difficulty.Normal => "Standard difficulty with balanced enemy stats. Recommended for most players.",
                    Difficulty.Hard => "Enemies are stronger and more challenging. For experienced players.",
                    Difficulty.Nightmare => "Extreme difficulty with very powerful enemies. Only for the most skilled players.",
                    _ => "Select a difficulty level."
                };
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save difficulty setting
                if (DifficultyComboBox.SelectedItem is Difficulty difficulty)
                {
                    GameSettings.Difficulty = difficulty;
                }

                // Save other settings (these would be saved to a settings file in a real implementation)
                // For now, we'll just show a success message
                
                MessageBox.Show("Settings saved successfully!", "Settings", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Save Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
} 