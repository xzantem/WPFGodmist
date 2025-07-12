using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GodmistWPF.Quests;

namespace GodmistWPF.Controls
{
    /// <summary>
    /// Kontrolka wyświetlająca powiadomienie o ukończeniu zadania.
    /// </summary>
    /// <remarks>
    /// Wyświetla animowane powiadomienie o ukończeniu zadania z przyciskiem kontynuacji.
    /// Obsługuje kolejkę zadań, wyświetlając je jedno po drugim.
    /// </remarks>
    public partial class QuestCompletedOverlay : UserControl
    {
        private Queue<Quest> _completedQuests = new Queue<Quest>();
        
        /// <summary>
        /// Identyfikuje właściwość zależności QuestName.
        /// </summary>
        public static readonly DependencyProperty QuestNameProperty =
            DependencyProperty.Register("QuestName", typeof(string), typeof(QuestCompletedOverlay), 
                new PropertyMetadata(string.Empty));
                
        /// <summary>
        /// Identyfikuje właściwość zależności QuestEnderText.
        /// </summary>
                
        public static readonly DependencyProperty QuestEnderTextProperty =
            DependencyProperty.Register(nameof(QuestEnder), typeof(string), typeof(QuestCompletedOverlay), 
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Pobiera lub ustawia nazwę ukończonego zadania.
        /// </summary>
        public string QuestName
        {
            get => (string)GetValue(QuestNameProperty);
            set => SetValue(QuestNameProperty, value);
        }
        
        /// <summary>
        /// Pobiera lub ustawia tekst informujący o osobie, do której należy się zgłosić po nagrodę.
        /// </summary>
        public string QuestEnder
        {
            get => (string)GetValue(QuestEnderTextProperty);
            set => SetValue(QuestEnderTextProperty, value);
        }

        /// <summary>
        /// Występuje, gdy wszystkie zadania z kolejki zostały wyświetlone.
        /// </summary>
        public event EventHandler AllQuestsShown;

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="QuestCompletedOverlay"/>.
        /// </summary>
        public QuestCompletedOverlay()
        {
            InitializeComponent();
            Visibility = Visibility.Collapsed;
            DataContext = this;
        }

        /// <summary>
        /// Wyświetla kolekcję ukończonych zadań.
        /// </summary>
        /// <param name="quests">Kolekcja ukończonych zadań do wyświetlenia.</param>
        /// <remarks>
        /// Jeśli kolekcja jest pusta lub równa null, od razu wywołuje zdarzenie AllQuestsShown.
        /// </remarks>
        public void ShowCompletedQuests(IEnumerable<Quest> quests)
        {
            if (quests == null || !quests.Any()) 
            {
                AllQuestsShown?.Invoke(this, EventArgs.Empty);
                return;
            }

            _completedQuests = new Queue<Quest>(quests);
            ShowNextQuest();
        }

        /// <summary>
        /// Wyświetla następne zadanie z kolejki.
        /// </summary>
        /// <remarks>
        /// Jeśli kolejka jest pusta, ukrywa kontrolkę i wywołuje zdarzenie AllQuestsShown.
        /// W przeciwnym razie aktualizuje interfejs użytkownika danymi następnego zadania.
        /// </remarks>
        private void ShowNextQuest()
        {
            if (_completedQuests.Count == 0)
            {
                Visibility = Visibility.Collapsed;
                AllQuestsShown?.Invoke(this, EventArgs.Empty);
                return;
            }

            var quest = _completedQuests.Dequeue();
            QuestName = quest.Name;
            QuestEnder = !string.IsNullOrEmpty(quest.QuestEnder) 
                ? $"Report back to {quest.QuestEnder} to collect your rewards!"
                : "Return to the quest giver to collect your rewards!";
            Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku kontynuacji.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            ShowNextQuest();
        }
    }
}
