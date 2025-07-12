using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using GodmistWPF.Characters.Player;
using GodmistWPF.Dialogs;
using GodmistWPF.Quests;
using GodmistWPF.Towns;
using GodmistWPF.Towns.NPCs;
using GodmistWPF.Utilities;
using MaterialDesignThemes.Wpf;
using CharacterClass = GodmistWPF.Enums.CharacterClass;
namespace GodmistWPF
{
    /// <summary>
    /// Główne okno aplikacji, służące jako główny interfejs użytkownika.
    /// Odpowiada za zarządzanie językiem interfejsu i podstawową nawigację w grze.
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Inicjalizuje nową instancję klasy MainWindow.
        /// </summary>
        /// <remarks>
        /// <para>Konstruktor wykonuje następujące operacje:</para>
        /// <list type="bullet">
        /// <item>Ustawia kulturę aplikacji na podstawie zapisanych ustawień użytkownika</item>
        /// <item>Inicjalizuje komponenty interfejsu użytkownika</item>
        /// <item>Ustawia kontekst danych na bieżące okno</item>
        /// <item>Aktualizuje zlokalizowane teksty w interfejsie</item>
        /// </list>
        /// <para>Jest to główny punkt wejścia dla interfejsu użytkownika aplikacji.</para>
        /// </remarks>
        public MainWindow()
        {
            SetCulture();
            InitializeComponent();
            DataContext = this;
            UpdateLocalizedTexts();
        }

        /// <summary>
        /// Ustawia kulturę aplikacji na podstawie zapisanych ustawień użytkownika.
        /// W przypadku braku zapisanego języka, ustawia domyślną kulturę systemu.
        /// </summary>
        /// <remarks>
        /// <para>Metoda wykonuje następujące operacje:</para>
        /// <list type="bullet">
        /// <item>Pobiera zapisane ustawienia językowe użytkownika</item>
        /// <item>Ustawia kulturę dla bieżącego wątku</item>
        /// <item>Konfiguruje domyślną kulturę dla nowych wątków</item>
        /// <item>Dostosowuje kierunek tekstu (RTL/LTR) do wybranej kultury</item>
        /// <item>Ustawia język XML dla elementów interfejsu</item>
        /// </list>
        /// <para>W przypadku błędu podczas ładowania ustawień, ustawia kulturę neutralną (InvariantCulture).</para>
        /// </remarks>
        private void SetCulture()
        {
            try
            {
                var settings = new WPFGodmist.Properties.Settings();
                string savedLanguage = settings.Language;
                
                if (string.IsNullOrEmpty(savedLanguage))
                {
                    savedLanguage = CultureInfo.CurrentUICulture.Name;
                    settings.Language = savedLanguage;
                    settings.Save();
                }

                var culture = new CultureInfo(savedLanguage);
                
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                CultureInfo.DefaultThreadCurrentCulture = culture;
                
                var flowDirection = culture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                var xmlLanguage = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
                
                FlowDirection = flowDirection;
                Language = xmlLanguage;
            }
            catch (Exception ex)
            {
                var culture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
            }
        }

