using System.Windows;
using GodmistWPF.Characters;
using GodmistWPF.Dungeons.Interactables;
using GodmistWPF.Items;
using GodmistWPF.Items.Galdurites;
using GodmistWPF.Items.Lootbags;
using GodmistWPF.Items.Potions;


namespace GodmistWPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Initialize game systems (same as console version)
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