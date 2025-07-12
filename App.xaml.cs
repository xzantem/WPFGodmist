// Importy przestrzeni nazw używanych w klasie App
using System.Windows;
using GodmistWPF.Characters;
using GodmistWPF.Dungeons.Interactables;
using GodmistWPF.Items;
using GodmistWPF.Items.Galdurites;
using GodmistWPF.Items.Lootbags;
using GodmistWPF.Items.Potions;

/// <summary>
/// Główna przestrzeń nazw aplikacji Godmist, zawierająca klasy odpowiedzialne za logikę gry i interfejs użytkownika.
/// </summary>
/// <remarks>
/// <para>Przestrzeń nazw zawiera m.in.:</para>
/// <list type="bullet">
/// <item>Główną klasę aplikacji (<see cref="App"/>)</item>
/// <item>Klasy postaci gracza i przeciwników</item>
/// <item>Systemy przedmiotów i ekwipunku</li>
/// <item>Systemy walki i umiejętności</li>
/// <item>Interfejs użytkownika i okna dialogowe</li>
/// </list>
/// </remarks>
namespace GodmistWPF
{
    /// <summary>
    /// Główna klasa aplikacji odpowiedzialna za konfigurację i inicjalizację systemów gry.
    /// </summary>
    /// <remarks>
    /// <para>Klasa <see cref="App"/> pełni następujące funkcje:</para>
    /// <list type="bullet">
    /// <item>Dziedziczy po klasie <see cref="Application"/>, zapewniając punkty wejścia dla aplikacji WPF</item>
    /// <item>Zarządza cyklem życia aplikacji</item>
    /// <item>Inicjalizuje i konfiguruje główne systemy gry</li>
    /// <item>Obsługuje globalne wyjątki i błędy</li>
    /// </list>
    /// <para>Zawiera metody do zarządzania zasobami, ustawieniami i konfiguracją aplikacji.</para>
    /// </remarks>
    public partial class App : Application
    {
        /// <summary>
        /// Metoda wywoływana podczas uruchamiania aplikacji. Inicjalizuje wszystkie niezbędne systemy gry.
        /// </summary>
        /// <param name="e">Argumenty zdarzenia uruchomienia aplikacji.</param>
        /// <remarks>
        /// <para>Metoda wykonuje następujące operacje inicjalizacyjne w podanej kolejności:</para>
        /// <list type="number">
        /// <item>Inicjalizacja menedżera przedmiotów (<see cref="ItemManager.InitItems"/>)</item>
        /// <item>Inicjalizacja menedżera łupów (<see cref="LootbagManager.InitItems"/>)</item>
        /// <item>Inicjalizacja menedżera części ekwipunku (<see cref="EquipmentPartManager.InitItems"/>)</item>
        /// <item>Inicjalizacja fabryki przeciwników (<see cref="EnemyFactory.InitializeEnemies"/>)</item>
        /// <item>Inicjalizacja zarządzania upuszczanymi roślinami (<see cref="PlantDropManager.InitPlantDrops"/>)</item>
        /// <item>Inicjalizacja menedżera mikstur (<see cref="PotionManager.InitComponents"/>)</item>
        /// <item>Inicjalizacja menedżera galdurytów (<see cref="GalduriteManager.InitComponents"/>)</item>
        /// </list>
        /// <para>W przypadku wystąpienia wyjątku podczas inicjalizacji:</para>
        /// <list type="bullet">
        /// <item>Wyświetlany jest komunikat o błędzie</item>
        /// <item>Aplikacja jest bezpiecznie zamykana</item>
        /// </list>
        /// </remarks>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                ItemManager.InitItems();
                LootbagManager.InitItems();
                EquipmentPartManager.InitItems();
                EnemyFactory.InitializeEnemies();
                PlantDropManager.InitPlantDrops();
                PotionManager.InitComponents();
                GalduriteManager.InitComponents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing game systems: {ex.Message}", "Initialization Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
} 