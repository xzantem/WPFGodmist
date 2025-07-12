using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;

namespace GodmistWPF.Dialogs
{
    /// <summary>
    /// Okno dialogowe umożliwiające zmianę języka interfejsu użytkownika.
    /// Obsługuje ustawianie kultury, kierunku tekstu (RTL/LTR) oraz zapisywanie wyboru użytkownika.
    /// </summary>
    public partial class LanguageSelectionDialog : UserControl
    {
        /// <summary>
        /// Zdarzenie wywoływane po zmianie języka interfejsu użytkownika.
        /// </summary>
        public event EventHandler LanguageChanged;

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="LanguageSelectionDialog">.
        /// Konfiguruje komponenty interfejsu i wybiera aktualnie ustawiony język.
        /// </summary>
        public LanguageSelectionDialog()
        {
            InitializeComponent();
            SelectCurrentLanguage();
        }

        /// <summary>
        /// Wybiera aktualnie ustawiony język na liście dostępnych języków.
        /// Porównuje bieżącą kulturę wątku z dostępnymi opcjami i ustawia odpowiedni element jako wybrany.
        /// </summary>
        private void SelectCurrentLanguage()
        {
            var currentCulture = Thread.CurrentThread.CurrentUICulture;
            foreach (ListBoxItem item in LanguagesListBox.Items)
            {
                if (item.Tag?.ToString() == currentCulture.Name)
                {
                    item.IsSelected = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Obsługuje zmianę wybranego języka na liście.
        /// Aktualizuje ustawienia kultury, kierunek tekstu i zapisuje preferencje użytkownika.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (lista języków).</param>
        /// <param name="e">Dane zdarzenia zmiany wyboru.</param>
        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Debug.WriteLine($"[LanguageSelection] Selection changed. Added items: {e.AddedItems.Count}, Removed items: {e.RemovedItems.Count}");

                if (e.AddedItems.Count == 0)
                {
                    Debug.WriteLine("[LanguageSelection] No language selected");
                    return;
                }

                if (!(e.AddedItems[0] is ListBoxItem selectedItem) || selectedItem.Tag == null)
                {
                    Debug.WriteLine("[LanguageSelection] Invalid selection - no valid ListBoxItem or Tag is null");
                    return;
                }

                string cultureName = selectedItem.Tag.ToString();
                Debug.WriteLine($"[LanguageSelection] Selected culture: {cultureName}");

                // Utwórz nową kulturę na podstawie wybranego języka
                var culture = new CultureInfo(cultureName);
                Debug.WriteLine($"[LanguageSelection] Culture created: {culture.DisplayName} (Native: {culture.NativeName}), RTL: {culture.TextInfo.IsRightToLeft}");

                // Ustaw kulturę dla bieżącego wątku i domyślną dla nowych wątków
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                CultureInfo.DefaultThreadCurrentCulture = culture;

                Debug.WriteLine($"[LanguageSelection] Thread cultures updated");

                // Zaktualizuj kierunek tekstu dla języków RTL (np. arabski, hebrajski)
                var flowDirection = culture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                var xmlLanguage = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
                
                // Zastosuj zmiany do interfejsu użytkownika
                FlowDirection = flowDirection;
                Language = xmlLanguage;
                
                Debug.WriteLine($"[LanguageSelection] UI flow direction set to: {flowDirection}");

                // Zapisz wybór użytkownika w ustawieniach aplikacji
                try
                {
                    var settings = new WPFGodmist.Properties.Settings();
                    string previousLanguage = settings.Language ?? "(not set)";
                    settings.Language = culture.Name;
                    settings.Save();
                    Debug.WriteLine($"[LanguageSelection] Language preference saved: {previousLanguage} -> {culture.Name}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[LanguageSelection] Error saving settings: {ex.Message}");
                }

                // Powiadom subskrybentów o zmianie języka
                Debug.WriteLine("[LanguageSelection] Notifying subscribers about language change");
                LanguageChanged?.Invoke(this, EventArgs.Empty);
                Debug.WriteLine("[LanguageSelection] Language change notification sent");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LanguageSelection] Error in Language_SelectionChanged: {ex}");
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku zamknięcia okna dialogowego.
        /// Wyszukuje i zamyka nadrzędny element DialogHost (jeśli istnieje).
        /// </summary>
        /// <param name="sender">Źródło zdarzenia (przycisk).</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialogHost = this.FindParent<MaterialDesignThemes.Wpf.DialogHost>();
            if (dialogHost != null)
            {
                dialogHost.IsOpen = false;
            }
        }
    }

    /// <summary>
    /// Klasa zawierająca metody rozszerzające dla typów z przestrzeni nazw WPF.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Rekurencyjnie znajduje obiekt nadrzędny określonego typu w drzewie wizualnym.
        /// </summary>
        /// <typeparam name="T">Typ obiektu nadrzędnego do znalezienia.</typeparam>
        /// <param name="child">Obiekt potomny, od którego rozpoczyna się wyszukiwanie.</param>
        /// <returns>Pierwszy napotkany obiekt nadrzędny określonego typu lub null, jeśli nie znaleziono.</returns>
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            if (child == null) return null;
            
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            if (parent == null) return null;
            return parent is T ? parent as T : FindParent<T>(parent);
        }
    }
}
