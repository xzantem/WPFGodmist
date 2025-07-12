

using GodmistWPF.Combat.Modifiers;
using GodmistWPF.Enums.Modifiers;

namespace GodmistWPF.Utilities
{
    /// <summary>
    /// Klasa zawierająca użyteczne metody pomocnicze używane w całej aplikacji.
    /// Zawiera metody do operacji matematycznych, losowania i formatowania tekstu.
    /// </summary>
    internal static class UtilityMethods
    {
        private static readonly Random Random = new();
        
        /// <summary>
        /// Generuje losową liczbę zmiennoprzecinkową z podanego zakresu.
        /// </summary>
        /// <param name="minValue">Minimalna wartość (włącznie).</param>
        /// <param name="maxValue">Maksymalna wartość (wyłącznie).</param>
        /// <returns>Losowa liczba z zakresu [minValue, maxValue).</returns>
        public static double RandomDouble(double minValue, double maxValue)
        {
            var number = Random.NextDouble();
            return number * (maxValue - minValue) + minValue;
        }
        /// <summary>
        /// Generuje losową liczbę zmiennoprzecinkową z podanego zakresu i zaokrągla ją do określonej liczby miejsc po przecinku.
        /// </summary>
        /// <param name="minValue">Minimalna wartość (włącznie).</param>
        /// <param name="maxValue">Maksymalna wartość (wyłącznie).</param>
        /// <param name="round">Liczba miejsc po przecinku, do której ma zostać zaokrąglony wynik.</param>
        /// <returns>Zaokrąglona losowa liczba z zakresu [minValue, maxValue).</returns>
        public static float RandomFloat(float minValue, float maxValue, int round)
        {
            float number = (float)Random.NextDouble();
            return (float)Math.Round(number * (maxValue - minValue) + minValue, round);
        }
        /// <summary>
        /// Oblicza szansę na efekt uwzględniając odporność i bazową szansę.
        /// </summary>
        /// <param name="resistance">Wartość odporności (0-1, gdzie 1 to pełna odporność).</param>
        /// <param name="baseChance">Bazowa szansa na efekt (0-1).</param>
        /// <returns>Rzeczywista szansa na efekt po uwzględnieniu odporności.</returns>
        public static double EffectChance(double resistance, double baseChance)
        {
            baseChance = Clamp(baseChance, 0, 1);
            return Math.Max(0, 1 - 2 * resistance) * (1 - baseChance) / (3 - baseChance) + baseChance * Math.Min(1, 2 * (1 - resistance));
        }
        /// <summary>
        /// Ogranicza wartość do podanego zakresu.
        /// </summary>
        /// <param name="number">Wartość do ograniczenia.</param>
        /// <param name="min">Minimalna wartość.</param>
        /// <param name="max">Maksymalna wartość.</param>
        /// <returns>Wartość z zakresu [min, max].</returns>
        public static double Clamp(double number, double min, double max)
        {
            return Math.Max(Math.Min(number, max), min); 
        }
        /// <summary>
        /// Ogranicza wartość całkowitą do podanego zakresu.
        /// </summary>
        /// <param name="number">Wartość do ograniczenia.</param>
        /// <param name="min">Minimalna wartość.</param>
        /// <param name="max">Maksymalna wartość.</param>
        /// <returns>Wartość całkowita z zakresu [min, max].</returns>
        /// <seealso cref="Clamp(int, int, int)"/>
        public static int Clamp(int number, int min, int max)
        {
            return Math.Max(Math.Min(number, max), min);
        }
        /// <summary>
        /// Oblicza skalowaną wartość statystyki na podstawie poziomu postaci.
        /// </summary>
        /// <param name="baseStat">Bazowa wartość statystyki.</param>
        /// <param name="scaleFactor">Współczynnik skalowania.</param>
        /// <param name="level">Poziom postaci.</param>
        /// <returns>Przeskalowana wartość statystyki.</returns>
        public static double ScaledStat(double baseStat, double scaleFactor, int level) {
            for (var i = 1; i < Math.Min(10, level); i++)
                baseStat += scaleFactor;
            for (var i = 10; i < Math.Min(20, level); i++)
                baseStat += 2 * scaleFactor;
            for (var i = 20; i < Math.Min(30, level); i++)
                baseStat += 3 * scaleFactor;
            for (var i = 30; i < Math.Min(40, level); i++)
                baseStat += 5 * scaleFactor;
            for (var i = 40; i < Math.Min(50, level); i++)
                baseStat += 9 * scaleFactor;
            return baseStat;
        }

