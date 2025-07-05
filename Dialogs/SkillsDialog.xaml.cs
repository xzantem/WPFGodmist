using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ActiveSkill = GodmistWPF.Combat.Skills.ActiveSkill;
using PlayerHandler = GodmistWPF.Characters.Player.PlayerHandler;

namespace GodmistWPF.Dialogs
{
    public partial class SkillsDialog : Window
    {
        private ObservableCollection<SkillViewModel> playerSkills;

        public SkillsDialog()
        {
            InitializeComponent();
            
            playerSkills = new ObservableCollection<SkillViewModel>();
            SkillsListBox.ItemsSource = playerSkills;
            
            // Set up event handlers
            SkillsListBox.SelectionChanged += SkillsListBox_SelectionChanged;
            
            // Load skills data
            LoadSkills();
        }

        private void LoadSkills()
        {
            playerSkills.Clear();
            
            if (PlayerHandler.player?.ActiveSkills != null)
            {
                foreach (var skill in PlayerHandler.player.ActiveSkills)
                {
                    if (skill != null)
                    {
                        playerSkills.Add(new SkillViewModel
                        {
                            Skill = skill,
                            DisplayName = $"{skill.Name} - {skill.ResourceCost} {PlayerHandler.player.ResourceType}"
                        });
                    }
                }
            }
        }

        private void SkillsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SkillsListBox.SelectedItem is SkillViewModel selectedSkill)
            {
                var skill = selectedSkill.Skill;
                var info = $"Name: {skill.Name}\n" +
                          $"Resource Cost: {skill.ResourceCost} {PlayerHandler.player.ResourceType}\n" +
                          $"Action Cost: {skill.ActionCost}\n" +
                          $"Accuracy: {skill.Accuracy}%\n" +
                          $"Hits: {skill.Hits}\n" +
                          $"Always Hits: {skill.AlwaysHits}\n\n";
                
                if (skill.Effects != null && skill.Effects.Count > 0)
                {
                    info += "Effects:\n";
                    foreach (var effect in skill.Effects)
                    {
                        info += $"• {effect.GetType().Name.Replace("ActiveSkillEffect", "")}\n";
                    }
                }
                
                SkillInfoText.Text = info;
            }
        }

        private void UseSkillButton_Click(object sender, RoutedEventArgs e)
        {
            if (SkillsListBox.SelectedItem is SkillViewModel selectedSkill)
            {
                // Note: Skills are typically used in combat, not from the skills dialog
                MessageBox.Show($"Skill {selectedSkill.Skill.Name} would be used in combat.\n" +
                               $"Resource Cost: {selectedSkill.Skill.ResourceCost} {PlayerHandler.player.ResourceType}\n" +
                               $"Action Cost: {selectedSkill.Skill.ActionCost}", 
                               "Skill Information", 
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a skill to use.", "No Skill Selected", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            if (SkillsListBox.SelectedItem is SkillViewModel selectedSkill)
            {
                // Note: Skill upgrading would need to be implemented in the backend
                MessageBox.Show($"Skill {selectedSkill.Skill.Name} upgrade functionality would be implemented here.", 
                               "Skill Upgrade", 
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a skill to upgrade.", "No Skill Selected", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class SkillViewModel
    {
        public ActiveSkill Skill { get; set; }
        public string DisplayName { get; set; }
    }
} 