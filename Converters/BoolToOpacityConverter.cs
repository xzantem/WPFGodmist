using System;
using System.Globalization;
using System.Windows.Data;

namespace GodmistWPF.Converters
{
    /// <summary>
    /// Konwerter, który zamienia wartość logiczną na wartość przezroczystości (opacity).
    /// </summary>
    /// <remarks>
    /// Umożliwia ustawienie różnych wartości przezroczystości dla wartości true i false.
    /// Format parametru: "wartość_dla_true:wartość_dla_false" (np. "1.0:0.5").
    /// </remarks>
    public class BoolToOpacityConverter : IValueConverter
    {
        /// <summary>
        /// Konwertuje wartość logiczną na wartość przezroczystości.
        /// </summary>
        /// <param name="value">Wartość do przekonwertowania (bool).</param>
        /// <param name="targetType">Typ docelowy (ignorowany).</param>
        /// <param name="parameter">Parametr w formacie "wartość_dla_true:wartość_dla_false".</param>
        /// <param name="culture">Kultura używana do konwersji (ignorowana).</param>
        /// <returns>Wartość przezroczystości jako double (domyślnie 1.0).</returns>
        /// <remarks>
        /// Jeśli parametr nie jest w odpowiednim formacie, zwraca domyślną wartość 1.0.
        /// </remarks>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string parameterString)
            {
                var parts = parameterString.Split(':');
                if (parts.Length == 2)
                {
                    if (boolValue && double.TryParse(parts[0], out double trueValue))
                        return trueValue;
                    if (!boolValue && double.TryParse(parts[1], out double falseValue))
                        return falseValue;
                }
            }
            return 1.0; // Default opacity
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