        /// <summary>
        /// Aktualizuje wszystkie teksty w interfejsie użytkownika zgodnie z aktualnie wybranym językiem.
        /// </summary>
        /// <remarks>
        /// <para>Metoda powinna być wywoływana po każdej zmianie języka w aplikacji.</para>
        /// <para>Zakres aktualizacji obejmuje:</para>
        /// <list type="bullet">
        /// <item>Tytuły okien i przycisków</item>
        /// <item>Etykiety i komunikaty</li>
        /// <item>Teksty interfejsu użytkownika</li>
        /// </list>
        /// <para>W przypadku błędu podczas aktualizacji tekstów wyświetlany jest komunikat o błędzie.</para>
        /// </remarks>
        private void UpdateLocalizedTexts()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating localized texts: {ex.Message}", 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Wyświetla okno dialogowe wyboru języka i obsługuje zmianę języka aplikacji.
        /// </summary>
        /// <remarks>
        /// <para>Metoda wykonuje następujące czynności:</para>
        /// <list type="bullet">
        /// <item>Wyświetla okno dialogowe wyboru języka</item>
        /// <item>Obsługuje zdarzenie zmiany języka</item>
        /// <item>Przy zmianie języka uruchamia ponownie aplikację z nowymi ustawieniami</item>
        /// </list>
        /// <para>Po zmianie języka aplikacja zostaje automatycznie uruchomiona ponownie, aby zastosować nowe ustawienia językowe.</para>
        /// <para>W przypadku błędu podczas wyświetlania okna wyświetlany jest odpowiedni komunikat o błędzie.</para>
        /// </remarks>
        private async void ShowLanguageDialog()
        {
            try
            {
                var dialog = new LanguageSelectionDialog();
                dialog.LanguageChanged += (sender, args) =>
                {
                    var currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
                    Process.Start(currentExecutablePath);
                    Application.Current.Shutdown();
                };
                await DialogHost.Show(dialog, "RootDialog");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while showing the language dialog: {ex.Message}", 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        #region Main Menu Events

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku nowej gry.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dodatkowe dane zdarzenia.</param>
        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            ShowNewGameDialog();
        }

        #endregion

        #region Town View Events

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku rozpoczęcia wyprawy do lochów.
        /// Otwiera okno dialogowe wyboru lochu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dodatkowe dane zdarzenia.</param>
        /// <remarks>
        /// W przypadku błędu podczas otwierania okna wyświetlany jest komunikat o błędzie.
        /// </remarks>
        private void StartExpeditionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dungeonDialog = new DungeonDialog();
                dungeonDialog.Owner = this;
                dungeonDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dungeonDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start expedition: {ex}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku wczytania gry.
        /// Otwiera menedżer zapisów i wczytuje wybraną grę.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dodatkowe dane zdarzenia.</param>
        /// <remarks>
        /// Po pomyślnym wczytaniu gry, rozpoczyna nową grę i przełącza widok na miasto.
        /// W przypadku błędu wyświetlany jest odpowiedni komunikat.
        /// </remarks>
        private void LoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new SaveManagerDialog();
                saveDialog.ShowDialog();

                // Check if a game was loaded
                if (PlayerHandler.player != null)
                {
                    StartGame();
                    ShowTownView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading game: {ex.Message}", "Load Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku zmiany języka.
        /// Otwiera okno dialogowe umożliwiające zmianę języka interfejsu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dodatkowe dane zdarzenia.</param>
        private void LanguageButton_Click(object sender, RoutedEventArgs e)
        {
            ShowLanguageDialog();
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku wyjścia z aplikacji.
        /// Bezpiecznie zamyka aplikację.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dodatkowe dane zdarzenia.</param>
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Dialog Methods

        /// <summary>
        /// Wyświetla okno dialogowe nowej gry i inicjalizuje nową grę po potwierdzeniu.
        /// </summary>
        /// <remarks>
        /// <para>Metoda wykonuje następujące czynności:</para>
        /// <list type="bullet">
        /// <item>Wyświetla okno wyboru postaci i poziomu trudności</item>
        /// <item>Inicjalizuje miasto Arungard</item>
        /// <item>Ustawia wybrany poziom trudności</item>
        /// <item>Inicjalizuje zadania główne i poboczne</item>
        /// <item>Tworzy nową postać gracza na podstawie wybranej klasy</item>
        /// <item>Uruchamia grę i przełącza widok na miasto</item>
        /// </list>
        /// <para>W przypadku błędu podczas inicjalizacji wyświetlany jest odpowiedni komunikat.</para>
        /// </remarks>
        private void ShowNewGameDialog()
        {
            var dialog = new NewGameDialog();
            if (dialog.ShowDialog() == true)
            {
                TownsHandler.Arungard = new Town("Arungard");
                GameSettings.Difficulty = dialog.SelectedDifficulty;
                QuestManager.InitMainQuests();
                QuestManager.InitSideQuests(true);
                
                PlayerHandler.player = dialog.SelectedCharacterClass switch
                {
                    CharacterClass.Warrior => new Warrior(dialog.CharacterName),
                    CharacterClass.Scout => new Scout(dialog.CharacterName),
                    CharacterClass.Sorcerer => new Sorcerer(dialog.CharacterName),
                    CharacterClass.Paladin => new Paladin(dialog.CharacterName),
                    _ => new Warrior(dialog.CharacterName)
                };
                
                StartGame();
                ShowTownView();
            }
        }

        /// <summary>
        /// Wyświetla okno dialogowe ekwipunku gracza.
        /// </summary>
        /// <remarks>
        /// <para>Okno ekwipunku umożliwia:</para>
        /// <list type="bullet">
        /// <item>Przeglądanie posiadanych przedmiotów</item>
        /// <item>Zakładanie i zdejmowanie ekwipunku</li>
        /// <item>Używanie przedmiotów użytkowych</li>
        /// <li>Zarządzanie ekwipunkiem</li>
        /// </list>
        /// <para>Okno jest modalne - blokuje interakcję z resztą aplikacji do momentu zamknięcia.</para>
        /// </remarks>
        private void ShowInventoryDialog()
        {
            var inventory = new InventoryDialog();
            inventory.ShowDialog();
        }

        /// <summary>
        /// Wyświetla okno dialogowe z listą zadań (questów) dostępnych dla gracza.
        /// </summary>
        /// <remarks>
        /// <para>Okno zadań umożliwia:</para>
        /// <list type="bullet">
        /// <item>Przeglądanie aktywnych zadań</item>
        /// <item>Sprawdzanie postępów w zadaniach</item>
        /// <item>Przeglądanie ukończonych zadań</item>
        /// <li>Śledzenie celów i nagród</li>
        /// </list>
        /// <para>Okno jest modalne - blokuje interakcję z resztą aplikacji do momentu zamknięcia.</para>
        /// </remarks>
        private void ShowQuestsDialog()
        {
            var quests = new QuestsDialog();
            quests.ShowDialog();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Inicjalizuje początek nowej gry, ukrywając menu główne.
        /// </summary>
        /// <remarks>
        /// Metoda jest wywoływana po wybraniu nowej gry lub załadowaniu zapisanej gry.
        /// Ukrywa widok menu głównego przed przejściem do widoku miasta.
        /// </remarks>
        private void StartGame()
        {
            MainMenuView.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Przełącza widok na widok miasta, ukrywając inne elementy interfejsu.
        /// </summary>
        /// <remarks>
        /// <para>Metoda wykonuje następujące czynności:</para>
        /// <list type="bullet">
        /// <item>Ukrywa widok menu głównego</item>
        /// <item>Pokazuje widok miasta</item>
        /// <item>Inicjalizuje zawartość widoku miasta</item>
        /// </list>
        /// <para>Jest wywoływana po rozpoczęciu nowej gry lub powrocie do miasta z innej lokacji.</para>
        /// </remarks>
        private void ShowTownView()
        {
            MainMenuView.Visibility = Visibility.Collapsed;
            TownView.Visibility = Visibility.Visible;
            InitializeTownView();
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku ekwipunku w widoku miasta.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dodatkowe dane zdarzenia.</param>
        private void InventoryButton_Town_Click(object sender, RoutedEventArgs e)
        {
            ShowInventoryDialog();
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku zapisu gry w widoku miasta.
        /// Otwiera menedżer zapisów, umożliwiając zapisanie aktualnego stanu gry.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dodatkowe dane zdarzenia.</param>
        private void SaveGameButton_Town_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveManagerDialog();
            dialog.ShowDialog();
        }
        
        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku postaci w interfejsie.
        /// Otwiera okno z informacjami o postaci gracza, jeśli postać istnieje.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dodatkowe dane zdarzenia.</param>
        /// <remarks>
        /// W przypadku błędu podczas otwierania okna postaci wyświetlany jest komunikat o błędzie.
        /// </remarks>
        private void CharacterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PlayerHandler.player != null)
                {
                    var characterWindow = new CharacterWindow(PlayerHandler.player);
                    characterWindow.Owner = this;
                    characterWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CharacterButton_Click: {ex.Message}");
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku dziennika zadań.
        /// Wyświetla okno z listą aktywnych i ukończonych zadań.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dodatkowe dane zdarzenia.</param>
        private void QuestLogButton_Click(object sender, RoutedEventArgs e)
        {
            ShowQuestsDialog();
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku alchemisty w mieście.
        /// Otwiera okno dialogowe z możliwością interakcji z alchemistą.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dodatkowe dane zdarzenia.</param>
        /// <remarks>
        /// W przypadku błędu podczas otwierania okna alchemisty wyświetlany jest komunikat o błędzie.
        /// </remarks>
        private void AlchemistButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var alchemist = new Alchemist("Alchemist");
                var dialog = new NPCDialog(alchemist)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Alchemist dialog: {ex}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku kowala w mieście.
        /// Otwiera okno dialogowe z możliwością interakcji z kowalem.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dodatkowe dane zdarzenia.</param>
        /// <remarks>
        /// W przypadku błędu podczas otwierania okna kowala wyświetlany jest komunikat o błędzie.
        /// </remarks>
        private void BlacksmithButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var blacksmith = new Blacksmith("Blacksmith");
                var dialog = new NPCDialog(blacksmith)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Blacksmith dialog: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku zaklinacza w mieście.
        /// Otwiera okno dialogowe z możliwością interakcji z zaklinaczem.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dodatkowe dane zdarzenia.</param>
        /// <remarks>
        /// W przypadku błędu podczas otwierania okna zaklinacza wyświetlany jest komunikat o błędzie.
        /// </remarks>
        private void EnchanterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var enchanter = new Enchanter("Enchanter");
                var dialog = new NPCDialog(enchanter)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Enchanter dialog: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeTownView()
        {
            if (TownsHandler.Arungard != null)
            {
                TownNameText.Text = TownsHandler.Arungard.TownName; ;
                FooterText.Text = $"Town of {TownsHandler.Arungard.TownName} - Your adventure begins here!";
            }
        }

        #endregion
    }
} 