        /// <summary>
        /// Wybiera losowy element na podstawie wag prawdopodobieństwa.
        /// </summary>
        /// <typeparam name="T">Typ elementów do wyboru.</typeparam>
        /// <param name="choices">Słownik zawierający elementy i ich wagi prawdopodobieństwa (suma wag powinna wynosić 1).</param>
        /// <returns>Wybrany losowo element.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Wyrzucany, gdy wagi nie sumują się do 1.</exception>
        public static T RandomChoice<T>(Dictionary<T, double> choices)
        {
            var result = Random.NextDouble();
            if (Math.Abs(choices.Values.Sum() - 1) > 0.00001)
            {
                throw new ArgumentOutOfRangeException(nameof(choices), "Choices chances should add up to one");
            }
            double sum = 0;
            foreach (var choice in choices)
            {
                sum += choice.Value;
                if (result < sum)
                    return choice.Key;
            }

            throw new NullReferenceException();
        }
        /// <summary>
        /// Wybiera losowy element na podstawie liczbowych wag.
        /// </summary>
        /// <typeparam name="T">Typ elementów do wyboru.</typeparam>
        /// <param name="choices">Słownik zawierający elementy i ich wagi.</param>
        /// <returns>Wybrany losowo element lub domyślna wartość typu T, jeśli słownik jest pusty.</returns>
        public static T RandomChoice<T>(Dictionary<T, int> choices)
        {
            var result = Random.Next(1, choices.Values.Sum() + 1);
            double sum = 0;
            foreach (var choice in choices)
            {
                sum += choice.Value;
                if (result <= sum)
                    return choice.Key;
            }

            return default;
        }
        /// <summary>
        /// Wybiera losowy element z listy z równym prawdopodobieństwem.
        /// </summary>
        /// <typeparam name="T">Typ elementów na liście.</typeparam>
        /// <param name="choices">Lista elementów do wyboru.</param>
        /// <returns>Losowy element z listy.</returns>
        public static T RandomChoice<T>(List<T> choices)
        {
            var result = Random.Next(0, choices.Count);
            return choices[result];
        }
        

        /// <summary>
        /// Oblicza końcową wartość statystyki po zastosowaniu modyfikatorów.
        /// Kolejność stosowania modyfikatorów:
        /// 1. Mnożenie przez wszystkie modyfikatory względne (Relative)
        /// 2. Dodawanie modyfikatorów addytywnych (Additive)
        /// 3. Mnożenie przez wszystkie modyfikatory multiplikatywne (Multiplicative)
        /// 4. Dodawanie modyfikatorów absolutnych (Absolute)
        /// </summary>
        /// <param name="initial">Początkowa wartość statystyki.</param>
        /// <param name="modifiers">Lista modyfikatorów do zastosowania.</param>
        /// <returns>Końcowa wartość statystyki po zastosowaniu modyfikatorów.</returns>
        public static double CalculateModValue(double initial, List<StatModifier> modifiers)
        {
            // Order of modifiers:
            // 1. Multiply by all relative modifiers
            // 2. Add all additive modifiers
            // 3. Multiply by all multiplicative modifiers
            // 4. Add all absolute modifiers
            initial = modifiers.Where(modifier => modifier.Type == ModifierType.Relative)
                .Aggregate(initial, (current, modifier) => current * (1 + modifier.Mod)) + modifiers
                .Where(modifier => modifier.Type == ModifierType.Additive).Sum(modifier => modifier.Mod);
            return modifiers.Where(modifier => modifier.Type == ModifierType.Multiplicative)
                .Aggregate(initial, (current, modifier) => current * (1 + modifier.Mod)) + modifiers
                .Where(modifier => modifier.Type == ModifierType.Absolute).Sum(modifier => modifier.Mod);
        }
    }
}
