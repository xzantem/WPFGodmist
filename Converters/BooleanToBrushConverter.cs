using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GodmistWPF.Converters
{
    /// <summary>
    /// Konwerter, który zamienia wartość logiczną na pędzel (Brush) z odpowiednim kolorem.
    /// </summary>
    /// <remarks>
    /// Domyślnie zwraca zielony pędzel dla wartości true i czerwony dla false.
    /// Można dostosować kolory poprzez parametr w formacie "kolor_true|kolor_false".
    /// Kolory muszą być w formacie szesnastkowym (np. "#4CAF50|#F44336").
    /// </remarks>
    public class BooleanToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Konwertuje wartość logiczną na pędzel z odpowiednim kolorem.
        /// </summary>
        /// <param name="value">Wartość do przekonwertowania (bool).</param>
        /// <param name="targetType">Typ docelowy (ignorowany).</param>
        /// <param name="parameter">Opcjonalny parametr w formacie "kolor_true|kolor_false".</param>
        /// <param name="culture">Kultura używana do konwersji (ignorowana).</param>
        /// <returns>Pędzel z odpowiednim kolorem (domyślnie zielony dla true, czerwony dla false, szary w przypadku błędu).</returns>
        /// <remarks>
        /// Jeśli parametr nie jest w odpowiednim formacie, używane są domyślne kolory.
        /// </remarks>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // Default colors: Green for true, Red for false
                string trueColor = "#4CAF50"; // Green
                string falseColor = "#F44336"; // Red
                
                // Parse custom colors from parameter if provided (format: "trueColor|falseColor")
                if (parameter is string colors)
                {
                    var colorParts = colors.Split('|');
                    if (colorParts.Length >= 2)
                    {
                        trueColor = colorParts[0];
                        falseColor = colorParts[1];
                    }
                }
                
                var color = boolValue ? trueColor : falseColor;
                return (SolidColorBrush)new BrushConverter().ConvertFrom(color);
            }
            return Brushes.Gray; // Default color if value is not boolean
        }

        /// <summary>
        /// Konwersja zwrotna nie jest obsługiwana.
        /// </summary>
        /// <exception cref="NotImplementedException">Zawsze zgłaszany, ponieważ konwersja zwrotna nie jest obsługiwana.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
