using System.Windows;
using GodmistWPF.Characters.Player;
using GodmistWPF.Items;
using GodmistWPF.Items.Equippable;
using GodmistWPF.Items.Equippable.Armors;
using GodmistWPF.Items.Equippable.Weapons;
using System.Linq;
using System.Text;
using GodmistWPF.Enums;
using GodmistWPF.Enums.Items;

namespace GodmistWPF.Dialogs
{
    /// <summary>
    /// Okno wyświetlające szczegółowe informacje o postaci gracza.
    /// Zawiera statystyki, ekwipunek oraz inne istotne informacje o postaci.
    /// </summary>
    public partial class CharacterWindow : Window
    {
        /// <summary>
        /// Referencja do postaci gracza, której dane są wyświetlane w oknie.
        /// </summary>
        private readonly PlayerCharacter _player;

        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="CharacterWindow"/>.
        /// </summary>
        /// <param name="player">Postać gracza, której dane mają być wyświetlone.</param>
        public CharacterWindow(PlayerCharacter player)
        {
            InitializeComponent();
            _player = player;
            Loaded += OnLoaded;
        }

        /// <summary>
        /// Obsługuje zdarzenie załadowania okna.
        /// Inicjalizuje i aktualizuje wyświetlane informacje o postaci.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateCharacterInfo();
        }

        /// <summary>
        /// Aktualizuje wyświetlane informacje o postaci.
        /// Pobiera i wyświetla podstawowe statystyki, zdrowie, zasoby oraz inne dane postaci.
        /// </summary>
        private void UpdateCharacterInfo()
        {
            if (_player == null) return;

            // Basic Info
            CharacterNameText.Text = _player.Name;
            LevelText.Text = $"Level {_player.Level}";
            ClassText.Text = _player.CharacterClass.ToString();
            ExperienceText.Text = $"{_player.CurrentExperience:N0} / {_player.RequiredExperience:N0} XP ({(double)_player.CurrentExperience / _player.RequiredExperience:P0})";
            GoldText.Text = _player.Gold.ToString("N0");
            HonorText.Text = _player.Honor.ToString("N0");

            // Combat Stats
            HealthText.Text = $"{_player.CurrentHealth:0.#} / {_player.MaximalHealth:0.#} ({(int)(_player.CurrentHealth / _player.MaximalHealth * 100)}%)";
            ResourceText.Text = $"{_player.CurrentResource:0.#} / {_player.MaximalResource:0.#} {_player.ResourceType}";
            
            // Attack and Defense
            PhysicalAttackText.Text = $"{_player.MinimalAttack:0.#} - {_player.MaximalAttack:0.#}";
            DefenseText.Text = $"{_player.PhysicalDefense:0.#} P | {_player.MagicDefense:0.#} M";
            
            // Secondary Stats
            DodgeText.Text = $"{_player.Dodge:0.#}";
            CritText.Text = $"{_player.CritChance:P1} (x{_player.CritMod:0.##})";
            SpeedText.Text = $"{_player.Speed:0.#}";
            
            // Update equipment display
            UpdateEquipmentDisplay();
        }
        
        /// <summary>
        /// Aktualizuje wyświetlane informacje o wyposażeniu postaci.
        /// Pobiera dane o broni i zbroi używając refleksji i aktualizuje interfejs użytkownika.
        /// </summary>
        private void UpdateEquipmentDisplay()
        {
            // Get equipment using reflection since we don't have direct access to the properties
            var weaponProp = _player.GetType().GetProperty("Weapon");
            var armorProp = _player.GetType().GetProperty("Armor");
            
            // Update Weapon Display
            if (weaponProp?.GetValue(_player) is Weapon weapon)
            {
                WeaponNameText.Text = weapon.Name;
                var sb = new StringBuilder();
                sb.AppendLine($"Damage: {weapon.MinimalAttack:0.#} - {weapon.MaximalAttack:0.#}");
                sb.AppendLine($"Crit: {weapon.CritChance:P1} (x{weapon.CritMod:0.##})");
                sb.AppendLine($"Accuracy: {weapon.Accuracy:0.#}");
                WeaponStatsText.Text = sb.ToString().TrimEnd();
                WeaponSpecialText.Text = GetItemSpecialText(weapon);
            }
            else
            {
                WeaponNameText.Text = "No Weapon Equipped";
                WeaponStatsText.Text = string.Empty;
                WeaponSpecialText.Text = string.Empty;
            }
            
            // Update Armor Display
            if (armorProp?.GetValue(_player) is Armor armor && armor != null)
            {
                ArmorNameText.Text = armor.Name;
                var sb = new StringBuilder();
                sb.AppendLine($"Defense: {armor.PhysicalDefense:0.#} P | {armor.MagicDefense:0.#} M");
                sb.AppendLine($"Dodge: {armor.Dodge:0.#}");
                sb.AppendLine($"Health: {armor.MaximalHealth:0.#}");
                ArmorStatsText.Text = sb.ToString().TrimEnd();
                ArmorSpecialText.Text = GetItemSpecialText(armor);
            }
            else
            {
                ArmorNameText.Text = "No Armor Equipped";
                ArmorStatsText.Text = string.Empty;
                ArmorSpecialText.Text = string.Empty;
            }
        }
        
        /// <summary>
        /// Generuje tekst z dodatkowymi informacjami o przedmiocie.
        /// </summary>
        /// <param name="item">Przedmiot, dla którego mają zostać wygenerowane informacje.</param>
        /// <returns>Sformatowany ciąg znaków zawierający dodatkowe informacje o przedmiocie.</returns>
        private string GetItemSpecialText(IItem item)
        {
            if (item is IEquippable equippable)
            {
                var parts = new List<string>();
                if (equippable.RequiredLevel > 1)
                    parts.Add($"Lv. {equippable.RequiredLevel}+");
                parts.Add(equippable.RequiredClass.ToString());
                if (equippable.Quality != Quality.Normal)
                    parts.Add(equippable.Quality.ToString());
                if (equippable.Galdurites?.Count > 0)
                    parts.Add($"{equippable.Galdurites.Count} Galdurites");
                return string.Join(" • ", parts);
            }
            return string.Empty;
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku zamknięcia okna.
        /// Zamyka okno i ustawia wynik dialogu na true.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
