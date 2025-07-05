

using GodmistWPF.Combat.Modifiers;
using GodmistWPF.Enums.Modifiers;

namespace GodmistWPF.Utilities
{
    internal static class UtilityMethods
    {
        private static readonly Random Random = new();
        public static double RandomDouble(double minValue, double maxValue)
        {
            var number = Random.NextDouble();
            return number * (maxValue - minValue) + minValue;
        }
        public static float RandomFloat(float minValue, float maxValue, int round)
        {
            float number = (float)Random.NextDouble();
            return (float)Math.Round(number * (maxValue - minValue) + minValue, round);
        }
        public static double EffectChance(double resistance, double baseChance)
        {
            baseChance = Clamp(baseChance, 0, 1);
            return Math.Max(0, 1 - 2 * resistance) * (1 - baseChance) / (3 - baseChance) + baseChance * Math.Min(1, 2 * (1 - resistance));
        }
        public static double Clamp(double number, double min, double max)
        {
            return Math.Max(Math.Min(number, max), min); 
        }
        public static int Clamp(int number, int min, int max)
        {
            return Math.Max(Math.Min(number, max), min);
        }
        public static void WriteLineSlowly(string text)
        {
            // This method is now handled by WPF dialogs
            // For console compatibility, do nothing
        }
        public static void WriteLineSlowly(string text, object style)
        {
            // This method is now handled by WPF dialogs
            // For console compatibility, do nothing
        }
        public static void WriteSlowly(string text)
        {
            // This method is now handled by WPF dialogs
            // For console compatibility, do nothing
        }
        public static void WriteSlowly(string text, object style)
        {
            // This method is now handled by WPF dialogs
            // For console compatibility, do nothing
        }
        static int PauseLength(char c)
        {
            switch (c)
            {
                case '.':
                case ';':
                case '?':
                case '!':
                case ':':
                    return 100;
                case ',':
                    return Random.Next(35, 50);
                case ' ':
                    return Random.Next(15, 25);
                default:
                    return char.IsLetterOrDigit(c) ? Random.Next(25, 35) : 0;
            }
        }
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
        public static T RandomChoice<T>(List<T> choices)
        {
            var result = Random.Next(0, choices.Count);
            return choices[result];
        }

        public static bool Confirmation(string message, bool defaultValue = false)
        {
            // This method is now handled by WPF dialogs
            // For console compatibility, return default value
            return defaultValue;
        }

        public static void ClearConsole(int lines = 1)
        {
            for (var i = 0; i < lines; i++)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop);
            }
        }

        public static void ClearWithLog(List<object> logs)
        {
            // This method is now handled by WPF dialogs
            // For console compatibility, do nothing
        }
        public static void ClearWithLog(object log)
        {
            // This method is now handled by WPF dialogs
            // For console compatibility, do nothing
        }

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
