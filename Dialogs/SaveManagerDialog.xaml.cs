using System;
using System.Collections.Generic;
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
    public partial class SaveManagerDialog : Window
    {
        private string saveDirectory;
        private List<SaveFileInfo> saveFiles;
        private SaveFileInfo? selectedSaveFile;

        public SaveManagerDialog()
        {
            InitializeComponent();
            InitializeSaveDirectory();
            LoadSaveFiles();
        }

        private void InitializeSaveDirectory()
        {
            saveDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Godmist", "saves");
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
        }

        private void LoadSaveFiles()
        {
            try
            {
                saveFiles = DataPersistanceManager.GetSaveFiles();
                SaveFilesListBox.ItemsSource = saveFiles;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading save files: {ex.Message}", "Load Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateSaveFileInfo()
        {
            if (selectedSaveFile != null)
            {
                SaveFileInfoText.Text = $"Name: {selectedSaveFile.Name}\n" +
                                       $"Created: {selectedSaveFile.CreatedDate:g}\n" +
                                       $"Modified: {selectedSaveFile.ModifiedDate:g}\n" +
                                       $"Size: {selectedSaveFile.Size / 1024:F1} KB";
            }
            else
            {
                SaveFileInfoText.Text = "No save file selected.";
            }
        }

        private void SaveFilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedSaveFile = SaveFilesListBox.SelectedItem as SaveFileInfo;
            UpdateSaveFileInfo();
            LoadSaveButton.IsEnabled = selectedSaveFile != null;
            DeleteSaveButton.IsEnabled = selectedSaveFile != null;
        }

        private void LoadSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSaveFile == null) return;
            try
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to load '{selectedSaveFile.Name}'? This will replace your current game.",
                    "Load Save Game",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var data = DataPersistanceManager.LoadGameFromFile(selectedSaveFile.FilePath);
                    DataPersistanceManager.ApplySaveData(data);
                    MessageBox.Show("Save game loaded successfully!", "Load Complete", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading save game: {ex.Message}", "Load Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSaveFile == null) return;
            try
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete '{selectedSaveFile.Name}'? This action cannot be undone.",
                    "Delete Save Game",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    DataPersistanceManager.DeleteSaveFile(selectedSaveFile.FilePath);
                    LoadSaveFiles();
                    selectedSaveFile = null;
                    UpdateSaveFileInfo();
                    MessageBox.Show("Save game deleted successfully.", "Delete Complete", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting save game: {ex.Message}", "Delete Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerHandler.player == null)
            {
                MessageBox.Show("No active game to save. Please start a new game first.", 
                    "No Game Active", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                var characterName = PlayerHandler.player?.Name ?? "Unknown";
                var saveName = DataPersistanceManager.GenerateSaveName(characterName);
                var saveData = new SaveData(
                    PlayerHandler.player,
                    GameSettings.Difficulty,
                    new[] { QuestManager.MainQuests, QuestManager.RandomizedSideQuests, QuestManager.BossSideQuests },
                    QuestManager.BossProgress,
                    TownsHandler.Arungard
                );
                DataPersistanceManager.SaveGameToFile(saveData, saveName);
                LoadSaveFiles();
                MessageBox.Show("Save game created successfully!", "Save Complete", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating save game: {ex.Message}", "Save Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 