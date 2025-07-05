using System.Collections;
using System.Windows;
using ArmorBase = GodmistWPF.Items.Equippable.Armors.ArmorBase;
using ArmorBinder = GodmistWPF.Items.Equippable.Armors.ArmorBinder;
using ArmorPlate = GodmistWPF.Items.Equippable.Armors.ArmorPlate;
using CraftingManager = GodmistWPF.Items.CraftingManager;

using GalduriteComponent = GodmistWPF.Items.Galdurites.GalduriteComponent;
using GalduriteManager = GodmistWPF.Items.Galdurites.GalduriteManager;
using ICraftable = GodmistWPF.Items.ICraftable;
using PlayerHandler = GodmistWPF.Characters.Player.PlayerHandler;
using PotionCatalyst = GodmistWPF.Items.Potions.PotionCatalyst;
using PotionCatalystEffect = GodmistWPF.Enums.Items.PotionCatalystEffect;
using PotionComponent = GodmistWPF.Items.Potions.PotionComponent;
using PotionManager = GodmistWPF.Items.Potions.PotionManager;
using Quality = GodmistWPF.Enums.Items.Quality;
using WeaponBinder = GodmistWPF.Items.Equippable.Weapons.WeaponBinder;
using WeaponHandle = GodmistWPF.Items.Equippable.Weapons.WeaponHandle;
using WeaponHead = GodmistWPF.Items.Equippable.Weapons.WeaponHead;


namespace GodmistWPF.Dialogs
{
    public partial class CraftingDialog : Window
    {
        public CraftingDialog()
        {
            InitializeComponent();
            LoadInventory();
            ShowGeneralCrafting();
        }

        private void LoadInventory()
        {
            var player = PlayerHandler.player;
            InventoryList.ItemsSource = player.Inventory.Items.ToList();
            PlayerGold.Text = $"Gold: {player.Gold}";
        }

        private void WeaponCraftingButton_Click(object sender, RoutedEventArgs e)
        {
            HideAllCraftingGrids();
            WeaponCraftingGrid.Visibility = Visibility.Visible;
            // Populate weapon part combos
            WeaponHeadCombo.ItemsSource = EquipmentPartManager.WeaponHeads;
            WeaponBinderCombo.ItemsSource = EquipmentPartManager.WeaponBinders;
            WeaponHandleCombo.ItemsSource = EquipmentPartManager.WeaponHandles;
        }

        private void ArmorCraftingButton_Click(object sender, RoutedEventArgs e)
        {
            HideAllCraftingGrids();
            ArmorCraftingGrid.Visibility = Visibility.Visible;
            // Populate armor part combos
            ArmorPlateCombo.ItemsSource = EquipmentPartManager.ArmorPlates;
            ArmorBinderCombo.ItemsSource = EquipmentPartManager.ArmorBinders;
            ArmorBaseCombo.ItemsSource = EquipmentPartManager.ArmorBases;
        }

        private void PotionCraftingButton_Click(object sender, RoutedEventArgs e)
        {
            HideAllCraftingGrids();
            PotionCraftingGrid.Visibility = Visibility.Visible;
            // Populate potion components and catalysts
            PotionComponentsList.ItemsSource = PotionManager.PotionComponents;
            PotionCatalystCombo.ItemsSource = Enum.GetValues(typeof(PotionCatalystEffect));
        }

        private void GalduriteCraftingButton_Click(object sender, RoutedEventArgs e)
        {
            HideAllCraftingGrids();
            GalduriteCraftingGrid.Visibility = Visibility.Visible;
            // Populate galdurite components
            GalduriteComponentsList.ItemsSource = GalduriteManager.GalduriteComponents;
        }

        private void GeneralCraftingButton_Click(object sender, RoutedEventArgs e)
        {
            ShowGeneralCrafting();
        }

        private void ShowGeneralCrafting()
        {
            HideAllCraftingGrids();
            GeneralCraftingGrid.Visibility = Visibility.Visible;
            // Populate craftable items
            var player = PlayerHandler.player;
            CraftableItemsList.ItemsSource = player.Inventory.Items
                .Where(x => x.Key is ICraftable)
                .Select(x => x.Key as ICraftable)
                .Distinct()
                .ToList();
        }

        private void HideAllCraftingGrids()
        {
            WeaponCraftingGrid.Visibility = Visibility.Collapsed;
            ArmorCraftingGrid.Visibility = Visibility.Collapsed;
            PotionCraftingGrid.Visibility = Visibility.Collapsed;
            GalduriteCraftingGrid.Visibility = Visibility.Collapsed;
            GeneralCraftingGrid.Visibility = Visibility.Collapsed;
        }

        private void AddComponentButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedComponent = PotionComponentsList.SelectedItem as PotionComponent;
            if (selectedComponent != null)
            {
                var components = SelectedComponentsList.ItemsSource as IList ?? new ArrayList();
                components.Add(selectedComponent);
                SelectedComponentsList.ItemsSource = components;
            }
        }

