using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using GodmistWPF.Dungeons.Interactables;
using GodmistWPF.Enums;

namespace GodmistWPF.Dialogs
{
    /// <summary>
    /// Okno dialogowe zawierające minigrę z pułapką, którą gracz musi rozbroić.
    /// Zawiera różne typy wyzwań, takie jak kod, reakcja, słówko i siatka.
    /// </summary>
    public partial class TrapMinigameDialog : Window
    {
        /// <summary>Poziom trudności aktualnej gry.</summary>
        private readonly Difficulty _difficulty;
        
        /// <summary>Akcja wywoływana po zakończeniu gry z wynikiem (true = wygrana, false = przegrana).</summary>
        private readonly Action<bool> _onComplete;
        
        /// <summary>Generator liczb losowych używany w grze.</summary>
        private readonly Random _random = new Random();
        
        /// <summary>Flaga wskazująca, czy pułapka została rozbrojona.</summary>
        private bool _isDisarmed = false;
        
        /// <summary>Flaga określająca, czy okno może zostać zamknięte.</summary>
        private bool _canClose = false;
        
        /// <summary>Słowo do odgadnięcia w grze Wordle.</summary>
        private string _wordToGuess = string.Empty;
        
        /// <summary>Lista dotychczasowych prób w grze Wordle.</summary>
        private readonly List<string> _wordGuesses = new();
        
        /// <summary>Maksymalna liczba prób w grze Wordle.</summary>
        private int _maxWordleAttempts = 6;
        
        /// <summary>Lista pól tekstowych używanych w grze Wordle.</summary>
        private readonly List<TextBox> _wordleLetterBoxes = new();
        
        /// <summary>Indeks aktualnie aktywnego pola w grze Wordle.</summary>
        private int _currentLetterIndex = 0;
        
        /// <summary>Kod do odgadnięcia w wyzwaniu kodowym.</summary>
        private string _codeToGuess;
        
        /// <summary>Pozycja bomby w siatce wyzwania siatkowego.</summary>
        private int _bombPosition;
        
        /// <summary>Liczba pól do wybrania w wyzwaniu siatkowym.</summary>
        private int _tilesToSelect;
        
        /// <summary>Lista indeksów wybranych pól w wyzwaniu siatkowym.</summary>
        private readonly List<int> _selectedTiles = new List<int>();
        
        /// <summary>Timer używany do mierzenia czasu reakcji.</summary>
        private DispatcherTimer _reactionTimer;
        
        /// <summary>Czas rozpoczęcia pomiaru reakcji.</summary>
        private DateTime _reactionStartTime;
        
        /// <summary>Maksymalny czas na reakcję w sekundach.</summary>
        private double _reactionTimeLimit;
        
        /// <summary>Flaga wskazująca, czy trwa pomiar czasu reakcji.</summary>
        private bool _isReactionActive = false;
        
        /// <summary>Czas zakończenia rundy reakcji.</summary>
        private DateTime _roundEndTime;
        
        /// <summary>Liczba trafień w wyzwaniu reakcji.</summary>
        private int _hitCount = 0;
        
        /// <summary>Liczba nieudanych prób w wyzwaniu reakcji.</summary>
        private int _failedAttempts = 0;
        
        /// <summary>Maksymalna liczba nieudanych prób w wyzwaniu reakcji.</summary>
        private int _maxFailedAttempts = 3;
        
        /// <summary>Maksymalna liczba prób w bieżącym wyzwaniu.</summary>
        private int _maxAttempts;
        
        /// <summary>Pozostała liczba prób w bieżącym wyzwaniu.</summary>
        private int _attemptsRemaining;
        
        /// <summary>Timer używany do odliczania czasu przed rozpoczęciem rundy.</summary>
        private DispatcherTimer _countdownTimer;
        
        /// <summary>Pozostały czas odliczania przed rozpoczęciem rundy.</summary>
        private int _countdown = 3;
        
        /// <summary>
        /// Tablica słów używanych w grze Wordle, podzielona na kategorie tematyczne.
        /// Zawiera słowa związane z bronią, zbroją, stworzeniami fantastycznymi, magią i przygodą.
        /// </summary>
        private readonly string[] _wordList = new[]
        {
            // Weapons & Armor
            "sword", "shield", "dagger", "mace", "spear", "staff", "bow", "arrow", "crossbow", "halberd",
            "armor", "helmet", "gauntlet", "greaves", "cuirass", "buckler", "rapier", "scimitar", "cutlass", "warhammer",
            "longbow", "shortbow", "battleaxe", "waraxe", "flail", "whip", "sling", "javelin", "pike", "trident",
            
            // Fantasy Creatures
            "dragon", "wizard", "goblin", "troll", "orc", "elf", "dwarf", "gnome", "harpy", "griffin",
            "phoenix", "basilisk", "centaur", "minotaur", "sphinx", "gorgon", "chimera", "kraken", "mermaid", "wyvern",
            "werewolf", "vampire", "zombie", "ghost", "specter", "wraith", "lich", "mummy", "ghoul", "revenant",
            
            // Magic & Spells
            "magic", "spell", "potion", "scroll", "rune", "glyph", "charm", "curse", "hex", "ward",
            "enchant", "summon", "banish", "conjure", "transmute", "telekinesis", "clairvoyance", "necromancy", "divination", "abjuration",
            "evocation", "illusion", "enchantment", "conjuration", "transmutation", "necromancy", "thaumaturgy", "alchemy", "sorcery", "witchcraft",
            
            // Adventure & Quests
            "quest", "treasure", "dungeon", "castle", "temple", "crypt", "labyrinth", "cavern", "ruins", "stronghold",
            "artifact", "relic", "amulet", "talisman", "crystal", "gemstone", "sapphire", "emerald", "ruby", "diamond",
            "gold", "silver", "copper", "platinum", "mithril", "adamantite", "orichalcum", "mythril", "cobalt", "titanium",
            
            // Character Types
            "knight", "archer", "ranger", "paladin", "cleric", "druid", "bard", "rogue", "thief", "assassin",
            "monk", "barbarian", "sorcerer", "warlock", "necromancer", "witch", "shaman", "alchemist", "artificer", "inquisitor",
            "templar", "berserker", "cavalier", "gunslinger", "swashbuckler", "warlord", "warden", "avenger", "invoker", "seeker"
        };

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="TrapMinigameDialog"/> z określonym poziomem trudności.
        /// </summary>
        /// <param name="difficulty">Poziom trudności minigry.</param>
        /// <param name="onComplete">Akcja wywoływana po zakończeniu minigry. Przyjmuje wartość bool wskazującą, czy gracz wygrał.</param>
        public TrapMinigameDialog(Difficulty difficulty, Action<bool> onComplete)
        {
            InitializeComponent();
            _difficulty = difficulty;
            _onComplete = onComplete;
            Loaded += OnLoaded;
        }

