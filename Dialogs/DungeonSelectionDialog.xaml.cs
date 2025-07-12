using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GodmistWPF.Characters.Player;
using GodmistWPF.Enums.Dungeons;

namespace GodmistWPF.Dialogs
{
    /// <summary>
    /// Okno dialogowe umożliwiające wybór typu i poziomu trudności lochu do eksploracji.
    /// Zawiera listę dostępnych lokacji wraz z ich opisami i suwakiem do ustawienia poziomu trudności.
    /// </summary>
    public partial class DungeonSelectionDialog : Window
    {
        /// <summary>
        /// Pobiera wybrany typ lochu.
        /// </summary>
        public DungeonType SelectedDungeonType { get; private set; }

        /// <summary>
        /// Pobiera wybrany poziom trudności lochu.
        /// </summary>
        public int SelectedLevel { get; private set; }

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="DungeonSelectionDialog">.
        /// </summary>
        public DungeonSelectionDialog()
        {
            InitializeComponent();
            Loaded += DungeonSelectionDialog_Loaded;
        }

        /// <summary>
        /// Obsługuje zdarzenie załadowania okna.
        /// Inicjalizuje listę dostępnych lochów i ustawia domyślne wartości.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void DungeonSelectionDialog_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize dungeon types
            var dungeonTypes = new Dictionary<DungeonType, string>
            {
                { DungeonType.Forest, "Ancient Forest" },
                { DungeonType.Catacombs, "Forgotten Catacombs" },
                { DungeonType.ElvishRuins, "Elvish Ruins" },
                { DungeonType.Cove, "Pirate's Cove" },
                { DungeonType.Desert, "Scorched Desert" },
                { DungeonType.Temple, "Sacred Temple" },
                { DungeonType.Mountains, "Dragon's Peak" },
                { DungeonType.Swamp, "Cursed Swamp" }
            };

            DungeonTypeComboBox.ItemsSource = dungeonTypes;
            DungeonTypeComboBox.SelectedIndex = 0;
            
            // Set default level to player's level (clamped between 1 and 50)
            var player = PlayerHandler.player;
            var playerLevel = player?.Level ?? 1;
            DungeonLevelSlider.Value = Math.Clamp(playerLevel, 1, 50);
            DungeonLevelText.Text = ((int)DungeonLevelSlider.Value).ToString();
            
            // Update description for the first dungeon type
            UpdateDungeonDescription((DungeonType)DungeonTypeComboBox.SelectedValue);
        }

        /// <summary>
        /// Obsługuje zmianę wybranego typu lochu na liście rozwijanej.
        /// Aktualizuje opis wybranego lochu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (ComboBox).</param>
        /// <param name="e">Dane zdarzenia zmiany wyboru.</param>
        private void DungeonTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DungeonTypeComboBox.SelectedValue is DungeonType selectedType)
            {
                UpdateDungeonDescription(selectedType);
            }
        }

        /// <summary>
        /// Obsługuje zmianę wartości suwaka poziomu trudności.
        /// Aktualizuje wyświetlaną wartość poziomu trudności.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (Slider).</param>
        /// <param name="e">Dane zdarzenia zmiany wartości.</param>
        private void DungeonLevelSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DungeonLevelText != null)
            {
                DungeonLevelText.Text = ((int)e.NewValue).ToString();
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku rozpoczęcia przygody.
        /// Zapisuje wybrane ustawienia i zamyka okno z wynikiem DialogResult = true.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (Button).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (DungeonTypeComboBox.SelectedValue is DungeonType selectedType)
            {
                SelectedDungeonType = selectedType;
                SelectedLevel = (int)DungeonLevelSlider.Value;
                DialogResult = true;
                Close();
            }
        }

        /// <summary>
        /// Aktualizuje opis wybranego lochu na podstawie jego typu.
        /// </summary>
        /// <param name="dungeonType">Typ lochu, dla którego ma zostać wyświetlony opis.</param>
        private void UpdateDungeonDescription(DungeonType dungeonType)
        {
            string description = dungeonType switch
            {
                DungeonType.Forest => "A dense, ancient forest filled with dangerous creatures and hidden treasures.",
                DungeonType.Catacombs => "Dark, winding tunnels beneath the city, home to the restless dead.",
                DungeonType.ElvishRuins => "The remains of an ancient elvish civilization, now overgrown and dangerous.",
                DungeonType.Cove => "A pirate hideout filled with traps and treasure.",
                DungeonType.Desert => "A scorching desert with ancient ruins and deadly creatures.",
                DungeonType.Temple => "A sacred temple filled with puzzles and guardians.",
                DungeonType.Mountains => "Towering peaks with treacherous paths and dragon lairs.",
                DungeonType.Swamp => "A murky swamp filled with poisonous creatures and ancient curses.",
                _ => "A mysterious location filled with danger and treasure."
            };
            
            DungeonDescriptionText.Text = description;
        }


    }
}
