using System.Globalization;
using ConsoleGodmist;
using GodmistWPF.Characters.Player;
using GodmistWPF.Enums;
using GodmistWPF.Quests;
using GodmistWPF.Towns;
using GodmistWPF.Utilities.DataPersistance;


namespace GodmistWPF.Utilities
{
    internal static class MainMenu
    {
        public static void Menu()
        {
            Console.WriteLine("=== GODMIST ===");
            while(true)
            {
                Console.WriteLine("\nMain Menu:");
                Console.WriteLine("1. New Game");
                Console.WriteLine("2. Load Game");
                Console.WriteLine("3. Delete Save File");
                Console.WriteLine("4. Choose Language");
                Console.WriteLine("5. Exit Game");
                Console.Write("Enter your choice (1-5): ");
                
                var input = Console.ReadLine();
                switch (input)
                {
                    case "1": NewGame(); return;
                    case "2": if (DataPersistanceManager.LoadGame()) return; break;
                    case "3": DataPersistanceManager.DeleteSaveFile(); break;
                    case "4": ChooseLanguage(); break;
                    case "5": Environment.Exit(0); break;
                    default: Console.WriteLine("Invalid choice. Please try again."); break;
                }
            }
        }

        private static void ChooseLanguage()
        {
            Console.WriteLine("\nChoose Language:");
            Console.WriteLine("1. English");
            Console.WriteLine("2. Polish");
            Console.Write("Enter your choice (1-2): ");
            
            var input = Console.ReadLine();
            var choice = input switch
            {
                "1" => 0,
                "2" => 1,
                _ => 0
            };

            Thread.CurrentThread.CurrentUICulture = choice switch
            {
                0 => CultureInfo.GetCultureInfo("en-US"),
                1 => CultureInfo.GetCultureInfo("pl-PL"),
                _ => Thread.CurrentThread.CurrentUICulture
            };
            CultureInfo.CurrentCulture = choice switch
            {
                0 => CultureInfo.GetCultureInfo("en-US"),
                1 => CultureInfo.GetCultureInfo("pl-PL"),
                _ => Thread.CurrentThread.CurrentCulture
            };
        }

        private static void NewGame()
        {
            TownsHandler.Arungard = new Town("Arungard");
            
            Console.WriteLine("\nChoose Difficulty:");
            Console.WriteLine("1. Easy");
            Console.WriteLine("2. Normal");
            Console.WriteLine("3. Hard");
            Console.WriteLine("4. Nightmare");
            Console.Write("Enter your choice (1-4): ");
            
            var input = Console.ReadLine();
            var choice = input switch
            {
                "1" => 0,
                "2" => 1,
                "3" => 2,
                "4" => 3,
                _ => 1
            };
            
            GameSettings.Difficulty = choice switch
            {
                0 => Difficulty.Easy,
                1 => Difficulty.Normal,
                2 => Difficulty.Hard,
                3 => Difficulty.Nightmare,
                _ => Difficulty.Normal
            };
            
            QuestManager.InitMainQuests();
            QuestManager.InitSideQuests(true);
            
            Console.WriteLine(locale.Opening_1);
            Console.WriteLine(locale.Opening_2);
            Console.WriteLine(locale.Opening_3);
            Console.WriteLine(locale.Opening_4);
            Console.WriteLine(locale.Opening_5);
            Console.WriteLine($"\n{locale.TownGuard_Name}: {locale.TownGuard_Line}");

            Console.WriteLine("\nChoose Character Class:");
            Console.WriteLine("1. Warrior");
            Console.WriteLine("2. Scout");
            Console.WriteLine("3. Sorcerer");
            Console.WriteLine("4. Paladin");
            Console.Write("Enter your choice (1-4): ");
            
            var input1 = Console.ReadLine();
            var choice1 = input1 switch
            {
                "1" => 0,
                "2" => 1,
                "3" => 2,
                "4" => 3,
                _ => 0
            };
            
            var characterClass = choice1 switch
            {
                0 => CharacterClass.Warrior,
                1 => CharacterClass.Scout,
                2 => CharacterClass.Sorcerer,
                3 => CharacterClass.Paladin,
                _ => CharacterClass.Warrior
            };
            
            Console.WriteLine($"\n???: {locale.Iam} {characterClass}");
            
            Console.Write($"???: {locale.MyNameIs} ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name) || name.Length > 32)
            {
                name = "Adventurer";
            }
            
            PlayerHandler.player = characterClass switch {
                CharacterClass.Warrior => new Warrior(name),
                CharacterClass.Scout => new Scout(name),
                CharacterClass.Sorcerer => new Sorcerer(name),
                CharacterClass.Paladin => new Paladin(name),
                _ => new Warrior(name)
            };
        }
    }
}