        /// <summary>
        /// Obsługuje zdarzenie załadowania okna. Inicjuje wyświetlanie pierwszej minigry.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await ShowNextMinigame();
        }

        /// <summary>
        /// Wyświetla losowo wybraną minigrę z dostępnych typów.
        /// Ukrywa wszystkie panele i czyści stan poprzedniej gry przed wyświetleniem nowej.
        /// </summary>
        private async Task ShowNextMinigame()
        {
            // Ukryj wszystkie panele
            CodeChallengePanel.Visibility = Visibility.Collapsed;
            ReactionChallengePanel.Visibility = Visibility.Collapsed;
            WordleChallengePanel.Visibility = Visibility.Collapsed;
            GridChallengePanel.Visibility = Visibility.Collapsed;
            _selectedTiles.Clear();

            // Losowo wybierz typ minigry (0-3)
            var gameType = _random.Next(0, 4);
            
            // Inicjalizuj wybrany typ minigry
            switch (gameType)
            {
                case 0:
                    SetupCodeChallenge();
                    break;
                case 1:
                    SetupReactionChallenge();
                    break;
                case 2:
                    SetupWordleChallenge();
                    break;
                case 3:
                    SetupGridChallenge();
                    break;
            }
        }

        #region Code Challenge (4-digit code)
        /// <summary>
        /// Inicjalizuje wyzwanie polegające na odgadnięciu 4-cyfrowego kodu.
        /// Ustawia interfejs użytkownika i generuje losowy kod do odgadnięcia.
        /// </summary>
        private void SetupCodeChallenge()
        {
            CodeChallengePanel.Visibility = Visibility.Visible;
            TrapTitle.Text = "Code Challenge";
            
            // Set number of attempts based on difficulty
            _maxAttempts = _difficulty switch
            {
                Difficulty.Easy => 6,
                Difficulty.Normal => 5,
                Difficulty.Hard => 4,
                Difficulty.Nightmare => 3,
                _ => 4
            };
            _attemptsRemaining = _maxAttempts;
            
            // Generate random 4-digit code
            _codeToGuess = _random.Next(1000, 10000).ToString("D4");
            
            // Clear input fields and feedback
            Digit1.Clear();
            Digit2.Clear();
            Digit3.Clear();
            Digit4.Clear();
            
            // Reset feedback indicators
            Digit1Feedback.Text = "?";
            Digit2Feedback.Text = "?";
            Digit3Feedback.Text = "?";
            Digit4Feedback.Text = "?";
            
            // Clear last attempt
            LastAttempt1.Text = " ";
            LastAttempt2.Text = " ";
            LastAttempt3.Text = " ";
            LastAttempt4.Text = " ";
            
            // Set initial colors
            foreach (var feedback in new[] { Digit1Feedback, Digit2Feedback, Digit3Feedback, Digit4Feedback })
            {
                feedback.Foreground = Brushes.White;
            }
            
            CodeFeedback.Text = "Enter the 4-digit code. You'll get hints after your first attempt.";
            UpdateCodeAttemptsDisplay();
            
            // Focus first digit
            Digit1.Focus();
        }