        private void AddGalduriteComponentButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedComponent = GalduriteComponentsList.SelectedItem as GalduriteComponent;
            if (selectedComponent != null)
            {
                var components = SelectedGalduriteComponentsList.ItemsSource as IList ?? new ArrayList();
                components.Add(selectedComponent);
                SelectedGalduriteComponentsList.ItemsSource = components;
            }
        }

        private void CraftItemButton_Click(object sender, RoutedEventArgs e)
        {
            var item = CraftableItemsList.SelectedItem as ICraftable;
            if (item == null)
            {
                StatusText.Text = "Select an item to craft.";
                return;
            }
            // TODO: Implement general item crafting
            StatusText.Text = $"Crafted item: {item.Name}";
            LoadInventory();
        }

        private void CraftButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (WeaponCraftingGrid.Visibility == Visibility.Visible)
                {
                    var head = WeaponHeadCombo.SelectedItem as WeaponHead;
                    var binder = WeaponBinderCombo.SelectedItem as WeaponBinder;
                    var handle = WeaponHandleCombo.SelectedItem as WeaponHandle;
                    if (head == null || binder == null || handle == null)
                    {
                        StatusText.Text = "Select all weapon parts.";
                        return;
                    }

                    if (!CraftingManager.CanCraftWeapon(head, binder, handle, Quality.Normal))
                    {
                        StatusText.Text = "Not enough materials to craft weapon.";
                        return;
                    }

                    var weapon = CraftingManager.CraftWeapon(head, binder, handle, Quality.Normal, $"Crafted {head.Name}");
                    StatusText.Text = $"Successfully crafted: {weapon.Name}";
                }
                else if (ArmorCraftingGrid.Visibility == Visibility.Visible)
                {
                    var plate = ArmorPlateCombo.SelectedItem as ArmorPlate;
                    var binder = ArmorBinderCombo.SelectedItem as ArmorBinder;
                    var basePart = ArmorBaseCombo.SelectedItem as ArmorBase;
                    if (plate == null || binder == null || basePart == null)
                    {
                        StatusText.Text = "Select all armor parts.";
                        return;
                    }

                    if (!CraftingManager.CanCraftArmor(plate, binder, basePart, Quality.Normal))
                    {
                        StatusText.Text = "Not enough materials to craft armor.";
                        return;
                    }

                    var armor = CraftingManager.CraftArmor(plate, binder, basePart, Quality.Normal, $"Crafted {plate.Name}");
                    StatusText.Text = $"Successfully crafted: {armor.Name}";
                }
                else if (PotionCraftingGrid.Visibility == Visibility.Visible)
                {
                    var components = SelectedComponentsList.ItemsSource as IList;
                    if (components == null || components.Count == 0)
                    {
                        StatusText.Text = "Select potion components.";
                        return;
                    }

                    var potionComponents = components.Cast<PotionComponent>().ToList();
                    var catalyst = PotionCatalystCombo.SelectedItem as PotionCatalyst;

                    if (!CraftingManager.CanCraftPotion(potionComponents, catalyst))
                    {
                        StatusText.Text = "Not enough materials to craft potion.";
                        return;
                    }

                    var potion = CraftingManager.CraftPotion(potionComponents, catalyst, "Crafted Potion");
                    StatusText.Text = $"Successfully crafted: {potion.Name}";
                }
                else if (GalduriteCraftingGrid.Visibility == Visibility.Visible)
                {
                    var components = SelectedGalduriteComponentsList.ItemsSource as IList;
                    if (components == null || components.Count == 0)
                    {
                        StatusText.Text = "Select galdurite components.";
                        return;
                    }

                    // For now, use default values - in a full implementation, these would come from UI controls
                    var tier = 1;
                    var color = "Red";
                    var isArmorGaldurite = false;

                    if (!CraftingManager.CanCraftGaldurite(tier, color, isArmorGaldurite))
                    {
                        StatusText.Text = "Not enough materials to craft galdurite.";
                        return;
                    }

                    var galdurite = CraftingManager.CraftGaldurite(tier, color, isArmorGaldurite, "Crafted Galdurite");
                    StatusText.Text = $"Successfully crafted: {galdurite.Name}";
                }
                else if (GeneralCraftingGrid.Visibility == Visibility.Visible)
                {
                    var item = CraftableItemsList.SelectedItem as ICraftable;
                    if (item == null)
                    {
                        StatusText.Text = "Select an item to craft.";
                        return;
                    }

                    if (!CraftingManager.CanCraftItem(item))
                    {
                        StatusText.Text = "Not enough materials to craft item.";
                        return;
                    }

                    CraftingManager.CraftItem(item);
                    StatusText.Text = $"Successfully crafted: {item.Name}";
                }

                LoadInventory();
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Crafting error: {ex.Message}";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
} 