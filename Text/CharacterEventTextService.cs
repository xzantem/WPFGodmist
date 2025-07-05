using Character = GodmistWPF.Characters.Character;
using DamageType = GodmistWPF.Enums.DamageType;
using HonorLevel = GodmistWPF.Enums.HonorLevel;
using PlayerCharacter = GodmistWPF.Characters.Player.PlayerCharacter;

namespace ConsoleGodmist.TextService;

public static class CharacterEventTextService
{
    // WPF doesn't need console text formatting, so these methods are now no-ops
    public static void DisplayTakeDamageText(Character character, Dictionary<DamageType, int> damage, 
        bool isFirstPerson = false)
    {
        // WPF handles damage display through UI
    }
    
    public static void DisplayBattleTakeDamageText(Character character, Dictionary<DamageType, int> damage)
    {
        // WPF handles battle damage display through UI
    } 

    public static void DisplayHealText(Character character, int heal)
    {
        // WPF handles heal display through UI
    }
    
    public static void DisplayResourceRegenText(Character character, int regen)
    {
        // WPF handles resource regen display through UI
    }
    
    public static void DisplayGoldGainText(PlayerCharacter character, int gold)
    {
        // WPF handles gold gain display through UI
    }
    
    public static void DisplayGoldLossText(PlayerCharacter character, int gold)
    {
        // WPF handles gold loss display through UI
    }
    
    public static void DisplayHonorGainText(PlayerCharacter character, int honor)
    {
        // WPF handles honor gain display through UI
    }
    
    public static void DisplayHonorLossText(PlayerCharacter character, int honor)
    {
        // WPF handles honor loss display through UI
    }

    public static void DisplayExperienceGainText(int experience)
    {
        // WPF handles experience gain display through UI
    }
    
    public static void DisplayLevelUpText(int level)
    {
        // WPF handles level up display through UI
    }

    public static void DisplayCurrentLevelText(PlayerCharacter character, int experience, int calculatedExperience)
    {
        // WPF handles level display through UI
    }
    
    private static void DisplayHonorText(HonorLevel honorLevel)
    {
        // WPF handles honor display through UI
    }

    public static void DisplayCharacterMenuText(PlayerCharacter player)
    {
        // WPF handles character menu display through UI
    }
}