        /// <summary>
        /// Obsługuje zdarzenie naciśnięcia klawisza w polu wprowadzania cyfry kodu.
        /// Automatycznie przechodzi do następnego pola po wprowadzeniu cyfry.
        /// </summary>
        /// <param name="sender">Pole tekstowe, w którym wprowadzana jest cyfra.</param>
        /// <param name="e">Dane zdarzenia klawisza.</param>
        private void CodeDigit_KeyUp(object sender, KeyEventArgs e)
        {
            var currentBox = (TextBox)sender;
            
            // Move to next box on digit input
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                if (currentBox.Text.Length > 0)
                    MoveToNextCodeDigit(currentBox);
            }
            // Handle backspace
            else if (e.Key == Key.Back)
            {
                if (currentBox.Text.Length == 0)
                    MoveToPreviousCodeDigit(currentBox);
            }
        }

        /// <summary>
        /// Przenosi fokus do następnego pola wprowadzania cyfry kodu.
        /// Jeśli aktualne pole jest ostatnim, wywołuje sprawdzenie kodu.
        /// </summary>
        /// <param name="currentBox">Aktualne pole tekstowe.</param>
        private void MoveToNextCodeDigit(TextBox currentBox)
        {
            if (currentBox == Digit1) Digit2.Focus();
            else if (currentBox == Digit2) Digit3.Focus();
            else if (currentBox == Digit3) Digit4.Focus();
            else if (currentBox == Digit4) CheckCode();
        }

        /// <summary>
        /// Przenosi fokus do poprzedniego pola wprowadzania cyfry kodu.
        /// Używane do nawigacji za pomocą klawisza Backspace.
        /// </summary>
        /// <param name="currentBox">Aktualne pole tekstowe.</param>
        private void MoveToPreviousCodeDigit(TextBox currentBox)
        {
            if (currentBox == Digit4) Digit3.Focus();
            else if (currentBox == Digit3) Digit2.Focus();
            else if (currentBox == Digit2) Digit1.Focus();
        }

        /// <summary>
        /// Sprawdza, czy wprowadzony kod jest poprawny.
        /// Aktualizuje interfejs użytkownika na podstawie wyniku sprawdzenia.
        /// Wywołuje zakończenie minigry w przypadku poprawnego kodu lub wyczerpania prób.
        /// </summary>
        private void CheckCode()
        {
            string userCode = $"{Digit1.Text}{Digit2.Text}{Digit3.Text}{Digit4.Text}";
            
            if (userCode == _codeToGuess)
            {
                _callbackInvoked = true;
                _onComplete?.Invoke(true);
                Task.Delay(1500).ContinueWith(_ => Dispatcher.Invoke(() => this.Close()));
                return;
            }
            
            // Wrong code
            _attemptsRemaining--;
            
            if (_attemptsRemaining <= 0)
            {
                _callbackInvoked = true;
                _onComplete?.Invoke(false);
                this.Close();
                return;
            }
            
            // Give detailed feedback for each digit
            int correctDigits = 0;
            var digitFeedbacks = new[] { Digit1Feedback, Digit2Feedback, Digit3Feedback, Digit4Feedback };
            var digitBoxes = new[] { Digit1, Digit2, Digit3, Digit4 };
            
            for (int i = 0; i < 4; i++)
            {
                if (string.IsNullOrEmpty(digitBoxes[i].Text)) continue;
                
                int userDigit = int.Parse(digitBoxes[i].Text);
                int targetDigit = int.Parse(_codeToGuess[i].ToString());
                
                var lastAttemptBoxes = new[] { LastAttempt1, LastAttempt2, LastAttempt3, LastAttempt4 };
                lastAttemptBoxes[i].Text = userDigit.ToString();
                lastAttemptBoxes[i].Foreground = Brushes.White;

                if (userDigit == targetDigit)
                {
                    digitFeedbacks[i].Text = "✓";
                    digitFeedbacks[i].Foreground = Brushes.LimeGreen;
                    correctDigits++;
                }
                else if (userDigit < targetDigit)
                {
                    digitFeedbacks[i].Text = "↑";
                    digitFeedbacks[i].Foreground = Brushes.Red;
                }
                else
                {
                    digitFeedbacks[i].Text = "↓";
                    digitFeedbacks[i].Foreground = Brushes.Goldenrod;
                }
            }
            
            // Update feedback message
            if (correctDigits > 0)
            {
                CodeFeedback.Text = $"You have {correctDigits} correct digit(s)! Use the arrows as hints:";
                CodeFeedback.Text += "\n✓ = Correct  ↑ = Too Low  ↓ = Too High";
            }
            else if (string.IsNullOrWhiteSpace(userCode))
            {
                CodeFeedback.Text = "Please enter a 4-digit code";
            }
            else
            {
                CodeFeedback.Text = "No correct digits. Use the arrows as hints:";
                CodeFeedback.Text += "\n✓ = Correct  ↑ = Too Low  ↓ = Too High";
            }
            
            UpdateCodeAttemptsDisplay();
            
            // Clear input for next attempt
            foreach (var box in digitBoxes)
            {
                box.Clear();
            }
            Digit1.Focus();
        }

        /// <summary>
        /// Aktualizuje wyświetlaną liczbę pozostałych prób w wyzwaniu kodowym.
        /// </summary>
        private void UpdateCodeAttemptsDisplay()
        {
            CodeAttempts.Text = $"Attempts remaining: {_attemptsRemaining}/{_maxAttempts}";
        }
        #endregion

        #region Reaction Challenge
        /// <summary>
        /// Inicjalizuje wyzwanie reakcji, w którym gracz musi kliknąć przycisk w określonym czasie.
        /// Ustawia interfejs użytkownika i parametry gry na podstawie poziomu trudności.
        /// </summary>
        private void SetupReactionChallenge()
        {
            ReactionChallengePanel.Visibility = Visibility.Visible;
            TrapTitle.Text = "Reaction Challenge";
            ReactionScore.Text = "Hits: 0/10";
            ReactionTimer.Text = "Time: 0.00s";
            ReactionAttempts.Text = "";
            
            // Reset game state
            _hitCount = 0;
            _failedAttempts = 0;
            _isReactionActive = false;
            
            // Set max failed attempts based on difficulty
            _maxFailedAttempts = _difficulty switch
            {
                Difficulty.Easy => 4,
                Difficulty.Normal => 3,
                Difficulty.Hard => 2,
                Difficulty.Nightmare => 1,
                _ => 2
            };
            
            // Set reaction time limit based on difficulty (in milliseconds for more precision)
            _reactionTimeLimit = _difficulty switch
            {
                Difficulty.Easy => 1250,   
                Difficulty.Normal => 1000,  
                Difficulty.Hard => 750,  
                Difficulty.Nightmare => 600,
                _ => 1000
            } / 1000.0;
            
            // Start countdown
            StartCountdown();
        }
        
        /// <summary>
        /// Rozpoczyna odliczanie przed rozpoczęciem rundy wyzwania reakcji.
        /// Wyświetla komunikat odliczania i przygotowuje interfejs użytkownika.
        /// </summary>
        private void StartCountdown()
        {
            
            _countdown = 2;
            ReactionStatus.Text = "Starting in 3...";
            ReactionButton.Visibility = Visibility.Collapsed;
            ReactionInstructions.Text = "Get ready to click the red button as fast as you can when it appears!";
            
            // Clean up any existing timer
            if (_countdownTimer != null)
            {
                _countdownTimer.Stop();
                _countdownTimer.Tick -= CountdownTimer_Tick;
            }
            
            _countdownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            
            _countdownTimer.Tick += CountdownTimer_Tick;
            _countdownTimer.Start();
            
        }
        
        /// <summary>
        /// Obsługuje zdarzenie tyknięcia timera odliczania.
        /// Aktualizuje wyświetlany czas i rozpoczyna rundę po zakończeniu odliczania.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (timer).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            
            if (_countdown > 0)
            {
                ReactionStatus.Text = $"Starting in {_countdown}...";
                _countdown--;
            }
            else
            {
                _countdownTimer.Stop();
                _countdownTimer.Tick -= CountdownTimer_Tick;
                StartReactionRound();
            }
        }

        /// <summary>
        /// Rozpoczyna nową rundę wyzwania reakcji.
        /// Ustawia przycisk w losowym miejscu i rozpoczyna pomiar czasu reakcji.
        /// </summary>
        private void StartReactionRound()
        {
            if (_hitCount >= 10) return; // Already won
            
            // Clean up any existing timer
            if (_reactionTimer != null)
            {
                _reactionTimer.Stop();
                _reactionTimer.Tick -= UpdateReactionTimer;
            }
            
            _isReactionActive = true;
            _reactionStartTime = DateTime.UtcNow; // Use UTC for more precise timing
            _roundEndTime = _reactionStartTime.AddMilliseconds(_reactionTimeLimit * 1000);
            
            // Set random position for the button
            var canvas = ReactionButton.Parent as Canvas;
            if (canvas != null)
            {
                double maxX = canvas.ActualWidth - ReactionButton.ActualWidth - 20;
                double maxY = canvas.ActualHeight - ReactionButton.ActualHeight - 20;
                
                if (maxX > 0 && maxY > 0)
                {
                    double x = 10 + _random.NextDouble() * maxX;
                    double y = 10 + _random.NextDouble() * maxY;
                    
                    // Ensure button stays within canvas bounds
                    x = Math.Max(10, Math.Min(x, maxX + 10));
                    y = Math.Max(10, Math.Min(y, maxY + 10));
                    
                    Canvas.SetLeft(ReactionButton, x);
                    Canvas.SetTop(ReactionButton, y);
                }
            }
            
            ReactionButton.Visibility = Visibility.Visible;
            ReactionStatus.Text = "CLICK NOW!";
            ReactionStatus.Foreground = Brushes.White;
            
            // Create and start the timer
            _reactionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS for smooth updates
            };
            _reactionTimer.Tick += UpdateReactionTimer;
            _reactionTimer.Start();
        }
        
        /// <summary>
        /// Aktualizuje wyświetlany czas reakcji i sprawdza, czy nie upłynął limit czasu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (timer).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void UpdateReactionTimer(object sender, EventArgs e)
        {
            if (!_isReactionActive) return;
            
            var now = DateTime.UtcNow;
            var timeLeft = (_roundEndTime - now).TotalSeconds;
            
            // Update the timer display
            var displayTime = Math.Max(0, timeLeft);
            ReactionTimer.Text = $"Time: {displayTime:0.00}s";
            
            // Check for timeout with a small buffer to account for timer inaccuracies
            if (now >= _roundEndTime)
            {
                _reactionTimer.Stop();
                Dispatcher.Invoke(() => ReactionTimeout(null, null));
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku reakcji.
        /// Mierzy czas reakcji i sprawdza, czy gracz zdążył w odpowiednim czasie.
        /// </summary>
        /// <param name="sender">Przycisk reakcji.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void ReactionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isReactionActive) return;
            
            _isReactionActive = false;
            if (_reactionTimer != null)
            {
                _reactionTimer.Stop();
            }
            
            var now = DateTime.UtcNow;
            var reactionTime = (now - _reactionStartTime).TotalSeconds;
            var timeLeft = (_roundEndTime - now).TotalSeconds;
            
            ReactionButton.Visibility = Visibility.Collapsed;
            
            if (reactionTime <= _reactionTimeLimit)
            {
                // Success - hit the button in time
                _hitCount++;
                ReactionScore.Text = $"Hits: {_hitCount}/10";
                ReactionStatus.Text = $"Perfect! ({reactionTime:F2}s)";
                ReactionStatus.Foreground = Brushes.LightGreen;
                
                if (_hitCount >= 10)
                {
                    // Challenge complete
                    ReactionInstructions.Text = "Challenge complete! Disarming trap...";
                    _callbackInvoked = true;
                    _onComplete?.Invoke(true);
                    Task.Delay(1500).ContinueWith(_ => Dispatcher.Invoke(() => this.Close()));
                    return;
                }
                
                // Start next round after a short delay
                Task.Delay(800).ContinueWith(_ => Dispatcher.Invoke(StartReactionRound));
            }
            else
            {
                // Clicked too late
                HandleReactionFailure();
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie przekroczenia czasu reakcji.
        /// Wywołuje obsługę nieudanej próby.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (timer).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void ReactionTimeout(object sender, EventArgs e)
        {
            
            if (!_isReactionActive) return;
            
            Dispatcher.Invoke(() =>
            {
                _isReactionActive = false;
                if (_reactionTimer != null)
                {
                    _reactionTimer.Stop();
                    _reactionTimer.Tick -= UpdateReactionTimer;
                }
                
                ReactionButton.Visibility = Visibility.Collapsed;
                HandleReactionFailure();
            });
        }
        
        /// <summary>
        /// Obsługuje nieudaną próbę w wyzwaniu reakcji.
        /// Aktualizuje interfejs użytkownika i sprawdza, czy gracz ma jeszcze próby.
        /// Kończy grę, jeśli przekroczono maksymalną liczbę nieudanych prób.
        /// </summary>
        private void HandleReactionFailure()
        {
            
            // Ensure the game state is clean
            _isReactionActive = false;
            
            // Clean up timer
            if (_reactionTimer != null)
            {
                _reactionTimer.Stop();
                _reactionTimer.Tick -= UpdateReactionTimer;
            }
            
            _failedAttempts++;
            ReactionStatus.Text = "Too slow! Try again.";
            ReactionStatus.Foreground = Brushes.OrangeRed;
            ReactionAttempts.Text = $"Attempts remaining: {_maxFailedAttempts - _failedAttempts}";
            
            if (_failedAttempts >= _maxFailedAttempts)
            {
                // Out of attempts
                ReactionStatus.Text = "Too slow! The trap triggers!";
                ReactionInstructions.Text = "Out of attempts! The trap triggers...";
                _onComplete?.Invoke(false);
                _callbackInvoked = true;
                Task.Delay(1500).ContinueWith(_ => Dispatcher.Invoke(() => this.Close()));
            }
            else
            {
                Task.Delay(800).ContinueWith(_ => Dispatcher.Invoke(StartReactionRound));
            }
        }
        #endregion

        #region Wordle Challenge
        /// <summary>
        /// Inicjalizuje wyzwanie słówkowe (Wordle).
        /// Wybiera losowe słowo na podstawie poziomu trudności i przygotowuje interfejs użytkownika.
        /// </summary>
        private void SetupWordleChallenge()
        {
            WordleChallengePanel.Visibility = Visibility.Visible;
            TrapTitle.Text = "Wordle Challenge";
            
            // Select word based on difficulty
            int wordLength = _difficulty switch
            {
                Difficulty.Easy => _random.Next(3, 5),
                Difficulty.Normal => 5,
                Difficulty.Hard => 6,
                Difficulty.Nightmare => _random.Next(7, 9),
                _ => 5
            };
            
            // Filter words by length and unique letters based on difficulty
            var validWords = _wordList
                .Where(w => w.Length == wordLength)
                .Where(w => 
                {
                    var uniqueLetters = new HashSet<char>(w);
                    return _difficulty switch
                    {
                        Difficulty.Easy => uniqueLetters.Count is >= 3 and <= 4,
                        Difficulty.Normal => uniqueLetters.Count == 5,
                        Difficulty.Hard => uniqueLetters.Count == 6,
                        Difficulty.Nightmare => uniqueLetters.Count >= 7,
                        _ => true
                    };
                })
                .ToList();
                
            if (validWords.Count == 0)
            {
                // Fallback to any word of the right length if no words match the unique letters criteria
                    validWords = _wordList
                    .Where(w => w.Length == wordLength)
                    .ToList();
                    
                // If still no words, use any word from the list
                if (validWords.Count == 0)
                {
                    validWords = _wordList.ToList();
                }
            }
            
            _wordToGuess = validWords[_random.Next(validWords.Count)].ToUpper();
            _maxWordleAttempts = _difficulty switch
            {
                Difficulty.Easy => 8,
                Difficulty.Normal => 7,
                Difficulty.Hard => 6,
                Difficulty.Nightmare => 5,
                _ => 6
            };
            
            // Clear previous guesses
            WordleGuesses.Children.Clear();
            _wordGuesses.Clear();
            WordleFeedback.Text = "";
            UpdateWordleAttemptsDisplay();
            
            // Setup Wordle challenge
            WordleWordLength.Text = $"The word has {_wordToGuess.Length} letters";
            
            // Clear previous letter boxes
            WordleInputContainer.Children.Clear();
            _wordleLetterBoxes.Clear();
            _currentLetterIndex = 0;
            
            // Create letter input boxes
            for (int i = 0; i < _wordToGuess.Length; i++)
            {
                var textBox = new TextBox
                {
                    Width = 40,
                    Height = 50,
                    Margin = new Thickness(2),
                    FontSize = 20,
                    MaxLength = 1,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Style = (Style)FindResource("MaterialDesignFloatingHintTextBox")
                };
                
                textBox.KeyUp += WordleLetterBox_KeyUp;
                textBox.PreviewTextInput += WordleLetterBox_PreviewTextInput;
                textBox.PreviewKeyDown += WordleLetterBox_PreviewKeyDown;
                
                _wordleLetterBoxes.Add(textBox);
                WordleInputContainer.Children.Add(textBox);
            }
            
            // Focus first letter box
            if (_wordleLetterBoxes.Count > 0)
            {
                Dispatcher.BeginInvoke(new Action(() => _wordleLetterBoxes[0].Focus()));
            }
            
            UpdateWordleAttemptsDisplay();
        }

        /// <summary>
        /// Obsługuje zdarzenie wprowadzania tekstu do pola literowego.
        /// Zapewnia, że wprowadzane są tylko litery i automatycznie przechodzi do następnego pola.
        /// </summary>
        /// <param name="sender">Pole tekstowe, do którego wprowadzany jest tekst.</param>
        /// <param name="e">Dane zdarzenia wprowadzania tekstu.</param>
        private void WordleLetterBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Only allow letters
            if (!char.IsLetter(e.Text[0]))
            {
                e.Handled = true;
                return;
            }
            
            // Convert to uppercase
            e.Handled = true;
            var textBox = (TextBox)sender;
            textBox.Text = e.Text.ToUpper();
            
            // Move to next box
            _currentLetterIndex = Math.Min(_currentLetterIndex + 1, _wordleLetterBoxes.Count - 1);
            if (_currentLetterIndex < _wordleLetterBoxes.Count)
            {
                _wordleLetterBoxes[_currentLetterIndex].Focus();
            }
        }
        
        /// <summary>
        /// Obsługuje zdarzenie naciśnięcia klawisza w polu literowym.
        /// Umożliwia nawigację między polami za pomocą strzałek, backspace i enter.
        /// </summary>
        /// <param name="sender">Pole tekstowe, w którym naciśnięto klawisz.</param>
        /// <param name="e">Dane zdarzenia klawisza.</param>
        private void WordleLetterBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = (TextBox)sender;
            int currentIndex = _wordleLetterBoxes.IndexOf(textBox);
            
            if (e.Key == Key.Back && string.IsNullOrEmpty(textBox.Text))
            {
                // Move to previous box on backspace when current is empty
                if (currentIndex > 0)
                {
                    _currentLetterIndex = currentIndex - 1;
                    _wordleLetterBoxes[_currentLetterIndex].Focus();
                    _wordleLetterBoxes[_currentLetterIndex].Clear();
                    _wordleLetterBoxes[_currentLetterIndex].SelectAll();
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Enter)
            {
                CheckWordleGuess();
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                // Move left with left arrow
                if (currentIndex > 0)
                {
                    _currentLetterIndex = currentIndex - 1;
                    _wordleLetterBoxes[_currentLetterIndex].Focus();
                    _wordleLetterBoxes[_currentLetterIndex].SelectAll();
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Right || (e.Key == Key.Space && !string.IsNullOrEmpty(textBox.Text)))
            {
                // Move right with right arrow or space when box has content
                if (currentIndex < _wordleLetterBoxes.Count - 1)
                {
                    _currentLetterIndex = currentIndex + 1;
                    _wordleLetterBoxes[_currentLetterIndex].Focus();
                    _wordleLetterBoxes[_currentLetterIndex].SelectAll();
                    e.Handled = true;
                }
            }
        }
        
        /// <summary>
        /// Obsługuje zdarzenie zwolnienia klawisza w polu literowym.
        /// Umożliwia nawigację między polami za pomocą klawisza Tab.
        /// </summary>
        /// <param name="sender">Pole tekstowe, w którym zwolniono klawisz.</param>
        /// <param name="e">Dane zdarzenia klawisza.</param>
        private void WordleLetterBox_KeyUp(object sender, KeyEventArgs e)
        {
            // Handle tab navigation between boxes
            if (e.Key == Key.Tab)
            {
                var textBox = (TextBox)sender;
                int currentIndex = _wordleLetterBoxes.IndexOf(textBox);
                
                if (currentIndex >= 0)
                {
                    _currentLetterIndex = (currentIndex + 1) % _wordleLetterBoxes.Count;
                    _wordleLetterBoxes[_currentLetterIndex].Focus();
                    _wordleLetterBoxes[_currentLetterIndex].SelectAll();
                }
                e.Handled = true;
            }
        }
        
        /// <summary>
        /// Sprawdza zgadywane słowo w wyzwaniu Wordle.
        /// Porównuje wprowadzone słowo z docelowym i aktualizuje interfejs użytkownika.
        /// Kończy grę, jeśli słowo zostało odgadnięte lub wyczerpano próby.
        /// </summary>
        private void CheckWordleGuess()
        {
            // Get the current guess from letter boxes
            var guess = string.Join("", _wordleLetterBoxes.Select(box => box.Text)).ToUpper();
            
            if (guess.Length != _wordToGuess.Length)
            {
                WordleFeedback.Text = $"Please enter all {_wordToGuess.Length} letters";
                return;
            }
            
            _wordGuesses.Add(guess);
            DisplayWordleGuess(guess);
            
            // Clear input boxes for next guess
            foreach (var box in _wordleLetterBoxes)
            {
                box.Clear();
            }
            _currentLetterIndex = 0;
            if (_wordleLetterBoxes.Count > 0)
            {
                _wordleLetterBoxes[0].Focus();
            }
            
            if (guess == _wordToGuess)
            {
                _onComplete?.Invoke(true);
                this.Close();
                return;
            }
            
            if (_wordGuesses.Count >= _maxWordleAttempts)
            {
                WordleFeedback.Text = $"Out of attempts! The word was: {_wordToGuess}";
                _onComplete?.Invoke(false);
                this.Close();
                return;
            }
            
            UpdateWordleAttemptsDisplay();
        }

        /// <summary>
        /// Wyświetla wynik zgadywania słowa w interfejsie użytkownika.
        /// Oznacza litery jako poprawne (zielone), nie na swoim miejscu (żółte) lub nieobecne (szare).
        /// </summary>
        /// <param name="guess">Zgadywane słowo.</param>
        private void DisplayWordleGuess(string guess)
        {
            // Create a row for this guess
            var guessRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
            
            // Check each letter
            bool isCorrect = true;
            var remainingLetters = _wordToGuess.ToList();
            
            // First pass: find correct letters in correct positions
            for (int i = 0; i < guess.Length; i++)
            {
                var letter = guess[i].ToString();
                var border = new Border 
                { 
                    Width = 30, 
                    Height = 30, 
                    Margin = new Thickness(2),
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1)
                };
                
                var textBlock = new TextBlock 
                { 
                    Text = letter, 
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold
                };
                
                if (letter[0] == _wordToGuess[i])
                {
                    // Correct letter in correct position (green)
                    border.Background = Brushes.DarkGreen;
                    remainingLetters.Remove(letter[0]);
                }
                else if (_wordToGuess.Contains(letter))
                {
                    // Correct letter in wrong position (yellow)
                    border.Background = Brushes.Goldenrod;
                    isCorrect = false;
                }
                else
                {
                    // Wrong letter (gray)
                    border.Background = Brushes.LightGray;
                    isCorrect = false;
                }
                
                border.Child = textBlock;
                guessRow.Children.Add(border);
            }
            
            WordleGuesses.Children.Add(guessRow);
        }

        /// <summary>
        /// Aktualizuje wyświetlaną liczbę pozostałych prób w wyzwaniu Wordle.
        /// </summary>
        private void UpdateWordleAttemptsDisplay()
        {
            WordleAttempts.Text = $"Attempts: {_wordGuesses.Count}/{_maxWordleAttempts}";
        }
        #endregion

        #region Grid Challenge
        /// <summary>
        /// Inicjalizuje wyzwanie siatkowe, w którym gracz musi wybrać bezpieczne kafelki.
        /// Ustawia parametry gry na podstawie poziomu trudności i przygotowuje interfejs użytkownika.
        /// </summary>
        private void SetupGridChallenge()
        {
            GridChallengePanel.Visibility = Visibility.Visible;
            TrapTitle.Text = "Grid Challenge";
            
            // Set number of tiles to select based on difficulty
            _tilesToSelect = _difficulty switch
            {
                Difficulty.Easy => 6,
                Difficulty.Normal => 7,
                Difficulty.Hard => 8,
                Difficulty.Nightmare => 8,
                _ => 4
            };;
            
            // Set number of attempts based on difficulty
            _maxAttempts = _difficulty switch
            {
                Difficulty.Easy => 4,
                Difficulty.Normal => 3,
                Difficulty.Hard => 3,
                Difficulty.Nightmare => 2,
                _ => 4
            };
            
            _attemptsRemaining = _maxAttempts;
            _bombPosition = _random.Next(0, 9);
            _selectedTiles.Clear();
            _attemptFailed = false;
            
            // Clear previous grid
            GridContainer.ItemsSource = null;
            
            // Create 3x3 grid
            var gridButtons = new List<GridButtonInfo>();
            for (int i = 0; i < 9; i++)
            {
                gridButtons.Add(new GridButtonInfo 
                { 
                    Index = i,
                    IsSelected = false,
                    IsBomb = (i == _bombPosition),
                    IsSafe = false
                });
            }
            
            GridContainer.ItemsSource = gridButtons;
            
            // Reset confirm button
            GridConfirmButton.IsEnabled = false;
            
            UpdateGridStatus();
        }
        
        /// <summary>
        /// Klasa reprezentująca informacje o przycisku w siatce wyzwania.
        /// Implementuje interfejs INotifyPropertyChanged do powiadamiania o zmianach właściwości.
        /// </summary>
        /// <summary>
        /// Klasa reprezentująca informacje o przycisku w siatce wyzwania.
        /// Implementuje interfejs INotifyPropertyChanged do powiadamiania o zmianach właściwości.
        /// </summary>
        private class GridButtonInfo : INotifyPropertyChanged
        {
            private bool _isSelected;
            private bool _isBomb;
            private bool _isSafe;
            
            /// <summary>Indeks przycisku w siatce.</summary>
            public int Index { get; set; }
            
            /// <summary>Określa, czy przycisk jest zaznaczony.</summary>
            public bool IsSelected 
            { 
                get => _isSelected;
                set { _isSelected = value; OnPropertyChanged(); OnPropertyChanged(nameof(Content)); }
            }
            
            /// <summary>Określa, czy przycisk zawiera bombę.</summary>
            public bool IsBomb 
            { 
                get => _isBomb;
                set { _isBomb = value; OnPropertyChanged(); OnPropertyChanged(nameof(Content)); }
            }
            
            /// <summary>Określa, czy przycisk jest bezpieczny (został już sprawdzony).</summary>
            public bool IsSafe 
            { 
                get => _isSafe;
                set { _isSafe = value; OnPropertyChanged(); }
            }
            
            /// <summary>Zawartość wyświetlana na przycisku (numer przycisku).</summary>
            public string Content => (Index + 1).ToString();
            
            /// <summary>Zdarzenie wywoływane przy zmianie właściwości.</summary>
            public event PropertyChangedEventHandler PropertyChanged;
            
            /// <summary>
            /// Wywołuje zdarzenie PropertyChanged z podaną nazwą właściwości.
            /// </summary>
            /// <param name="propertyName">Nazwa zmienionej właściwości (automatycznie uzupełniana).</param>
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku w siatce.
        /// Zaznacza lub odznacza wybrany kafelek, jeśli jest to możliwe.
        /// </summary>
        /// <param name="sender">Przycisk, który został kliknięty.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void GridButton_Click(object sender, RoutedEventArgs e)
        {
            if (_attemptFailed) return; // Don't allow selection during failed attempt state
            
            var button = (Button)sender;
            var buttonInfo = (GridButtonInfo)button.DataContext;
            
            // Don't allow changing safe tiles after they're revealed
            if (buttonInfo.IsSafe) return;
            
            if (buttonInfo.IsSelected)
            {
                // Toggle selection
                buttonInfo.IsSelected = false;
                _selectedTiles.Remove(buttonInfo.Index);
            }
            else if (_selectedTiles.Count < _tilesToSelect)
            {
                // Select new tile
                buttonInfo.IsSelected = true;
                _selectedTiles.Add(buttonInfo.Index);
            }
            
            // Update confirm button state
            GridConfirmButton.IsEnabled = _selectedTiles.Count == _tilesToSelect;
            
            UpdateGridStatus();
        }
        
        private bool _attemptFailed = false;
        
        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku potwierdzenia wyboru w wyzwaniu siatkowym.
        /// Sprawdza, czy wybrane kafelki są bezpieczne i aktualizuje stan gry.
        /// </summary>
        /// <param name="sender">Przycisk potwierdzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private async void GridConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (_attemptFailed) return;
            
            _attemptsRemaining--;
            
            // Check if any selected tile is the bomb
            bool hitBomb = _selectedTiles.Contains(_bombPosition);
            
            if (hitBomb)
            {
                _attemptFailed = true;
                
                
                // Mark all unselected tiles as safe (except the bomb)
                foreach (var item in GridContainer.Items)
                {
                    var buttonInfo = (GridButtonInfo)item;
                    if (!buttonInfo.IsSelected)
                    {
                        if (buttonInfo.IsSafe) continue;
                        _tilesToSelect--;
                        buttonInfo.IsSafe = true;
                    }
                }
                
                GridChallengeStatus.Text = "You hit a bomb! The trap triggers!";
                GridConfirmButton.IsEnabled = false;
                
                if (_attemptsRemaining > 0)
                {
                    // Show failure state for a moment
                    await Task.Delay(1500);
                    
                    // Reset for next attempt
                    _selectedTiles.Clear();
                    _attemptFailed = false;
                    
                    // Clear all selections but keep safe tiles marked
                    foreach (var item in GridContainer.Items)
                    {
                        var buttonInfo = (GridButtonInfo)item;
                        if (!buttonInfo.IsSafe)
                        {
                            buttonInfo.IsSelected = false;
                        }
                    }
                    
                    UpdateGridStatus();
                }
                else
                {
                    // No more attempts, fail
                    _callbackInvoked = true;
                    _onComplete?.Invoke(false);
                    await Task.Delay(1500);
                    _canClose = true;
                    Dispatcher.Invoke(() => this.Close());
                }
            }
            else if (_selectedTiles.Count == _tilesToSelect)
            {
                // Successfully selected all safe tiles
                GridChallengeStatus.Text = "Success! You found safe tiles and disarmed the trap!";
                _callbackInvoked = true;
                _onComplete?.Invoke(true);
                _canClose = true;
                await Task.Delay(1500);
                Dispatcher.Invoke(() => this.Close());
                return;
            }
            
            UpdateGridStatus();
        }

        /// <summary>
        /// Aktualizuje stan interfejsu użytkownika dla wyzwania siatkowego.
        /// Wyświetla odpowiednie komunikaty i aktualizuje przycisk potwierdzenia.
        /// </summary>
        private void UpdateGridStatus()
        {
            if (_attemptFailed)
            {
                GridChallengeStatus.Text = "You hit a bomb! The trap triggers!";
                GridAttempts.Text = $"Attempt unsuccessful! Attempts remaining: {_attemptsRemaining}";
            }
            else
            {
                GridChallengeStatus.Text = $"Select {_tilesToSelect} safe tiles. Choose carefully!";
                GridAttempts.Text = $"Attempts remaining: {_attemptsRemaining}";
            }
            
            // Update confirm button text based on selection
            if (_selectedTiles.Count > 0 && !_attemptFailed)
            {
                GridConfirmButton.Content = $"Confirm Selection ({_selectedTiles.Count}/{_tilesToSelect})";
            }
            else if (_attemptFailed)
            {
                GridConfirmButton.Content = "Attempt Failed";
            }
            else
            {
                GridConfirmButton.Content = "Select Tiles First";
            }
        }
        #endregion

        /// <summary>
        /// Flaga wskazująca, czy wywołano już callback z wynikiem gry.
        /// Zapobiega wielokrotnemu wywołaniu callbacku.
        /// </summary>
        private bool _callbackInvoked = false;

        /// <summary>
        /// Obsługuje zdarzenie zamykania okna.
        /// Zapobiega zamknięciu okna, jeśli gra nie została zakończona.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (okno).</param>
        /// <param name="e">Dane zdarzenia anulowania zamknięcia.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_canClose)
            {
                e.Cancel = true;
                MessageBox.Show("You must complete the trap minigame. You cannot close this window directly.", 
                    "Trap Minigame In Progress", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Wywoływana, gdy okno zostało zamknięte.
        /// Zapewnia poprawne zakończenie gry i wywołanie callbacku, jeśli nie zostało to zrobione wcześniej.
        /// </summary>
        /// <param name="e">Dane zdarzenia.</param>
        protected override void OnClosed(EventArgs e)
        {
            if (!_isDisarmed && !_callbackInvoked)
            {
                _callbackInvoked = true;
                _onComplete?.Invoke(false);
            }
            base.OnClosed(e);
        }
    }
}
